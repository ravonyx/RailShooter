using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RailShooter.Utils;

namespace RailShooter.Assets
{
    public class RailShooterEnemy : MonoBehaviour
    {
        [SerializeField]
        private PathWalker m_pathWalker;

        void Start()
        {
            StartCoroutine(m_pathWalker.PlayUpdateBackward());
        }

    }
}