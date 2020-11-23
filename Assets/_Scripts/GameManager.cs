using System;
using System.Collections.Generic;
using Bladiator.Entities.Players;
using Bladiator.Managers.EnemyManager;
using UnityEngine;

namespace Bladiator.Managers
{
    public enum GameState
    {
        Idle,
        Fighting,
        Ending
    }
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public Action<GameState> OnGameStateChange;
        
        [SerializeField] private GameState m_State;
        
        // List containing all the active players.
        private List<Player> m_Players = new List<Player>();

        public GameState GameState
        {
            get => m_State;
        }

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            EnemyManager.EnemyManager.Instance.OnAllEnemiesDied += OnAllEnemiesDied;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) && m_State == GameState.Idle) ChangeState(GameState.Fighting);
        }

        private void OnAllEnemiesDied()
        {
            print("Enemies died");
            ChangeState(GameState.Idle);
        }

        void ChangeState(GameState state)
        {
            print($"State changed to: {state.ToString()}");
            m_State = state;
            OnGameStateChange?.Invoke(state);
        }

        public void AddPlayer(Player playerToAdd)
        {
            m_Players.Add(playerToAdd);
        }

        /// <summary>
        /// Get the list of all active players.
        /// </summary>
        /// <returns></returns>
        public List<Player> GetPlayers()
        {
            return m_Players;
        }
    }
}
