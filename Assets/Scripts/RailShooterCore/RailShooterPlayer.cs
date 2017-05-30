using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.RailShooter;
using UnityEngine.UI;

public class RailShooterPlayer : MonoBehaviour
{
    [SerializeField]
    private float m_lifeMax;
    [SerializeField]
    private float m_lifeToRemove;
    [SerializeField]
    private Image m_lifeBar;
    [SerializeField]
    private float m_timeRegen;
    [SerializeField]
    private float m_timeLose;

    [SerializeField]
    private PKFxFX m_FXHealth;
    private float m_currentLife;

    void Start()
    {
        m_currentLife = m_lifeMax;
    }

    public IEnumerator EvolveLife(bool increase)
    {
        float timer = 0f;
        m_FXHealth.StartEffect();
        while (timer <= m_timeRegen)
        {
            if(increase)
                m_currentLife += 1.0f;
            else
                m_currentLife -= 1.0f;

            float lifeValue = m_currentLife / m_lifeMax;
            m_lifeBar.fillAmount = lifeValue;

            timer += 1;
            yield return new WaitForSeconds(1.0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        m_currentLife -= m_lifeToRemove;
        m_currentLife = m_currentLife > 0 ? m_currentLife : 0;
        float lifeValue = m_currentLife / m_lifeMax;

        m_lifeBar.fillAmount = lifeValue;
        other.GetComponent<RailShooterBullet>().Remove();
    }
}
