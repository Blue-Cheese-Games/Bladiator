using UnityEngine;

namespace Bladiator.Managers
{
    public class MouseManager : MonoBehaviour
    {
        public static MouseManager Instance;
        
        private MouseAxis m_MouseAxis = new MouseAxis();

        public Vector2 GetMouseAxisAsVector()
            => new Vector2(GetMouseAxis().MouseX, GetMouseAxis().MouseY);
        
        private MouseAxis GetMouseAxis()
            => m_MouseAxis;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
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
