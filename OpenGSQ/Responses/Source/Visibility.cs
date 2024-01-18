namespace OpenGSQ.Responses.Source
{
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
}