using System;
using System.Collections;
using Bladiator.Managers;
using Bladiator.UI;
using UnityEngine;

namespace Bladiator.Entities.Players
{
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Animator m_Animator;
		[SerializeField] private float m_MovementSpeed = 1f;

		[SerializeField] private Vector3 m_SpawnPosition;

		private Rigidbody m_Rig;

		private bool m_AllowedToMove = true;

		private void Start()
		{
			m_Rig = GetComponent<Rigidbody>();
			m_SpawnPosition = transform.position;
			GameManager.Instance.ResetEvent += ResetEvent;
			GameManager.Instance.OnGameStateChange += OnGameStateChange;
		}

		private Vector3 m_Velocity;

		private void OnGameStateChange(GameState state)
		{
			if (state == GameState.Pause)
			{
				m_Velocity = m_Rig.velocity;
				m_Rig.isKinematic = true;
				m_Animator.speed = 0;
			}
			else if (state == GameState.Ending)
			{
				m_AllowedToMove = false;
				m_Rig.velocity = Vector3.zero;
				m_Rig.isKinematic = true;
			}
			else
			{
				m_Rig.isKinematic = false;
				m_Rig.velocity = m_Velocity;
				m_Animator.speed = 1;
			}
		}

		public void OnDeath(EntityBase player)
		{
			GameManager.Instance.OnPlayerDied(player);
			m_AllowedToMove = false;
			m_Rig.velocity = Vector3.zero;
			m_Animator.Play("death");
		}

		private void ResetEvent()
		{
			m_Rig.velocity = Vector3.zero;
			transform.position = m_SpawnPosition;
			m_AllowedToMove = true;
			m_Rig.isKinematic = false;

			m_Animator.SetBool("died", false);
			m_Animator.SetBool("moving", false);
			m_Animator.Play("Idle");
		}

		public void MoveHandle()
		{
			if (Camera.main == null) return;

			#if UNITY_EDITOR
			if (Input.GetKey(KeyCode.K))
				GetComponent<Player>().Damage(9999);
			#endif

			if (!m_AllowedToMove || GameManager.Instance.GameState == GameState.Pause)
			{
				return;
			}

			InputHandle();
		}

		public void Knockback(Vector3 knockback, float knockbackDuration)
		{
			if (!m_AllowedToMove) { return; }

			m_Rig.velocity = Vector3.zero;
			m_Rig.AddForce(knockback, ForceMode.Impulse);
			LockMovement();
		}

		private void InputHandle()
		{
			if (GameManager.Instance.GameState == GameState.MainMenu || GetComponent<EntityBase>().CurrentHealth <= 0) return;
			
			float horizontalAxis = Input.GetAxisRaw("Horizontal");
			float verticalAxis = Input.GetAxisRaw("Vertical");

			Vector3 forward = Camera.main.transform.forward;
			Vector3 right = Camera.main.transform.right;

			forward.y = 0;
			right.y = 0;
			forward.Normalize();
			right.Normalize();

			Vector3 axis = forward * verticalAxis + right * horizontalAxis;


			if (axis != Vector3.zero)
			{
				Vector3 selfForward = transform.forward;

				if (Vector3.Dot(selfForward, axis) < -0.5f)
				{
					// turn around.
					m_Animator.SetBool("changeDirection", true);
					m_Rig.velocity = Vector3.zero;
					LockMovement();
					return;
				}
				else
				{
					m_Animator.SetBool("moving", true);
					RotateTowardsMovement();
				}

				m_Rig.velocity = axis * m_MovementSpeed;
			}
			else
			{
				m_Animator.SetBool("moving", false);
				m_Rig.velocity = Vector3.zero;
			}
		}

		public void RotateTowardsMovement()
		{
			transform.LookAt(transform.position + m_Rig.velocity);
		}

		public void LockMovement()
		{
			m_AllowedToMove = false;
		}

		public void UnlockMovement()
		{
			m_AllowedToMove = true;
		}

		public Animator GetAnimator()
		{
			return m_Animator;
		}
	}
}