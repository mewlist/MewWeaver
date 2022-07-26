using System.Diagnostics;

namespace Mewlist.Weaver.Sample
{
    public static class MethodProfilingLogger
    {
        public static void Log(string tag, Stopwatch stopwatch)
        {
            UnityEngine.Debug.Log($"[{tag}] {stopwatch.Elapsed}");
        }
    }
}