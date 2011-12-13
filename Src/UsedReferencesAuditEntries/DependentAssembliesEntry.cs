using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using RemoveUnusedRef.UsedReferencesAuditEntries.Workers;

namespace RemoveUnusedRef.UsedReferencesAuditEntries
{
    public class DependentAssembliesEntry : IUsedReferencesAuditEntry
    {
        private readonly ClassTypeHierarchyWorker m_classTypeHierarchyWorker;
        private readonly InterfacesTypeWorker m_interfacesTypeWorker;
        private readonly MemberReferencesWorker m_memberReferencesWorker;
        
        public DependentAssembliesEntry()
        {
            m_interfacesTypeWorker = new InterfacesTypeWorker();
            m_classTypeHierarchyWorker = new ClassTypeHierarchyWorker();
            m_memberReferencesWorker = new MemberReferencesWorker();
        }
        
        public IEnumerable<ProjectReference> Execute(AuditEntryParameters parameters)
        {
            var references = new List<ProjectReference>();
            foreach(var assemblyDefinition in parameters.ProjectMetadata.ManifestAssemblies)
            {
                foreach(var reference in GetAssemblyTypesReferences(assemblyDefinition, parameters))
                {
                    if (!references.Contains(reference))
                        references.Add(reference);
                }
            }
            return references;
        }
        
        private IEnumerable<ProjectReference> GetAssemblyTypesReferences(
            AssemblyDefinition assemblyDefinition, AuditEntryParameters parameters)
        {
            var referencedAssemblyTypes = parameters.ProjectMetadata.TypesReferences.
                Select(typeReference => typeReference).
                Join(Helper.GetTypesDefinitions(assemblyDefinition.Modules),
                     typeReference => typeReference.FullName.ToLower(),
                     moduleTypeDefinition => moduleTypeDefinition.FullName.ToLower(),
                     (typeReference, moduleTypeDefinition) => typeReference);
            foreach(var assemblyType in referencedAssemblyTypes)
            {
                if (!parameters.IsTypeChecked(assemblyType))
                {
                    TypeDefinition assemblyBaseType = null;
                    assemblyBaseType = assemblyType.BaseType != null ?
                        assemblyType.BaseType.Resolve()
                        : null;
                    if (assemblyBaseType != null)
                    {
                        var pr = parameters.FindProjectReference(assemblyBaseType.Scope);
                        if (pr != null)
                            yield return pr;
                        foreach(var projectReference in 
                                m_classTypeHierarchyWorker.Execute(assemblyBaseType, parameters))
                            yield return projectReference;
                        foreach(var projectReference in 
                                m_interfacesTypeWorker.Execute(assemblyBaseType, parameters))
                            yield return projectReference;
                    }
                }
                foreach(var projectReference in m_memberReferencesWorker.Execute(assemblyType, parameters))
                    yield return projectReference;
            }
        }
    }
}