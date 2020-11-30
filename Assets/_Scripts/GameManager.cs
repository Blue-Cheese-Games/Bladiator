using System;
using System.Collections.Generic;
using Bladiator.Entities;
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
        public Action ResetEvent;
        
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
            playerToAdd.OnDeath += PlayerDied;
        }

        private void PlayerDied(EntityBase player)
        {
            Player p = player.GetComponent<Player>();
            m_Players.Remove(p);
            
            // TODO Move the reset event
            // Reset all managers
            if(m_Players.Count <= 0) ResetEvent?.Invoke();
            ResetGameManager();
        }

        /// <summary>
        /// Resets the Game Manager
        /// </summary>
        private void ResetGameManager()
        {
            ChangeState(GameState.Idle);
            
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
    }
}
