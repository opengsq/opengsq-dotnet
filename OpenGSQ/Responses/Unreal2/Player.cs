namespace OpenGSQ.Responses.Unreal2
{
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets or sets the ID of the player.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ping of the player.
        /// </summary>
        public int Ping { get; set; }

        /// <summary>
        /// Gets or sets the score of the player.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the stats ID of the player.
        /// </summary>
        public int StatsId { get; set; }
    }
}