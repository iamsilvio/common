namespace doe.Common.Security.Impersonation
{
    /// <summary>
    ///     Container for a Domain User
    /// </summary>
    public class DomainUser
    {
        /// <summary>
        ///     Gets or sets the domain.
        /// </summary>
        /// <value>
        ///     The domain.
        /// </value>
        public string Domain { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        /// <value>
        ///     The password.
        /// </value>
        public string Pwd { get; set; }
    }
}