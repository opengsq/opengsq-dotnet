namespace OpenGSQ.Responses.KillingFloor
{
    /// <summary>
    /// Represents the status of a server.
    /// </summary>
    public class Status : Unreal2.Status
    {
        /// <summary>
        /// Gets or sets the current wave number in a game.
        /// </summary>
        public int WaveCurrent { get; set; }

        /// <summary>
        /// Gets or sets the total number of waves in a game.
        /// </summary>
        public int WaveTotal { get; set; }
    }
}