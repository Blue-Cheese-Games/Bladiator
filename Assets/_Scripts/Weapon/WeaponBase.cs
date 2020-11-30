using System;
using Bladiator.Entities;
using Bladiator.Managers;
using UnityEngine;

namespace Bladiator.Weapons
{
    public class WeaponBase : MonoBehaviour
    {
        [SerializeField] private GameObject m_Player = null;
        private Weapon m_Weapon = null;

        private void Awake()
        {
            m_Weapon = GetComponent<Weapon>();
        }

        private void Update()
        {
            if (MouseManager.Instance.HasHit())
                Move();

            Rotate();
        }

        private void OnTriggerEnter(Collider other)
        {
            try
            {
                EntityBase entity = other.GetComponent<EntityBase>();
                entity.Damage((int) m_Weapon.WeaponObject.WeaponData.Damage);
            }
            catch (Exception exception)
            {
                Debug.Log("Object is not an entity");
            }
        }

        private void Move()
        {
            Vector3 position = MouseManager.Instance.RaycastMousePosition();
            position.y += 1;

            transform.position = position;
        }

        private void Rotate()
        {
            Vector3 direction = transform.position - m_Player.transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
