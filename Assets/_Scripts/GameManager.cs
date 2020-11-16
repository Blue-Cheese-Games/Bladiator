using System.Collections.Generic;
using Bladiator.Entity.Player;
using UnityEngine;

namespace Bladiator.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        // List containing all the active players.
        private List<Player> m_Players = new List<Player>();

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
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
