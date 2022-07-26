using Mono.Cecil;

namespace Mewlist.Weaver
{
    public interface IILInjector
    {
        void Validate(ICustomAttribute customAttribute);
        void Inject(
            CustomAttribute customAttribute,
            ModuleDefinition moduleDefinition,
            MethodDefinition methodDefinition);

    }
}