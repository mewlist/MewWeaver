using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Mewlist.Weaver.Sample
{
    public class MethodProfilingILInjector : IILInjector
    {
        private MethodInfo MethodInfo { get; }

        public MethodProfilingILInjector(Action<string, Stopwatch> method)
        {
            MethodInfo = method.Method;
        }

        public void Validate(ICustomAttribute customAttribute)
        {
            var parameters = MethodInfo.GetParameters();

            if (parameters.Length != 2)
                throw new ArgumentException();
            if (parameters[0].ParameterType != typeof(string))
                throw new ArgumentException();
            if (parameters[1].ParameterType != typeof(Stopwatch))
                throw new ArgumentException();
        }

        public void Inject(CustomAttribute customAttribute, ModuleDefinition moduleDefinition, MethodDefinition methodDefinition)
        {
            var stopWatchType = typeof(Stopwatch);
            var stopWatchTypeRef = moduleDefinition.ImportReference(stopWatchType);
            var stringType = typeof(string);
            var stopWatchCtor = stopWatchType.GetConstructor(new Type[] { });
            var stopWatchCtorRef = moduleDefinition.ImportReference(stopWatchCtor);
            var stopWatchStartRef = moduleDefinition.ImportReference(stopWatchType.GetMethod("Start"));
            var stopWatchStopRef = moduleDefinition.ImportReference(stopWatchType.GetMethod("Stop"));
            var loggerRef = moduleDefinition.ImportReference(MethodInfo);
            var methodName = methodDefinition.FullName;

            var stopWatchVariable = new VariableDefinition(stopWatchTypeRef);
            methodDefinition.Body.Variables.Add(stopWatchVariable);

            var processor = methodDefinition.Body.GetILProcessor();
            var first = methodDefinition.Body.Instructions.First();
            var last = methodDefinition.Body.Instructions.Last();

            processor.InsertBefore(first, Instruction.Create(OpCodes.Newobj, stopWatchCtorRef));
            processor.InsertBefore(first, Instruction.Create(OpCodes.Stloc, stopWatchVariable));
            processor.InsertBefore(first, Instruction.Create(OpCodes.Ldloc, stopWatchVariable));
            processor.InsertBefore(first, Instruction.Create(OpCodes.Callvirt, stopWatchStartRef));

            processor.InsertBefore(last, Instruction.Create(OpCodes.Ldloc, stopWatchVariable));
            processor.InsertBefore(last, Instruction.Create(OpCodes.Callvirt, stopWatchStopRef));

            processor.InsertBefore(last, Instruction.Create(OpCodes.Ldstr, methodName));
            processor.InsertBefore(last, Instruction.Create(OpCodes.Ldloc, stopWatchVariable));
            processor.InsertBefore(last, Instruction.Create(OpCodes.Call, loggerRef));
        }
    }
}