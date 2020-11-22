using System;
using UnityEngine;

namespace Bladiator.Entity.Player
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
			float horizontalAxis = -Input.GetAxisRaw("Horizontal");
			float verticalAxis = -Input.GetAxisRaw("Vertical");
			
			Vector3 axis = new Vector3(horizontalAxis, 0, verticalAxis);
			
			if (axis != Vector3.zero)
			{
				m_Rig.MovePosition(m_Rig.position + axis * (m_MovementSpeed * Time.deltaTime));
			}
		}
	}
}