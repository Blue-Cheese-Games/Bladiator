using Bladiator.Entities;
using Bladiator.Pathing;
using Bladiator.Entities.Players;
using Bladiator.Managers;
using Bladiator.Managers.EnemyManager;
using System;
using System.Collections;
using System.Collections.Generic;
using Bladiator.CameraController;
using UnityEngine;
using Bladiator.Collisions;

namespace Bladiator.Entities.Enemies
{
	[RequireComponent(typeof(Rigidbody), typeof(PathFinder))]
	public class Enemy : EntityBase
	{
		[Header("Enemy Settings")]
		[Tooltip("The range in units in which enemies will be added to this enemy's group.")]
		[SerializeField] private float m_GroupingRange;

		[Tooltip("The amount of degrees that this enemy can turn every second.")]
		[SerializeField] private float m_RotationSpeed;

		[Space()]
		[Tooltip("The objects that contains all the attacks for this enemy.")]
		[SerializeField] private GameObject m_AttacksParent = null;

		[Tooltip("The interval at which the enemy will reroute it's path to find it's target player.")]
		[SerializeField] private float m_ReroutePathInterval;

		[Tooltip("The score to be added to the total score when this enemy dies.")]
		[SerializeField] private int m_ScoreOnDeath;

		protected Animator m_Animator;

		public Animator Animator => m_Animator;
		
		// Attacks
		protected List<EnemyAttackBase> m_Attacks = new List<EnemyAttackBase>();

		public List<EnemyAttackBase> Attacks => m_Attacks;
		
		public float m_CurrentAttackRecoveryTime = 0f;

		// Components on the enemy.
		protected Player m_TargetPlayer = null;
		private Rigidbody m_RigidBody = null;
		private PathFinder m_PathFinder = null;

		private EnemyManager m_EnemyManager;

		protected EnemyState m_State = EnemyState.LOOKING_FOR_GROUP;

		// Grouping
		private Vector3 m_GroupGatheringPoint = Vector3.zero;
		private int m_GroupID = -1; // -1 means "no group".

		// Pathing
		protected Stack<PathNode> m_PathTowardsPlayer = new Stack<PathNode>();
		protected Coroutine m_ReroutingCoroutine;

		// Stuck detection / prevention.
		// A list with timestamps of when the enemy starts pathing. used for detecting if the enemy is stuck.
		protected List<float> m_StartPathingTimestamps = new List<float>();
		protected int m_LastTimeStampSecond;

		protected override void Start()
		{
			base.Start();

			m_RigidBody = GetComponent<Rigidbody>();

			OnDeath += EnemyDeath;

			m_PathFinder = GetComponent<PathFinder>();
			m_EnemyManager = EnemyManager.Instance;
			m_EnemyManager.AddEnemy(this);

			FindNearsestPlayerAndSetAsTarget();

			m_Animator = GetComponentInChildren<Animator>();

			// Get all the attacks from the "m_AttacksParent" object.
			m_AttacksParent.GetComponentsInChildren<EnemyAttackBase>(m_Attacks);
			foreach (EnemyAttackBase attack in m_Attacks)
			{
				attack.InitializeConditions(this);
			}

			GameManager.Instance.ResetEvent += ResetEvent;
		}

		private void ResetEvent()
		{
			Destroy(gameObject);
		}

		private void OnDestroy()
		{
			GameManager.Instance.ResetEvent -= ResetEvent;
		}

		protected virtual void Update()
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
					Animator.speed = 1;
					m_Animator.Play("run");
					break;

				case EnemyState.FOLLOWING_PATH:
					FollowAlongPath();
					StuckDetection();
					Animator.speed = 1;
					m_Animator.Play("run");
					break;

				case EnemyState.GROUP_WITH_OTHERS:
					GroupUpWithGroup();
					StuckDetection();
					Animator.speed = 1;
					m_Animator.Play("run");
					break;

				// -- Grouping --
				case EnemyState.LOOKING_FOR_GROUP:
					InitializeGroup();
					Animator.speed = 1;
					m_Animator.Play("idle");
					break;

				// -- Attacking --
				case EnemyState.RECOVERING_FROM_ATTACK:
					RecoverFromAttack();
					break;
			}

