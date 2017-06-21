using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.RailShooter;
using UnityEngine.UI;
using RailShooter.Utils;

public class RailShooterPlayer : MonoBehaviour
{
    [SerializeField]
    private float m_lifeMax;
    [SerializeField]
    private float m_lifeToRemove;

    [SerializeField]
    private float m_timeRegen;
    [SerializeField]
    private float m_timeLose;

    [SerializeField]
    private PKFxFX m_FXHealth;
    private float m_currentLife;

    private Image m_lifeBar;
    private bool m_ending;

    [SerializeField]
    private Image m_lifeBarContainer;
    [SerializeField]
    private Image m_lifeBarContainerVR;

    [SerializeField]
    private RailShooterController m_railShooterController;
    [SerializeField]
    private CamerasAndInputsManager m_camInputManager;

    void Start()
    {
        m_ending = false;
        m_currentLife = m_lifeMax;
        if (m_camInputManager.CurrentInputName == "Touch")
            m_lifeBar = m_lifeBarContainerVR;
        else
            m_lifeBar = m_lifeBarContainer;
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

    void Update()
    {
        if (m_currentLife <= 0 && m_ending == false)
        {
            m_ending = true;
            StartCoroutine(m_railShooterController.GameOver());
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
