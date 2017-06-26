using UnityEngine;
using UnityEngine.UI;
using RailShooter.Utils;

namespace RailShooter.Assets
{
    // This script displays the player's score during the
    // shooter scenes.
    public class RailShooterScore : MonoBehaviour
    {
        [SerializeField] private Text m_ScoreText;


        private void Update()
        {
            m_ScoreText.text = SessionData.Score.ToString();
        }
    }
}