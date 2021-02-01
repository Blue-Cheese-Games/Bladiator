using Bladiator.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using Bladiator.Managers;
using Bladiator.Settings;
using UnityEngine;

namespace Bladiator.Entities
{
	public class EntityBase : MonoBehaviour, IDamageable
	{
		public Action<int> OnDamage;
		public Action<EntityBase> OnDeath;

		[Header("Audio")]
		[SerializeField] protected AudioSource m_audioSource;

		[SerializeField] private GameObject m_tempAudioPrefab;
		[SerializeField] protected AudioClip[] m_SpawnAudioClip;
		[SerializeField] protected AudioClip[] m_HitAudioClip;
		[SerializeField] protected AudioClip[] m_RandomIntervalAudioClip;
		[SerializeField] protected float m_MaxIntervalOfAudioPlay, m_MinIntervalOfAudioPlay;

		[Header("Stats")]
		[SerializeField] protected int m_Maxhealth = 10;

		[SerializeField] protected int m_Health = 10;

		[SerializeField] protected float m_Movespeed = 5f;

		public int CurrentHealth => m_Health;
		public int Maxhealth => m_Maxhealth;


		protected virtual void Awake()
		{
			if (m_SpawnAudioClip.Length > 0)
			{
				PlaySound(m_SpawnAudioClip[UnityEngine.Random.Range(0, m_SpawnAudioClip.Length)]);
			}

			StartCoroutine(PlaySoundAtRandomInterval());
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
		}

		public void SetHealth(int health, int maxHealth)
		{
			m_Maxhealth = maxHealth;
			m_Health = health;
		}

		public void Damage(int damage)
		{
			// Damage the entity.
			m_Health -= damage;

			OnDamage?.Invoke(damage);

			if (m_Health <= 0)
			{
				OnDeath?.Invoke(this);
			}

			if (m_HitAudioClip.Length > 0)
			{
				PlaySound(m_HitAudioClip[UnityEngine.Random.Range(0, m_HitAudioClip.Length)]);
			}
		}

		public virtual void Knockback(Vector3 knockback, float knockbackDuration)
		{
			Debug.LogError("This method should be overriden to apply the knockback to the entity.");
		}

		protected virtual IEnumerator ResetAllowedToMove(float delay)
		{
			yield return new WaitForSeconds(delay);
			// Unlock the entities movement here.
			Debug.LogError("This method should be overriden to unlock the movement of the entity.");
		}

		protected IEnumerator PlaySoundAtRandomInterval()
		{
			while (true)
			{
				yield return new WaitForSeconds(UnityEngine.Random.Range(m_MinIntervalOfAudioPlay,
					m_MaxIntervalOfAudioPlay));
				if (GameManager.Instance.GameState != GameState.Pause)
				{
					if (m_RandomIntervalAudioClip.Length > 0)
					{
						PlaySound(m_RandomIntervalAudioClip[
							UnityEngine.Random.Range(0, m_RandomIntervalAudioClip.Length)]);
					}
				}
			}
		}

		public void PlaySound(AudioClip clip)
		{
			m_audioSource.clip = clip;

			if (m_Health <= 0)
			{
				AudioSource source = Instantiate(m_tempAudioPrefab, transform.position, Quaternion.identity)
					.GetComponent<AudioSource>();
				source.clip = clip;
				source.Play();
			}
			else
			{
				m_audioSource.Play();
			}
		}
	}
}