using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace RemoveUnusedRef
{
    public class ShellProxy : IShellProxy
    {
        private readonly IProject m_project;
        
        public ShellProxy(IProject project)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            m_project = project;
        }
        
        public ProjectInfo GetProjectInfo()
        {
            var referenceProjectItems =
                from item in m_project.Items
                where item is ReferenceProjectItem
                select item as ReferenceProjectItem;
            var references =
                new List<ProjectReference>(referenceProjectItems.Count());
            referenceProjectItems.
                ForEach(item => references.Add(
                    new ProjectReference
                    {
                        Aliases = item.Aliases,
                        Name = item.Name,
                        Location = item.FileName,
                        Version = item.Version,
                        Culture = item.Culture,
                        PublicKeyToken = item.PublicKeyToken
                    }));
            var projectInfo = new ProjectInfo
            {
                Name = m_project.Name,
                Configuration = m_project.ActiveConfiguration,
                Platform = m_project.ActivePlatform,
                OutputAssemblyPath = m_project.OutputAssemblyFullPath,
                Type = Guid.Parse(m_project.TypeGuid),
                References = references
            };
            return projectInfo;
        }
        
        private bool IsEqualReference(
            ProjectReference projectReference, ReferenceProjectItem referenceProjectItem)
        {
            if ((projectReference == null) || (referenceProjectItem == null))
                return false;
            string projectReferenceVersion = projectReference.Version == null ? 
                null 
                : projectReference.Version.ToString();
            string referenceProjectItemVersion = referenceProjectItem.Version == null ? 
                null 
                : referenceProjectItem.Version.ToString();
            return 
                projectReference.Name.Equals(
                    referenceProjectItem.ShortName, StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(
                    projectReference.Culture, referenceProjectItem.Culture, 
                    StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(
                    projectReference.PublicKeyToken, referenceProjectItem.PublicKeyToken,
                    StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(
                    projectReferenceVersion, referenceProjectItemVersion,
                    StringComparison.InvariantCultureIgnoreCase);
        }
        
        public void RemoveProjectReferences(IEnumerable<ProjectReference> projectReferences)
        {
            var projectItems = m_project.Items.
                Where(item => item is ReferenceProjectItem).
                Select(item => item as ReferenceProjectItem).
                Where(item => projectReferences.FirstOrDefault(
                    projectReference => IsEqualReference(projectReference, item)) != null);
            ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.SafeThreadCall(
                () =>
                {
                    foreach(var projectItem in projectItems)
                    {
                        ProjectService.RemoveProjectItem(m_project, projectItem);
                    }
                    if (projectItems.Count() > 0)
                    {
                        m_project.Save();
                        ProjectBrowserPad.RefreshViewAsync();
                    }
                });
        }
        
        public void OutputAppendLine(string line)
        {
            TaskService.BuildMessageViewCategory.AppendLine(line);
        }
        
        public void SetStatusBarMessage(string message)
        {
            ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.StatusBar.SetMessage(message);
        }
        
        public IWin32Window MainWin32Window
        {
            get
            {
                return ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window;
            }
        }
    }
}