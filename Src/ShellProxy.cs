using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}