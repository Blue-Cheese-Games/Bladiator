﻿using Bladiator.EnemyAttacks;
using Bladiator.Entities.Enemies;
using Bladiator.Entity.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackBase : MonoBehaviour
{
    public System.Action OnUpdate;
    public System.Action OnActivate;

    // The reference to the EnemyAttackStats object for this attack.
    [SerializeField] private EnemyAttackStats m_Stats;

    // The list containing all the activation conditions for this attack.
    [SerializeField] protected List<BaseActivationCondition> m_ActivationConditions = new List<BaseActivationCondition>();


    private void Awake()
    {
        InitializeConditions();
    }

    private void Update()
    {
        OnUpdate?.Invoke();
    }

    /// <summary>
    ///  Try to activate this attack.
    /// </summary>
    /// <returns>true if activated, false if not.</returns>
    public bool TryActivate(Enemy enemy, Player player)
    {
        // Check every activation condition.
        foreach (BaseActivationCondition condition in m_ActivationConditions)
        {
            // Check if every condition has been met.
            if (!condition.CheckCondition(this, player))
            {
                // A condition has not been met, don't activate the attack.
                return false;
            }
        }

        Activate(enemy, player);
        OnActivate?.Invoke();

        return true;
    }

    public EnemyAttackStats GetStats()
    {
        return m_Stats;
    }

    private void InitializeConditions()
    {
        foreach (BaseActivationCondition con in m_ActivationConditions)
        {
            con.Initialize(this);
        }
    }

    /// <summary>
    /// The attack itself, override this function in custom attacks to decide the behaviour of the actual attack.
    /// </summary>
    /// <param name="enemy">The enemy that is attacking (this.gameobject.Enemy)</param>
    /// <param name="player">The target player of this enemy</param>
    protected virtual void Activate(Enemy enemy, Player player)
    {
        Debug.LogError("\"EnemyAttackBase\"'s activation was called, make sure to override this method for enemy's attacks.");
    }
}