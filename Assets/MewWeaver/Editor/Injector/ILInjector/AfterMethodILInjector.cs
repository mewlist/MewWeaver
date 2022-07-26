using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Mewlist.Weaver
{
    public class AfterMethodILInjector : AbstractMethodILInjector
    {
        public AfterMethodILInjector(MethodInfo methodInfo) : base(methodInfo) { }

        public override void Inject(CustomAttribute customAttribute, ModuleDefinition moduleDefinition, MethodDefinition methodDefinition)
        {
            var parameterTypes = MethodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
            var parameterTypeNames = MethodInfo.GetParameters().Select(x => x.ParameterType.ToString()); 
            WeaverLogger.Log($"    [After Injection] {MethodInfo.DeclaringType.Namespace}.{MethodInfo.Name}({string.Join(",",  parameterTypeNames)})");

            var processor = methodDefinition.Body.GetILProcessor();
            var injectionPoint = methodDefinition.Body.Instructions.Last();
            var methodRef = moduleDefinition.ImportReference(MethodInfo);

            foreach (var parameterType in parameterTypes)
            {
                if (parameterType == typeof(string))
                    processor.InsertBefore(injectionPoint, Instruction.Create(OpCodes.Ldstr, customAttribute.ConstructorArguments[0].Value.ToString()));
                else if (parameterType == typeof(int))
                    processor.InsertBefore(injectionPoint, Instruction.Create(OpCodes.Ldc_I4, (int)customAttribute.ConstructorArguments[0].Value));
                else if (parameterType.IsEnum)
                    processor.InsertBefore(injectionPoint, Instruction.Create(OpCodes.Ldc_I4, (int)customAttribute.ConstructorArguments[0].Value));
            }
            processor.InsertBefore(injectionPoint, Instruction.Create(OpCodes.Call, methodRef));
        }
    }
}