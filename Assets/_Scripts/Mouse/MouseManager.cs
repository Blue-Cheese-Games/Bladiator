using UnityEngine;

namespace Bladiator.Managers
{
    public class MouseManager : MonoBehaviour
    {
        public static MouseManager Instance = null;
        
        private KeyCode m_ToggleCursorLockMode = KeyCode.Mouse1;
        private CursorLockMode m_CursorLockMode = CursorLockMode.None;
        
        private Camera m_MainCamera = null;

        [SerializeField] private LayerMask m_LayerMask;
        
        /// <summary>
        /// Has the raycast hit something
        /// </summary>
        /// <returns> True or false based on the mouse hit </returns>
        public bool HasHit() 
            => RaycastMousePosition() != Vector3.zero;
        
        /// <summary>
        /// Return the vector3 position that the mouse hits with a raycast
        /// </summary>
        /// <returns> Vector3 mouse position </returns>
        public Vector3 RaycastMousePosition()
        {
            float rayLength = 35f;
            Ray ray = m_MainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.blue);

            // Shoot a raycast from the camera pov to the mouse position
            if (Physics.Raycast(ray, out RaycastHit raycastHit, rayLength, m_LayerMask))
                return raycastHit.point;

            return Vector3.zero;
        }

        private void Awake()
        {
            // Create a singleton
            if (Instance == null)
                Instance = this;
            
            m_MainCamera = Camera.main;
            Cursor.visible = true;
            ChangeCursorLockMode(m_CursorLockMode);
        }

        /// <summary>
        /// Set the lock mode of the cursor
        /// </summary>
        /// <param name="mode"> The lock mode of the cursor </param>
        private void ChangeCursorLockMode(CursorLockMode mode)
        {
            Cursor.lockState = mode;
            m_CursorLockMode = mode;
        }
    }
}
