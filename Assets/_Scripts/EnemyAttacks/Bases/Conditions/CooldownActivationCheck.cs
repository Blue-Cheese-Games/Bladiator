using Bladiator.Entities;
using Bladiator.Entities.Enemies;
using Bladiator.Entities.Players;
using Bladiator.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.EnemyAttacks
{
    [CreateAssetMenu(fileName = "new Cooldown", menuName = "Bladiator/ActivationConditions/Cooldown")]
    [System.Serializable]
    public class CooldownActivationCheck : BaseActivationCondition
    {
        [Tooltip("How often can the enemy use this attack?")]
        public float m_Cooldown = 2f;

        public bool m_StartAvaiable = true;

        // DEBUG: Serializefield
        [SerializeField] private List<EnemyCooldownReference> m_cooldownReferences = new List<EnemyCooldownReference>();

        public override void Initialize(EnemyAttackBase attack, Enemy enemy)
        {
            attack.OnActivate += OnActivate;
            
            enemy.OnDeath += DeleteEnemyFromListing;
            EnemyCooldownReference enemyCooldownReference = new EnemyCooldownReference()
            {
                Enemy = enemy,
                LastUseTimeStamp = m_StartAvaiable ? -m_Cooldown : 0
            };

            m_cooldownReferences.Add(enemyCooldownReference);

            GameManager.Instance.ResetEvent -= Clear;
            GameManager.Instance.ResetEvent += Clear;

            GameManager.Instance.OnApplicationQuitEvent -= Clear;
            GameManager.Instance.OnApplicationQuitEvent += Clear;
        }

        public override bool CheckCondition(EnemyAttackBase attack, Enemy enemy, Player targetPlayer)
        {
            EnemyCooldownReference cooldownReference = m_cooldownReferences.Find(r => r.Enemy == enemy);
            if ((cooldownReference.LastUseTimeStamp + m_Cooldown) < Time.time)
            {
                return true;
            }

            return false;
        }

        public void OnActivate(Enemy enemy)
        {
            m_cooldownReferences.Find(r => r.Enemy == enemy).LastUseTimeStamp = Time.time;
        }

        public void DeleteEnemyFromListing(EntityBase enemy)
        {
            m_cooldownReferences.Remove(m_cooldownReferences.Find(c => c.Enemy == enemy));
        }

        public void Clear()
        {
            m_cooldownReferences.Clear();
        }
    }

    [Serializable]
    public class EnemyCooldownReference
    {
        public Enemy Enemy;
        public float LastUseTimeStamp;
    }
}
