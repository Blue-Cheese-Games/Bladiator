using Bladiator.Entities.Enemies;
using Bladiator.Entities;
using Bladiator.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.EnemyAttacks
{
    public class EnemyMeleeAttack : EnemyAttackBase
    {
        // Override "Activate" to do a melee attack.
        protected override void Activate(Enemy enemy, Player player)
        {
            // Damage the player.
            player.Damage(GetStats().Damage);

            // push the player back with knockback.
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0;
            player.Knockback(direction.normalized * GetStats().Knockback, GetStats().KnockbackDuration);
        }
    }
}
