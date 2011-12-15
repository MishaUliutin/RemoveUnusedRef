using System;
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