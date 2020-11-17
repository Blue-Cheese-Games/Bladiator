using Bladiator.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Entities
{
    public class EntityBase : MonoBehaviour, IDamageable
    {
        public Action<int> OnDamage;

        [SerializeField] protected float m_Maxhealth = 10f;
        [SerializeField] protected float m_Health = 10f;

        [SerializeField] protected float m_Movespeed = 5f;

        protected virtual void Awake()
        {
            
        }

        private void Update()
        {
            Move();
        }

        public virtual void Move()
        {
            Debug.Log("The method \"Move\" of this entity has not been overridden, make sure it is.");
        }

        public void Damage(int _damage)
        {
            // Damage the entity.
            m_Health -= _damage;

            OnDamage.Invoke(_damage);
        }
    }
}
