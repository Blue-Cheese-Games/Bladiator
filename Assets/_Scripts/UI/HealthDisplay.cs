using System;
using Bladiator.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bladiator.UI
{
	public class HealthDisplay : MonoBehaviour
	{
		public EntityBase m_EntityOverride;
		public string m_BossName = "";

		[SerializeField] private float m_ChangeSpeed = 1f;
		[SerializeField] private TMP_Text m_NameObject;
		
		private EntityBase m_Entity;
		private Slider m_HealthBar;

		private float m_CurrentHealth;
		private int m_TargetHealth;
		private int m_MaxHealth;
		
		private void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			m_Entity = (m_EntityOverride != null) ? m_EntityOverride : GetComponent<EntityBase>();
			m_HealthBar = GetComponentInChildren<Slider>();

			if (m_Entity == null)
			{
				gameObject.SetActive(false);
				return;
			}
			
			m_MaxHealth = m_Entity.Maxhealth;

			m_CurrentHealth = m_MaxHealth;
			m_TargetHealth = m_MaxHealth;

			m_Entity.OnDamage += OnDamage;
			m_Entity.OnDeath += OnDeath;
		}

		private void OnDeath(EntityBase obj)
		{
			gameObject.SetActive(false);
		}

		public void OnEnable()
		{
			if(m_NameObject == null || string.IsNullOrEmpty(m_BossName)) return;

			m_NameObject.text = m_BossName;
			Initialize();
		}

		private void OnDamage(int amount)
		{
			if (!m_HealthBar.gameObject.activeSelf) m_HealthBar.gameObject.SetActive(true);
			
			m_TargetHealth -= amount;
		}

		void Update()
		{
			if (m_Entity == null) return;
			
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

		public void ResetHealthBar()
		{
			m_TargetHealth = m_MaxHealth;
			m_CurrentHealth = m_MaxHealth;
			m_HealthBar.value = 1;
		}
	}
}