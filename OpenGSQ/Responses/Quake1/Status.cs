using System.Collections.Generic;

namespace OpenGSQ.Responses.Quake1
{
    /// <summary>
    /// Represents the status of the server.
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Gets or sets the server information.
        /// </summary>
        public Dictionary<string, string> Info { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the list of players.
        /// </summary>
        public List<Player> Players { get; set; } = new List<Player>();
    }
}