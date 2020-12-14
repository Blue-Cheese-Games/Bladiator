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

		private Vector3 m_TargetPosition;

		private float m_WarmupCooldown = .5f;
		private float m_WarmupTimer = 0f;

		private bool m_SecondAttack;
		private bool m_WaitSecondAttack;

		private float m_AttackCooldown = .25f;
		private float m_AttackCooldownTimer = 0f;
		
		private void Awake()
		{
			m_Weapon = GetComponent<Weapon>();
		}

		private void Start()
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

			Rotate();

			if (m_AttackCooldownTimer > 0)
			{
				m_AttackCooldownTimer -= Time.deltaTime;
				return;
			}
			
			if (Input.GetMouseButtonDown(0) && !m_Attack && !m_SecondAttack && !m_WaitSecondAttack)
			{
				m_Attack = true;
			}

			if (!m_SecondAttack && m_WaitSecondAttack && m_WarmupTimer < m_WarmupCooldown)
			{
				if (Input.GetMouseButtonDown(0))
				{
					m_WaitSecondAttack = false;
					m_SecondAttack = true;
				}

				m_WarmupTimer += Time.deltaTime;
			}
			else
			{
				m_WaitSecondAttack = false;
				m_WarmupTimer = 0;
			}
		}

		private void OnHit(Collider other)
		{
			if (other.CompareTag("Player")) return;

			if (m_Attack || m_SecondAttack)
			{
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
		}

		//TODO This needs some improvement for the secondary attack.
		private void Move()
		{
			Vector3 playerPos = m_Weapon.Player.transform.position;
			Vector3 newPos = MouseManager.Instance.RaycastMousePosition();

			// Clamp the weapon position inside a circle
			Vector3 offset = newPos - playerPos;
			Vector3 position = playerPos + offset;

			position.y = 1.5f;

			if (m_Attack || m_SecondAttack)
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
				else if (!m_SecondAttack)
				{
					m_WaitSecondAttack = true;
					m_Attack = false;
					m_TargetPosition = Vector3.zero;
				}
				else
				{
					m_WaitSecondAttack = false;
					m_SecondAttack = false;
					m_TargetPosition = Vector3.zero;
					m_AttackCooldownTimer = m_AttackCooldown;
				}
			}
			else
			{
				if (!m_WaitSecondAttack)
				{
					transform.position = Vector3.Lerp(transform.position,
						(playerPos + new Vector3(.75f, 0, .75f)),
						m_Weapon.WeaponObject.WeaponData.AttackDragVelocity * Time.deltaTime);
				}
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