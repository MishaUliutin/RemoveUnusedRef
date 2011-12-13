using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace RemoveUnusedRef.Commands
{
    public class RemoveUnusedReferences : BuildProject
    {
        public override void AfterBuild()
        {
            if (LastBuildResults.Result == BuildResultCode.Success)
            {
                var project = ProjectToBuild as IProject;
                var engine = new AddInEngine(new ShellProxy(project));
                engine.Cleanup();
                //TaskService.BuildMessageViewCategory.AppendLine("Test");
                //WorkbenchSingleton.StatusBar.SetMessage("test");
            }
        }
        
        public override bool CanRunBuild
        {
            get
            {
                return base.CanRunBuild && (ProjectToBuild != null) && (ProjectToBuild is IProject);
            }
        }
    }
}