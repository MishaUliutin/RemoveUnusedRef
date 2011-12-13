using System;
using System.Collections.Generic;

namespace RemoveUnusedRef
{
    public class ProjectInfo
    {
        public string Name { get; set; }
        
        public Guid Type { get; set; }
        
        public string Configuration { get; set; }
        
        public string Platform { get; set; }
        
        public string OutputAssemblyPath { get; set; }
        
        public IEnumerable<ProjectReference> References { get; set; }
    }
}