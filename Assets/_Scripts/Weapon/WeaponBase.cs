using System;
using Bladiator.Entities;
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

            Rotate();
        }

        private void OnTriggerEnter(Collider other)
        {
            try
            {
                EntityBase entity = other.GetComponent<EntityBase>();
                entity.Damage((int) m_Weapon.WeaponObject.WeaponData.Damage);
                m_Weapon.HitParticle.Play();
            }
            catch (Exception exception)
            {
                Debug.Log("Object is not an entity");
            }
        }

        private void Move()
        {
            Vector3 position = MouseManager.Instance.RaycastMousePosition();
            position.y += 0.5f;

            transform.position = position;
        }

        private void Rotate()
        {
            Vector3 direction = m_Weapon.Player.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
