
using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using AddressableSystem;
using GameCore.SaveSystem;
using GameCore.Enums;
using System.Linq;
using System.Threading;

namespace GameCore.Sound
{
    public partial class SoundCore : BaseSingleton<SoundCore>
    {
        // =============================================================
        // 爆速キャッシュ（LINQ完全排除、Dictionary 1段）
        // =============================================================
        private readonly Dictionary<(SoundGroup group, SoundID id), AudioClip> clipCache = new();
        private readonly Dictionary<(SoundGroup group, SoundID id), float> volumeCache = new();
        private readonly Dictionary<(SoundGroup group, SoundID id), SoundType> typeCache = new();
        private readonly HashSet<(SoundGroup group, SoundID id)> loadingKeys = new();

        // Addressable本体は (group, id) 単位で保持。
        // グループ一括ロード／個別ロード／SubGroupロードのいずれでも同じ辞書を使い、
        // 専用の管理は持たない（Unloadは対象のkeyを絞り込んで、このDictionaryから解放するだけ）。
        private readonly Dictionary<(SoundGroup group, SoundID id), AddressableData<AudioClip>> soundAddressables = new();

        private SoundDatabase database;

        // =============================================================
        // AudioSource管理（プールは循環インデックスで最速）
        // =============================================================
        private AudioSource bgmSource;
        private AudioSource crossFadeTempSource;
        private readonly List<AudioSource> sePool = new();
        private const int PoolSize = 30;
        private int poolIndex = 0;

        private bool isCrossFading = false;

        // =============================================================
        // キャンセルトークン（シーン遷移時の完全停止＆ゾンビタスク防止）
        // =============================================================
        private CancellationToken destroyToken;
        private CancellationTokenSource manualCancelSource = new();
        private CancellationToken combinedToken;

        public bool IsLoadDatabase { get; private set; }

        public override void AwakeSingleton()
        {
            base.AwakeSingleton();
            instance = this;
            DontDestroyOnLoad(gameObject);

            destroyToken = this.GetCancellationTokenOnDestroy();
            manualCancelSource = new CancellationTokenSource();
            combinedToken = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, manualCancelSource.Token).Token;

            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;

            for (int i = 0; i < PoolSize; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                sePool.Add(source);
            }

            LoadDatabaseAsync().Forget();
        }

        private async UniTask LoadDatabaseAsync()
        {
            string path = SupportFiles.ADDRESSABLE_CHECK ? SupportFiles.ALL_SOUND_BIN_FILE : SupportFiles.ALL_SOUND_BIN;
            database =  await SoundBinaryReader.LoadSoundDatabaseFromBinaryAsync(path,SupportFiles.ADDRESSABLE_CHECK);
            if (database == null)
                Debug.LogError("[SoundCore] Failed to load SoundDatabase.");
            await UniTask.CompletedTask.AttachExternalCancellation(destroyToken);

            IsLoadDatabase = true;
        }

        // =============================================================
        // シーン遷移時に必ず呼ぶ！！（これが全てを守る）
        // =============================================================
        public void StopAllAndCancelAllTasks()
        {
            manualCancelSource.Cancel();
            manualCancelSource.Dispose();
            manualCancelSource = new CancellationTokenSource();
            combinedToken = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, manualCancelSource.Token).Token;

            foreach (var source in sePool)
            {
                if (source != null)
                {
                    if (source.isPlaying) source.Stop();
                    source.clip = null;
                }
            }

            if (bgmSource != null)
            {
                if (bgmSource.isPlaying) bgmSource.Stop();
                bgmSource.clip = null;
            }

            if (crossFadeTempSource != null)
            {
                crossFadeTempSource.Stop();
                Destroy(crossFadeTempSource);
                crossFadeTempSource = null;
            }

