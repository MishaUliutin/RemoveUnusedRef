using System;
using RemoveUnusedRef.ProjectsUnusedReferencesAudit;

namespace RemoveUnusedRef
{
    public class UnusedReferencesAuditFactory
    {
        private const string CSharpProjectTypeString = "fae04ec0-301f-11d3-bf4b-00c04f79efbc";
        private const string VBasicProjectTypeString = "f184b08f-c81c-45f6-a57f-5abd9991f28f";
        private const string CppCliProjectTypeString = "8bc9ceb8-8b4a-11d0-8d11-00a0c91bc942";
        
        public static UnusedReferencesAudit Create(ProjectInfo projectInfo)
        {
            if (projectInfo == null)
                throw new ArgumentNullException("projectInfo");
            switch (projectInfo.Type.ToString("D"))
            {
                case CSharpProjectTypeString:
                    return new CSharpProjectAudit(projectInfo);
                case VBasicProjectTypeString:
                    return new VBasicProjectAudit(projectInfo);
                case CppCliProjectTypeString:
                    return new CppCliProjectAudit(projectInfo);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}