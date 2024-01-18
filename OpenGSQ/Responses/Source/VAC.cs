namespace OpenGSQ.Responses.Source
{
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
}