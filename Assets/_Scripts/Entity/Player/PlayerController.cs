using System;
using System.Collections;
using Bladiator.Managers;
using UnityEngine;

namespace Bladiator.Entities.Players
{
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private float m_MovementSpeed = 1f;

		private Vector3 m_SpawnPosition;

		private Rigidbody m_Rig;

		private bool m_AllowedToMove = true;

		private void Start()
		{
			m_Rig = GetComponent<Rigidbody>();
			m_SpawnPosition = transform.position;
			GameManager.Instance.ResetEvent += ResetEvent;
		}

		private void ResetEvent()
		{
			m_Rig.velocity = Vector3.zero;
			m_Rig.position = m_SpawnPosition;
		}

		public void MoveHandle()
		{
			if (Camera.main == null) return;
			
			if(!m_AllowedToMove) { return; }

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
				//m_Rig.MovePosition(m_Rig.position + axis * (m_MovementSpeed * Time.deltaTime));
				m_Rig.velocity = axis * m_MovementSpeed;
			}
		}
	}
}