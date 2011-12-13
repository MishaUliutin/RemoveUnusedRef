using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoveUnusedRef
{	
    public abstract class UnusedReferencesAudit
    {
        private const string MsCorLib = "mscorlib";
        private const string SystemCore = "System.Core";
        
        private readonly ProjectInfo m_projectInfo;

        protected UnusedReferencesAudit(ProjectInfo projectInfo)
        {
            if (projectInfo == null)
                throw new ArgumentNullException("projectInfo");
            m_projectInfo = projectInfo;
        }
        
        public IEnumerable<ProjectReference> Execute()
        {
            var auditEntryCollection = GetUsedReferencesAuditEntryCollection();
            var unusedReferences = CreateUnusedReferencesList(m_projectInfo);
            var projectMetadata = CreateProjectMetadata(m_projectInfo);
            var parameters = new AuditEntryParameters
            {
                References = unusedReferences,
                ProjectMetadata = projectMetadata,
                CheckedTypes = new List<string>()
            };
            for (var i = 0; (i < auditEntryCollection.Count) && (unusedReferences.Count != 0); i++)
            {
                var usedReferences = 
                    auditEntryCollection[i].Execute(parameters);
                RemoveUsedReferences(unusedReferences, usedReferences);
            }
            return unusedReferences;
        }
        
        protected abstract IList<IUsedReferencesAuditEntry> GetUsedReferencesAuditEntryCollection();
        
        private ProjectMetadata CreateProjectMetadata(ProjectInfo projectInfo)
        {
            return new ProjectMetadata(projectInfo);
        }
        
        private void RemoveUsedReferences(
            IList<ProjectReference> unusedReferences, IEnumerable<ProjectReference> usedReferences)
        {
            foreach(var item in usedReferences)
                unusedReferences.Remove(item);
        }
        
        private IList<ProjectReference> CreateUnusedReferencesList(ProjectInfo projectInfo)
        {
            var references = 
                from item in projectInfo.References
                where !item.Name.Equals(MsCorLib, StringComparison.InvariantCultureIgnoreCase) &&
                    !item.Name.Equals(SystemCore, StringComparison.InvariantCultureIgnoreCase)
                select item;
            return references.ToList();
        }
    }
}