using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.RailShooter
{
    public class ShooterBullet : MonoBehaviour
    {
        public float life = 5f;
        [SerializeField]
        private Transform m_pool;
        [SerializeField]
        private PKFxFX m_FX;

        private Rigidbody m_Rigidbody;
        public Rigidbody Rigidbody
        {
            get
            {
                return m_Rigidbody;
            }
            set
            {
                m_Rigidbody = value;
            }
        }

        private Vector3 m_Direction;
        public Vector3 Direction
        {
            get
            {
                return m_Direction;
            }
            set
            {
                m_Direction = value;
            }
        }

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            m_Rigidbody.velocity = Vector3.zero;
            Invoke("Remove", life);
        }

        private void OnDisable()
        {
            CancelInvoke();
        }

        private void Remove()
        {
            transform.rotation = Quaternion.identity;
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;
            transform.parent = m_pool;
            transform.position = m_pool.position;
            m_FX.StopEffect();
            gameObject.SetActive(false);
        }

        void Update()
        {
            m_FX.SetAttribute(new PKFxManager.Attribute("Direction", m_Direction));
        }
    }
}
