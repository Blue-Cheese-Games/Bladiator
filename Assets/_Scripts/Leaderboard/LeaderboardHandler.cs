using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Bladiator.Leaderboard.Struct;
using Newtonsoft.Json;
using UnityEngine;

namespace Bladiator.Leaderboard
{
    public class LeaderboardHandler : MonoBehaviour
    {
        public static LeaderboardHandler INSTANCE = null;
        
        public List<LeaderboardItemData> LeaderboardItem = new List<LeaderboardItemData>();
        
        public LeaderboardHandler()
        {
            if (INSTANCE == null)
            {
                INSTANCE = this;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                GetLeaderboard();
                LeaderboardUIHandler.Instance.SetAllContentForLeaderboard();
            } else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                AddPlayerToLeaderboard(new LeaderboardItemData()
                {
                    name = "Roberto",
                    score = -4,
                    wave = 8
                });
            }
        }
        
        /// <summary>
        /// Add a player to the database with their achieved data
        /// </summary>
        /// <param name="itemData"> Data of the player </param>
        public void AddPlayerToLeaderboard(LeaderboardItemData itemData)
        {
            const string databaseUrl = "https://bladiator.larsbeijaard.com/scripts/add_leaderboard_item.php";
            SendDatabaseRequest(databaseUrl, itemData);
        }

        /// <summary>
        /// Get all of the players that are on the leaderboard
        /// </summary>
        public void GetLeaderboard()
        {
            const string databaseUrl = "https://bladiator.larsbeijaard.com/scripts/get_leaderboard.php";
            
            string json = GetLeaderboardRequest(databaseUrl);
            try
            {
                // Clear the current leaderboard
                LeaderboardItem.Clear();
                
                // Add each player from the leaderboard to a list
                foreach (LeaderboardItemData data in JsonConvert.DeserializeObject<List<LeaderboardItemData>>(json))
                {
                    LeaderboardItem.Add(new LeaderboardItemData()
                    {
                        name = data.name,
                        score = data.score,
                        wave = data.wave
                    });
                }
            }
            // This gets executed when there are no players on the leaderboard
            catch (Exception e)
            {
                LeaderboardItemData leaderboardItemData = new LeaderboardItemData();
                leaderboardItemData.name = leaderboardItemData.GetEmptyLeaderboardName();
                
                LeaderboardItem.Add(new LeaderboardItemData()
                {
                    name = leaderboardItemData.GetEmptyLeaderboardName()
                });
            }
            
            SortLeaderboard(LeaderboardSortingType.SORT_ON_SCORE);
        }
        
        /// <summary>
        /// Bubble Sort Algorithm:
        /// Sort the leaderboard from best achieved (score|wave)
        /// </summary>
        /// <param name="sortingType"> The type of sorting (score|wave) </param>
        public void SortLeaderboard(LeaderboardSortingType sortingType)
        {
            // Loop though each of the leaderboard items and sort them by score count
            for (int i = 0; i < LeaderboardItem.Count; i++)
            {
                for (int j = 0; j < LeaderboardItem.Count - 1; j++)
                {
                    // Swap the indexes
                    if (LeaderboardItem[j].score < LeaderboardItem[j + 1].score)
                    {
                        LeaderboardItemData temp = LeaderboardItem[j + 1];
                        LeaderboardItem[j + 1] = LeaderboardItem[j];
                        LeaderboardItem[j] = temp;
                    }  
                }  
            }
        }
        
        /// <summary>
        /// Send a request to the database requesting to add a player to the database
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        private void SendDatabaseRequest(string url, LeaderboardItemData data)
        {
            string json = JsonConvert.SerializeObject(data);
            
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("data", json);

            using (WebClient client = new WebClient())
            {
                string result = Encoding.UTF8.GetString(client.UploadValues(url, nvc));
                Debug.Log(result);
            }
        }
        
        /// <summary>
        /// Request the players from the leaderboard from the database
        /// </summary>
        /// <param name="url"> Url to the database </param>
        /// <returns> All the players in the database </returns>
        private string GetLeaderboardRequest(string url)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }   
}
