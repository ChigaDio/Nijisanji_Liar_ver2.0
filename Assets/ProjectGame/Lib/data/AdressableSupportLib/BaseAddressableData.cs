
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AddressableSystem
{
    /// <summary>
    /// 管理用の非ジェネリック基底。
    /// ロードメソッド自体は派生側に実装させる（T 型のラムダを受け取る）。
    /// </summary>
    
    public abstract class BaseAddressableData
    {
        protected bool isArray;
        public bool IsArray => isArray;
        protected bool isSetup;
        protected bool isLoaded;
        protected bool isAutoRelease;
        protected bool isUsed;
        public bool isCopy = false;

        protected UnityEngine.Object addressableObject;
        protected UnityEngine.Object[] addressableArray;

        public string path { get; protected set; }
        public GroupCategory groupCategory { get; protected set; }
        public AssetCategory assetCategory { get; protected set; }

        public Scene? SceneLink { get; set; }

        protected BaseAddressableData(GroupCategory group, AssetCategory category,string path,Scene? sceneLink = null)
        {
            SceneLink = sceneLink;
            groupCategory = group;
            assetCategory = category;
            this.path = path;
            AddressableDataCore.Instance.AddAddressableData(group, category, this, sceneLink);
        }

        public bool IsLoadedAndSetup => isSetup && isLoaded;
        public bool IsAutoRelease => isAutoRelease;
        public UnityEngine.Object GetAddressableObject() => addressableObject;
        public UnityEngine.Object[] GetAddressableArray() => addressableArray;
        public int GetArrayCount() => addressableArray?.Length ?? 0;

        public void EnableAutoRelease() => isAutoRelease = true;
        public void MarkAsUsed() => isUsed = true;

        /// <summary>
        /// 派生で実装する（型付きの Load/LoadArray を実装すること）。
        /// </summary>
        public abstract void Release();

        /// <summary>
        /// Single / SubGroup 単位の解放用。
        /// Release() に加えて AddressableDataCore の追跡リスト（AddressableDataContainer）からも
        /// 自分自身を除去する。これを呼ばないと、同じ path で再ロードした際に
        /// 「解放済みだが追跡リストに残ったままの古いエントリ」が Find() にヒットしてしまい、
        /// 新しいインスタンスが isCopy = true にされ、実体が二度とロードされない古いデータの
        /// 完了待ちで無限ループ（デッドロック）する不具合につながる。
        ///
        /// 注意: Group / Category 単位の一括解放（ReleaseGroup / ReleaseCategory 等）では
        /// 使用しないこと。呼び出し元がリストを foreach しながら Clear() する処理と衝突し、
        /// コレクション変更例外（InvalidOperationException）を誘発する。
        /// </summary>
        public void ReleaseAndUntrack()
        {
            Release();
            AddressableDataCore.Instance.RemoveData(this);
        }
    }
}




    