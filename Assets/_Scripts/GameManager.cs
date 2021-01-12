using System;
using System.Collections.Generic;
using Bladiator.Entities;
using Bladiator.Entities.Players;
using Bladiator.Managers.EnemyManager;
using Bladiator.UI;
using UnityEngine;

namespace Bladiator.Managers
{
	public enum GameState
	{
		MainMenu,
		Pause,
		Idle,
		Animating,
		Fighting,
		PlayersDied,
		Ending
	}

	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance;

		public Action<GameState> OnGameStateChange;
		public Action ResetEvent;
		public Action OnApplicationQuitEvent;

		[SerializeField] private GameState m_State;

		// List containing all the active players.
		private List<Player> m_Players = new List<Player>();

		public GameState GameState
		{
			get => m_State;
		}

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
		}

		private void Start()
		{
			EnemyManager.EnemyManager.Instance.OnAllEnemiesDied += OnAllEnemiesDied;
			ResetEvent += ResetGameManager;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space) && m_State == GameState.Idle) ChangeState(GameState.Animating);
			if (Input.GetKeyDown(KeyCode.Escape) && m_State != GameState.MainMenu) PauseGame();
		}

		private void OnApplicationQuit()
		{
			OnApplicationQuitEvent?.Invoke();
		}

		private void OnAllEnemiesDied()
		{
			UiManager.Instance.ShowWaveDone();
			ChangeState(GameState.Idle);
		}

		private void ChangeState(GameState state)
		{
			m_PreviousState = m_State;
			m_State = state;
			OnGameStateChange?.Invoke(state);
		}

		public void AddPlayer(Player playerToAdd)
		{
			m_Players.Add(playerToAdd);
		}

		public void PlayerDied()
		{
			if (m_Players.Count <= 0) ChangeState(GameState.Ending);
		}

		public void OnPlayerDied(EntityBase player)
		{
			Player p = player.GetComponent<Player>();
			if (!m_Players.Contains(p)) return;

			m_Players.Remove(p);
			
			if (m_Players.Count <= 0)
				ChangeState(GameState.PlayersDied);
		}

		/// <summary>
		/// Resets the Game Manager
		/// </summary>
		private void ResetGameManager()
		{
			ChangeState(GameState.MainMenu);

			// Cleanup the memory
			GC.Collect();
		}

		/// <summary>
		/// Get the list of all active players.
		/// </summary>
		/// <returns></returns>
		public List<Player> GetPlayers()
		{
			return m_Players;
		}

		public void StartGame()
		{
			if (m_State == GameState.MainMenu)
				ChangeState(GameState.Idle);
		}

		private GameState m_PreviousState;

		public void PauseGame()
		{
			ChangeState(m_State == GameState.Pause ? m_PreviousState : GameState.Pause);
		}

		public void AnimationDone()
		{
			ChangeState(GameState.Fighting);
		}

		public void SubsribeToOnApplicationQuit(Action method)
		{
			OnApplicationQuitEvent += method;
		}
	}
}