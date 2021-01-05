using Bladiator.Entities;
using Bladiator.Entities.Enemies;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Managers.EnemyManager
{
	public class EnemyManager : MonoBehaviour
	{
		public static EnemyManager Instance;

		[Tooltip("The delay between creating a group, and initiating it's attack.")]
		[SerializeField] private float m_GroupAttackDelay = 3f;

		private List<Enemy> m_ActiveEnemies = new List<Enemy>();
		[SerializeField] private List<EnemyGroup> m_ActiveGroups = new List<EnemyGroup>();

		public Action OnAllEnemiesDied;

		void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
		}

		private void Start()
		{
			GameManager.Instance.ResetEvent += ResetEvent;
		}

		private void ResetEvent()
		{
			foreach (Enemy enemy in m_ActiveEnemies)
			{
				Destroy(enemy.gameObject);
			}

			m_ActiveGroups.Clear();
			m_ActiveEnemies.Clear();
		}

		/// <summary>
		/// Remove "enemyToRemove" from the "m_ActiveEnemies" & "m_ActiveGroups" lists.
		/// </summary>
		public void RemoveEnemyFromLisitings(EntityBase enemyToRemove)
		{
			Enemy enemy = (Enemy) enemyToRemove;

			m_ActiveGroups.Find(g => g.ID == enemy.GetGroupID()).Enemies.Remove(enemy);
		}

		/// <summary>
		/// Create a new group and add "enemy" to it, returns the new groups "ID".
		/// </summary>
		/// <param name="enemy"></param>
		/// <returns>The new groups ID.</returns>
		public int CreateGroup(Enemy enemy)
		{
			EnemyGroup newGroup = new EnemyGroup()
			{
				ID = m_ActiveGroups.Count,
				Enemies = new List<Enemy>() {enemy}
			};
			StartCoroutine(newGroup.InitiateAttack(m_GroupAttackDelay));
			m_ActiveGroups.Add(newGroup);

			return m_ActiveGroups.Count - 1;
		}

		/// <summary>
		/// Add "enemyToAdd" to the group with ID "groupToAddTo".
		/// </summary>
		public void AddEnemyToGroup(Enemy enemyToAdd, int groupToAddTo)
		{
			enemyToAdd.SetGroupID(groupToAddTo);

			m_ActiveGroups.Find(g => g.ID == groupToAddTo).Enemies.Add(enemyToAdd);
			if (m_ActiveGroups.Count <= 0) OnAllEnemiesDied?.Invoke();
		}

		/// <summary>
		/// Get the EnemyGroup with the ID "groupId".
		/// </summary>
		/// <returns>The found group.</returns>
		public EnemyGroup GetEnemyGroup(int groupId)
		{
			return m_ActiveGroups.Find(g => g.ID == groupId);
		}

		// Add and get for "m_ActiveEnemies".
		public void AddEnemy(Enemy enemyToAdd)
		{
			m_ActiveEnemies.Add(enemyToAdd);
			enemyToAdd.OnDeath += OnDeath;
		}

		private void OnDeath(EntityBase entity)
		{
			if (entity.TryGetComponent(out Enemy enemy))
			{
				m_ActiveEnemies.Remove(enemy);

				if (m_ActiveEnemies.Count <= 0) OnAllEnemiesDied?.Invoke();
			}
		}

		public List<Enemy> GetActiveEnemies()
		{
			return m_ActiveEnemies;
		}
	}

	/// <summary>
	/// The object that contains a group of enemies. ( GetGatheringPosition() & InitiateAttack() )
	/// </summary>
	[Serializable]
	public struct EnemyGroup
	{
		// The ID to the group.
		public int ID;

		// The active enemies in the group.
		public List<Enemy> Enemies;

		// The gathering position of the group.
		private Vector3 GatheringPosition;

		/// <summary>
		/// Calculate the "m_GatheringPosition" for the group and return it.
		/// </summary>
		public Vector3 GetGatheringPosition()
		{
			// Calculate the mean position of all the active enemies withing the group.

			Vector3 meanPosition = Vector3.zero;

			foreach (Enemy enemy in Enemies)
			{
				meanPosition += enemy.transform.position;
			}

			meanPosition /= Enemies.Count;

			return meanPosition;
		}

		/// <summary>
		/// Make all the enemies in the group attack their nearest player.
		/// </summary>
		public IEnumerator InitiateAttack(float delay)
		{
			yield return new WaitForSeconds(delay);

			foreach (Enemy enemy in Enemies)
			{
				enemy.SetState(EnemyState.MOVE_TOWARDS_PLAYER);
			}
		}
	}
}