using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace RemoveUnusedRef.UsedReferencesAuditEntries.Workers
{
    public class ClassTypeHierarchyWorker
    {
        public IEnumerable<ProjectReference> Execute(
            TypeDefinition typeDefinition, AuditEntryParameters parameters)
        {
            if (typeDefinition == null)
                yield break;
            var baseTypeReference = typeDefinition.BaseType;
            if (baseTypeReference == null || parameters.IsTypeChecked(baseTypeReference))
            {
                parameters.AddToCheckedTypes(typeDefinition);
                yield break;
            }
            var baseTypeDefinition = baseTypeReference.Resolve();
            if (baseTypeDefinition != null)
            {
                var projectReference = parameters.FindProjectReference(baseTypeDefinition);
                if (projectReference != null)
                    yield return projectReference;
            }
            foreach(var projectReference in Execute(baseTypeDefinition, parameters)) 
                yield return projectReference;
            parameters.AddToCheckedTypes(typeDefinition);
        }
    }
}