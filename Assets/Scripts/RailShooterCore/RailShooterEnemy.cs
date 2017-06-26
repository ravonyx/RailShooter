using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RailShooter.Utils;

namespace Assets.RailShooter
{
    public class RailShooterEnemy : MonoBehaviour
    {
        [SerializeField]
        private PathWalker m_pathWalker;

        private InteractiveItem m_interactiveItem;

        void Start()
        {
            StartCoroutine(m_pathWalker.PlayUpdateBackward());
        }
    }
}