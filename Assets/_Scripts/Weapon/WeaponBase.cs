using System;
using System.Collections;
using Bladiator.Entities;
using Bladiator.Entities.Players;
using Bladiator.Managers;
using UnityEngine;

namespace Bladiator.Weapons
{
	[RequireComponent(typeof(Rigidbody))]
	public class WeaponBase : MonoBehaviour
	{
		private Weapon m_Weapon = null;

		[SerializeField] private bool m_Attack;
		[SerializeField] private Transform m_Root, m_Center;
		[SerializeField] private float m_RotationSpeed = 5f;

		[SerializeField] private float m_MaxSwordTravelDistance = 100;
		[SerializeField] private LayerMask m_SwordCollisionLayerMask;

		private Rigidbody m_RigidBody;
		private Coroutine BounceBackRoutine;

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
			m_RigidBody = GetComponent<Rigidbody>();
			m_Weapon = GetComponent<Weapon>();
		}

		private void Start()
		{
			GetComponentInChildren<Hitbox>().OnHit += OnHit;

			m_StartPosition = transform.localPosition;
			m_StartRotation = transform.localRotation;

			GameManager.Instance.ResetEvent += ResetEvent;

			m_Offset = m_Root.localPosition;
			m_StartingRotation = transform.rotation;
		}

		private void ResetEvent()
		{
			m_SecondAttack = false;
			m_Attack = false;
			m_WaitSecondAttack = false;

			m_Root.transform.localPosition = m_Offset;
			m_Center.transform.rotation = Quaternion.identity;
			m_TargetPosition = Vector3.zero;

			transform.localPosition = m_StartPosition;
			transform.localRotation = m_StartRotation;
			
			m_Center.transform.position = m_Weapon.Player.transform.position;
		}

		private void Update()
		{
			if (GameManager.Instance.GameState == GameState.Pause ||
			    GameManager.Instance.GameState == GameState.MainMenu ||
			    GameManager.Instance.GameState == GameState.PlayersDied ||
			    GameManager.Instance.GameState == GameState.Ending) return;

			if (MouseManager.Instance.HasHit())
				Move();

			m_Center.transform.position = m_Weapon.Player.transform.position;
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
			if (!other.TryGetComponent(out EntityBase entity))
			{
				// Check if the layer of the entity is in the "m_SwordCollisionLayerMask" Layermask.
				if (m_SwordCollisionLayerMask == (m_SwordCollisionLayerMask | (1 << other.gameObject.layer)))
				{
					if(BounceBackRoutine == null)
					{
						BounceBackRoutine = StartCoroutine(BounceBack());
					}
				}
			}
			else if (m_Attack || m_SecondAttack)
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
			Vector3 target = MouseManager.Instance.RaycastMousePosition();
			target.y = 1.5f;

			Vector3 direction = target - transform.position;

			if (m_Attack || m_SecondAttack)
			{
				if (m_TargetPosition == Vector3.zero)
				{
					float distanceToTarget = Vector3.Distance(transform.position, target);

					Mathf.Clamp(distanceToTarget, 0, m_MaxSwordTravelDistance);

					RaycastHit hit;
					Ray r = new Ray(transform.position, direction);
					Physics.SphereCast(r, 0.5f, out hit, distanceToTarget, m_SwordCollisionLayerMask);

					if(hit.collider != null)
					{
						m_TargetPosition = r.GetPoint(hit.distance);
					}
					else
					{
						m_TargetPosition = target;
					}
				}

				if (Vector3.Distance(m_Root.position, m_TargetPosition) > 0.5f)
				{
					// Move the sword towards the target.
					m_Root.position = Vector3.Lerp(m_Root.position,
						m_TargetPosition, m_Weapon.WeaponObject.WeaponData.AttackDragVelocity * Time.deltaTime);
				}
				else
				{
					// Wrap in statement to check if hit wall:
					//StartCoroutine(BounceBack());

					if (!m_SecondAttack)
					{
						ResetAttack();
					}
					else
					{
						ResetSecondAttack();
					}
				}
			}
			else
			{
				if (!m_WaitSecondAttack)
				{
					// Return the sword to it's idle position.
					m_Root.localPosition = Vector3.Lerp(m_Root.localPosition,
						m_Offset, m_Weapon.WeaponObject.WeaponData.AttackDragVelocity * Time.deltaTime);
				}
			}
		}


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

		private IEnumerator BounceBack()
		{
			Vector3 vec = m_Root.transform.position + -(m_TargetPosition.normalized * 3);
			vec.y = 1.5f;

			// DRY VVVVVVVVVVVVVVVVVVVVVVVVV
			if (!m_SecondAttack){
				ResetAttack();
			}

			//m_Root.position = vec;
			float m_PreviousDistance = float.MaxValue;

			for (int i = 0; i < 100; i++)
			{
                m_Root.position = Vector3.Lerp(m_Root.position,
                           vec, m_Weapon.WeaponObject.WeaponData.AttackDragVelocity * Time.deltaTime); //  * 2

				float distance = Vector3.Distance(m_Root.position, vec);
				if (distance < 0.1f || m_PreviousDistance < distance)
				{
                    if (m_SecondAttack)
                    {
						ResetSecondAttack();
                    }
					break; 
				}
                else
                {
					m_PreviousDistance = distance;
                }

				yield return new WaitForEndOfFrame();
			}

			BounceBackRoutine = null;
		}

		private void ResetAttack()
		{
			m_WaitSecondAttack = true;
			m_Attack = false;
			m_TargetPosition = Vector3.zero;
		}

		private void ResetSecondAttack()
		{
			m_WaitSecondAttack = false;
			m_SecondAttack = false;
			m_TargetPosition = Vector3.zero;
			m_AttackCooldownTimer = m_AttackCooldown;
		}
	}
}