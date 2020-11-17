using Bladiator.Entities;
using Bladiator.Entity;
using Bladiator.Entity.Player;
using Bladiator.Managers;
using Bladiator.Managers.EnemyManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Entities.Enemies
{
    [RequireComponent(typeof(Rigidbody))]
    public class Enemy : EntityBase
    {
        [Header("Enemy Settings")]
        [Tooltip("The range in units in which enemies will be added to this enemy's group.")]
        [SerializeField] private float m_GroupingRange;
        [Tooltip("The amount of degrees that this enemy can turn every second.")]
        [SerializeField] private float rotationSpeed;

        [Header("Attacks:")]

        // Standard "Melee Attack"
        [SerializeField] private int m_MeleeDamage = 2;
        [SerializeField] private int m_MeleeKnockback = 2;
        [SerializeField] private float m_MeleeCooldown = 2f;
        [SerializeField] private float m_MeleeRecoveryTime = 1f;
        private float m_MeleeLastUseTimeStamp = 0f;

        private Player m_TargetPlayer = null; // Could be changed to Player type later.
        private Rigidbody m_RigidBody = null;

        private EnemyState m_State = EnemyState.LOOKING_FOR_GROUP;
        private EnemyManager m_EnemyManager;

        private Vector3 m_GroupGatheringPoint = Vector3.zero;
        private int m_GroupID = -1; // -1 means "no group".

        private float m_currentAttackRecoveryTime = 0f;

        protected override void Awake()
        {
            base.Awake();

            m_RigidBody = GetComponent<Rigidbody>();

            OnDeath += EnemyDeath;

            m_EnemyManager = FindObjectOfType<EnemyManager>();
            m_EnemyManager.AddEnemy(this);

            FindNearsestPlayerAndSetAsTarget();
        }

        

        protected override void Update()
        {
            switch (m_State)
            {
                // -- Movement --
                case EnemyState.MOVE_TOWARDS_PLAYER:
                    MoveForward();
                    LookAtTarget(m_TargetPlayer.transform.position);

                    break;

                case EnemyState.GROUP_WITH_OTHERS:
                    GroupUpWithGroup();
                    break;

                // -- Grouping --
                case EnemyState.LOOKING_FOR_GROUP:
                    InitializeGroup();
                    break;

                // -- Attacking --
                case EnemyState.RECOVERING_FROM_ATTACK:
                    RecoverFromAttack();
                    break;
            }

            AttackCheck();
        }

        /// <summary>
        /// Move towards "target".
        /// </summary>
        private void MoveForward()
        {
            // Move towards the target.

            m_RigidBody.transform.localPosition = (transform.position + (transform.forward * Time.deltaTime) * m_Movespeed);
        }

        /// <summary>
        /// Makes sure the enemy is assigned and setup in a group, if the enemy is not already assigned and setup in one, it creates a new group.
        /// </summary>
        private void InitializeGroup()
        {
            // If the enemy already has a group, it doesn't need to find one. (-1 means "no group").
            if (m_GroupID == -1)
            {
                // Not assigned to a group, create one instead.

                // Get the currently active enemies.
                List<Enemy> activeEnemies = m_EnemyManager.GetActiveEnemies();

                // Create a group and get it's ID.
                m_GroupID = m_EnemyManager.CreateGroup(this);

                float farthestDistance = float.MaxValue;

                // Check every enemy if it's close enough to join a group, and if it is looking for a group.
                foreach (Enemy otherEnemy in activeEnemies)
                {
                    // Check if the enemy that is currently being checked is not this enemy itself.
                    if (otherEnemy == this) { continue; }

                    // Check if the other enemy is looking for a group.
                    if (otherEnemy.GetState() != EnemyState.LOOKING_FOR_GROUP) { continue; }

                    // Check if the other enemy is within grouping range. 
                    // (grouping range is measured from the mean position of the group).
                    float distanceBetweenGatheringPositionAndEnemy = Vector3.Distance(m_EnemyManager.GetEnemyGroup(m_GroupID).GetGatheringPosition(), otherEnemy.transform.position);

                    if (distanceBetweenGatheringPositionAndEnemy > m_GroupingRange) { continue; }

                    // Other enemy is withing grouping range.
                    m_EnemyManager.AddEnemyToGroup(otherEnemy, m_GroupID);
                }
            }

            SetState(EnemyState.GROUP_WITH_OTHERS);
            m_GroupGatheringPoint = m_EnemyManager.GetEnemyGroup(m_GroupID).GetGatheringPosition();
        }

        /// <summary>
        /// Move towards the groups gathering point.
        /// </summary>
        private void GroupUpWithGroup()
        {
            // Get the "m_GroupGatheringPoint" if it is not already set.
            if (m_GroupGatheringPoint == null) 
            { 
                m_GroupGatheringPoint = m_EnemyManager.GetEnemyGroup(m_GroupID).GetGatheringPosition(); 
            }

            // Move towards the "m_GroupGatheringPoint".
            MoveForward();
            LookAtTarget(m_GroupGatheringPoint);


        }

        private void LookAtTarget(Vector3 target)
        {
            // Look at "target".

            // Get the direction from this object to the target.
            Vector3 direction = target - transform.position;

            // Set Y to 0 as we don't want to rotate up- or downwards.
            direction.y = 0;

            // Get the rotation that would make the object look at "target".
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Lerp from this object's current rotation to the "targetRotation" using "rotationSpeed".
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, (rotationSpeed / 360));
        }

        private void FindNearsestPlayerAndSetAsTarget()
        {
            // Find the nearest player and set it as "m_TargetPlayer".
            List<Player> players = GameManager.Instance.GetPlayers();

            float shortestDistance = 1000f;
            float currentDistance = 0;

            foreach (Player player in players)
            {
                currentDistance = Vector3.Distance(player.transform.position, transform.position);
                if (currentDistance < shortestDistance)
                {
                    m_TargetPlayer = player;
                    shortestDistance = currentDistance;
                }
            }
        }

        /// <summary>
        /// Checks if one or multiple attacks can be activated, Runs every frame, overridable for different attacks / patterns.
        /// </summary>
        protected virtual void AttackCheck()
        {
            if(GetState() == EnemyState.RECOVERING_FROM_ATTACK) { return; }

            // Standard "Melee Attack".
            if((m_MeleeLastUseTimeStamp + m_MeleeCooldown) < Time.time)
            {
                if(Vector3.Distance(transform.position, m_TargetPlayer.transform.position) < 1.5f)
                {
                    // Damage the player.
                    m_TargetPlayer.GetComponent<IDamageable>().Damage(m_MeleeDamage);

                    // push the player back with knockback.
                    Vector3 direction = m_TargetPlayer.transform.position - transform.position;
                    m_TargetPlayer.GetComponent<Rigidbody>().AddForce(direction * m_MeleeKnockback, ForceMode.Impulse);

                    // Set the timestamp of this use.
                    m_MeleeLastUseTimeStamp = Time.time;

                    m_currentAttackRecoveryTime = m_MeleeRecoveryTime;
                    SetState(EnemyState.RECOVERING_FROM_ATTACK);

                }
            }
        }

        protected virtual void RecoverFromAttack()
        {
            m_currentAttackRecoveryTime -= Time.deltaTime;

            if(m_currentAttackRecoveryTime <= 0)
            {
                SetState(EnemyState.MOVE_TOWARDS_PLAYER);
            }
        }

        protected virtual void EnemyDeath(EntityBase enemy)
        {
            Destroy(gameObject);
        }

        public void SetGroupID(int newGroupID)
        {
            m_GroupID = newGroupID;
        }

        public int GetGroupID()
        {
            return m_GroupID;
        }

        public EnemyState GetState()
        {
            return m_State;
        }

        public void SetState(EnemyState newState)
        {
            m_State = newState;

            switch (m_State)
            {
                case EnemyState.MOVE_TOWARDS_PLAYER:
                    FindNearsestPlayerAndSetAsTarget();
                    break;
            }
        }   
    }

    public enum EnemyState
    {
        // Movement ---
        MOVE_TOWARDS_PLAYER,

        // Grouping --
        LOOKING_FOR_GROUP,
        GROUP_WITH_OTHERS,

        // Attacking ---
        ATTACK,
        RECOVERING_FROM_ATTACK
    }
}
