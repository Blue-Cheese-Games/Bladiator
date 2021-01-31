using Bladiator.Managers;
using TMPro;
using UnityEngine;

namespace Bladiator.Leaderboard
{
    public class LeaderboardUIHandler : MonoBehaviour
    {
        public static LeaderboardUIHandler Instance = null;
        
        [SerializeField] private Transform m_LeaderboardContentParent = null;
        [SerializeField] private GameObject m_LeaderboardContentPrefab = null;

        [SerializeField] private TMP_InputField m_PlayerName;

        private const int m_ContentItemStartY = -50;
        private const int m_IncrementContentYBy = -100;
        private int m_TotalContentItemDistance = 0;

        private LeaderboardHandler m_LeaderboardHandler;

        private void Awake()
        {
            Instance = this;
            m_LeaderboardHandler = GetComponent<LeaderboardHandler>();
            
            m_TotalContentItemDistance = m_ContentItemStartY;
        }

        public void BTN_Continue()
        {
            if (!string.IsNullOrEmpty(m_PlayerName.text))
            {
                m_LeaderboardHandler.AddPlayerToLeaderboard(new LeaderboardItemData()
                {
                    name = m_PlayerName.text,
                    score = ScoreManager.Instance.GetScore(),
                    wave = WaveSystem.Instance.WaveCount
                });
            }
            
            GameManager.Instance.ChangeState(GameState.Ending);
        }
        
        /// <summary>
        /// Place all the players from the database onto the leaderboard
        /// </summary>
        public void SetAllContentForLeaderboard()
        {
            RemoveAllContentItemsFromLeaderboard();
            
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

        /// <summary>
        /// Remove all the child objects from the leaderboard items to remove duplicate items on game restart
        /// </summary>
        private void RemoveAllContentItemsFromLeaderboard()
        {
            foreach (Transform child in m_LeaderboardContentParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
