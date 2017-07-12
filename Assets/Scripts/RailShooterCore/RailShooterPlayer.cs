using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RailShooter.Utils;
using RailShooter.Assets;

public class RailShooterPlayer : MonoBehaviour
{
    [SerializeField]
    private float m_lifeMax;
    [SerializeField]
    private float m_lifeToRemoveProjectile;
    [SerializeField]
    private float m_lifeToRemoveFlyEnemy;

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

    private AudioSource m_audio;

    void Start()
    {
        m_ending = false;
        m_currentLife = m_lifeMax;
        if (m_camInputManager.CurrentInputName == "Touch")
            m_lifeBar = m_lifeBarContainerVR;
        else
            m_lifeBar = m_lifeBarContainer;

        m_audio = GetComponent<AudioSource>();
    }

    public IEnumerator EvolveLife()
    {
        float timer = 0f;
        m_FXHealth.StartEffect();
        while (timer <= m_timeRegen)
        {
            m_currentLife += 3.0f;
            if (m_currentLife > m_lifeMax)
                m_currentLife = m_lifeMax;
            float lifeValue = m_currentLife / m_lifeMax;
            m_lifeBar.fillAmount = lifeValue;

            Color color = Color.white;
            if (m_currentLife <= m_lifeMax && m_currentLife >= ((m_lifeMax / 3) * 2))
                ColorUtility.TryParseHtmlString("#00FF17FF", out color);
            else if (m_currentLife < ((m_lifeMax / 3) * 2) && m_currentLife >= ((m_lifeMax / 3)))
                ColorUtility.TryParseHtmlString("#FF8E36FF", out color);
            else
                color = Color.red;

            m_lifeBar.color = color;

            timer += 1;
            yield return new WaitForSeconds(1.0f);
        }
    }

    void Update()
    {
        if (m_currentLife <= 0 && m_ending == false)
        {
            m_ending = true;
            StartCoroutine(m_railShooterController.GameOverPhase());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        RailShooterBullet bullet = other.GetComponent<RailShooterBullet>();
        if(bullet)
            bullet.Remove();

        float m_lifeToRemove = 0;
        if (other.tag == "Projectile")
            m_lifeToRemove = m_lifeToRemoveProjectile;
        else if(other.tag == "FlyEnemy")
            m_lifeToRemove = m_lifeToRemoveFlyEnemy;

        m_currentLife -= m_lifeToRemove;
        m_currentLife = m_currentLife > 0 ? m_currentLife : 0;
        float lifeValue = m_currentLife / m_lifeMax;

        SessionData.ResetMultiplicateur();

        Color color = Color.white;
        if (m_currentLife <= m_lifeMax && m_currentLife >= ((m_lifeMax / 3) * 2))
            ColorUtility.TryParseHtmlString("#00FF17FF", out color);
        else if (m_currentLife < ((m_lifeMax / 3) * 2) && m_currentLife >= ((m_lifeMax / 3)))
            ColorUtility.TryParseHtmlString("#FF8E36FF", out color);
        else
            color = Color.red;

        if (m_audio)
            m_audio.Play();

        m_lifeBar.color = color;
        m_lifeBar.fillAmount = lifeValue;
    }
}
