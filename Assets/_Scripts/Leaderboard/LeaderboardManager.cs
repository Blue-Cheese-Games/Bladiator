using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Leaderboard
{
    public class LeaderboardManager : MonoBehaviour
    {
        [SerializeField] private List<LeaderboardItem> m_LeaderboardItem = new List<LeaderboardItem>();

        private void Awake()
        {
            SortLeaderboard();
        }

        private void SortLeaderboard()
        {
            // Loop though each of the leaderboard items and sort them by score count
            for (int i = 0; i < m_LeaderboardItem.Count; i++)
            {
                for (int j = 0; j < m_LeaderboardItem.Count - 1; j++)
                {
                    // Swap to indexes
                    if (m_LeaderboardItem[j].Score < m_LeaderboardItem[j + 1].Score)
                    {
                        LeaderboardItem temp = m_LeaderboardItem[j + 1];
                        m_LeaderboardItem[j + 1] = m_LeaderboardItem[j];
                        m_LeaderboardItem[j] = temp;
                    }  
                }  
            }
            
            foreach (LeaderboardItem item in m_LeaderboardItem)
            {
                Debug.Log($"Name: {item.Name} --- Score: {item.Score}");
            }
        }
    }   
}
