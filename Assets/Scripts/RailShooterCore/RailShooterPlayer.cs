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
        float m_lifeToRemove = 0;
        if (other.tag == "Projectile")
            m_lifeToRemove = m_lifeToRemoveProjectile;
        else if(other.tag == "FlyEnemy")
            m_lifeToRemove = m_lifeToRemoveFlyEnemy;

        m_currentLife -= m_lifeToRemove;
        m_currentLife = m_currentLife > 0 ? m_currentLife : 0;
        float lifeValue = m_currentLife / m_lifeMax;

        Color color = Color.white;
        if (m_currentLife <= m_lifeMax && m_currentLife >= ((m_lifeMax / 3) * 2))
            ColorUtility.TryParseHtmlString("#00FF17FF", out color);
        else if (m_currentLife < ((m_lifeMax / 3) * 2) && m_currentLife >= ((m_lifeMax / 3)))
            ColorUtility.TryParseHtmlString("#FF8E36FF", out color);
        else
            color = Color.red;

        m_lifeBar.color = color;
        m_lifeBar.fillAmount = lifeValue;
    }
}
