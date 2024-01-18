using System;

namespace OpenGSQ.Responses.Source
{
    /// <summary>
    /// Extra Data Flag (EDF)
    /// </summary>
    [Flags]
    public enum ExtraDataFlag : byte
    {
        /// <summary>
        /// Port
        /// </summary>
        Port = 0x80,

        /// <summary>
        /// SteamID
        /// </summary>
        SteamID = 0x10,

        /// <summary>
        /// Spectator
        /// </summary>
        Spectator = 0x40,

        /// <summary>
        /// Keywords
        /// </summary>
        Keywords = 0x20,

        /// <summary>
        /// GameID
        /// </summary>
        GameID = 0x01,
    }
}