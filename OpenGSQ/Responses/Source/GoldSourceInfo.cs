namespace OpenGSQ.Responses.Source
{
    /// <summary>
    /// Obsolete GoldSource Response
    /// </summary>
    public class GoldSourceInfo : PartialInfo
    {
        /// <summary>
        /// IP address and port of the server.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the game is a mod
        /// <list type="bullet">
        /// <item>0 for Half-Life</item>
        /// <item>1 for Half-Life mod</item>
        /// </list>
        /// </summary>
        public byte Mod { get; set; }

        /// <summary>
        /// URL to mod website.
        /// </summary>
        public string Link { get; set; } = string.Empty;

        /// <summary>
        /// URL to download the mod.
        /// </summary>
        public string DownloadLink { get; set; } = string.Empty;

        /// <summary>
        /// Version of mod installed on server.
        /// </summary>
        public long? Version { get; set; }

        /// <summary>
        /// Space (in bytes) the mod takes up.
        /// </summary>
        public long? Size { get; set; }

        /// <summary>
        /// Indicates the type of mod:
        /// <list type="bullet">
        /// <item>0 for single and multiplayer mod</item>
        /// <item>1 for multiplayer only mod</item>
        /// </list>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Indicates whether mod uses its own DLL:
        /// <list type="bullet">
        /// <item>0 if it uses the Half-Life DLL</item>
        /// <item>1 if it uses its own DLL</item>
        /// </list>
        /// </summary>
        public byte DLL { get; set; }
    }
}