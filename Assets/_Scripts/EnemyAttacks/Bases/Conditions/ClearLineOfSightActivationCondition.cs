using Bladiator.Collisions;
using Bladiator.Entities.Enemies;
using Bladiator.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.EnemyAttacks
{
    [CreateAssetMenu(fileName = "new LineOfSight", menuName = "Bladiator/ActivationConditions/LineOfSight")]
    [System.Serializable]
    public class ClearLineOfSightActivationCondition : BaseActivationCondition
    {
        [Tooltip("Layers that will be ignored when performing the clear line of sight check.")]
        [SerializeField] LayerMask m_IgnoredLayers;

        public override void Initialize(EnemyAttackBase attack, Enemy enemy)
        {

        }

        public override bool CheckCondition(EnemyAttackBase attack, Enemy enemy, Player targetPlayer)
        {
            // Check if there is a clear line of sight from this enemy to it's target player.
            if (!CollisionCheck.CheckForCollision(enemy.transform.position, targetPlayer.transform.position, m_IgnoredLayers))
            {
                // Clear line of sight.
                return true;
            }
            else
            {
                // There is an obstacle in the way.
                return false;
            }
        }
    }
}