            isCrossFading = false;
        }

        /// <summary>
        /// その音のBaseVolume（データベースに登録された元の音量）を取得
        /// 存在しない場合は1.0fを返す
        /// </summary>
        public float GetSoundVolume(SoundGroup group, SoundID id)
        {
            var key = (group, id);
            return volumeCache.TryGetValue(key, out var volume) ? volume : 1f;
        }

        /// <summary>
        /// ロード済みのAudioClipを直接取得（UIプレビューや特殊処理用）
        /// ロードされてなければnull
        /// </summary>
        public AudioClip GetSoundClip(SoundGroup group, SoundID id)
        {
            var key = (group, id);
            clipCache.TryGetValue(key, out var clip);
            return clip;
        }

        /// <summary>
        /// そのサウンドがSEかBGMかを取得（ロード前でも判定可能にするなら別途キャッシュ必要）
        /// </summary>
        public SoundType GetSoundType(SoundGroup group, SoundID id)
        {
            var key = (group, id);
            return typeCache.TryGetValue(key, out var type) ? type : SoundType.SE;
        }

        // =============================================================
        // グループロード／アンロード
        // =============================================================
        public void LoadGroup(SoundGroup group, GroupCategory category, Action onCompleted = null)
            => LoadGroupAsync(group, category, onCompleted).Forget();

        public async UniTask LoadGroupAsync(SoundGroup group, GroupCategory category, Action onCompleted)
        {
            while (!IsLoadDatabase)
                await UniTask.Yield(combinedToken);

            var groupData = database.GroupedSoundsList.FirstOrDefault(x => x.Group == group);
            if (groupData == null) { onCompleted?.Invoke(); return; }

            var tasks = new List<UniTask>();

            foreach (var sound in groupData.Sounds)
            {
                var key = (group, sound.SoundID);
                if (clipCache.ContainsKey(key) || loadingKeys.Contains(key)) continue;

                loadingKeys.Add(key);

                var addressable = new AddressableData<AudioClip>(category, AssetCategory.Audio, sound.AddressablePath);

                tasks.Add(addressable.LoadAsync(clip =>
                {
                    if (addressable.IsLoadedAndSetup)
                    {
                        clipCache[key] = clip;
                        volumeCache[key] = sound.BaseVolume;
                        typeCache[key] = sound.Type;
                        soundAddressables[key] = addressable;
                    }
                    loadingKeys.Remove(key);
                }, ex =>
                {
                    Debug.LogError($"[SoundCore] Load failed {sound.SoundID}: {ex.Message}");
                    loadingKeys.Remove(key);
                }).AttachExternalCancellation(combinedToken));
            }

            await UniTask.WhenAll(tasks);
            onCompleted?.Invoke();
        }

        public void UnloadGroup(SoundGroup group, GroupCategory category, Action onCompleted = null)
            => UnloadGroupAsync(group, onCompleted).Forget();

        private async UniTask UnloadGroupAsync(SoundGroup group, Action onCompleted)
        {
            var keysToRemove = new List<(SoundGroup, SoundID)>();
            foreach (var kv in clipCache)
                if (kv.Key.group == group) keysToRemove.Add(kv.Key);

            foreach (var key in keysToRemove)
            {
                if (soundAddressables.TryGetValue(key, out var addressable))
                {
                    addressable.ReleaseAndUntrack();
                    soundAddressables.Remove(key);
                }
                clipCache.Remove(key);
                volumeCache.Remove(key);
                typeCache.Remove(key);
            }

            onCompleted?.Invoke();
            await UniTask.CompletedTask;
        }

        // =============================================================
        // 個別ID単位のロード／アンロード
        // グループロードと同じ soundAddressables / clipCache 等をそのまま使う。
        // =============================================================
        internal void LoadSingle(SoundGroup group, SoundID id, GroupCategory category, Action onCompleted = null)
            => LoadSingleAsync(group, id, category, onCompleted).Forget();

        internal async UniTask LoadSingleAsync(SoundGroup group, SoundID id, GroupCategory category, Action onCompleted = null)
        {
            while (!IsLoadDatabase)
                await UniTask.Yield(combinedToken);

            var key = (group, id);
            if (clipCache.ContainsKey(key) || loadingKeys.Contains(key))
            {
                onCompleted?.Invoke();
                return;
            }

            var groupData = database.GroupedSoundsList.FirstOrDefault(x => x.Group == group);
            var sound = groupData?.Sounds.FirstOrDefault(s => s.SoundID == id);
            if (sound == null) { onCompleted?.Invoke(); return; }

            loadingKeys.Add(key);
            var addressable = new AddressableData<AudioClip>(category, AssetCategory.Audio, sound.AddressablePath);
            await addressable.LoadAsync(clip =>
            {
                if (addressable.IsLoadedAndSetup)
                {
                    clipCache[key] = clip;
                    volumeCache[key] = sound.BaseVolume;
                    typeCache[key] = sound.Type;
                    soundAddressables[key] = addressable;
                }
                loadingKeys.Remove(key);
            }, ex =>
            {
                Debug.LogError($"[SoundCore] Load failed (single) {id}: {ex.Message}");
                loadingKeys.Remove(key);
            }).AttachExternalCancellation(combinedToken);

            onCompleted?.Invoke();
        }

        public void UnloadSingle(SoundGroup group, SoundID id, Action onCompleted = null)
            => UnloadSingleAsync(group, id, onCompleted).Forget();

        public async UniTask UnloadSingleAsync(SoundGroup group, SoundID id, Action onCompleted = null)
        {
            var key = (group, id);
            if (soundAddressables.TryGetValue(key, out var addressable))
            {
                addressable.ReleaseAndUntrack();
                soundAddressables.Remove(key);
                clipCache.Remove(key);
                volumeCache.Remove(key);
                typeCache.Remove(key);
            }
            onCompleted?.Invoke();
            await UniTask.CompletedTask;
        }

        // =============================================================
        // SubGroup単位のロード／アンロード（内部実装）
        // 公開APIは SoundCoreSubGroups.cs 側で、グループごとの
        // 専用enum（例: Sound_EnemyID）を受け取るオーバーロードとして生成される。
        // どのサウンドがどのSubGroupに属するかは SoundData.SubGroupId から都度判定する。
        // グループロードと同じ soundAddressables / clipCache 等をそのまま使い、
        // 専用の管理は持たない。
        // =============================================================
        internal void LoadSubGroupInternal(SoundGroup group, int subGroupId, GroupCategory category, Action onCompleted = null)
            => LoadSubGroupInternalAsync(group, subGroupId, category, onCompleted).Forget();

        internal async UniTask LoadSubGroupInternalAsync(SoundGroup group, int subGroupId, GroupCategory category, Action onCompleted = null)
        {
            while (!IsLoadDatabase)
                await UniTask.Yield(combinedToken);

            var groupData = database.GroupedSoundsList.FirstOrDefault(x => x.Group == group);
            if (groupData == null) { onCompleted?.Invoke(); return; }

            var tasks = new List<UniTask>();
            foreach (var sound in groupData.Sounds)
            {
                if (sound.SubGroupId != subGroupId) continue;
                var key = (group, sound.SoundID);
                if (clipCache.ContainsKey(key) || loadingKeys.Contains(key)) continue;

                loadingKeys.Add(key);
                var addressable = new AddressableData<AudioClip>(category, AssetCategory.Audio, sound.AddressablePath);

                tasks.Add(addressable.LoadAsync(clip =>
                {
                    if (addressable.IsLoadedAndSetup)
                    {
                        clipCache[key] = clip;
                        volumeCache[key] = sound.BaseVolume;
                        typeCache[key] = sound.Type;
                        soundAddressables[key] = addressable;
                    }
                    loadingKeys.Remove(key);
                }, ex =>
                {
                    Debug.LogError($"[SoundCore] Load failed (subgroup) {sound.SoundID}: {ex.Message}");
                    loadingKeys.Remove(key);
                }).AttachExternalCancellation(combinedToken));
            }

            await UniTask.WhenAll(tasks);
            onCompleted?.Invoke();
        }

        internal void UnloadSubGroupInternal(SoundGroup group, int subGroupId, Action onCompleted = null)
            => UnloadSubGroupInternalAsync(group, subGroupId, onCompleted).Forget();

        internal async UniTask UnloadSubGroupInternalAsync(SoundGroup group, int subGroupId, Action onCompleted = null)
        {
            if (database != null)
            {
                var groupData = database.GroupedSoundsList.FirstOrDefault(x => x.Group == group);
                if (groupData != null)
                {
                    foreach (var sound in groupData.Sounds)
                    {
                        if (sound.SubGroupId != subGroupId) continue;
                        var key = (group, sound.SoundID);
                        if (soundAddressables.TryGetValue(key, out var addressable))
                        {
                            addressable.ReleaseAndUntrack();
                            soundAddressables.Remove(key);
                        }
                        clipCache.Remove(key);
                        volumeCache.Remove(key);
                        typeCache.Remove(key);
                    }
                }
            }

            onCompleted?.Invoke();
            await UniTask.CompletedTask;
        }

        // =============================================================
        // SE再生（最速・安全）
        // =============================================================
        public void PlaySE(SoundGroup group, SoundID id, float volume = 1f, bool is3D = false, Vector3 position = default, float maxDistance = 500f)
            => PlaySEAsync(group, id, volume, is3D, position, maxDistance).Forget();

        private async UniTask PlaySEAsync(SoundGroup group, SoundID id, float volume, bool is3D, Vector3 position, float maxDistance)
        {
            var key = (group, id);

            if (!clipCache.TryGetValue(key, out var clip) ||
                !volumeCache.TryGetValue(key, out var baseVolume) ||
                !typeCache.TryGetValue(key, out var type) || type != SoundType.SE)
                return;

            var source = GetPooledSourceFast();
            if (source == null) return;

            source.clip = clip;
            source.volume = baseVolume * volume * SaveManagerCore.instance.SystemSettings.seVolume;
            source.loop = false;
            source.spatialBlend = is3D ? 1f : 0f;
            source.maxDistance = maxDistance;
            if (is3D) source.transform.position = position;

            source.Play();

            try
            {
                await UniTask.WaitUntil(() => !source.isPlaying, cancellationToken: combinedToken);
            }
            catch (OperationCanceledException) { }
            finally
            {
                ResetSource(source);
            }
        }

        private AudioSource GetPooledSourceFast()
        {
            int startIndex = poolIndex;
            do
            {
                var source = sePool[poolIndex];
                if (!source.isPlaying)
                {
                    ResetSource(source);
                    poolIndex = (poolIndex + 1) % PoolSize;
                    return source;
                }
                poolIndex = (poolIndex + 1) % PoolSize;
            } while (poolIndex != startIndex);

            var victim = sePool[0];
            ResetSource(victim);
            return victim;
        }

        // =============================================================
        // BGM再生・フェード・クロスフェード
        // =============================================================
        public void PlayBGM(SoundGroup group, SoundID id, float volume = 1f, float fadeTime = 0f)
            => PlayBGMAsync(group, id, volume, fadeTime).Forget();

        private async UniTask PlayBGMAsync(SoundGroup group, SoundID id, float volume, float fadeTime)
        {
            var key = (group, id);
            if (!clipCache.TryGetValue(key, out var clip) ||
                !volumeCache.TryGetValue(key, out var baseVolume) ||
                !typeCache.TryGetValue(key, out var type) || type != SoundType.BGM)
                return;

            if (bgmSource.isPlaying && fadeTime > 0f)
                await FadeOutAsync(fadeTime);

            bgmSource.clip = clip;
            bgmSource.volume = 0f;
            bgmSource.Play();

            float targetVolume = baseVolume * volume * SaveManagerCore.instance.SystemSettings.bgmVolume;
            if (fadeTime > 0f)
                await FadeInAsync(targetVolume, fadeTime);
            else
                bgmSource.volume = targetVolume;
        }

        public void CrossFadeBGM(SoundGroup group, SoundID id, float volume = 1f, float fadeTime = 1f)
            => CrossFadeBGMAsync(group, id, volume, fadeTime).Forget();

        private async UniTask CrossFadeBGMAsync(SoundGroup group, SoundID id, float volume, float fadeTime)
        {
            var key = (group, id);
            if (!clipCache.TryGetValue(key, out var clip) ||
                !volumeCache.TryGetValue(key, out var baseVolume) ||
                !typeCache.TryGetValue(key, out var type) || type != SoundType.BGM)
                return;

            isCrossFading = true;

            crossFadeTempSource = gameObject.AddComponent<AudioSource>();
            crossFadeTempSource.loop = true;
            crossFadeTempSource.clip = clip;
            crossFadeTempSource.volume = 0f;
            crossFadeTempSource.Play();

            float startVolume = bgmSource.volume;
            float targetVolume = baseVolume * volume * SaveManagerCore.instance.SystemSettings.bgmVolume;

            float timer = 0f;
            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / fadeTime);
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, t);
                crossFadeTempSource.volume = Mathf.Lerp(0f, targetVolume, t);
                await UniTask.Yield(combinedToken);
            }

            bgmSource.Stop();
            Destroy(bgmSource);
            bgmSource = crossFadeTempSource;
            bgmSource.volume = targetVolume;
            crossFadeTempSource = null;
            isCrossFading = false;
        }

        private async UniTask FadeOutAsync(float fadeTime, Action onCompleted = null)
        {
            if (!bgmSource.isPlaying) { onCompleted?.Invoke(); return; }

            float startVolume = bgmSource.volume;
            float timer = 0f;

            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeTime);
                if (timer >= fadeTime) break;
                await UniTask.Yield(combinedToken);
            }

            bgmSource.volume = 0f;
            bgmSource.Stop();
            bgmSource.clip = null;
            onCompleted?.Invoke();
        }

        private async UniTask FadeInAsync(float targetVolume, float fadeTime, Action onCompleted = null)
        {
            float timer = 0f;
            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeTime);
                if (timer >= fadeTime) break;
                await UniTask.Yield(combinedToken);
            }
            bgmSource.volume = targetVolume;
            onCompleted?.Invoke();
        }

        // =============================================================
        // ユーティリティ
        // =============================================================
        private void ResetSource(AudioSource source)
        {
            if (source == null) return;
            source.Stop();
            source.clip = null;
            source.volume = 0f;
            source.spatialBlend = 0f;
        }

        public void SetSystemBGMVolume()
        {
            if (bgmSource != null && bgmSource.isPlaying)
                bgmSource.volume = bgmSource.volume / SaveManagerCore.instance.SystemSettings.bgmVolume * SaveManagerCore.instance.SystemSettings.bgmVolume;
        }

        public void SetSystemSEVolume()
        {
            float vol = SaveManagerCore.instance.SystemSettings.seVolume;
            foreach (var s in sePool) s.volume = vol;
        }

        private void Update()
        {
            if (bgmSource == null && !isCrossFading)
                bgmSource = gameObject.AddComponent<AudioSource>();

            sePool.RemoveAll(s => s == null);
        }

        private void OnDestroy()
        {
            StopAllAndCancelAllTasks();

            foreach (var addr in soundAddressables.Values)
                addr.Release();

            clipCache.Clear();
            volumeCache.Clear();
            typeCache.Clear();
            soundAddressables.Clear();
        }
    }
}


