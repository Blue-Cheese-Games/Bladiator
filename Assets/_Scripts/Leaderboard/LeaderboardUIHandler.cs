using UnityEngine;

namespace Bladiator.Leaderboard
{
    public class LeaderboardUIHandler : MonoBehaviour
    {
        public static LeaderboardUIHandler Instance = null;
        
        [SerializeField] private Transform m_LeaderboardContentParent = null;
        [SerializeField] private GameObject m_LeaderboardContentPrefab = null;
        
        private const int m_LeaderboardContentDistance = 49;
        private int m_LeaderboardTotalContentDistance = -49;
        private const int m_ContentItemOffset = 5;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Place all the players from the database onto the leaderboard
        /// </summary>
        public void SetAllContentForLeaderboard()
        {
            foreach (LeaderboardItemData itemData in LeaderboardHandler.INSTANCE.LeaderboardItem)
            {
                GameObject tmp = GameObject.Instantiate(m_LeaderboardContentPrefab);
                
                LeaderboardItemData leaderboardItemData = tmp.GetComponent<LeaderboardItemData>();
                leaderboardItemData.name = itemData.name;
                leaderboardItemData.score = itemData.score;
                leaderboardItemData.wave = itemData.wave;

                LeaderboardContentItem leaderboardContentItem = tmp.GetComponent<LeaderboardContentItem>();
                leaderboardContentItem = new LeaderboardContentItem(leaderboardItemData);
                
            }
        }
    }
}
