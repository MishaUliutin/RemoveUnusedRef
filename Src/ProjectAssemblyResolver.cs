using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace RemoveUnusedRef
{
    public class ProjectAssemblyResolver : BaseAssemblyResolver
    {	
        #region Factory methods
        
        public static ProjectAssemblyResolver Create(ProjectInfo projectInfo)
        {
            if (projectInfo == null)
                throw new ArgumentNullException("projectInfo");
            var assemblyResolver = new ProjectAssemblyResolver(projectInfo);
            assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(projectInfo.OutputAssemblyPath));
            //assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            foreach (var path in GetLocations(projectInfo.References))
            {
                assemblyResolver.AddSearchDirectory(path);
            }
            return assemblyResolver;
        }
        
        private static IEnumerable<string> GetLocations(IEnumerable<ProjectReference> projectReferences)
        {
            var gac = Path.Combine(Environment.ExpandEnvironmentVariables("%systemroot%"), @"assembly\");
            return projectReferences.
                Select(reference => Path.GetDirectoryName(reference.Location).ToLower()).
                Distinct()
                .Where(location => !location.StartsWith(gac, StringComparison.InvariantCultureIgnoreCase)); 
        }
        
        #endregion
        
        private readonly ProjectInfo m_projectInfo;
        private readonly IDictionary<string, AssemblyDefinition> m_cache;
        
        private ProjectAssemblyResolver(ProjectInfo projectInfo)
        {
            m_projectInfo = projectInfo;
            m_cache = new Dictionary<string, AssemblyDefinition>();
        }
        
        private IEnumerable<ProjectReference> References
        {
            get
            {
                return m_projectInfo.References;
            }
        }
        
        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            if (name == null)
                throw new ArgumentNullException ("name");
            AssemblyDefinition assembly;
            if (m_cache.TryGetValue(name.FullName, out assembly))
                return assembly;
            var projectReference = References.SingleOrDefault(item => IsEquals(name, item));
            assembly = projectReference != null ? 
                CreateAssemblyDefinition(projectReference) 
                : base.Resolve(name);
            m_cache[name.FullName] = assembly;
            return assembly;
        }
        
        private AssemblyDefinition CreateAssemblyDefinition(ProjectReference projectReference)
        {
            var parameters = new ReaderParameters
            {
                ReadingMode = ReadingMode.Deferred,
                AssemblyResolver = this
            };
            return AssemblyDefinition.ReadAssembly(projectReference.Location, parameters);
        }
        
        private static bool IsEquals(AssemblyNameReference name, ProjectReference projectReference)
        {
            var result = 
                string.
                    Equals(name.Name, projectReference.Name, StringComparison.InvariantCultureIgnoreCase) &&
                string.
                    Equals(name.Culture, projectReference.Culture, StringComparison.InvariantCultureIgnoreCase);
            if (result)
            {
                var token = (name.PublicKeyToken == null || name.PublicKeyToken.Length == 0)
                    ? string.Empty
                    : Helper.PublicKeyTokenConvertFrom(name.PublicKeyToken);
                result = string.
                    Equals(token, projectReference.PublicKeyToken, StringComparison.InvariantCultureIgnoreCase);
            }
            return result;
        }
    }
}