namespace OpenGSQ.Responses.Source
{
    /// <summary>
    /// A2S_INFO Partial Info
    /// </summary>
    public class PartialInfo
    {
        /// <summary>
        /// Protocol version used by the server.
        /// </summary>
        public byte Protocol { get; set; }

        /// <summary>
        /// Name of the server.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Map the server has currently loaded.
        /// </summary>
        public string Map { get; set; } = string.Empty;

        /// <summary>
        /// Name of the folder containing the game files.
        /// </summary>
        public string Folder { get; set; } = string.Empty;

        /// <summary>
        /// Full name of the game.
        /// </summary>
        public string Game { get; set; } = string.Empty;

        /// <summary>
        /// Number of players on the server.
        /// </summary>
        public byte Players { get; set; }

        /// <summary>
        /// Maximum number of players the server reports it can hold.
        /// </summary>
        public byte MaxPlayers { get; set; }

        /// <summary>
        /// Number of bots on the server.
        /// </summary>
        public byte Bots { get; set; }

        /// <summary>
        /// Indicates the type of server
        /// </summary>
        public ServerType ServerType { get; set; }

        /// <summary>
        /// Indicates the operating system of the server
        /// </summary>
        public Environment Environment { get; set; }

        /// <summary>
        /// Indicates whether the server requires a password
        /// </summary>
        public Visibility Visibility { get; set; }

        /// <summary>
        /// Specifies whether the server uses VAC
        /// </summary>
        public VAC VAC { get; set; }
    }
}