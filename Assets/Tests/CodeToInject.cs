using System.Collections.Generic;

namespace Mewlist.Weaver.Test
{
    public static class CodeToInject
    {
        public static List<string> Log = new();
        public static void Clear() => Log.Clear();
        public static void CallBeforeInjectionCode()
        {
            Log.Add("Before Injected");
        }

        public static void CallAfterInjectionCode()
        {
            Log.Add("After Injected");
        }
        
        public static void Call()
        {
            Log.Add("Call");
        }
    }
}