using Bladiator.Collisions;
using Bladiator.Managers;
using Bladiator.Managers.EnemyManager;
using Bladiator.Pathing;
using Bladiator.Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Entities.Enemies
{
    public class GrenadeLobber : Enemy
    {
		[Tooltip("If the enemy gets this close to the player, it will walk away from the player.")]
		[SerializeField] private float m_MinimalDistanceToPlayer = 8;

		[Tooltip("How far away from the target player can this enemy move before it walks towards the player again.")]
		[SerializeField] private float m_MaximumDistanceFromPlayer = 14;
		
		private GrenadeLobberExtraState m_ExtraState = GrenadeLobberExtraState.MOVE_TOWARDS_PLAYER;

		private Grenade m_ThrownGrenade;
		
		private Vector3 m_MoveAwayFromGrenadeTarget;
		private Vector3 m_MoveAwayFromPlayerTarget;

		private bool m_EnableSpecialMovement = false;

		protected override void Update()
		{
			if (GameManager.Instance.GameState == GameState.Pause ||
				GameManager.Instance.GameState == GameState.Ending ||
				GameManager.Instance.GameState == GameState.PlayersDied) return;

			switch (m_State)
			{
				// -- Movement --
				case EnemyState.MOVE_TOWARDS_PLAYER:
					MoveTowardsPlayer();
					StuckDetection();
                    m_Animator.Play("run");
                    break;

				case EnemyState.FOLLOWING_PATH:
					FollowAlongPath();
					StuckDetection();
                    m_Animator.Play("run");
                    break;

				case EnemyState.GROUP_WITH_OTHERS:
					GroupUpWithGroup();
					StuckDetection();
                    m_Animator.Play("run");
                    break;

				// -- Grouping --
				case EnemyState.LOOKING_FOR_GROUP:
					InitializeGroup();
                    m_Animator.Play("idle");
                    break;

				// -- Attacking --
				case EnemyState.RECOVERING_FROM_ATTACK:
					RecoverFromAttack();
                    m_Animator.Play("idle");
                    break;
			}

			AttackCheck();
		}

		protected override void MoveTowardsPlayer()
		{
			if (!m_EnableSpecialMovement)
			{
				base.MoveTowardsPlayer();
				return;
			}

			Vector3 target = GetTarget();

			if (Vector3.Distance(transform.position, target) < 0.05f)
			{
				if(m_ExtraState == GrenadeLobberExtraState.MOVE_AWAY_FROM_PLAYER || m_ExtraState == GrenadeLobberExtraState.MOVE_AWAY_FROM_GRENADE)
				{
					SetExtraState(GrenadeLobberExtraState.MOVE_TOWARDS_PLAYER);
				}

                m_Animator.Play("idle");
                return;
			}

			LookAtTarget(target);
			MoveForward();

			// Adjustment as the enemy can walk into an obstacle, and the the raycast won't detect the obstacle.
			Vector3 adjustedPosition = transform.position - transform.forward * 0.3f;
			if (CollisionCheck.CheckForCollision(adjustedPosition, target,
				PathingManager.Instance.GetIgnoreLayers()))
			{
				// Reset the path to the player by setting the state to move towards the player.
				SetState(EnemyState.MOVE_TOWARDS_PLAYER);
			}
		}

		protected override void AttackCheck()
		{
			foreach (EnemyAttackBase attack in m_Attacks)
			{
				if (attack.TryActivate(this, m_TargetPlayer))
				{
					// Attack was activated.
					m_CurrentAttackRecoveryTime = attack.GetStats().RecoveryTime;
					SetState(EnemyState.RECOVERING_FROM_ATTACK);
					
					// Overrides:
					// Set the extra state to move away from the grenade.
					SetExtraState(GrenadeLobberExtraState.MOVE_AWAY_FROM_GRENADE);
					m_EnableSpecialMovement = true;
				}
			}
		}

		private Vector3 GetTarget()
		{
			switch (m_ExtraState)
			{
				case GrenadeLobberExtraState.MOVE_TOWARDS_PLAYER:
					m_MoveAwayFromPlayerTarget = Vector3.zero;

					if (Vector3.Distance(transform.position, m_TargetPlayer.transform.position) < m_MinimalDistanceToPlayer)
					{
						SetExtraState(GrenadeLobberExtraState.MOVE_AWAY_FROM_PLAYER);
					}

					return m_TargetPlayer.transform.position;

				case GrenadeLobberExtraState.MOVE_AWAY_FROM_GRENADE:
					if(m_MoveAwayFromGrenadeTarget == Vector3.zero)
					{
						m_MoveAwayFromGrenadeTarget = GetPositionFromInvertedDirectionContinued(transform.position, m_ThrownGrenade.transform.position);
					}
					return m_MoveAwayFromGrenadeTarget;

				case GrenadeLobberExtraState.MOVE_AWAY_FROM_PLAYER:
					if (m_MoveAwayFromPlayerTarget == Vector3.zero)
					{
						m_MoveAwayFromPlayerTarget = GetPositionFromInvertedDirectionContinued(transform.position, m_TargetPlayer.transform.position);
					}

					if(Vector3.Distance(transform.position, m_TargetPlayer.transform.position) > m_MaximumDistanceFromPlayer)
					{
						SetExtraState(GrenadeLobberExtraState.MOVE_TOWARDS_PLAYER);
					}
					return m_MoveAwayFromPlayerTarget;
			}

			return Vector3.zero;
		}

		private Vector3 GetPositionFromInvertedDirectionContinued(Vector3 from, Vector3 to)
		{
			// Get the position to the grenade, but with the enemies Y position.
			Vector3 toLocation = new Vector3(to.x, from.y, to.z);

			// Get the direction from the enemy to the grenade.
			Vector3 direction = toLocation - from;

			// Invert the direction to face away from the grenade.
			direction *= -1;

			RaycastHit hit;
			Ray r = new Ray(from, direction);

			LayerMask mask = ~(PathingManager.Instance.GetIgnoreLayers() - LayerMask.GetMask("Player_Only"));

			Physics.Raycast(r, out hit, 10, mask);

			return r.GetPoint(hit.distance - 0.5f);
		}

		// Called when a grenade is thrown, so the lobber can move away from it.
		public void SetThrownGrenade(Grenade grenade)
		{
			m_ThrownGrenade = grenade;
		}

		public void SetExtraState(GrenadeLobberExtraState newExtraState)
		{
			m_ExtraState = newExtraState;

			switch (m_ExtraState)
			{
				case GrenadeLobberExtraState.MOVE_TOWARDS_PLAYER:
					m_MoveAwayFromGrenadeTarget = Vector3.zero;
					break;
			}
		}
	}
	public enum GrenadeLobberExtraState
	{
		MOVE_TOWARDS_PLAYER,
		MOVE_AWAY_FROM_GRENADE,
		MOVE_AWAY_FROM_PLAYER
	}
}