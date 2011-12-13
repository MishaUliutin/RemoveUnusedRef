using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoveUnusedRef.UsedReferencesAuditEntries
{
    public class AssemblyManifestEntry : IUsedReferencesAuditEntry
    {
        public IEnumerable<ProjectReference> Execute(AuditEntryParameters parameters)
        {
            return
                (from reference in parameters.References
                 join assemblyDefinition in parameters.ProjectMetadata.ManifestAssemblies
                     on reference.FullName.ToLower() equals assemblyDefinition.FullName.ToLower()
                 select reference).ToList();
        }
    }
}