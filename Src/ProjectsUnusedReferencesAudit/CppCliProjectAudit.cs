using System;
using System.Collections.Generic;
using RemoveUnusedRef.UsedReferencesAuditEntries;

namespace RemoveUnusedRef.ProjectsUnusedReferencesAudit
{
    public class CppCliProjectAudit : UnusedReferencesAudit
    {
        public CppCliProjectAudit(ProjectInfo projectInfo)
            : base(projectInfo)
        {
        }
        
        protected override IList<IUsedReferencesAuditEntry> GetUsedReferencesAuditEntryCollection()
        {
            return new List<IUsedReferencesAuditEntry>
            {
                new ClassesBaseTypesEntry(),
                new InterfacesTypesEntry(),
                new TypesAttributesEntry(),
                new DependentAssembliesEntry(),
                new ImportedTypesEntry()
            };
        }
    }
}