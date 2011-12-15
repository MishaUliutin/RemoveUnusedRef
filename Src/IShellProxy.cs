using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RemoveUnusedRef
{
    public interface IShellProxy
    {
        ProjectInfo GetProjectInfo();
        void RemoveProjectReferences(IEnumerable<ProjectReference> projectReferences);
        void OutputAppendLine(string line);
        void SetStatusBarMessage(string message);
        IWin32Window MainWin32Window { get; }
    }
}