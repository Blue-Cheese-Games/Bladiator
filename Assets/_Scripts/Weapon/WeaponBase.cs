using System;
using Bladiator.Entities;
using Bladiator.Managers;
using UnityEngine;

namespace Bladiator.Weapons
{
	public class WeaponBase : MonoBehaviour
	{
		private Weapon m_Weapon = null;

		[SerializeField] private bool m_Attack;

		private Vector3 m_StartPosition;
		private Quaternion m_StartRotation;

		private void Awake()
		{
			m_Weapon = GetComponent<Weapon>();
		}

		void Start()
		{
			GetComponentInChildren<Hitbox>().OnHit += OnHit;

			m_StartPosition = transform.position;
			m_StartRotation = transform.rotation;

			GameManager.Instance.ResetEvent += ResetEvent;
		}

		private void ResetEvent()
		{
			transform.position = m_StartPosition;
			transform.rotation = m_StartRotation;
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

		private Vector3 m_TargetPosition;
		
		private void Move()
		{
			Vector3 playerPos = m_Weapon.Player.transform.position;
			Vector3 newPos = MouseManager.Instance.RaycastMousePosition();

			// Clamp the weapon position inside a circle
			Vector3 offset = newPos - playerPos;
			Vector3 position = playerPos + Vector3.ClampMagnitude(offset, m_Weapon.WeaponObject.WeaponData.Reach);

			position.y = 1.5f;

			if (m_Attack)
			{
				if (m_TargetPosition == Vector3.zero)
				{
					m_TargetPosition = position;
				} 
				
				if (Vector3.Distance(transform.position, m_TargetPosition) > 0.5f)
				{
					transform.position = Vector3.Lerp(transform.position,
						m_TargetPosition, m_Weapon.WeaponObject.WeaponData.AttackDragVelocity * Time.deltaTime);
				}
				else
				{
					m_TargetPosition = Vector3.zero;
					m_Attack = false;
				}
			}
			else
			{
				transform.position = Vector3.Lerp(transform.position,
					position, m_Weapon.WeaponObject.WeaponData.IdleDragVelocity * Time.deltaTime);
			}
		}

		private void Rotate()
		{
			Vector3 direction = m_Weapon.Player.transform.position - transform.position;
			direction.y = 0;
			transform.rotation = Quaternion.LookRotation(direction);
		}
	}
}