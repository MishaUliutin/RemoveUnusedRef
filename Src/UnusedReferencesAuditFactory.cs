using System;
using RemoveUnusedRef.ProjectsUnusedReferencesAudit;

namespace RemoveUnusedRef
{
    public class UnusedReferencesAuditFactory
    {
        private const string CSharpProjectTypeString = "fae04ec0-301f-11d3-bf4b-00c04f79efbc";
        
        public static UnusedReferencesAudit Create(ProjectInfo projectInfo)
        {
            if (projectInfo == null)
                throw new ArgumentNullException("projectInfo");
            switch (projectInfo.Type.ToString("D"))
            {
                case CSharpProjectTypeString:
                    return new CSharpProjectAudit(projectInfo);
                default:
                    throw new NotImplementedException();	
            }
        }
    }
}