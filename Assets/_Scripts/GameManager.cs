using Baldiator.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        // List containing all the active players.
        private List<EntityBase> m_Players = new List<EntityBase>();

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        public void AddPlayer(EntityBase playerToAdd)
        {
            m_Players.Add(playerToAdd);
        }

        /// <summary>
        /// Get the list of all active players.
        /// </summary>
        /// <returns></returns>
        public List<EntityBase> GetPlayers()
        {
            return m_Players;
        }
    }
}
