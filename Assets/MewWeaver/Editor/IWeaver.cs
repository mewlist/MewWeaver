using UnityEditor.Compilation;

namespace Mewlist.Weaver
{
    public interface IWeaver
    {
        void Weave(AssemblyInjector assemblyInjector);
    }
}