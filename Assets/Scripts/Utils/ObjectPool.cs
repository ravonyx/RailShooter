using System.Collections.Generic;
using UnityEngine;
using Assets.RailShooter;

namespace RailShooter.Utils
{
    // This is a simple object pooling script that
    // allows for random variation in prefabs.
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject m_Prefab;            
        [SerializeField] private int m_NumberInPool;                

        private List<ShooterBullet> m_Pool = new List<ShooterBullet> ();  

        private void Awake ()
        {
            for (int i = 0; i < m_NumberInPool; i++)
            {
                AddToPool ();
            }
        }

        private void AddToPool ()
        {
            GameObject instance = Instantiate(m_Prefab);
            instance.transform.parent = transform;
            instance.gameObject.SetActive (false);

            m_Pool.Add(instance.GetComponent<ShooterBullet>());
        }


        public ShooterBullet GetGameObjectFromPool ()
        {
            //resize if none left in pool
            if (m_Pool.Count == 0)
                AddToPool ();
            
            ShooterBullet ret = m_Pool[0];
            m_Pool.RemoveAt(0);

            ret.gameObject.SetActive (true);
            ret.transform.parent = null;

            return ret;
        }


        public void ReturnGameObjectToPool (ShooterBullet go)
        {
            m_Pool.Add (go);

            go.gameObject.SetActive (false);
            go.transform.parent = transform;
        }
    }
}