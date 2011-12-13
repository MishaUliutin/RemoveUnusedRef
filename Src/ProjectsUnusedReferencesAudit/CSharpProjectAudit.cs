using System;
using System.Collections.Generic;
using RemoveUnusedRef.UsedReferencesAuditEntries;

namespace RemoveUnusedRef.ProjectsUnusedReferencesAudit
{
    public class CSharpProjectAudit : UnusedReferencesAudit
    {
        public CSharpProjectAudit(ProjectInfo projectInfo)
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