using Bladiator.Managers;
using UnityEngine;

namespace Bladiator.Weapons
{
    public class WeaponBase : MonoBehaviour
    {
        private Weapon m_Weapon = null;

        private void Awake()
        {
            m_Weapon = GetComponent<Weapon>();
        }

        private void Update()
        {
            if (MouseManager.Instance.HasHit())
                Move();
        }

        private void Move()
        {
            Vector3 position = MouseManager.Instance.RaycastMousePosition();
            position.y += 1;
            
            gameObject.transform.position = position;
        }
    }
}
