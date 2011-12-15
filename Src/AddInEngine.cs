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
            m_shellProxy.SetStatusBarMessage("Removing unsed references...");
            m_shellProxy.OutputAppendLine("Removing unsed references started");
            if (projectInfo == null)
            {            
                m_shellProxy.SetStatusBarMessage("Can't remove unsed references");
                m_shellProxy.OutputAppendLine("projectInfo == null, oops...");
                return;
            }
            var unusedReferences = GetUnusedReferences(projectInfo);
            if (unusedReferences.Count() > 0)
            {
                using(var dialog = new SelectUnusedrefDialog(unusedReferences))
                {
                    if (dialog.ShowDialog(m_shellProxy.MainWin32Window) == DialogResult.OK)
                    {
                        m_shellProxy.RemoveProjectReferences(dialog.SelectedProjectReferences);
                        m_shellProxy.SetStatusBarMessage("Unsed references removed");
                        m_shellProxy.OutputAppendLine("Unsed references removed");
                    }
                    else
                    {
                        m_shellProxy.SetStatusBarMessage("Remove unsed references aborted");
                        m_shellProxy.OutputAppendLine("Remove unsed references aborted");
                    }
                }
            }
            else
            {
                m_shellProxy.SetStatusBarMessage("Unsed references removed");
                m_shellProxy.OutputAppendLine("Unsed references removed");
            }
        }
        
        private static IEnumerable<ProjectReference> GetUnusedReferences(ProjectInfo projectInfo)
        {
            var auditor = UnusedReferencesAuditFactory.Create(projectInfo);
            return auditor.Execute();
        }
    }
}