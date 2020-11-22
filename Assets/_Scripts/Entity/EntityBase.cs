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
        public Action<EntityBase> OnDeath;

        [SerializeField] protected float m_Maxhealth = 10f;
        [SerializeField] protected float m_Health = 10f;

        [SerializeField] protected float m_Movespeed = 5f;

        protected virtual void Awake()
        {
            
        }

        protected virtual void Update()
        {

        }

        public void Damage(int damage)
        {
            // Damage the entity.
            m_Health -= damage;

            OnDamage?.Invoke(damage);

            if(m_Health <= 0)
            {
                OnDeath?.Invoke(this);
            }
        }
    }
}
