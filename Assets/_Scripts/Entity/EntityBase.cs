using Bladiator.Entities;
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
        [SerializeField] protected float m_CurrentHealth = 10f;
        [SerializeField] protected float m_TargetHealth = 10f;

        [SerializeField] protected float m_Movespeed = 5f;

        public float Maxhealth => m_Maxhealth; 

        protected virtual void Awake()
        {
            
        }
        
        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {

        }

        public void Damage(int damage)
        {
            // Damage the entity.
            m_TargetHealth -= damage;

            OnDamage?.Invoke(damage);

            if(m_TargetHealth <= 0)
            {
                OnDeath?.Invoke(this);
            }
        }
    }
}
