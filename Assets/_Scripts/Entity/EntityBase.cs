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
        [SerializeField] protected float m_Health = 10f;

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
            m_Health -= damage;

            OnDamage?.Invoke(damage);

            if(m_Health <= 0)
            {
                OnDeath?.Invoke(this);
            }
        }

        public virtual void Knockback(Vector3 knockback, float knockbackDuration)
        {
            Debug.LogError("This method should be overriden to apply the knockback to the entity.");
        }

        protected virtual IEnumerator ResetAllowedToMove(float delay)
        {
            yield return new WaitForSeconds(delay);
            // Unlock the entities movement here.
            Debug.LogError("This method should be overriden to unlock the movement of the entity.");
        }
    }
}
