using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RailShooter.Utils;

namespace Assets.RailShooter
{
    public class RailShooterEntityAttack : MonoBehaviour
    {
        [SerializeField]
        private CamerasAndInputsManager m_camInputManager;
        [SerializeField]
        private float m_distanceToAttack;
        [SerializeField]
        private ObjectPool m_projectilesPool;

        [SerializeField]
        private float m_fireRate;

        private float m_nextFire = 0.0F;
        private Renderer m_renderer;
        private Transform m_player;

        void Start()
        {
            m_renderer = GetComponent<Renderer>();
            Camera camera = m_camInputManager.CurrentCamera;

            for (int i = 0; i < camera.transform.childCount; i++)
            {
                Transform child = camera.transform.GetChild(i);
                if (child.tag == "Player")
                    m_player = child;
            }
        
        }
        void Update()
        {
            if (!m_renderer.enabled)
                return;
            float distance = Vector3.Distance(m_renderer.bounds.center, m_player.position);
            if (Time.time > m_nextFire && distance <= m_distanceToAttack)
            {
                Debug.Log("Fire");
                m_nextFire = Time.time + m_fireRate;
                Fire();
            }
        }

        private void Fire()
        {
            RailShooterBullet projectile = m_projectilesPool.GetGameObjectFromPool();
            Vector3 direction = (m_player.position - m_renderer.bounds.center).normalized;

            Debug.DrawRay(m_renderer.bounds.center, direction, Color.green, 5.0f);

            projectile.transform.parent = transform;
            projectile.transform.position = m_renderer.bounds.center;
            projectile.transform.parent = null;

            projectile.Rigidbody.AddForce(direction * 10.0f);
        }
    }
}