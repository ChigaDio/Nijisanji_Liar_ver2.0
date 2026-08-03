using NUnit.Framework;
using UnityEngine;

namespace GameCore
{
    public class BaseSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    // まず、既にシーン内にあるかチェック
                    instance =  GameObject.FindAnyObjectByType<T>(FindObjectsInactive.Exclude);

                    if (instance == null)
                    {
                        // まだなければ新しく生成
                        GameObject instanceObj = new GameObject();
                        instance = instanceObj.AddComponent<T>();
                        instanceObj.name = typeof(T).Name;
                    }
                }

                return instance;
            }
        }

        public virtual void AwakeSingleton()
        {
            if (instance == null)
            {
                instance = gameObject.GetComponent<T>();
            }
        }

        public void Awake()
        {
            AwakeSingleton();
        }

    }
}
