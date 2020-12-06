using System;
using Bladiator.Entities.Enemies;
using Bladiator.Managers;
using UnityEngine;

namespace Bladiator
{
	public class WaveSystem : MonoBehaviour
	{
		public static WaveSystem Instance;
		public Action OnSpawnStarted, OnSpawnDone;
		public Action<int> OnNextWave;

		[Range(0.01f, 1f)] [SerializeField] private float m_SpawnInterval = 0.25f;
		[SerializeField] private Vector3 m_DetectionBox;
		[SerializeField] private LayerMask m_DetectionMasks;
		[SerializeField] private int m_SpawnIncrease = 8;
		[SerializeField] private Transform[] m_SpawnPoints;
		[SerializeField] private Transform m_BossSpawnPoint;
		[SerializeField] private GameObject m_Enemy, m_MiniBoss, m_Boss;

		private bool m_IsSpawning;

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
			if (!m_IsSpawning || GameManager.Instance.GameState == GameState.Pause) return;

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

					Collider[] output = Physics.OverlapBox(m_SpawnPoints[i].position, m_DetectionBox,
						m_SpawnPoints[i].rotation, m_DetectionMasks);
					if (output.Length > 0)
					{
						continue;
					}

					Enemy e = Instantiate(m_Enemy, m_SpawnPoints[i].position, m_SpawnPoints[i].rotation)
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
			if (m_WaveCount % 10 == 0)
			{
				// Boss
				Instantiate(m_Boss, m_BossSpawnPoint.position, m_BossSpawnPoint.rotation);
			}
			else
			{
				// Mini boss
				Instantiate(m_MiniBoss, m_BossSpawnPoint.position, m_BossSpawnPoint.rotation);
			}

			StopSpawn();
		}

		public void StartSpawn()
		{
			m_TargetSpawnAmount += m_SpawnIncrease;

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
}