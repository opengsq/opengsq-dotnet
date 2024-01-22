namespace OpenGSQ.Responses.Samp
{
    /// <summary>
    /// Represents the Player class.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets or sets the ID of the player.
        /// </summary>
        public byte Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the score of the player.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the ping of the player.
        /// </summary>
        public int Ping { get; set; }
    }
}