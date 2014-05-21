namespace doe.Common.Security.Impersonation
{
    /// <summary>
    ///     Defines the possible security levels for impersonation.
    /// </summary>
    public enum ImpersonationLevel
    {
        /// <summary>
        ///     Anonymous access, the process is unable to identify the security context.
        /// </summary>
        Anonymous = 0,

        /// <summary>
        ///     The process can identify the security context.
        /// </summary>
        Identification = 1,

        /// <summary>
        ///     The security context can be used to access local resources.
        /// </summary>
        Impersonation = 2,

        /// <summary>
        ///     The security context can be used to access remote resources.
        /// </summary>
        Delegation = 3
    }
}