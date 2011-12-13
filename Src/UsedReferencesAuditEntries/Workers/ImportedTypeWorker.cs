using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace RemoveUnusedRef.UsedReferencesAuditEntries.Workers
{
    public class ImportedTypeWorker
    {
        private readonly Dictionary<ProjectReference, AssemblyDefinition> m_cache;
        
        public ImportedTypeWorker()
        {
            m_cache = new Dictionary<ProjectReference, AssemblyDefinition>();
        }
        
        public ProjectReference Execute(
            TypeDefinition typeDefinition, AuditEntryParameters parameters)
        {
            foreach (ProjectReference projectReference in parameters.References)
            {
                var assemblyDefinition = ReadAssembly(projectReference);
                if (assemblyDefinition != null)
                {
                    var existType = assemblyDefinition.Modules
                        .SelectMany(module => module.Types)
                        .Count(type => type.FullName.Equals(
                            typeDefinition.FullName, StringComparison.InvariantCultureIgnoreCase)) > 0;
                    if (existType)
                        return projectReference;
                }
            }
            return null;
        }
        
        private AssemblyDefinition ReadAssembly(ProjectReference projectReference)
        {
            AssemblyDefinition assemblyDefinition = null;
            if (!m_cache.TryGetValue(projectReference, out assemblyDefinition))
            {
                assemblyDefinition = ReadAssembly(
                    projectReference.Location, Path.GetDirectoryName(projectReference.Location));
                if (assemblyDefinition != null)
                    m_cache[projectReference] = assemblyDefinition;
            }
            return assemblyDefinition;
        }
        
        private AssemblyDefinition ReadAssembly(string assemblyPath, string searchDir)
        {
            var assemblyResolver = new DefaultAssemblyResolver();
            assemblyResolver.AddSearchDirectory(searchDir);
            return 
                AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters
                                                {
                                                    AssemblyResolver = assemblyResolver,
                                                    ReadingMode = ReadingMode.Deferred
                                                });
        }
    }
}