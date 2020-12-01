﻿using System;
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
            if (GameManager.Instance.GameState == GameState.Pause) return;
            
            if (MouseManager.Instance.HasHit())
                Move();

            Rotate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) return;
            
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
            Vector3 mousePos = m_Weapon.Player.transform.position;
            Vector3 newPos = MouseManager.Instance.RaycastMousePosition() + mousePos;

            // Clamp the weapon position inside a circle
            Vector3 offset = newPos - mousePos;
            Vector3 position = mousePos + Vector3.ClampMagnitude(offset, m_Weapon.WeaponObject.WeaponData.Reach);

            position.y = 0.5f;
            transform.position = Vector3.Lerp(transform.position, 
                position, m_Weapon.WeaponObject.WeaponData.DragVelocity * Time.deltaTime);
        }

        private void Rotate()
        {
            Vector3 direction = m_Weapon.Player.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
