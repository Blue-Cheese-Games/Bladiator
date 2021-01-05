using UnityEngine;

namespace Bladiator.EnemyAttacks
{
    [CreateAssetMenu(fileName = "new EnemyAttackStats", menuName = "Bladiator/EnemyAttackStats", order = 0)]
    public class EnemyAttackStats : ScriptableObject
    {
        [Header("Stats:")]
        [Tooltip("How much damage does the attack do?")]
        public int Damage = 1;

        [Tooltip("How much does the attack knock the player away from the attacker?")]
        public int Knockback = 10;

        [Tooltip("How long will the knockback lock the target's movement?")]
        public float KnockbackDuration;

        [Tooltip("For how long can the enemy not move / attack after using this attack?")]
        public float RecoveryTime = 1f;
    }
}
