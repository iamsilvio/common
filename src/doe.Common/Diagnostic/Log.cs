using System;
using System.Diagnostics;

namespace doe.Common.Diagnostic
{
    public static class Log
    {
        /// <summary>
        ///     Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Error(string message)
        {
            var stackTrace = new StackTrace();

            WriteLog(message, "error",
                stackTrace.GetFrame(1).GetMethod().Module.Name,
                stackTrace.GetFrame(1).GetMethod().ReflectedType.FullName,
                stackTrace.GetFrame(1).GetMethod().Name);
        }

        /// <summary>
        ///     Errors the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void Error(Exception exception)
        {
            var stackTrace = new StackTrace();

            var msg = exception.ToString();

            while (exception.InnerException != null)
            {
                msg += msg + "\n";
                msg += exception.InnerException.ToString();
                exception = exception.InnerException;
            }

            WriteLog(exception.Message, "error",
                stackTrace.GetFrame(1).GetMethod().Module.Name,
                stackTrace.GetFrame(1).GetMethod().ReflectedType.FullName,
                stackTrace.GetFrame(1).GetMethod().Name);
        }

        /// <summary>
        ///     Warnings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Warning(string message)
        {
            var stackTrace = new StackTrace();

            WriteLog(message, "warning",
                stackTrace.GetFrame(1).GetMethod().Module.Name,
                stackTrace.GetFrame(1).GetMethod().ReflectedType.FullName,
                stackTrace.GetFrame(1).GetMethod().Name);
        }

        /// <summary>
        ///     Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Info(string message)
        {
            var stackTrace = new StackTrace();

            WriteLog(message, "info",
                stackTrace.GetFrame(1).GetMethod().Module.Name,
                stackTrace.GetFrame(1).GetMethod().ReflectedType.FullName,
                stackTrace.GetFrame(1).GetMethod().Name);
        }


        /// <summary>
        ///     Writes the log.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        /// <param name="module">The module.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="method">The method.</param>
        private static void WriteLog(string message, string type, string module, string scope, string method)
        {
            Trace.WriteLine(string.Format("{0} ,{1} , {2}, {3}, {4},{5}", DateTime.Now, type, module, method, scope,
                message));
        }


        /*
                /// <summary>
                ///     Writes the system event log.
                /// </summary>
                /// <param name="logsource">The logsource.</param>
                /// <param name="message">The message.</param>
                /// <param name="logType">Type of the log.</param>
                private static void WriteSystemEventLog(string logsource, string message,
                    EventLogEntryType logType = EventLogEntryType.Error)
                {
                    if (!EventLog.SourceExists(logsource))
                        EventLog.CreateEventSource(logsource, "Application");

                    EventLog.WriteEntry(logsource, message, logType);
                }
        */
    }
}