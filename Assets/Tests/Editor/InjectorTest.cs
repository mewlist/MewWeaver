using System.Collections;
using System.IO;
using System.Linq;
using Mewlist.Weaver;
using Mewlist.Weaver.Test;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine.TestTools;

public class BeforeAfterInjectionTest
{
    private const string TestScriptPath = "Assets/Tests/TestSubject.cs";

    private const string TestSubjectCode = @"
namespace Mewlist.Weaver.Test
{
    public class TestSubject
    {
        [ATTRIBUTE]
        public void A() { CodeToInject.Call(); }
    }
}
";

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Code is not injected
        CodeToInject.Clear();
        var subject = new TestSubject();
        subject.A();
        Assert.IsTrue(CodeToInject.Log.SequenceEqual(new[] { "Call" }));

        // Create Target Code
        using (var file = File.CreateText(TestScriptPath))
        {
            file.Write(TestSubjectCode.Replace("[ATTRIBUTE]", "[WeaverTest]"));
        }

        // Compile Target Code
        try
        {
            CompilationPipeline.compilationFinished += CompilationPipelineOnCompilationFinished;
            AssetDatabase.Refresh();
        
            yield return new RecompileScripts();
            yield return new WaitForDomainReload();
            UnityEngine.Debug.Log("-------------Domain Reloaded---------------");
        }
        finally
        {
            CompilationPipeline.compilationFinished -= CompilationPipelineOnCompilationFinished;
        }
    }

    private void CompilationPipelineOnCompilationFinished(object obj)
    {
        UnityEngine.Debug.Log("-------------Inject---------------");
        var injector = new AssemblyInjector(CompilationPipeline.GetAssemblies());
        var weaver = new TestWeaver();
        weaver.Weave(injector);
        UnityEngine.Debug.Log($"[MewWeaver] Weaving Finished.\n ---------- \n{string.Join("\n", WeaverLogger.Logs)}\n ----------");
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Restore Target Code
        using (var file = File.CreateText(TestScriptPath))
        {
            file.Write(TestSubjectCode.Replace("[ATTRIBUTE]", string.Empty));
        }
        AssetDatabase.Refresh();
        yield return new RecompileScripts();
    }

    [Test]
    public void Test()
    {
        CodeToInject.Clear();

        var subject = new TestSubject();
        subject.A();

        Assert.IsTrue(CodeToInject.Log.SequenceEqual(new[] { "Before Injected", "Call", "After Injected" }));
    }
}