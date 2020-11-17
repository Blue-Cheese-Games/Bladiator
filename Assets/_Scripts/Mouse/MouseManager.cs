using UnityEngine;

namespace Bladiator.Managers
{
    public class MouseManager : MonoBehaviour
    {
        private MouseAxis m_MouseAxis = new MouseAxis();

        public MouseAxis GetMouseAxis()
            => m_MouseAxis;

        public Vector2 GetMouseAxisAsVector()
            => new Vector2(GetMouseAxis().MouseX, GetMouseAxis().MouseY);

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            SetMouseAxis();
        }

        private void SetMouseAxis()
        {
            m_MouseAxis.MouseX = Input.GetAxis("Mouse X");
            m_MouseAxis.MouseY = Input.GetAxis("Mouse Y");
        }
    }
    
    public struct MouseAxis
    {
        public float MouseX;
        public float MouseY;
    }
}
