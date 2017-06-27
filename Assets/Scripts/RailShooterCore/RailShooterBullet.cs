using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RailShooter.Utils;

namespace RailShooter.Assets
{
    public class RailShooterBullet : MonoBehaviour
    {
        [SerializeField]
        private CamerasAndInputsManager m_camInputManager;

        public float life = 5f;
        [SerializeField]
        private Transform m_pool;
        [SerializeField]
        private PKFxFX m_FX;
        [SerializeField]
        private PKFxFX m_explosionFX;

        [SerializeField]
        private bool m_autoGuided;
        [SerializeField]
        private bool m_directionStatus;

        [SerializeField]
        private float m_speed;
        private InteractiveItem m_interactiveItem;

        private Transform m_player;
        private Rigidbody m_rigidbody;
        public Rigidbody Rigidbody
        {
            get
            {
                return m_rigidbody;
            }
            set
            {
                m_rigidbody = value;
            }
        }

        private Vector3 m_direction;
        public Vector3 Direction
        {
            get
            {
                return m_direction;
            }
            set
            {
                m_direction = value;
            }
        }

        void Awake()
        {
            m_interactiveItem = GetComponent<InteractiveItem>();
            m_rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Camera camera = m_camInputManager.CurrentCamera;
            for (int i = 0; i < camera.transform.childCount; i++)
            {
                Transform child = camera.transform.GetChild(i);
                if (child.tag == "Player")
                    m_player = child;
            }
        }

        private void OnEnable()
        {
            m_interactiveItem.OnDown += HandleDown;
            m_interactiveItem.OnDownLeft += HandleDown;

            m_rigidbody.velocity = Vector3.zero;
            if (!m_autoGuided)
                Invoke("Remove", life);
        }

        private void OnDisable()
        {
            m_interactiveItem.OnDown -= HandleDown;
            m_interactiveItem.OnDownLeft -= HandleDown;
            CancelInvoke();
        }

        public void Remove()
        {
            transform.rotation = Quaternion.identity;
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;

            transform.parent = m_pool;
            transform.position = m_pool.position;

            m_FX.StopEffect();
            gameObject.SetActive(false);
        }

        private void HandleDown()
        {
            m_explosionFX.StartEffect();
            Remove();
        }

        void Update()
        {
            if (m_FX.IsPlaying() == false)
                m_FX.StartEffect();

            if(m_directionStatus)
                m_FX.SetAttribute(new PKFxManager.Attribute("Direction", m_direction));
            if(m_autoGuided)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_player.position, m_speed * Time.deltaTime);
                transform.LookAt(m_player.position);
            }
        }
    }
}
