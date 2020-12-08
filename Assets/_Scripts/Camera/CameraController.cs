using System;
using UnityEngine;

namespace Bladiator.CameraControlle
{
	public enum CameraState
	{
		Idle,
		Moving
	}

	public class CameraController : MonoBehaviour
	{
		private CameraState m_CameraState;

		[SerializeField] private float m_MouseXCurrent = 0;
		[SerializeField] private float m_MouseXPrevious = 0;

		public CameraState CameraState => m_CameraState;

		private void Start()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Confined;
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetMouseButtonDown(2))
			{
				Cursor.visible = true;
				m_MouseXPrevious = Input.mousePosition.x;
				m_MouseXCurrent = Input.mousePosition.x;
				m_CameraState = CameraState.Moving;
			}

			if (Input.GetMouseButton(2))
			{
				m_MouseXCurrent = Input.mousePosition.x;

				float delta = m_MouseXCurrent - m_MouseXPrevious;
				transform.eulerAngles += new Vector3(0, delta, 0);

				m_MouseXPrevious = m_MouseXCurrent;
			}

			if (Input.GetMouseButtonUp(2))
			{
				Cursor.visible = false;
				m_CameraState = CameraState.Idle;
			}
		}
	}
}