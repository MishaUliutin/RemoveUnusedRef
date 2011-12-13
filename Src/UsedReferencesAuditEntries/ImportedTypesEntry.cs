using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using RemoveUnusedRef.Extensions;
using RemoveUnusedRef.UsedReferencesAuditEntries.Workers;

namespace RemoveUnusedRef.UsedReferencesAuditEntries
{
    public class ImportedTypesEntry : IUsedReferencesAuditEntry
    {
        private readonly ImportedTypeWorker m_importedTypeWorker;
        
        public ImportedTypesEntry()
        {
            m_importedTypeWorker = new ImportedTypeWorker();
        }
        
        public IEnumerable<ProjectReference> Execute(AuditEntryParameters parameters)
        {
            var references = new List<ProjectReference>();
            AddUniqueReferences(GetImportedClassesReferences(parameters), references);
            AddUniqueReferences(GetImportedInterfacesReferences(parameters), references);
            yield break;
        }
        
        private IEnumerable<ProjectReference> GetImportedClassesReferences(AuditEntryParameters parameters)
        {
            foreach (var typeDefinition in parameters.ProjectMetadata.ImportedTypesDefinitions)
            {
                var projectReference = m_importedTypeWorker.Execute(typeDefinition, parameters);
                if (projectReference != null)
                    yield return projectReference;
            }
        }
        
        private IEnumerable<ProjectReference> GetImportedInterfacesReferences(
            AuditEntryParameters parameters)
        {
            var typeReferences = parameters.ProjectMetadata.ImportedTypesDefinitions.
                SelectMany(item => item.GetInterfacesHierarchy());
            foreach(var typeReference in typeReferences)
            {
                var typeDefinition = typeReference.Resolve();
                if (typeDefinition != null)
                {
                    var projectReference = m_importedTypeWorker.Execute(typeDefinition, parameters);
                    if (projectReference != null)
                        yield return projectReference;
                }
            }
            yield break;
        }
        
        private static void AddUniqueReferences(
            IEnumerable<ProjectReference> projectReferences, IList<ProjectReference> listReferences)
        {
            foreach(var reference in projectReferences)
            {
                if (!listReferences.Contains(reference))
                    listReferences.Add(reference);
            }
        }
    }
}