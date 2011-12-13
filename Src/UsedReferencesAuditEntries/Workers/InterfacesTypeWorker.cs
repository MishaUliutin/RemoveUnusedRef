using System;
using System.Collections.Generic;
using Mono.Cecil;
using RemoveUnusedRef.Extensions;

namespace RemoveUnusedRef.UsedReferencesAuditEntries.Workers
{
    public class InterfacesTypeWorker
    {
        public IEnumerable<ProjectReference> Execute(
            TypeDefinition typeDefinition, AuditEntryParameters parameters)
        {
            var importedTypeWorker = new ImportedTypeWorker();
            foreach (var interfaceReference in typeDefinition.GetInterfacesHierarchy())
            {
                var interfaceDefinition = interfaceReference.Resolve();
                if (interfaceDefinition != null)
                {
                    ProjectReference projectReference = interfaceDefinition.IsImport ? 
                        importedTypeWorker.Execute(interfaceDefinition, parameters)
                        : parameters.FindProjectReference(interfaceDefinition);
                    if (projectReference != null)
                        yield return projectReference;
                    parameters.AddToCheckedTypes(interfaceDefinition);
                }
            }
        }
    }
}