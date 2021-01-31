using Bladiator.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Projectiles
{
    public class Grenade : MonoBehaviour
    {
        private Action OnExplode;

        public ParticleSystem m_Indicator, m_Explosion;

        [Header("Stats")]
        [Header("Stats are overwritten by the Initialize method if passed.")]

        [Tooltip("The damage that is applied to hit entities.")]
        [SerializeField] private int m_Damage;

        [Tooltip("The knockback that is applied to hit entities.")]
        [SerializeField] private float m_Knockback;

        [Tooltip("The duration of the knockback that is applied to hit entities.")]
        [SerializeField] private float m_KnockbackDuration;

        [Tooltip("The range of the attack in which entities will be affected.")]
        [SerializeField] private float m_GrenadeAoERange = 3;

        [Tooltip("After how long will this grenade be able to explode?")]
        [SerializeField] private float m_ArmDelay = 0.5f;

        private bool m_Initialized;
        private bool m_IsExploded;

        public void Initialize(int damage = -1, float knockback = -1, float knockbackDuration = -1, float grenadeAoERange = -1, float armDelay = -1)
        {
            if(damage != -1)
            {
                m_Damage = damage;
            }
            if(knockback != -1)
            {
                m_Knockback = knockback;
            }
            if(knockbackDuration != -1)
            {
                m_KnockbackDuration = knockbackDuration;
            }
            if(grenadeAoERange != -1)
            {
                m_GrenadeAoERange = grenadeAoERange;
            }
            if(armDelay != -1)
            {
                m_ArmDelay = armDelay;
            }
            
            m_Indicator.Play();
            m_Initialized = true;
        }

        private void Update()
        {
            if (m_Initialized && !m_Indicator.isPlaying)
            {
                m_Initialized = false;
                Explode();
            }

            if (m_IsExploded && !m_Explosion.isPlaying)
            {
                m_IsExploded = false;
                Destroy(gameObject);
            }
        }

        private void Explode()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, m_GrenadeAoERange);

            List<EntityBase> hitEntities = new List<EntityBase>();

            foreach(Collider coll in colls)
            {
                EntityBase eb = coll.gameObject.GetComponent<EntityBase>();
                if (eb == null)
                {
                    eb = coll.gameObject.GetComponentInParent<EntityBase>();
                }

                if (eb != null)
                {
                    hitEntities.Add(eb);
                }
            }

            foreach(EntityBase entity in hitEntities)
            {
                entity.Damage(m_Damage);

                Vector3 direction = entity.transform.position - transform.position;
                direction.y = 0;

                entity.Knockback(direction.normalized * m_Knockback, m_KnockbackDuration);
            }

            m_Explosion.Play();

            OnExplode?.Invoke();
            m_IsExploded = true;
        }

        public void SubscribeToOnExplode(Action listener)
        {
            OnExplode += listener;
        }
    }
}