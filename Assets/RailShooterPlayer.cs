using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.RailShooter;
using UnityEngine.UI;

public class RailShooterPlayer : MonoBehaviour
{
    [SerializeField]
    private float m_LifeMax;
    [SerializeField]
    private float m_LifeToRemove;

    private float m_CurrentLife;

    [SerializeField]
    private Image m_LifeBar;

    void Start()
    {
        m_CurrentLife = m_LifeMax;
    }

    void Update()
    {
        Debug.Log(m_CurrentLife);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("On trigger enter");
        m_CurrentLife -= m_LifeToRemove;
        float lifeValue = m_CurrentLife / m_LifeMax;
        Debug.Log(lifeValue);

        m_LifeBar.fillAmount = lifeValue;

        other.GetComponent<ShooterBullet>().Remove();
    }
}
