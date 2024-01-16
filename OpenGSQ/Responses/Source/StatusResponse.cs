using System;

namespace OpenGSQ.Responses.Source
{
    /// <summary>
    /// Indicates the type of server
    /// </summary>
    public enum ServerType : byte
    {
        /// <summary>
        /// Dedicated server
        /// </summary>
        Dedicated = 0x64,

        /// <summary>
        /// Listen server
        /// </summary>
        Listen = 0x6C,

        /// <summary>
        /// SourceTV relay (proxy)
        /// </summary>
        Proxy = 0x70,
    }

    /// <summary>
    /// Indicates the operating system of the server
    /// </summary>
    public enum Environment : byte
    {
        /// <summary>
        /// Linux
        /// </summary>
        Linux = 0x6C,

        /// <summary>
        /// Windows
        /// </summary>
        Windows = 0x77,

        /// <summary>
        /// Mac
        /// </summary>
        Mac = 0x6D,
    }

    /// <summary>
    /// Indicates whether the server requires a password
    /// </summary>
    public enum Visibility : byte
    {
        /// <summary>
        /// Public
        /// </summary>
        Public,

        /// <summary>
        /// Private
        /// </summary>
        Private,
    }

    /// <summary>
    /// Specifies whether the server uses VAC
    /// </summary>
    public enum VAC : byte
    {
        /// <summary>
        /// Unsecured
        /// </summary>
        Unsecured,

        /// <summary>
        /// Secured
        /// </summary>
        Secured,
    }

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

    /// <summary>
    /// A2S_INFO Response Interface
    /// </summary>
    public interface IInfoResponse
    {
        /// <summary>
        /// Protocol version used by the server.
        /// </summary>
        byte Protocol { get; set; }

        /// <summary>
        /// Name of the server.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Map the server has currently loaded.
        /// </summary>
        string Map { get; set; }

        /// <summary>
        /// Name of the folder containing the game files.
        /// </summary>
        string Folder { get; set; }

        /// <summary>
        /// Full name of the game.
        /// </summary>
        string Game { get; set; }

        /// <summary>
        /// Number of players on the server.
        /// </summary>
        byte Players { get; set; }

        /// <summary>
        /// Maximum number of players the server reports it can hold.
        /// </summary>
        byte MaxPlayers { get; set; }

        /// <summary>
        /// Number of bots on the server.
        /// </summary>
        byte Bots { get; set; }

        /// <summary>
        /// Indicates the type of server
        /// </summary>
        ServerType ServerType { get; set; }

        /// <summary>
        /// Indicates the operating system of the server
        /// </summary>
        Environment Environment { get; set; }

        /// <summary>
        /// Indicates whether the server requires a password
        /// </summary>
        Visibility Visibility { get; set; }

        /// <summary>
        /// Specifies whether the server uses VAC
        /// </summary>
        VAC VAC { get; set; }
    }

    /// <summary>
    /// Source Response
    /// </summary>
    public class SourceInfoResponse : IInfoResponse
    {
        /// <inheritdoc/>
        public byte Protocol { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Map { get; set; }

        /// <inheritdoc/>
        public string Folder { get; set; }

        /// <inheritdoc/>
        public string Game { get; set; }

        /// <summary>
        /// <a href="http://developer.valvesoftware.com/wiki/Steam_Application_ID">Steam Application ID</a> of game.
        /// </summary>
        public short ID { get; set; }

        /// <inheritdoc/>
        public byte Players { get; set; }

        /// <inheritdoc/>
        public byte MaxPlayers { get; set; }

        /// <inheritdoc/>
        public byte Bots { get; set; }

        /// <inheritdoc/>
        public ServerType ServerType { get; set; }

        /// <inheritdoc/>
        public Environment Environment { get; set; }

        /// <inheritdoc/>
        public Visibility Visibility { get; set; }

        /// <inheritdoc/>
        public VAC VAC { get; set; }

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

    /// <summary>
    /// Obsolete GoldSource Response
    /// </summary>
    public class GoldSourceInfoResponse : IInfoResponse
    {
        /// <summary>
        /// IP address and port of the server.
        /// </summary>
        public string Address { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Map { get; set; }

        /// <inheritdoc/>
        public string Folder { get; set; }

        /// <inheritdoc/>
        public string Game { get; set; }

        /// <inheritdoc/>
        public byte Players { get; set; }

        /// <inheritdoc/>
        public byte MaxPlayers { get; set; }

        /// <inheritdoc/>
        public byte Protocol { get; set; }

        /// <inheritdoc/>
        public ServerType ServerType { get; set; }

        /// <inheritdoc/>
        public Environment Environment { get; set; }

        /// <inheritdoc/>
        public Visibility Visibility { get; set; }

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
        public string Link { get; set; }

        /// <summary>
        /// URL to download the mod.
        /// </summary>
        public string DownloadLink { get; set; }

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

        /// <inheritdoc/>
        public VAC VAC { get; set; }

        /// <inheritdoc/>
        public byte Bots { get; set; }
    }

    /// <summary>
    /// Player Data
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Player Name
        /// </summary>
        public string Name { get; set; }

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