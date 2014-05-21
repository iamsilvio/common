using System.IO;
using doe.Common.Security.Impersonation;

namespace doe.Common.IO
{
    /// <summary>
    /// <see cref="System.IO.FileInfo"/> extensions to provide impersonation features
    /// </summary>
    public static class FileInfoExtension
    {
        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <returns>
        /// true if the caller has the required permissions and FullName contains the name of an existing file; 
        /// otherwise, false. 
        /// This method also returns false if FullName is null, an invalid path,
        /// or a zero-length string. 
        /// If the caller does not have sufficient permissions to read the specified file, 
        /// no exception is thrown and the method returns false regardless of the existence of FullName.
        /// </returns>
        public static bool Exists(this FileInfo fileInfo, DomainUser user)
        {
            // ReSharper disable once UnusedVariable
            using (
                var impersonation = new Impersonation(user.Domain, user.Name, user.Pwd, ImpersonationLevel.Delegation))
            {
                return File.Exists(fileInfo.FullName);
            }
        }
    }
}