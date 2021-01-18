using System;
using TMPro;
using UnityEngine;

namespace Bladiator.Leaderboard
{
    public class LeaderboardContentItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_PlayerName = null;
        [SerializeField] private TextMeshProUGUI m_PlayerScore = null;
        [SerializeField] private TextMeshProUGUI m_PlayerWave = null;

        public LeaderboardContentItem(LeaderboardItemData itemData)
        {
            m_PlayerName.text = itemData.name;
            m_PlayerScore.text = Convert.ToString(itemData.score);
            m_PlayerWave.text = Convert.ToString(itemData.wave);
        }
    }
}
