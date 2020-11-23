using Bladiator.Entities.Players;
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

        private float m_LastUseTimeStamp = 0f;

        public override void Initialize(EnemyAttackBase attack)
        {
            m_LastUseTimeStamp = 0;
            attack.OnActivate += OnActivate;
        }

        public override bool CheckCondition(EnemyAttackBase attack, Player targetPlayer)
        {
            if ((m_LastUseTimeStamp + m_Cooldown) < Time.time)
            {
                return true;
            }

            return false;
        }

        public void OnActivate()
        {
            m_LastUseTimeStamp = Time.time;
        }
    }
}
