using UnityEngine;

namespace Bladiator.Leaderboard
{
    public class LeaderboardUIHandler : MonoBehaviour
    {
        public static LeaderboardUIHandler Instance = null;
        
        [SerializeField] private Transform m_LeaderboardContentParent = null;
        [SerializeField] private GameObject m_LeaderboardContentPrefab = null;

        private const int m_ContentItemStartY = -15;
        private const int m_IncrementContentYBy = -30;
        private int m_TotalContentItemDistance = 0;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Place all the players from the database onto the leaderboard
        /// </summary>
        public void SetAllContentForLeaderboard()
        {
            m_TotalContentItemDistance = m_ContentItemStartY;
            
            foreach (LeaderboardItemData itemData in LeaderboardHandler.INSTANCE.LeaderboardItem)
            {
                 GameObject itemPrefab = Instantiate(m_LeaderboardContentPrefab);
                 
                 // Set the prefab its position
                 Vector3 itemPrefabPosition = itemPrefab.transform.position;
                 itemPrefab.transform.position = new Vector3(
                     itemPrefabPosition.x, 
                     m_TotalContentItemDistance,
                     itemPrefabPosition.z);
                 
                 // Set the parent of the prefab
                 itemPrefab.transform.SetParent(m_LeaderboardContentParent, false);

                 // Set the data of the prefab
                 LeaderboardContentItem item = itemPrefab.GetComponent<LeaderboardContentItem>();
                 item.PlayerName.text = itemData.name;
                 item.PlayerScore.text = $"Score: {itemData.score.ToString()}";
                 item.PlayerWave.text = $"Wave:  {itemData.wave.ToString()}";

                 // Increment the value of what the Y axis must be in the next iteration
                 m_TotalContentItemDistance += m_IncrementContentYBy;
            }
        }
    }
}
