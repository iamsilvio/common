using System.IO;
using System.Reflection;

namespace deleteonerror.Common.Application
{
  public class Env
  {
    public static string GetApplicationPath()
    {
      return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
      //return Assembly.GetEntryAssembly().Location;
    }
  }
}
