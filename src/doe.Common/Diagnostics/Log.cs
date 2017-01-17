using System;
using System.Diagnostics;

namespace deleteonerror.Common.Diagnostics
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
          stackTrace.GetFrame(1).GetMethod().ReflectedType?.FullName,
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
          stackTrace.GetFrame(1).GetMethod().ReflectedType?.FullName,
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
          stackTrace.GetFrame(1).GetMethod().ReflectedType?.FullName,
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
          stackTrace.GetFrame(1).GetMethod().ReflectedType?.FullName,
          stackTrace.GetFrame(1).GetMethod().Name);
    }

    /// <summary>
    ///     Only gets written in debug mode
    /// </summary>
    /// <param name="message">The message.</param>
    public static void Debug(string message)
    {
#if DEBUG    
      var stackTrace = new StackTrace();
      WriteLog(message, "debug",
          stackTrace.GetFrame(1).GetMethod().Module.Name,
          stackTrace.GetFrame(1).GetMethod().ReflectedType?.FullName,
          stackTrace.GetFrame(1).GetMethod().Name);
#endif
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
      Trace.WriteLine($"{DateTime.Now} ,{type} , {module}, {method}, {scope}, {message}");
    }
  }
}