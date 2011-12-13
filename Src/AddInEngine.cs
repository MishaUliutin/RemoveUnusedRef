using System;
using System.Collections.Generic;

namespace RemoveUnusedRef
{
    public class AddInEngine
    {
        private readonly IShellProxy m_shellProxy;
        
        public AddInEngine(IShellProxy shellProxy)
        {
            if (shellProxy == null)
                throw new ArgumentNullException("shellProxy");
            m_shellProxy = shellProxy;
        }
        
        public void Cleanup()
        {
            var projectInfo = m_shellProxy.GetProjectInfo();
            if (projectInfo == null)
            {
                //TODO: Add log
                return;
            }
            //TODO: Add log start proccess
            var unusedReferences = GetUnusedReferences(projectInfo);
        }
        
        private static IEnumerable<ProjectReference> GetUnusedReferences(ProjectInfo projectInfo)
        {
            var auditor = UnusedReferencesAuditFactory.Create(projectInfo);
            return auditor.Execute();
        }
    }
}