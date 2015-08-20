using System;
using System.Diagnostics;
using doe.Common.Diagnostics.Model;

namespace doe.Common.Diagnostics
{
  public class BackgroundProcess
  {
    /// <summary>
    /// Executes the process.
    /// </summary>
    /// <param name="startInfo">Takes a ProcessStartInfo object to start</param>
    /// <returns></returns>
    public static BackgroundProcessResult Execute(ProcessStartInfo startInfo)
    {
      try
      {
        startInfo.RedirectStandardError = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;

        using (var process = Process.Start(startInfo))
        {
          var output = string.Empty;
          string error = null;

          if (process != null)
          {
            output = process.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();
            process.WaitForExit();
          }

          if (string.IsNullOrEmpty(error))
          {
            return 
              new BackgroundProcessResult {IsError = false, Result = output};
          }

          Log.Error(error);
          return 
            new BackgroundProcessResult
            {
              IsError = true, 
              Result = output,
              Error = error
            };
        }
      }
      catch (Exception e)
      {
        Log.Error(e);
        return
            new BackgroundProcessResult
            {
              IsError = true,
              Result = "",
              Error = e.Message
            };
      }
    }

  }
}
