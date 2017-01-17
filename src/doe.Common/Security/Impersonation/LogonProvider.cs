namespace deleteonerror.Common.Security.Impersonation
{
    /// <summary>
    /// Specifies the logon provider. 
    /// advapi32.dll
    /// </summary>
    public enum LogonProvider
    {
        /// <summary>
        /// LOGON32_PROVIDER_DEFAULT
        /// Use the standard logon provider for the system. 
        /// The default security provider is negotiate, 
        /// unless you pass NULL for the domain name 
        /// and the user name is not in UPN format. 
        /// In this case, the default provider is NTLM.
        /// </summary>
        Default = 0,
        /// <summary>
        /// LOGON32_PROVIDER_WINNT35
        /// </summary>
        WinNT35 = 1,
        /// <summary>
        /// LOGON32_PROVIDER_WINNT40
        /// Use the NTLM logon provider.
        /// </summary>
        WinNT40 = 2,
        /// <summary>
        /// LOGON32_PROVIDER_WINNT50
        /// Use the negotiate logon provider.
        /// </summary>
        WinNT50 = 3
    }
}