using System;
using UnityEngine;

namespace Bladiator.Entities.Players
{
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private float m_MovementSpeed = 1f;
		[SerializeField] private GameObject m_SpriteObject;

		private Rigidbody m_Rig;

		private void Start()
		{
			m_Rig = GetComponent<Rigidbody>();
		}

		public void MoveHandle()
		{
			if (Camera.main == null || m_SpriteObject == null) return;
			
			InputHandle();
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
				m_Rig.MovePosition(m_Rig.position + axis * (m_MovementSpeed * Time.deltaTime));
			}
		}
	}
}