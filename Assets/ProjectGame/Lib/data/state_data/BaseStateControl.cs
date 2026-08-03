
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GameCore.States.ID;

namespace GameCore.States.Control
{
    public abstract class BaseStateControl<T, E, F> : IDisposable
        where T : Enum
        where E : GameCore.States.Managers.BaseStateManagerData<T>,new()
        where F : GameCore.States.BaseState<T,E>
    {

        protected E state_manager_data = new E();
        public E StateManagerData{get { return state_manager_data; }}

        protected F state;

        protected bool is_finish = false;
        public bool IsFinish { get { return is_finish; } }

        /// <summary>
        /// このStateControlが担当するStateグループの識別子。
        /// Start系実行時にStateGroupTrackerへ反映される。
        /// </summary>
        protected abstract StateGroupID GroupID { get; }

        // ------------------------------------------------------------
        // ルートのCancellationTokenSource。
        // 非同期API・Combined APIが内部で使う「生存期間トークン」のもとになる。
        // Setup(externalToken) を呼ぶと、以後はそのトークンに連結される
        // （渡さなければ自前で新規作成して管理する）。
        // ------------------------------------------------------------
        private CancellationTokenSource rootCts;
        private bool isSetup;

        /// <summary>
        /// StateControlの初期セットアップ。外部からCancellationTokenを渡すと、
        /// StateControl全体（非同期API・Combined APIが内部で使うルートトークン）の
        /// 生存期間をそのトークンに入れ替える（例: this.GetCancellationTokenOnDestroy()）。
        /// 渡さなければ自前のトークンで管理する。
        /// Start系を呼ぶ前に一度だけ呼ぶのが望ましいが、呼ばれていなくても
        /// 最初のStart時に自動的に自前トークンでセットアップされる。
        /// 複数回呼ぶとその都度ルートトークンが張り替わり、それまでの非同期処理は
        /// キャンセルされる。
        /// </summary>
        public void Setup(CancellationToken externalToken = default)
        {
            rootCts?.Cancel();
            rootCts?.Dispose();
            rootCts = externalToken != default
                ? CancellationTokenSource.CreateLinkedTokenSource(externalToken)
                : new CancellationTokenSource();
            isSetup = true;
        }

        protected CancellationToken RootToken
        {
            get
            {
                if (!isSetup) Setup();
                return rootCts.Token;
            }
        }

        /// <summary>
        /// Fire-and-Forgetした非同期処理内の例外を握りつぶさずログへ出す共通ハンドラ。
        /// .Forget() の代わりに .Forget(LogAsyncException) を使う。
        /// （キャンセルによる例外は正常系なので無視する）
        /// </summary>
        protected static void LogAsyncException(Exception ex)
        {
            if (ex is OperationCanceledException) return;
            Debug.LogException(ex);
        }

        // 遷移(Exit→Enter)処理中の再入防止フラグ。
        // StartState/BranchState系の多重呼び出し（例: 誤って同フレームで2回呼んだ等）による
        // Stateの多重生成・Exit漏れを防ぐ。生成される Base{name}StateControl の
        // BranchState/BranchStateAsync/BranchStateCombined がこれをtrue/falseする。
        protected bool isTransitioning;

        // 現在の状態(Enter/Update/Exit)実行中のCancellationTokenSource。
        // 状態が切り替わるたびに古いものはキャンセル＆破棄され、
        // 外部から渡された（または RootToken の）「生存期間トークン」に
        // 連結した新しいものが作られる。※非同期API(StartStateAsync等)を使う場合のみ利用。
        protected CancellationTokenSource stateCts;

        protected CancellationToken RenewStateToken(CancellationToken life_time_token)
        {
            stateCts?.Cancel();
            stateCts?.Dispose();
            stateCts = CancellationTokenSource.CreateLinkedTokenSource(life_time_token);
            return stateCts.Token;
        }

        protected abstract T GetInitStartID();

        // ------------------------------------------------------------
        // 同期API: シンプルなStateマシン向け。CancellationTokenは扱わない。
        // Enter/Update/Exit（同期版）を直接呼び出す。
        // ------------------------------------------------------------
        public void StartState(Action<E> action = null)
        {
            if (state != null)
            {
                Debug.LogWarning("StartState は既に開始済みです。二重呼び出しを無視しました。");
                return;
            }
            OnStartState(GetInitStartID(), action);
        }
        public void StartState(T state_id)
        {
            if (state != null)
            {
                Debug.LogWarning("StartState は既に開始済みです。二重呼び出しを無視しました。");
                return;
            }
            OnStartState(state_id, null);
        }

        protected void OnStartState(T state_id, Action<E> action)
        {
            StateGroupTracker.ChangeGroup(GroupID);
            state = FactoryState(state_id);
            state_manager_data.ChangeStateNowID(state_id);
            action?.Invoke(state_manager_data);
            state.Enter(state_manager_data);
        }

        public void UpdateState(Action<E> befor_action = null, Action<E> after_action = null)
        {
            if (state == null) StartState();
            OnUpdateState(befor_action, after_action);
        }

        protected void OnUpdateState(Action<E> befor_action = null, Action<E> after_action = null)
        {
            befor_action?.Invoke(state_manager_data);
            state.Update(state_manager_data);
            if (!isTransitioning) BranchState();
            after_action?.Invoke(state_manager_data);
        }

        /// <summary>
        /// 同期版の遷移判定。CancellationTokenは扱わない版。
        /// アセットロード待ちなど非同期処理が絡まないStateマシンはこちらを使う。
        /// </summary>
        public abstract void BranchState();

        // ------------------------------------------------------------
        // 非同期API: 待ち処理(アセットロード等)が絡むStateマシン向け。
        // CancellationTokenSourceの張替え・連結を StateControl 側で管理する。
        // life_time_token を省略した場合は RootToken（Setup()で用意したもの）を使う。
        // ------------------------------------------------------------
        public UniTask StartStateAsync(CancellationToken life_time_token = default, Action<E> action = null)
        {
            if (state != null)
            {
                Debug.LogWarning("StartStateAsync は既に開始済みです。二重呼び出しを無視しました。");
                return UniTask.CompletedTask;
            }
            return OnStartStateAsync(GetInitStartID(), action, life_time_token == default ? RootToken : life_time_token);
        }
        public UniTask StartStateAsync(T state_id, CancellationToken life_time_token = default)
        {
            if (state != null)
            {
                Debug.LogWarning("StartStateAsync は既に開始済みです。二重呼び出しを無視しました。");
                return UniTask.CompletedTask;
            }
            return OnStartStateAsync(state_id, null, life_time_token == default ? RootToken : life_time_token);
        }

        protected async UniTask OnStartStateAsync(T state_id, Action<E> action, CancellationToken life_time_token)
        {
            StateGroupTracker.ChangeGroup(GroupID);
            state = FactoryState(state_id);
            state_manager_data.ChangeStateNowID(state_id);
            action?.Invoke(state_manager_data);
            CancellationToken ct = RenewStateToken(life_time_token);
            await state.EnterAsync(state_manager_data, ct);
        }

        public async UniTask UpdateStateAsync(CancellationToken life_time_token = default, Action<E> befor_action = null, Action<E> after_action = null)
        {
            CancellationToken token = life_time_token == default ? RootToken : life_time_token;
            if (state == null) await StartStateAsync(token);
            await OnUpdateStateAsync(token, befor_action, after_action);
        }

        protected async UniTask OnUpdateStateAsync(CancellationToken life_time_token, Action<E> befor_action = null, Action<E> after_action = null)
        {
            befor_action?.Invoke(state_manager_data);
            await state.UpdateAsync(state_manager_data, stateCts.Token);
            if (!isTransitioning) await BranchStateAsync(life_time_token);
            after_action?.Invoke(state_manager_data);
        }

        /// <summary>
        /// 現在の状態の遷移判定を行い、必要ならExit→(次の状態を生成)→Enterを非同期で実行する。
        /// life_time_token は次状態のEnter/Update/Exitの生存期間トークンとして引き継がれる。
        /// </summary>
        public abstract UniTask BranchStateAsync(CancellationToken life_time_token);

        // ------------------------------------------------------------
        // Combined API: 同期(Enter/Update/Exit)と非同期(EnterAsync/UpdateAsync/ExitAsync)を
        // 同時に動かすモード。呼び出し側は同期APIと同じ感覚（awaitなし）で毎フレーム呼べる。
        // 内部で使うCancellationTokenは RootToken（Setup()で入れ替え可能）に連結される。
        //
        // ・Enter/Exit: state自身が宣言する UseEnterSync/UseEnterAsync等に従って呼ぶ。
        //   同期はその場で、非同期は Forget(LogAsyncException) で発火するだけ（待たない）。
        // ・Update: 毎フレーム同期版を呼びつつ、非同期版は状態が変わった際に一度だけ発火する。
        //   遷移するかどうかは state.IsActive と state.IsActiveAsync の「両方」が
        //   false になった時点で判定する（同期側だけ・非同期側だけが終わっても遷移しない）。
        // ------------------------------------------------------------
        protected CancellationTokenSource combinedCts;
        protected bool combinedAsyncUpdateStarted;

        public void StartStateCombined(Action<E> action = null)
        {
            if (state != null)
            {
                Debug.LogWarning("StartStateCombined は既に開始済みです。二重呼び出しを無視しました。");
                return;
            }
            OnStartStateCombined(GetInitStartID(), action);
        }
        public void StartStateCombined(T state_id)
        {
            if (state != null)
            {
                Debug.LogWarning("StartStateCombined は既に開始済みです。二重呼び出しを無視しました。");
                return;
            }
            OnStartStateCombined(state_id, null);
        }

        protected void OnStartStateCombined(T state_id, Action<E> action)
        {
            StateGroupTracker.ChangeGroup(GroupID);
            state = FactoryState(state_id);
            state_manager_data.ChangeStateNowID(state_id);
            action?.Invoke(state_manager_data);

            combinedCts?.Cancel();
            combinedCts?.Dispose();
            combinedCts = CancellationTokenSource.CreateLinkedTokenSource(RootToken);
            combinedAsyncUpdateStarted = false;

            // Enter: state自身が宣言する UseEnterSync/UseEnterAsync に従って呼ぶ
            if (state.UseEnterSync) state.Enter(state_manager_data);
            if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
        }

        public void UpdateStateCombined(Action<E> befor_action = null, Action<E> after_action = null)
        {
            if (state == null) StartStateCombined();
            OnUpdateStateCombined(befor_action, after_action);
        }

        protected void OnUpdateStateCombined(Action<E> befor_action = null, Action<E> after_action = null)
        {
            befor_action?.Invoke(state_manager_data);

            // Update: state自身が宣言する UseUpdateSync/UseUpdateAsync に従って呼ぶ。
            // 非同期は、この状態になってから一度だけ発火する（毎フレーム再発火しない）。
            if (state.UseUpdateSync) state.Update(state_manager_data);
            if (state.UseUpdateAsync && !combinedAsyncUpdateStarted)
            {
                combinedAsyncUpdateStarted = true;
                state.UpdateAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
            }

            // 同期・非同期の両方が「終わった」と自己申告した時だけ次へ進む。
            // （isTransitioningで、遷移処理自体からの再入も防止する）
            if (!isTransitioning && !state.IsActive && !state.IsActiveAsync)
            {
                BranchStateCombined();
            }

            after_action?.Invoke(state_manager_data);
        }

        /// <summary>
        /// Combinedモードでの遷移判定。IsActive/IsActiveAsyncが両方falseの時だけ
        /// StateControl側から呼ばれる（呼ばれた時点で無条件にExit・遷移してよい）。
        /// Exitも同期はその場、非同期はFire-and-Forgetで発火する。
        /// </summary>
        public abstract void BranchStateCombined();

        public abstract F FactoryState(T state_id);

        /// <summary>
        /// StateControlが使う全てのCancellationTokenSourceを解放する。
        /// MonoBehaviourのOnDestroy等から呼ぶこと。
        /// </summary>
        public virtual void Dispose()
        {
            stateCts?.Cancel();
            stateCts?.Dispose();
            stateCts = null;
            combinedCts?.Cancel();
            combinedCts?.Dispose();
            combinedCts = null;
            rootCts?.Cancel();
            rootCts?.Dispose();
            rootCts = null;
        }

    }
}
