using System;
using Bladiator.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Bladiator.UI
{
	public class HealthDisplay : MonoBehaviour
	{
		[SerializeField] private float m_ChangeSpeed = 1f;
		
		private EntityBase m_Entity;
		private Slider m_HealthBar;

		private float m_CurrentHealth;
		private float m_TargetHealth;
		private float m_MaxHealth;
		
		private void Start()
		{
			m_Entity = GetComponent<EntityBase>();
			m_HealthBar = GetComponentInChildren<Slider>();

			m_MaxHealth = m_Entity.Maxhealth;

			m_CurrentHealth = m_MaxHealth;
			m_TargetHealth = m_MaxHealth;

			m_Entity.OnDamage += OnDamage;
		}

		private void OnDamage(int amount)
		{
			if (!m_HealthBar.gameObject.activeSelf) m_HealthBar.gameObject.SetActive(true);
			
			m_TargetHealth -= amount;
		}

		void Update()
		{
			if (Math.Abs(m_TargetHealth - m_CurrentHealth) > 0.1f)
			{
				m_CurrentHealth -= Time.deltaTime * m_ChangeSpeed;
			}
			else
			{
				m_CurrentHealth = m_TargetHealth;
			}

			m_HealthBar.value = m_CurrentHealth / m_MaxHealth;
		}
	}
}