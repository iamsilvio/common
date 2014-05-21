using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using doe.Common.Diagnostic;

namespace doe.Common.Security.Impersonation
{
    /// <summary>
    ///     Provides a mechanism for impersonating a user.  This is intended to be disposable, and
    ///     used in a using ( ) block.
    /// </summary>
    public class Impersonation : IDisposable
    {
        #region Externals

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken);

        // ReSharper disable once InconsistentNaming
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool DuplicateToken(IntPtr existingTokenHandle, int
            SECURITY_IMPERSONATION_LEVEL, out IntPtr duplicateTokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        #endregion

        #region Fields

        private readonly string _domain;
        private readonly ImpersonationLevel _level;
        private readonly string _password;
        private readonly string _username;
        private WindowsImpersonationContext _context;
        private WindowsIdentity _identity;
        private IntPtr _token;
        private IntPtr _tokenDuplicate;
        private readonly LogonProvider _logonProvider;
        private readonly LogonType _logonType;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="Impersonation" />.
        /// </summary>
        /// <param name="domain">The domain of the target user.</param>
        /// <param name="username">The target user to impersonate.</param>
        /// <param name="password">The target password of the user to impersonate.</param>
        public Impersonation(string domain, string username, string password)
        {
            _domain = domain;
            _username = username;
            _password = password;
            _level = ImpersonationLevel.Impersonation;
            _logonProvider = LogonProvider.WinNT50;
            _logonType = LogonType.NewCredentials;

            Logon();
        }

        /// <summary>
        ///     Initialises a new instance of <see cref="Impersonation" />.
        /// </summary>
        /// <param name="domain">The domain of the target user.</param>
        /// <param name="username">The target user to impersonate.</param>
        /// <param name="password">The target password of the user to impersonate.</param>
        /// <param name="level">The security level of this impersonation.</param>
        public Impersonation(string domain, string username, string password, ImpersonationLevel level)
        {
            _domain = domain;
            _username = username;
            _password = password;
            _level = level;
            _logonProvider = LogonProvider.WinNT50;
            _logonType = LogonType.NewCredentials;

            Logon();
        }


        /// <summary>
        /// Initialises a new instance of <see cref="Impersonation" />.
        /// </summary>
        /// <param name="domain">The domain of the target user.</param>
        /// <param name="username">The target user to impersonate.</param>
        /// <param name="password">The target password of the user to impersonate.</param>
        /// <param name="level">The security level of this impersonation.</param>
        /// <param name="logontype">The logontype.</param>
        /// <param name="logonProvider">The logon provider.</param>
        public Impersonation(string domain,
            string username,
            string password,
            ImpersonationLevel level,
            LogonType logontype,
            LogonProvider logonProvider)
        {
            _domain = domain;
            _username = username;
            _password = password;
            _level = level;
            _logonProvider = logonProvider;
            _logonType = logontype;


            Logon();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Reverts the impersonation.
        /// </summary>
        public void Dispose()
        {
            if (_context != null)
                _context.Undo();

            if (_token != IntPtr.Zero)
                CloseHandle(_token);

            if (_tokenDuplicate != IntPtr.Zero)
                CloseHandle(_tokenDuplicate);
        }

        /// <summary>
        ///     Performs the logon.
        /// </summary>
        private void Logon()
        {
            if (LogonUser(_username, _domain, _password, (int)_logonType, (int)_logonProvider, out _token))
            {

                if (DuplicateToken(_token, (int)_level, out _tokenDuplicate))
                {
                    _identity = new WindowsIdentity(_tokenDuplicate);
                    _context = _identity.Impersonate();
                }
                else
                {
                    Log.Warning(string.Format("{0} {1} {2}", _domain, _username, _password));
                    Log.Error(new SecurityException("Unable to impersonate the user."));
                }
            }
            else
            {
                Log.Warning(string.Format("{0} {1} {2}", _domain, _username, _password));
                Log.Error(new SecurityException("The login details you have entered were incorrect."));
            }
        }

        #endregion
    }
}