using System;
using Bladiator.Entities;
using Bladiator.Managers;
using UnityEngine;

namespace Bladiator.Weapons
{
	[RequireComponent(typeof(Animator))]
	public class WeaponBase : MonoBehaviour
	{
		private Weapon m_Weapon = null;

		[SerializeField] private bool m_Attack;

		private Vector3 m_StartPosition;
		private Quaternion m_StartRotation;
		
		private Animator m_Animator;

		private void Awake()
		{
			m_Weapon = GetComponent<Weapon>();
		}

		void Start()
		{
			m_Animator = GetComponent<Animator>();

			GetComponentInChildren<Hitbox>().OnHit += OnHit;
			
			m_StartPosition = transform.position;
			m_StartRotation = transform.rotation;
			
			GameManager.Instance.OnGameStateChange += OnGameStateChange;
			GameManager.Instance.ResetEvent += ResetEvent;
		}

		private void ResetEvent()
		{
			transform.position = m_StartPosition;
			transform.rotation = m_StartRotation;
		}

		private void OnGameStateChange(GameState obj)
		{
			switch (obj)
			{
				case GameState.Pause:
					m_Animator.speed = 0;
					break;
				
				case GameState.MainMenu:
					m_Animator.speed = 1;
					m_Animator.Play("idle");
					break;
				
				default:
					m_Animator.speed = 1;
					break;
			}
		}
		
		

		private void Update()
		{
			if (GameManager.Instance.GameState == GameState.Pause ||
			    GameManager.Instance.GameState == GameState.MainMenu ||
			    GameManager.Instance.GameState == GameState.PlayersDied ||
			    GameManager.Instance.GameState == GameState.Ending) return;

			if (MouseManager.Instance.HasHit())
				Move();

			if (Input.GetMouseButtonDown(0) && !m_Attack)
			{
				m_Animator.Play("Attack");
				m_Attack = true;
			}
				

			Rotate();
		}

		private void OnHit(Collider other)
		{
			if (other.CompareTag("Player") || !m_Attack) return;

			try
			{
				EntityBase entity = other.GetComponent<EntityBase>();
				entity.Damage((int) m_Weapon.WeaponObject.WeaponData.Damage);
				m_Weapon.HitParticle.Play();
			}
			catch (Exception exception)
			{
				Debug.Log("Object is not an entity");
			}
		}

		private void Move()
		{
			Vector3 playerPos = m_Weapon.Player.transform.position;
			Vector3 newPos = MouseManager.Instance.RaycastMousePosition();

			// Clamp the weapon position inside a circle
			Vector3 offset = newPos - playerPos;
			Vector3 position = playerPos + Vector3.ClampMagnitude(offset, m_Weapon.WeaponObject.WeaponData.Reach);

			position.y = 1.5f;
			transform.position = Vector3.Lerp(transform.position,
				position, m_Weapon.WeaponObject.WeaponData.DragVelocity * Time.deltaTime);
		}

		private void Rotate()
		{
			Vector3 direction = m_Weapon.Player.transform.position - transform.position;
			direction.y = 0;
			transform.rotation = Quaternion.LookRotation(direction);
		}

		public void EndAttack()
		{
			m_Animator.Play("Idle");
			m_Attack = false;
		}
	}
}