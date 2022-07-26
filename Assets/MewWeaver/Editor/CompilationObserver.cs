using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Mewlist.Weaver
{
    [InitializeOnLoad]
    public static class CompilationObserver
    {
        private const string EditorAssemblyPath = "Library/ScriptAssemblies";
        private const string PlayerAssemblyPath = "Library/PlayerScriptAssemblies";

        private static readonly string[] ExcludedAssemblies = new[] { "MewWeaver.Tests.Editor.dll" };

        private static void CompilationPipelineOnCompilationFinished(object obj)
        {
            if (!CompilationPipeline.GetAssemblies().Any(x => x.outputPath.StartsWith(EditorAssemblyPath)))
            {
                Debug.Log("[MewWeaver] Inject into Player Assembly");
                Inject(CompilationPipeline.GetAssemblies());
            }
        }

        static CompilationObserver()
        { 
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
            CompilationPipeline.compilationFinished += CompilationPipelineOnCompilationFinished;
        }

        private static void OnDomainUnload(object sender, EventArgs e)
        {
            CompilationPipeline.compilationFinished -= CompilationPipelineOnCompilationFinished;
            AppDomain.CurrentDomain.DomainUnload -= OnDomainUnload;
            WeaverLogger.Clear();
            if (CompilationPipeline.GetAssemblies().Any(x => x.outputPath.StartsWith(EditorAssemblyPath)))
            {
                Debug.Log("[MewWeaver] Inject into Editor Assembly");
                Inject(CompilationPipeline.GetAssemblies());
            }
        }

        private static void Inject(Assembly[] assemblies)
        {
            foreach (var target in AppDomain.CurrentDomain.GetAssemblies()
                         .Where(x => !x.IsDynamic))
            {
                if (ExcludedAssemblies.Any(x => target.Location.Contains(x)))
                {
                    continue;
                }

                foreach (var type in target.GetTypes())
                {
                    if (type.IsInterface || !typeof(IWeaver).IsAssignableFrom(type)) continue;
                    WeaverLogger.Log($"Start Weaver({type.Name})");
                    var weaver = (IWeaver)Activator.CreateInstance(type);
                    weaver.Weave(new AssemblyInjector(assemblies));
                }
            }

            Debug.Log($"[MewWeaver] Weaving Finished.\n" +
                      $" ---------- \n" +
                      $"{string.Join("\n", WeaverLogger.Logs)}\n" +
                      $" ----------");
        }
    }
}