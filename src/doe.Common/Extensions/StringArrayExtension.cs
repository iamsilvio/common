using System.Text;

namespace doe.Common.Extensions
{
  public static class StringArrayExtension
  {
    public static string ConvertToCommaSeparatedString(this string[] array)
    {
      var builder = new StringBuilder();

      for (var i = 0; i < array.Length; i++)
      {
        if (i != array.Length - 1)
        {
          builder.Append(array[i]);
          builder.Append(',');
        }
        else
        {
          builder.Append(array[i]);
        }
      }

      return builder.ToString();
    }

  }
}
