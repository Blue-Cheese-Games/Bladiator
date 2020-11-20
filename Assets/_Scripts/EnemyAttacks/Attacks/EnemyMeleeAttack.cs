using Bladiator.Entities.Enemies;
using Bladiator.Entity;
using Bladiator.Entity.Player;
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
            player.GetComponent<IDamageable>().Damage(GetStats().Damage);

            // push the player back with knockback.
            Vector3 direction = player.transform.position - transform.position;
            player.GetComponent<Rigidbody>().AddForce(direction * GetStats().Knockback, ForceMode.Impulse);
        }
    }
}
