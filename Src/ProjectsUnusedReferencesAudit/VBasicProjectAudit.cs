using System;
using System.Collections.Generic;
using RemoveUnusedRef.UsedReferencesAuditEntries;

namespace RemoveUnusedRef.ProjectsUnusedReferencesAudit
{
    public class VBasicProjectAudit : UnusedReferencesAudit
    {
        public VBasicProjectAudit(ProjectInfo projectInfo)
            : base(projectInfo)
        {
        }
        
        protected override IList<IUsedReferencesAuditEntry> GetUsedReferencesAuditEntryCollection()
        {
            return new List<IUsedReferencesAuditEntry>
            {
                new AssemblyManifestEntry(),
                new ClassesBaseTypesEntry(),
                new InterfacesTypesEntry(),
                new TypesAttributesEntry(),
                new DependentAssembliesEntry(),
                new ImportedTypesEntry()
            };
        }
    }
}