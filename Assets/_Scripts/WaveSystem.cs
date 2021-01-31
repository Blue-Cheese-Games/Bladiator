using System;
using Bladiator.Entities;
using Bladiator.Entities.Enemies;
using Bladiator.Managers;
using Bladiator.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bladiator
{
	public class WaveSystem : MonoBehaviour
	{
		public static WaveSystem Instance;
		public Action OnSpawnStarted, OnSpawnDone;
		public Action<int> OnNextWave;

		[Range(0.01f, 2f)] [SerializeField] private float m_SpawnInterval = 0.25f;
		[SerializeField] private Vector3 m_DetectionBox;
		[SerializeField] private LayerMask m_DetectionMasks;
		[SerializeField] private int m_SpawnIncrease = 8;
		[SerializeField] private Transform[] m_SpawnPoints;
		[SerializeField] private Transform m_BossSpawnPoint;
		[SerializeField] private GameObject m_MiniBoss, m_Boss;

		[SerializeField] private SpawnableEntities[] m_SpawnableEntities;

		private bool m_IsSpawning;
		public bool IsSpawning => m_IsSpawning;

		private int m_TargetSpawnAmount;
		private int m_SpawnCount;

		private int m_WaveCount = 1;

		private float m_SpawnTimer;

		public int WaveCount => m_WaveCount;

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
		}

		void Start()
		{
			GameManager.Instance.ResetEvent += ResetEvent;
		}

		private void ResetEvent()
		{
			m_IsSpawning = false;
			m_SpawnCount = 0;
			m_WaveCount = 1;
			m_TargetSpawnAmount = 0;
		}

		void Update()
		{
		#if UNITY_EDITOR
			if (Input.GetKey(KeyCode.LeftControl) &&
			    (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)))
			{
				m_WaveCount++;
				print($"Current Wave: {m_WaveCount}");
			}

			if (Input.GetKey(KeyCode.LeftControl) &&
			    (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)))
			{
				if (m_WaveCount - 1 > 0)
				{
					m_WaveCount--;
					print($"Current Wave: {m_WaveCount}");
				}
			}
		#endif

			if (!m_IsSpawning || GameManager.Instance.GameState == GameState.Pause ||
			    GameManager.Instance.GameState == GameState.Ending ||
			    GameManager.Instance.GameState == GameState.PlayersDied) return;

			if (m_WaveCount % 5 == 0 || m_WaveCount % 10 == 0)
				SpawnBoss();
			else
				Spawner();
		}

		private void Spawner()
		{
			if (m_SpawnTimer >= m_SpawnInterval)
			{
				for (int i = 0; i < m_SpawnPoints.Length; i++)
				{
					if (m_SpawnCount >= m_TargetSpawnAmount)
					{
						StopSpawn();
						return;
					}

					Collider[] output = Physics.OverlapBox(m_SpawnPoints[i].position, m_DetectionBox / 2,
						m_SpawnPoints[i].rotation, m_DetectionMasks);
					if (output.Length > 0)
					{
						continue;
					}

					int index = -1;
					while (index == -1)
					{
						index = Random.Range(0, m_SpawnableEntities.Length);
						if (m_WaveCount < m_SpawnableEntities[index].m_WaveStartSpawning)
						{
							index = -1;
						}
					}

					Enemy e = Instantiate(m_SpawnableEntities[index].m_EnemyPrefab, m_SpawnPoints[i].position,
							m_SpawnPoints[i].rotation)
						.GetComponent<Enemy>();

					e.SetState(EnemyState.MOVE_TOWARDS_PLAYER);
					m_SpawnCount++;
				}

				m_SpawnTimer = 0;
			}
			else
			{
				m_SpawnTimer += Time.deltaTime;
			}
		}

		public void SpawnBoss()
		{
			GameObject target;
			int health;
			
			if (m_WaveCount % 10 == 0)
			{
				// Boss
				target = m_Boss;
				health = m_WaveCount * 2;
			}
			else
			{
				// Mini boss
				target = m_MiniBoss;
				health = (int)Mathf.Floor(m_WaveCount * 1.5f);
			}

			Enemy enemy = Instantiate(target, m_BossSpawnPoint.position, m_BossSpawnPoint.rotation).GetComponent<Enemy>();
			enemy.gameObject.GetComponentInChildren<Animator>().speed = 0.25f;
			enemy.SetHealth(health, health);
			BossUi.Instance.BossSpawned(enemy);
			enemy.m_CurrentAttackRecoveryTime = 2f;
			enemy.SetState(EnemyState.RECOVERING_FROM_ATTACK);
			
			StopSpawn();
		}
		
		public void StartSpawn()
		{
			m_TargetSpawnAmount = m_SpawnIncrease * m_WaveCount;

			OnNextWave?.Invoke(m_WaveCount);
			OnSpawnStarted?.Invoke();
			m_IsSpawning = true;
		}

		public void StopSpawn()
		{
			OnSpawnDone?.Invoke();
			m_IsSpawning = false;
			m_SpawnCount = 0;
			m_WaveCount++;
		}

	#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;

			for (int i = 0; i < m_SpawnPoints.Length; i++)
			{
				Gizmos.DrawWireCube(m_SpawnPoints[i].position, m_DetectionBox / 2);
			}
		}
	#endif
	}

	[Serializable]
	internal class SpawnableEntities
	{
		[Tooltip("Just a name for the inspector")]
		public string m_EnemyName = "";

		[Tooltip("The enemy prefab that needs to be spawned")]
		public GameObject m_EnemyPrefab;

		[Tooltip("Which wave this entity needs to start spawning")]
		public int m_WaveStartSpawning = 0;
	}
}