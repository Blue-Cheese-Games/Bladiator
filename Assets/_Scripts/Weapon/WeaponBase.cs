using System;
using Bladiator.Entities;
using Bladiator.Entities.Players;
using Bladiator.Managers;
using UnityEngine;

namespace Bladiator.Weapons
{
	public class WeaponBase : MonoBehaviour
	{
		private Weapon m_Weapon = null;

		[SerializeField] private bool m_Attack;
		[SerializeField] private Transform m_Root, m_Center;
		[SerializeField] private float m_RotationSpeed = 5f;
		
		private Vector3 m_Offset;
		private Quaternion m_StartingRotation;
		
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

			m_Offset = m_Root.localPosition;
			m_StartingRotation = transform.rotation;
		}

		private void ResetEvent()
		{
			transform.position = m_StartPosition;
			transform.rotation = m_StartRotation;
		}

		private void Update()
		{
			m_Center.transform.position = m_Weapon.Player.transform.position;
			
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
			if (!other.TryGetComponent(out EntityBase entity)) return;

			if (m_Attack || m_SecondAttack)
			{
				if (other.CompareTag("Player"))
					entity.Damage((int) Mathf.Floor(m_Weapon.WeaponObject.WeaponData.Damage / 2));
				else
					entity.Damage((int) m_Weapon.WeaponObject.WeaponData.Damage);

				m_Weapon.HitParticle.Play();
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

				if (Vector3.Distance(m_Root.position, m_TargetPosition) > 0.5f)
				{
					m_Root.position = Vector3.Lerp(m_Root.position,
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
					m_Root.localPosition = Vector3.Lerp(m_Root.localPosition,
						m_Offset, m_Weapon.WeaponObject.WeaponData.AttackDragVelocity * Time.deltaTime);
				}
			}
		}


		// TODO Lerp rotation
		private void Rotate()
		{
			if (!m_Attack && !m_SecondAttack && !m_WaitSecondAttack)
			{
				transform.localRotation = m_StartingRotation;
				Vector3 direction = m_Weapon.Player.transform.position - MouseManager.Instance.RaycastMousePosition();
				direction.y = 0;
				Quaternion targetRotation = Quaternion.LookRotation(-direction, m_Weapon.Player.transform.up);
				m_Center.rotation = Quaternion.Lerp(m_Center.rotation, targetRotation, m_RotationSpeed * Time.deltaTime);
				return;
			}

			if (m_Attack || m_SecondAttack)
			{
				Vector3 direction = m_Weapon.Player.transform.position - m_TargetPosition;
				direction.y = 0;
				transform.rotation = Quaternion.LookRotation(direction, m_Weapon.Player.transform.up);
			}
		}
	}
}