			AttackCheck();
		}

	#region Non-overrideble but calleable methods for derived enemies

		/// <summary>
		/// Move towards "target".
		/// </summary>
		protected void MoveForward()
		{
			m_RigidBody.transform.localPosition =
				(transform.position + (transform.forward * Time.deltaTime) * m_Movespeed);
		}

		/// <summary>
		/// Makes sure the enemy is assigned and setup in a group, if the enemy is not already assigned and setup in one, it creates a new group.
		/// </summary>
		protected void InitializeGroup()
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
					if (otherEnemy == this)
					{
						continue;
					}

					// Check if the other enemy is looking for a group.
					if (otherEnemy.GetState() != EnemyState.LOOKING_FOR_GROUP)
					{
						continue;
					}

					// Check if the other enemy is within grouping range. 
					// (grouping range is measured from the mean position of the group).
					float distanceBetweenGatheringPositionAndEnemy = Vector3.Distance(
						m_EnemyManager.GetEnemyGroup(m_GroupID).GetGatheringPosition(), otherEnemy.transform.position);

					if (distanceBetweenGatheringPositionAndEnemy > m_GroupingRange)
					{
						continue;
					}

					// Check if there is a collision between the grouping point and the other enemy.
					if (CollisionCheck.CheckForCollision(m_EnemyManager.GetEnemyGroup(m_GroupID).GetGatheringPosition(),
						otherEnemy.transform.position))
					{
						continue;
					}

					// Other enemy is withing grouping range.
					m_EnemyManager.AddEnemyToGroup(otherEnemy, m_GroupID);
				}
			}

			SetState(EnemyState.GROUP_WITH_OTHERS);
			m_GroupGatheringPoint = m_EnemyManager.GetEnemyGroup(m_GroupID).GetGatheringPosition();
		}

		protected void LookAtTarget(Vector3 target)
		{
			// Look at "target".

			// Get the direction from this object to the target.
			Vector3 direction = target - transform.position;

			// Set Y to 0 as we don't want to rotate up- or downwards.
			direction.y = 0;

			// Get the rotation that would make the object look at "target".
			Quaternion targetRotation = Quaternion.LookRotation(direction);

			// Lerp from this object's current rotation to the "targetRotation" using "rotationSpeed".
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, (m_RotationSpeed / 360));
		}

		protected void FindNearsestPlayerAndSetAsTarget()
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
		/// Move towards the groups gathering point.
		/// </summary>
		protected void GroupUpWithGroup()
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

		/// <summary>
		/// Coroutine for rerouting the PathFinder to target the player again.
		/// </summary>
		/// <returns></returns>
		protected IEnumerator RerouteInterval()
		{
			while (GetState() == EnemyState.FOLLOWING_PATH)
			{
				yield return new WaitForSeconds(m_ReroutePathInterval);
				m_PathFinder.RerouteToGoal(m_TargetPlayer.transform.position);
			}
		}

	#endregion

	#region Methods that can be overridden by derived enemies

		protected virtual void MoveTowardsPlayer()
		{
			LookAtTarget(m_TargetPlayer.transform.position);
			MoveForward();

			// Adjustment as the enemy can walk into an obstacle, and the the raycast won't detect the obstacle.
			Vector3 adjustedPosition = transform.position - transform.forward * 0.3f;
			if (CollisionCheck.CheckForCollision(adjustedPosition, m_TargetPlayer.transform.position,
				PathingManager.Instance.GetIgnoreLayers()))
			{
				// Reset the path to the player by setting the state to move towards the player.
				SetState(EnemyState.MOVE_TOWARDS_PLAYER);
			}
		}

		protected virtual void FollowAlongPath()
		{
			// If there is no path yet, get it.
			if (m_PathTowardsPlayer.Count <= 0)
			{
				m_PathTowardsPlayer = m_PathFinder.FindPath(transform.position, m_TargetPlayer.transform.position);
			}

			if (m_PathTowardsPlayer.Count <= 0)
			{
				return;
			}

			Vector3 ownPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
			Vector3 nextNodePosFlat = new Vector3(m_PathTowardsPlayer.Peek().transform.position.x, 0, m_PathTowardsPlayer.Peek().transform.position.z);

			if (Vector3.Distance(ownPosFlat, nextNodePosFlat) < 1)
			{
				m_PathTowardsPlayer.Pop();
				if (m_PathTowardsPlayer.Count <= 0)
				{
					return;
				}
			}

			LookAtTarget(m_PathTowardsPlayer.Peek().transform.position);
			MoveForward();

			if (!CollisionCheck.CheckForCollision(transform.position, m_TargetPlayer.transform.position,
				PathingManager.Instance.GetIgnoreLayers()))
			{
				SetState(EnemyState.MOVE_TOWARDS_PLAYER);
			}
		}

		/// <summary>
		/// Checks if one or multiple attacks can be activated, Runs every frame, overridable for different attacks / patterns.
		/// </summary>
		protected virtual void AttackCheck()
		{
			foreach (EnemyAttackBase attack in m_Attacks)
			{
				if (attack.TryActivate(this, m_TargetPlayer))
				{
					// Attack was activated.
					m_CurrentAttackRecoveryTime = attack.GetStats().RecoveryTime;
					SetState(EnemyState.RECOVERING_FROM_ATTACK);
				}
			}
		}

		protected virtual void RecoverFromAttack()
		{
			m_CurrentAttackRecoveryTime -= Time.deltaTime;

			if (m_CurrentAttackRecoveryTime <= 0)
			{
				SetState(EnemyState.MOVE_TOWARDS_PLAYER);
			}
		}

		protected virtual void EnemyDeath(EntityBase enemy)
		{
			CamShake.Instance.ShakeCamera(.25f, 0.09f);
			ScoreManager.Instance.AddScore(m_ScoreOnDeath);
			Destroy(gameObject);
		}

		protected virtual void StuckDetection()
		{
			if(m_LastTimeStampSecond < Mathf.Floor(Time.time))
			{
				if(m_StartPathingTimestamps.Count > 4)
				{
					// The enemy is stuck.

					transform.Rotate(0, 180, 0);
				}

				m_StartPathingTimestamps.Clear();
				m_LastTimeStampSecond = (int)Mathf.Floor(Time.time);
			}
		}

	#endregion

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

					// Adjustment as the enemy can into an obstacle, and the the raycast won't detect the obstacle.
					Vector3 adjustedPosition = transform.position - transform.forward * 0.3f; 
					// Check if there is a clear line of sight from this enemy to the target player.
					if (CollisionCheck.CheckForCollision(adjustedPosition, m_TargetPlayer.transform.position,
						PathingManager.Instance.GetIgnoreLayers()))
					{
						// There is a collision, pathfind towards the player.
						m_State = EnemyState.FOLLOWING_PATH;
						m_PathTowardsPlayer = new Stack<PathNode>();

						if (m_ReroutingCoroutine != null)
						{
							StopCoroutine(m_ReroutingCoroutine);
						}

						// Stuck detection / prevention.
						m_StartPathingTimestamps.Add(Time.time);

						m_ReroutingCoroutine = StartCoroutine(RerouteInterval());
					}

					break;
			}
		}

		public override void Knockback(Vector3 knockback, float knockbackDuration)
		{
			if (m_State == EnemyState.STUNNED) { return; }

			m_RigidBody.velocity = Vector3.zero;
			m_RigidBody.AddForce(knockback, ForceMode.Impulse);
			SetState(EnemyState.STUNNED);
			StartCoroutine(ResetAllowedToMove(knockbackDuration));
		}

		protected override IEnumerator ResetAllowedToMove(float delay)
		{
			yield return new WaitForSeconds(delay);
			SetState(EnemyState.MOVE_TOWARDS_PLAYER);
		}
	}

	public enum EnemyState
	{
		// Movement ---
		MOVE_TOWARDS_PLAYER,
		FOLLOWING_PATH,
		STUNNED,

		// Grouping --
		LOOKING_FOR_GROUP,
		GROUP_WITH_OTHERS,

		// Attacking ---
		ATTACK,
		RECOVERING_FROM_ATTACK
	}
}