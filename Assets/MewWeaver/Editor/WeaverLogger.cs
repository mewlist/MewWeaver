using System.Collections.Generic;

namespace Mewlist.Weaver
{
    public class WeaverLogger
    {
        private static List<string> logs = new List<string>();
        public static IReadOnlyList<string> Logs => logs;
        public static void Log(string log) => logs.Add(log);
        public static void Clear() => logs.Clear();
    }
}