using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop;
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
        
        public void SaveProject()
        {
            m_project.Save();
        }
        
        public void RemoveProjectReference(ProjectReference projectReference)
        {
            //TODO: Change where predicate
            var projectItem = m_project.Items.
                Select(item => item).
                Where(item => (item is ReferenceProjectItem) && 
                      (item.FileName.Equals(projectReference.Location, StringComparison.InvariantCultureIgnoreCase))).
                Single();
            ProjectService.RemoveProjectItem(m_project, projectItem);
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