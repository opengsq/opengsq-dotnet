namespace OpenGSQ.Responses.Quake1
{
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets or sets the player's ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the player's score.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the player's time.
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// Gets or sets the player's ping.
        /// </summary>
        public int Ping { get; set; }

        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the player's skin.
        /// </summary>
        public string Skin { get; set; }

        /// <summary>
        /// Gets or sets the player's first color.
        /// </summary>
        public int Color1 { get; set; }

        /// <summary>
        /// Gets or sets the player's second color.
        /// </summary>
        public int Color2 { get; set; }
    }
}