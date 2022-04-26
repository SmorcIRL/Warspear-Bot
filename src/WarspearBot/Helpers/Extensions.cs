using System.Collections.Generic;

namespace WarspearBot.Helpers;

public static class Extensions
{
    public static string Join(this IEnumerable<object> value, string separator = ", ")
    {
        return string.Join(separator, value);
    }
}