namespace OpenGSQ.Responses.Battlefield
{
    /// <summary>
    /// Represents the version of a game mod.
    /// </summary>
    public class VersionInfo
    {
        /// <summary>
        /// Gets or sets the mod of the game.
        /// </summary>
        public string Mod { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the version of the mod.
        /// </summary>
        public string Version { get; set; } = string.Empty;
    }
}