namespace Bladiator.Leaderboard
{
    [System.Serializable]
    public struct LeaderboardItemData
    {
        public string name;
        public int score;
        public int wave;

        /// <summary>
        /// Get the data to display when there are no players on the leaderboard
        /// </summary>
        /// <returns> Default data </returns>
        public string GetEmptyLeaderboardName()
        {
            const string emptyLeaderboardName = "Nothing on the leaderboard.";
            return emptyLeaderboardName;
        }
    }
}
