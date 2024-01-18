namespace OpenGSQ.Responses.Source
{
    /// <summary>
    /// Source Response
    /// </summary>
    public class SourceInfo : PartialInfo
    {
        /// <summary>
        /// <a href="http://developer.valvesoftware.com/wiki/Steam_Application_ID">Steam Application ID</a> of game.
        /// </summary>
        public short ID { get; set; }

        /// <summary>
        /// Indicates the game mode
        /// </summary>
        public byte? Mode { get; set; }

        /// <summary>
        /// The number of witnesses necessary to have a player arrested.
        /// </summary>
        public byte? Witnesses { get; set; }

        /// <summary>
        /// Time (in seconds) before a player is arrested while being witnessed.
        /// </summary>
        public byte? Duration { get; set; }

        /// <summary>
        /// Version of the game installed on the server.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// If present, this specifies which additional data fields will be included.
        /// </summary>
        public ExtraDataFlag? EDF { get; set; }

        /// <summary>
        /// The server's game port number.
        /// </summary>
        public short? Port { get; set; }

        /// <summary>
        /// Server's SteamID.
        /// </summary>
        public ulong? SteamID { get; set; }

        /// <summary>
        /// Spectator port number for SourceTV.
        /// </summary>
        public short? SpectatorPort { get; set; }

        /// <summary>
        /// Name of the spectator server for SourceTV.
        /// </summary>
        public string SpectatorName { get; set; }

        /// <summary>
        /// Tags that describe the game according to the server (for future use.)
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// The server's 64-bit GameID. If this is present, a more accurate AppID is present in the low 24 bits. The earlier AppID could have been truncated as it was forced into 16-bit storage.
        /// </summary>
        public ulong? GameID { get; set; }
    }
}