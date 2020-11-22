using System;
using System.Security.Cryptography;
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
		[SerializeField] private int m_SpawnIncrease = 8;
		[SerializeField] private Transform[] m_SpawnPoints;
		[SerializeField] private Transform m_BossSpawnPoint;
		[SerializeField] private GameObject m_Enemy, m_MiniBoss, m_Boss;

		private bool m_IsSpawning;

		private int m_TargetSpawnAmount;
		private int m_SpawnCount;
		private int m_WaveCount;

		private float m_SpawnTimer;
		

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
			GameManager.Instance.OnGameStateChange += OnGameStateChange;
		}

		private void OnGameStateChange(GameState obj)
		{
			if(obj == GameState.Fighting) StartSpawn();
		}

		void Update()
		{
			if (!m_IsSpawning) return;
			
			if (m_WaveCount % 5 != 0 || m_WaveCount % 10 != 0)
				Spawner();
			else
				SpawnBoss();
		}

		private void Spawner()
		{
			if (m_SpawnTimer >= m_SpawnInterval)
			{
				for (int i = 0; i < m_SpawnPoints.Length; i++)
				{
					if (m_SpawnCount > m_TargetSpawnAmount)
					{
						StopSpawn();
						return;
					}

					Instantiate(m_Enemy, m_SpawnPoints[i].position, m_SpawnPoints[i].rotation);
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
			if (m_WaveCount % 5 == 0)
			{
				// Mini boss
				Instantiate(m_MiniBoss, m_BossSpawnPoint.position, m_BossSpawnPoint.rotation);
			}
			else
			{
				// Boss
				Instantiate(m_Boss, m_BossSpawnPoint.position, m_BossSpawnPoint.rotation);
			}
			
			StopSpawn();
		}

		public void StartSpawn()
		{
			m_WaveCount++;
			m_TargetSpawnAmount += m_SpawnIncrease;
			
			OnNextWave?.Invoke(m_WaveCount);
			OnSpawnStarted?.Invoke();
			m_IsSpawning = true;
		}

		public void StopSpawn()
		{
			OnSpawnDone?.Invoke();
			m_IsSpawning = false;
		}
	}
}