using Bladiator.EnemyAttacks;
using Bladiator.Entity.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.EnemyAttacks
{
    [CreateAssetMenu(fileName = "new DistanceCheck", menuName = "Bladiator/ActivationConditions/Distance")]
    public class DistanceActivationCheck : BaseActivationCondition
    {
        [Tooltip("From how far away can the attack activate?")]
        public float m_Distance = 2f;

        public override bool CheckCondition(EnemyAttackBase attack, Player targetPlayer)
        {
            if (Vector3.Distance(attack.transform.position, targetPlayer.transform.position) <= m_Distance)
            {
                return true;
            }

            return false;
        }
    }
}
