using System.Collections.Generic;

namespace OpenGSQ.Responses.Battlefield
{
    /// <summary>
    /// Represents the info of a game.
    /// </summary>
    public class Info
    {
        /// <summary>
        /// Gets or sets the hostname of the game server.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the number of players in the game.
        /// </summary>
        public int NumPlayers { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of players allowed in the game.
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Gets or sets the type of the game.
        /// </summary>
        public string GameType { get; set; }

        /// <summary>
        /// Gets or sets the current map of the game.
        /// </summary>
        public string Map { get; set; }

        /// <summary>
        /// Gets or sets the number of rounds played.
        /// </summary>
        public int RoundsPlayed { get; set; }

        /// <summary>
        /// Gets or sets the total number of rounds.
        /// </summary>
        public int RoundsTotal { get; set; }

        /// <summary>
        /// Gets or sets the list of teams.
        /// </summary>
        public List<float> Teams { get; set; }

        /// <summary>
        /// Gets or sets the target score.
        /// </summary>
        public int TargetScore { get; set; }

        /// <summary>
        /// Gets or sets the status of the game.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the game is ranked.
        /// </summary>
        public bool Ranked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PunkBuster is enabled.
        /// </summary>
        public bool PunkBuster { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a password is required.
        /// </summary>
        public bool Password { get; set; }

        /// <summary>
        /// Gets or sets the uptime of the game server.
        /// </summary>
        public int Uptime { get; set; }

        /// <summary>
        /// Gets or sets the round time.
        /// </summary>
        public int RoundTime { get; set; }

        // Additional properties based on your code

        /// <summary>
        /// Gets or sets the game mod. This property is optional.
        /// </summary>
        public string Mod { get; set; }

        /// <summary>
        /// Gets or sets the IP port of the game server. This property is optional.
        /// </summary>
        public string IpPort { get; set; }

        /// <summary>
        /// Gets or sets the version of PunkBuster. This property is optional.
        /// </summary>
        public string PunkBusterVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the join queue is enabled. This property is optional.
        /// </summary>
        public bool? JoinQueue { get; set; }

        /// <summary>
        /// Gets or sets the region of the game server. This property is optional.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the ping site of the game server. This property is optional.
        /// </summary>
        public string PingSite { get; set; }

        /// <summary>
        /// Gets or sets the country of the game server. This property is optional.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the number of players in the Blaze game state. This property is optional.
        /// </summary>
        public int? BlazePlayerCount { get; set; }

        /// <summary>
        /// Gets or sets the Blaze game state. This property is optional.
        /// </summary>
        public string BlazeGameState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether quick match is enabled. This property is optional.
        /// </summary>
        public bool? QuickMatch { get; set; }
    }
}