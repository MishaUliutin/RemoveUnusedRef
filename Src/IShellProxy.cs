using System;
using System.Windows.Forms;

namespace RemoveUnusedRef
{
    public interface IShellProxy
    {
        ProjectInfo GetProjectInfo();
        IWin32Window MainWin32Window { get; }
        void SaveProject();
        void RemoveProjectReference(ProjectReference projectReference);
    }
}