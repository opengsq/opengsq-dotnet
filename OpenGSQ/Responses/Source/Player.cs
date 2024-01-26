namespace OpenGSQ.Responses.Source
{
    /// <summary>
    /// Player Data
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Player Name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Player Score
        /// </summary>
        public long Score { get; set; }

        /// <summary>
        /// Player Duration
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Player Deaths
        /// </summary>
        public long? Deaths { get; set; }

        /// <summary>
        /// Player Money
        /// </summary>
        public long? Money { get; set; }
    }
}