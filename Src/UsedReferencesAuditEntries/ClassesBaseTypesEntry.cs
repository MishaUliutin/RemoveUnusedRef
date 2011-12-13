using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using RemoveUnusedRef.UsedReferencesAuditEntries.Workers;

namespace RemoveUnusedRef.UsedReferencesAuditEntries
{
    public class ClassesBaseTypesEntry : IUsedReferencesAuditEntry
    {
        public IEnumerable<ProjectReference> Execute(AuditEntryParameters parameters)
        {
            var types = 
                from type in parameters.ProjectMetadata.TypesDefinitions
                where type.BaseType != null && 
                    type.BaseType.Scope.MetadataScopeType == MetadataScopeType.AssemblyNameReference
                select type;
            var worker = new ClassTypeHierarchyWorker();
            return 
                (from type in types
                   from reference in worker.Execute(type, parameters)
                 select reference).Distinct().ToList();
        }
    }
}