using UnityEngine;

namespace Bladiator.Entity.Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] float m_MovementSpeed = 1f;
		[SerializeField] GameObject m_SpriteObject;
		public void MoveHandle()
		{
			if (Camera.main == null || m_SpriteObject == null) return;
			
			m_SpriteObject.transform.forward = -Camera.main.transform.forward;
			
			InputHandle();
		}

		private void InputHandle()
		{
			float horizontalAxis = Input.GetAxisRaw("Horizontal");
			float verticalAxis = Input.GetAxisRaw("Vertical");
			
			Vector3 axis = new Vector3(horizontalAxis, 0, verticalAxis);
			
			if (axis != Vector3.zero)
			{
				Vector3 targetPos = axis * (m_MovementSpeed * Time.deltaTime);
				targetPos.y = transform.position.y;

				transform.position += targetPos;
			}
		}
	}
}