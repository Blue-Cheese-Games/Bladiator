using Bladiator.Entities.Enemies;
using Bladiator.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.EnemyAttacks
{
    [System.Serializable]
    public class BaseActivationCondition : ScriptableObject
    {
        /// <summary>
        /// Initialize the ActivationCondition (overwritten by attacks to contains everything it needs).
        /// </summary>
        /// <param name="attack"> The attack itself </param>
        public virtual void Initialize(EnemyAttackBase attack, Enemy enemy)
        {

        }

        /// <summary>
        /// Checks the condition for this activation, overridable by child conditions.
        /// </summary>
        /// <param name="attack">The attack itself </param>
        /// <param name="targetPlayer">The target player of the Enemy </param>
        /// <returns></returns>
        public virtual bool CheckCondition(EnemyAttackBase attack, Enemy enemy, Player targetPlayer)
        {
            Debug.LogError("The base function \"CheckCondition\" was called, this should be overridden, make sure it is.");
            return false;
        }
    }
}
