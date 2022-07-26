using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Pdb;

namespace Mewlist.Weaver
{
    public class AssemblyInjector
    {
        private DefaultAssemblyResolver resolver;
        private UnityEditor.Compilation.Assembly[] assemblies;
        
        public AssemblyInjector(UnityEditor.Compilation.Assembly[] assemblies)
        {
            var domain = AppDomain.CurrentDomain;
            this.assemblies = assemblies;
            resolver = new DefaultAssemblyResolver();

            var referencedAssemblies = domain.GetAssemblies()
                .Where(x => !x.IsDynamic)
                .Where(x => !string.IsNullOrEmpty(x.Location));

            var referencedAssemblyLocations = referencedAssemblies
                .Select(x => Path.GetDirectoryName(x.Location))
                .Distinct();

            foreach (var location in referencedAssemblyLocations)
                resolver.AddSearchDirectory(location); 
        }
 
        public UnityEditor.Compilation.Assembly Find(string name)
        {
            return assemblies.FirstOrDefault(x => x.name == name);
        }

        public void Inject(UnityEditor.Compilation.Assembly assembly, IMethodInjector methodInjector)
        {
            using var assemblyStream = new FileStream(assembly.outputPath, FileMode.Open, FileAccess.ReadWrite);
            using var moduleDefinition = ModuleDefinition.ReadModule(assemblyStream, new ReaderParameters
            {
                ReadingMode = ReadingMode.Immediate,
                ReadWrite = true,
                AssemblyResolver = resolver,
                ReadSymbols = true,
                SymbolReaderProvider = new PdbReaderProvider()
            });
            
            foreach (var typeDefinition in moduleDefinition.Types)
            {
                foreach (var methodDefinition in typeDefinition.Methods)
                {
                    methodInjector.Inject(moduleDefinition, methodDefinition);
                }
            }

            RemoveSelfReference(moduleDefinition);

            moduleDefinition.Write(new WriterParameters()
            {
                WriteSymbols = true,
                SymbolWriterProvider = new PdbWriterProvider()
            });
        }

        private void RemoveSelfReference(ModuleDefinition moduleDefinition)
        {
            var sameAssembly = moduleDefinition.AssemblyReferences
                .FirstOrDefault(x => x.FullName == moduleDefinition.Assembly.FullName);

            if (sameAssembly is not null)
                moduleDefinition.AssemblyReferences.Remove(sameAssembly);
        }

        public AssemblyInjectorBinder OnAssembly(string name)
        {
            return new AssemblyInjectorBinder(this).OnAssembly(name);
        }

        public AssemblyInjectorBinder OnMainAssembly()
        {
            return OnAssembly("Assembly-CSharp");
        }
    }
}