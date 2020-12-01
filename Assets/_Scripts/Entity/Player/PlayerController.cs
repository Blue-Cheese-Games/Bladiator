using System;
using System.Collections;
using Bladiator.Managers;
using UnityEngine;

namespace Bladiator.Entities.Players
{
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Animator m_Animator;
		[SerializeField] private float m_MovementSpeed = 1f;

		private Vector3 m_SpawnPosition;

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
		private void OnGameStateChange(GameState obj)
		{
			if (obj == GameState.Pause)
			{
				m_Velocity = m_Rig.velocity;
				m_Rig.isKinematic = true;
			}
			else
			{
				m_Rig.isKinematic = false;
				m_Rig.velocity = m_Velocity;
			}
		}

		public void OnDeath(EntityBase player)
		{
			m_Animator.SetBool("died", true);
		}

		private void ResetEvent()
		{
			m_Rig.velocity = Vector3.zero;
			m_Rig.position = m_SpawnPosition;

			m_Animator.SetBool("died", true);
			m_Animator.SetBool("moving", false);
		}

		public void MoveHandle()
		{
			if (Camera.main == null) return;
			
			if(!m_AllowedToMove || GameManager.Instance.GameState == GameState.Pause) { return; }

			InputHandle();
		}

		public void Knockback(Vector3 knockback, float knockbackDuration)
		{
			m_Rig.velocity = Vector3.zero;
			m_Rig.AddForce(knockback, ForceMode.Impulse);
			m_AllowedToMove = false;
			StartCoroutine(ResetAllowedToMove(knockbackDuration));
		}

		private IEnumerator ResetAllowedToMove(float duration)
		{
			yield return new WaitForSeconds(duration);
			m_AllowedToMove = true;

		}

		private void InputHandle()
		{
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
				m_Animator.SetBool("moving", true);
				m_Rig.velocity = axis * m_MovementSpeed;

				int rawX = axis.x.AwayFromZero();
				int rawZ = axis.z.AwayFromZero();

				print($"input, x: {rawX} - z: {rawZ}, velocity: x: {Mathf.Clamp(m_Rig.velocity.x, -1f, 1f).AwayFromZero()} - z: {Mathf.Clamp(m_Rig.velocity.z, -1f, 1f).AwayFromZero()}");

				// Check if the movement input would make the player move the exact oposite way of it's current velocity.
				if (Mathf.Clamp(m_Rig.velocity.x, -1f, 1f).AwayFromZero() - rawX != 0f)
				{
					if(Mathf.Clamp(m_Rig.velocity.z, -1f, 1f).AwayFromZero() - rawZ != 0f)
					{
						// Input is in the opposite direction from the player's velocity.
						print("dir invert.");
						m_Animator.SetTrigger("changeDirection");
					}
				}
			}
			else
			{
				m_Animator.SetBool("moving", false);
				m_Rig.velocity = Vector3.zero;
			}
		}
	}
}