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
            m_shellProxy.SetStatusBarMessage(
                "${res:RemoveUnusedRef.StatusBarMessage.RemovingUnusedReferences}");
            m_shellProxy.OutputAppendLine("${res:RemoveUnusedRef.Output.RemovingUnsedRefStarted}");
            if (projectInfo == null)
            {            
                m_shellProxy.SetStatusBarMessage(
                    "${res:RemoveUnusedRef.StatusBarMessage.CantRemoveUnsedRef}");
                m_shellProxy.OutputAppendLine("${res:RemoveUnusedRef.Output.Oops}");
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
                        m_shellProxy.SetStatusBarMessage(
                            "${res:RemoveUnusedRef.StatusBarMessage.UnsedReferencesRemoved}");
                        m_shellProxy.OutputAppendLine("${res:RemoveUnusedRef.Output.UnsedRefRemoved}");
                    }
                    else
                    {
                        m_shellProxy.SetStatusBarMessage(
                            "${res:RemoveUnusedRef.StatusBarMessage.RemoveUnsedRefAborted}");
                        m_shellProxy.OutputAppendLine("${res:RemoveUnusedRef.Output.RemoveUnsedRefAborted}");
                    }
                }
            }
            else
            {
                m_shellProxy.SetStatusBarMessage(
                    "${res:RemoveUnusedRef.StatusBarMessage.UnsedReferencesRemoved}");
                m_shellProxy.OutputAppendLine("${res:RemoveUnusedRef.Output.UnsedRefRemoved}");
            }
        }
        
        private static IEnumerable<ProjectReference> GetUnusedReferences(ProjectInfo projectInfo)
        {
            var auditor = UnusedReferencesAuditFactory.Create(projectInfo);
            return auditor.Execute();
        }
    }
}