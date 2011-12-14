using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RemoveUnusedRef.Gui;

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
            if (unusedReferences.Count() > 0)
            {
                using(var dialog = new SelectUnusedrefDialog(unusedReferences))
                {
                    if (dialog.ShowDialog(m_shellProxy.MainWin32Window) == DialogResult.OK)
                    {
                        foreach(var projectReference in dialog.SelectedProjectReferences)
                        {
                            m_shellProxy.RemoveProjectReference(projectReference);
                        }
                        m_shellProxy.SaveProject();
                    }
                }
            }
            else
            {
                //TODO: Add log
            }
        }
        
        private static IEnumerable<ProjectReference> GetUnusedReferences(ProjectInfo projectInfo)
        {
            var auditor = UnusedReferencesAuditFactory.Create(projectInfo);
            return auditor.Execute();
        }
    }
}