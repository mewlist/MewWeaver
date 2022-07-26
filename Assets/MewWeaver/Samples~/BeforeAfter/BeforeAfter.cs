using UnityEngine;

namespace Mewlist.Weaver.Sample
{
    public class BeforeAfter
    {
        public static void Before()
        {
            Debug.Log($"Before");
        }

        public static void BeforeInt(int value)
        {
            Debug.Log($"Before {value}");
        }

        public static void BeforeString(string value)
        {
            Debug.Log($"Before {value}");
        }

        public static void BeforeEnum(BeforeAfterEnum value)
        {
            Debug.Log($"Before {value}");
        } 

        public static void After()
        {
            Debug.Log($"After");
        }

        public static void AfterInt(int value)
        {
            Debug.Log($"After {value}");
        }

        public static void AfterEnum(BeforeAfterEnum value)
        {
            Debug.Log($"After {value}");
        } 

        public static void AfterString(string value)
        {
            Debug.Log($"After {value}");
        }
    }
}