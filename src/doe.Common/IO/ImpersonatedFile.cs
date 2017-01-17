using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using deleteonerror.Common.Diagnostics;
using deleteonerror.Common.Security.Impersonation;

namespace deleteonerror.Common.IO
{
    /// <summary>
    /// Wrapper around <seealso cref="System.IO.File"/>  
    /// to impersonate the application on the File System
    /// </summary>
    public class ImpersonatedFile
    {
        /// <summary>
        /// Gets the files at target.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="user">the Domain User</param>
        /// <param name="recursive"></param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <returns></returns>
        public static List<FileInfo> GetFiles(string path, DomainUser user, 
            bool recursive, bool debug = false)
        {
          if (string.IsNullOrEmpty(user.Domain)
            || string.IsNullOrEmpty(user.Name)
            || string.IsNullOrEmpty(user.Pwd))
          {
            return GetFiles(path, recursive);
          }
          
          // ReSharper disable once UnusedVariable
          using (
            var impersonation = new Impersonation(user.Domain, user.Name,
              user.Pwd, ImpersonationLevel.Delegation))
          {
            return GetFiles(path, recursive);
          }
        }

        private static List<FileInfo> GetFiles(string path,
            bool recursive)
        {
            var fileList = new List<FileInfo>();

            try
            {
              fileList.AddRange(Directory.GetFiles(path).Select(file => new FileInfo(file)));

              if (recursive)
                {
                    foreach (var dir in Directory.GetDirectories(path))
                    {
                        fileList.AddRange(GetFiles(dir, true));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return fileList;
        }

        /// <summary>
        /// Deletes the specified directory and, if indicated, 
        /// any subdirectories and files in the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="user">The user.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <returns></returns>
        public static FileOperationResult DeleteDirectory(string path, 
            DomainUser user, bool recursive = false, bool debug = false)
        {
            FileOperationResult result;
            // ReSharper disable once UnusedVariable
            using (
                var impersonation = new Impersonation(user.Domain, user.Name, 
                    user.Pwd, ImpersonationLevel.Delegation))
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        Directory.Delete(path, recursive);
                        result = new FileOperationResult(path,
                            FileOperation.Delete, true);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        result = new FileOperationResult(path,
                            FileOperation.Delete, ex);
                    }
                }
                else
                {
                    result = new FileOperationResult(path,
                            FileOperation.Delete, false, "Directory not Found");
                }
            }
            return result;
        }

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="user">The user.</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <returns></returns>
        public static List<FileOperationResult> DeleteFiles(
            IEnumerable<FileInfo> files, DomainUser user, bool debug = false)
        {
            return files.Select(file => DeleteFile(file, user, debug)).ToList();
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="user">The user.</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <returns></returns>
        public static FileOperationResult DeleteFile(FileInfo file, 
            DomainUser user, bool debug = false)
        {
            FileOperationResult result; 
            // ReSharper disable once UnusedVariable
            using (
                var impersonation = new Impersonation(user.Domain, user.Name, 
                    user.Pwd, ImpersonationLevel.Delegation))
            {
                try
                {
                    File.Delete(file.FullName);
                    result = new FileOperationResult(file.FullName,
                            FileOperation.Delete, true);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    result = new FileOperationResult(file.FullName,
                        FileOperation.Delete, ex);
                }
            }
            return result;
        }

        /// <summary>
        /// Copies the files.
        /// </summary>
        /// <param name="sourceFiles">The files.</param>
        /// <param name="destination">the copy target path</param>
        /// <param name="overwrite">determines if the files at target should be overwriten.</param>
        /// <param name="user">The user.</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <returns></returns>
        public static List<FileOperationResult> CopyFiles(
            IEnumerable<FileInfo> sourceFiles, string destination, bool overwrite, DomainUser user, bool debug = false)
        {
            var results = new List<FileOperationResult>();

            // ReSharper disable once UnusedVariable
            using (
                var impersonation = new Impersonation(user.Domain, user.Name,
                    user.Pwd, ImpersonationLevel.Delegation))
            {
                results.AddRange(sourceFiles.Select(file => CopyFile(file, destination, overwrite, debug)));
            }

            return results;
        }

        /// <summary>
        /// moves the files.
        /// </summary>
        /// <param name="sourceFiles">The files.</param>
        /// <param name="destination">the copy target path</param>
        /// <param name="overwrite">determines if the files at target should be overwriten.</param>
        /// <param name="user">The user.</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <returns></returns>
        public static List<FileOperationResult> MoveFiles(
            IEnumerable<FileInfo> sourceFiles, string destination, bool overwrite, DomainUser user, bool debug = false)
        {
            var results = new List<FileOperationResult>();

            // ReSharper disable once UnusedVariable
            using (
                var impersonation = new Impersonation(user.Domain, user.Name,
                    user.Pwd, ImpersonationLevel.Delegation))
            {
                results.AddRange(sourceFiles.Select(file => MoveFile(file, destination, overwrite, debug)));
            }

            return results;
        }


        private static FileOperationResult CopyFile(FileSystemInfo sourceFile, string destination, bool overwrite,
            bool debug = false)
        {
            FileOperationResult result;
            try
            {
                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                    if (debug)
                    {
                        Log.Info($"cdir {destination}");
                    }
                }

                var dst = Path.Combine(destination, sourceFile.Name);

                if (!File.Exists(dst) || (File.Exists(dst) && overwrite))
                {
                    File.Copy(sourceFile.FullName, dst, true);

                    result = new FileOperationResult(dst, FileOperation.Copy, true);
                }
                else
                {
                    if (debug)
                    {
                        Log.Warning($"file already exists {dst}");
                    }
                    result = new FileOperationResult(dst, FileOperation.Copy, false, "file already exists");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                result = new FileOperationResult(sourceFile.FullName, FileOperation.Copy, ex);
            }

            return result;
        }

        private static FileOperationResult MoveFile(FileSystemInfo sourceFile, string destination, bool overwrite,
            bool debug = false)
        {
            FileOperationResult result;
            try
            {
                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                    if (debug)
                    {
                        Log.Info($"cdir {destination}");
                    }
                }

                var dst = Path.Combine(destination, sourceFile.Name);

                if (File.Exists(dst) && overwrite)
                {
                    File.Delete(dst);
                }

                if (!File.Exists(dst))
                {
                    File.Move(sourceFile.FullName, dst);

                    result = new FileOperationResult(dst, FileOperation.Move, true);
                }
                else
                {
                    if (debug)
                    {
                        Log.Warning($"file already exists {dst}");
                    }
                    result = new FileOperationResult(dst, FileOperation.Move, false, "file already exists");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                result = new FileOperationResult(sourceFile.FullName, FileOperation.Move, ex);
            }

            return result;
        }

    }
}