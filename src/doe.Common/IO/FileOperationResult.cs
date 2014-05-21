using System;

namespace doe.Common.IO
{
    public class FileOperationResult
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FileOperationResult" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="opperation">The opperation.</param>
        /// <param name="succeeded">if set to <c>true</c> [succeeded].</param>
        /// <param name="message">The message.</param>
        public FileOperationResult(string fileName, FileOperation opperation, bool succeeded, string message)
        {
            FileName = fileName;
            FileOperation = opperation;
            Succeeded = succeeded;
            Message = message;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileOperationResult" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="opperation">The opperation.</param>
        /// <param name="exception">The exception.</param>
        public FileOperationResult(string fileName, FileOperation opperation, Exception exception)
        {
            FileName = fileName;
            FileOperation = opperation;
            Succeeded = false;
            Message = exception.Message;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileOperationResult" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="opperation">The opperation.</param>
        /// <param name="succeeded">if set to <c>true</c> [succeeded].</param>
        public FileOperationResult(string fileName, FileOperation opperation, bool succeeded)
        {
            FileName = fileName;
            FileOperation = opperation;
            Succeeded = succeeded;
        }

        /// <summary>
        ///     Gets or sets the name of the file.
        /// </summary>
        /// <value>
        ///     The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        ///     Gets or sets the file operation.
        /// </summary>
        /// <value>
        ///     The file operation.
        /// </value>
        public FileOperation FileOperation { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [succeeded].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [succeeded]; otherwise, <c>false</c>.
        /// </value>
        public bool Succeeded { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>
        ///     The message.
        /// </value>
        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", FileOperation, Succeeded ? "succeded" : "failed", FileName,
                Message ?? "");
        }
    }
}