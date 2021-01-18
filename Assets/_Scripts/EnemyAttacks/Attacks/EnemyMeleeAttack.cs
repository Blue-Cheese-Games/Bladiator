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
        private Player m_Player;
        
        // Override "Activate" to do a melee attack.
        protected override void Activate(Enemy enemy, Player player)
        {
            enemy.Animator.speed = 2.75f;
            enemy.Animator.Play("Attack");

            m_Player = player;
        }

        public void DoAttack()
        {
            // Damage the player.
            m_Player.Damage(GetStats().Damage);

            // push the player back with knockback.
            Vector3 direction = m_Player.transform.position - transform.position;
            direction.y = 0;
            m_Player.Knockback(direction.normalized * GetStats().Knockback, GetStats().KnockbackDuration);
        }
    }
}
