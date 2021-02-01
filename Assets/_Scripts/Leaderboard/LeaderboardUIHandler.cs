using Bladiator.Managers;
using TMPro;
using UnityEngine;

namespace Bladiator.Leaderboard
{
    public class LeaderboardUIHandler : MonoBehaviour
    {
        public static LeaderboardUIHandler Instance = null;
        
        [Header("Leaderboard content")]
        [SerializeField] private Transform m_LeaderboardContentParent;
        [SerializeField] private GameObject m_LeaderboardContentPrefab;

        [Header("Leaderboard player name")]
        [SerializeField] private TMP_InputField m_PlayerName;
        
        [Header("Displayed on leaderboard")]
        [SerializeField] private TextMeshProUGUI m_FinalScore;
        [SerializeField] private TextMeshProUGUI m_FinalWave;
        
        private const int m_ContentItemStartY = -50;
        private const int m_IncrementContentYBy = -100;
        private int m_TotalContentItemDistance;

        private LeaderboardHandler m_LeaderboardHandler;

        private void Awake()
        {
            Instance = this;
            m_LeaderboardHandler = GetComponent<LeaderboardHandler>();
            
            m_TotalContentItemDistance = m_ContentItemStartY;

            SetPlayerNameSettings();
        }

        private void SetPlayerNameSettings()
        {
            m_PlayerName.characterLimit = 16;
            m_PlayerName.characterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
        }

        public void BTN_Continue()
        {
            Debug.Log("BtnContinue call");
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

        private void SetFinalScore()
        {
            m_FinalScore.text = $"Score: {ScoreManager.Instance.GetScore()}";
            m_FinalWave.text = $"Wave: {WaveSystem.Instance.WaveCount}";
        }
        
        /// <summary>
        /// Place all the players from the database onto the leaderboard
        /// </summary>
        public void SetAllContentForLeaderboard()
        {
            SetFinalScore();

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
