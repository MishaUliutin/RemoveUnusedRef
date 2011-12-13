using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace RemoveUnusedRef
{
    public class ProjectMetadata
    {
        private IAssemblyResolver m_assemblyResolver;
        private readonly ProjectInfo m_projectInfo;
        
        public ProjectMetadata(ProjectInfo projectInfo)
        {
            if (projectInfo == null)
                throw new ArgumentNullException("projectInfo");
            m_projectInfo = projectInfo;
            Init();
        }
        
        public AssemblyDefinition Assembly { get; private set; }
        public IEnumerable<TypeDefinition> ImportedTypesDefinitions
        {
            get
            {
                return
                    from type in TypesDefinitions
                    where (type.BaseType == null) && type.IsImport
                    select type;
            }
        }
        public IEnumerable<AssemblyDefinition> ManifestAssemblies { get; private set; }
        public IEnumerable<MemberReference> MemberReferences
        {
            get
            {
                return Assembly.Modules.SelectMany(module => module.GetMemberReferences());
            }
        }
        public IEnumerable<TypeDefinition> TypesDefinitions { get; private set; }
        public IEnumerable<TypeDefinition> TypesReferences { get; private set; }
        
        private void Init()
        {
            m_assemblyResolver = ProjectAssemblyResolver.Create(m_projectInfo);
            var parameters = new ReaderParameters
            {
                ReadingMode = ReadingMode.Deferred,
                AssemblyResolver = m_assemblyResolver
            };
            Assembly = AssemblyDefinition.ReadAssembly(m_projectInfo.OutputAssemblyPath, parameters);
            ManifestAssemblies = GetManifestAssemblies();
            TypesDefinitions = GetTypesDefinitions();
            TypesReferences = GetTypesReferences();
        }
        
        private IEnumerable<TypeDefinition> GetTypesReferences()
        {
            var typeReferences = Assembly.Modules.SelectMany(module => module.GetTypeReferences());
            foreach (var typeReference in typeReferences)
            {
                var typeDefinition = typeReference.Resolve();
                if (typeDefinition != null)
                    yield return typeDefinition;
            }
        }
        
        private IEnumerable<AssemblyDefinition> GetManifestAssemblies()
        {
            return
                from module in Assembly.Modules
                    from assemblyReference in module.AssemblyReferences
                    select m_assemblyResolver.Resolve(assemblyReference);
        }
        
        private IEnumerable<TypeDefinition> GetTypesDefinitions()
        {
            return
                from type in Helper.GetTypesDefinitions(Assembly.Modules)
                select type;
        }
    }
}