using System.IO;
using System.Reflection;

namespace doe.Common.Application
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
