using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RailShooter.Utils;

namespace Assets.RailShooter
{
    public class RailShooterEntityAttack : MonoBehaviour
    {
        [SerializeField]
        private Transform m_Player;
        [SerializeField]
        private float m_DistanceToAttack;
        [SerializeField]
        private PKFxFX m_FX;
        [SerializeField]
        private ObjectPool m_ProjectilesPool;

        [SerializeField]
        private float m_FireRate;
        [SerializeField]
        private float m_Speed;

        private float nextFire = 0.0F;

        void Update()
        {
            float distance = Vector3.Distance(transform.position, m_Player.position);
            if(Time.time > nextFire && distance <= m_DistanceToAttack)
            {
                nextFire = Time.time + m_FireRate;
                Fire();
            }
        }

        private void Fire()
        {
           // m_GunAudio.Play();
            ShooterBullet projectile = m_ProjectilesPool.GetGameObjectFromPool();
            Vector3 direction = (m_Player.position - transform.position).normalized;

            Debug.DrawRay(transform.position, direction, Color.green, 5.0f);

            projectile.transform.parent = transform;
            projectile.transform.position = transform.position;
            projectile.transform.parent = null;

            projectile.Rigidbody.AddForce(direction * m_Speed);
        }
    }

}