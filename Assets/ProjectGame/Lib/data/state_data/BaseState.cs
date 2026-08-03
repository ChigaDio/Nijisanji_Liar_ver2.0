using Cysharp.Threading.Tasks;
using System.Threading;
using GameCore.States.Managers;
using System;
namespace GameCore.States
{
    public abstract class  BaseState<E,T>where E : Enum where T : BaseStateManagerData<E>
    {
        private bool is_active = true;
        public bool IsActive => is_active;

        /// <summary>
        /// IsActive・IsActiveAsyncの「両方」がfalseになった瞬間に一度だけ発火する。
        /// 通知用のフックであり、StateControl自身はこれを遷移のトリガーには使わない
        /// （Update()の呼び出しスタック内から再入で遷移処理を始めるのは安全ではないため）。
        /// UIやログなど、「このStateが両方終わったこと」を早く知りたい用途向け。
        /// </summary>
        public event Action OnStateFullyInactive;

        private void CheckFullyInactive()
        {
            if (!is_active && !is_active_async)
            {
                OnStateFullyInactive?.Invoke();
            }
        }

        protected void IsActiveOff()
        {
            is_active = false;
            CheckFullyInactive();
        }

        /// <summary>
        /// Combinedモード（同期・非同期を同時に走らせるAPI）専用のフラグ。
        /// 非同期側のUpdateAsyncが自分の処理を終えた時に IsActiveAsyncOff() を呼ぶ。
        /// StateControl の Combined API は IsActive と IsActiveAsync の両方が
        /// falseになって初めて次の状態へ遷移する。
        /// </summary>
        private bool is_active_async = true;
        public bool IsActiveAsync => is_active_async;

        protected void IsActiveAsyncOff()
        {
            is_active_async = false;
            CheckFullyInactive();
        }

        /// <summary>
        /// Combinedモードで、このStateが各フェーズで同期/非同期のどちらを使うかを宣言する。
        /// 遷移図のノードごとのチェックボックス設定に応じて、生成される具象クラス側で
        /// overrideされる（デフォルトは同期のみ）。
        /// StateControlのCombined APIはこれを見て呼び出す関数を決めるため、
        /// 「チェックしていない方を二重に呼んでしまう」ことがない。
        /// </summary>
        public virtual bool UseEnterSync => true;
        public virtual bool UseEnterAsync => false;
        public virtual bool UseUpdateSync => true;
        public virtual bool UseUpdateAsync => false;
        public virtual bool UseExitSync => true;
        public virtual bool UseExitAsync => false;

        protected BaseState()
        {
        }

        /// <summary>
        /// 同期版ライフサイクル。単純なStateはこちらだけoverrideすればよい。
        /// StateControlの同期API（StartState/UpdateState）から呼ばれる。
        /// </summary>
        public virtual void Enter(T state_manager_data) { }
        public virtual void Update(T state_manager_data) { }
        public virtual void Exit(T state_manager_data) { }

        /// <summary>
        /// 非同期版ライフサイクル。アセットのロード待ちなど、await が必要なStateは
        /// こちらをoverrideする。デフォルトでは同期版を呼び出すだけなので、
        /// 同期版だけをoverrideしたStateもStateControlの非同期APIから問題なく呼び出せる。
        /// （Combinedモードでは UseEnterAsync 等がfalseの時はこのメソッド自体が
        /// 呼ばれないため、二重実行にはならない）
        /// ct は StateControl 側で管理される CancellationToken で、
        /// 状態遷移が起きた時点で自動的にキャンセルされる。
        /// </summary>
        public virtual async UniTask EnterAsync(T state_manager_data, CancellationToken ct)
        {
            Enter(state_manager_data);
            await UniTask.CompletedTask;
        }
        public virtual async UniTask UpdateAsync(T state_manager_data, CancellationToken ct)
        {
            Update(state_manager_data);
            await UniTask.CompletedTask;
        }
        public virtual async UniTask ExitAsync(T state_manager_data, CancellationToken ct)
        {
            Exit(state_manager_data);
            await UniTask.CompletedTask;
        }

        public virtual E BranchNextState(T state_manager_data)
        {
            return default;
        }

    }
}
