using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.RailShooter
{
    public class ShooterBullet : MonoBehaviour
    {
        public float life = 5f;
        public ParticleSystem bulletHit;
        public Transform parent;

        public Rigidbody Rb;
      //  private TrailRenderer trailRenderer;
        private MeshRenderer mesh;

        void Awake()
        {
            Rb = GetComponent<Rigidbody>();
         //   trailRenderer = GetComponent<TrailRenderer>();
            mesh = GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {
            mesh.enabled = true;
      //      trailRenderer.enabled = true;
            Rb.velocity = Vector3.zero;
            Invoke("Remove", life);
        }

        private void OnDisable()
        {
            CancelInvoke();
        }

        private void Remove()
        {
            transform.rotation = Quaternion.identity;
         //   trailRenderer.enabled = false;
            mesh.enabled = false;
            Rb.velocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
            transform.parent = parent;
            transform.position = parent.position;
            gameObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision other)
        {
            Remove();
        }
    }
}