using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Navigation;
using Mono.Cecil;
using RemoveUnusedRef.Extensions;

namespace RemoveUnusedRef
{
    internal sealed class Helper
    {
        public static string PublicKeyTokenConvertFrom(byte[] value)
        {
            var publicKeyToken = new StringBuilder();
            Array.ForEach(value, e => publicKeyToken.Append(e.ToString("x2")));
            return publicKeyToken.ToString();
        }
        
        public static AssemblyNameReference GetAssemblyNameReference(IMetadataScope scope)
        {
            switch (scope.MetadataScopeType) 
            {
                case MetadataScopeType.AssemblyNameReference:
                    return (AssemblyNameReference)scope;
                case MetadataScopeType.ModuleDefinition:
                    return ((ModuleDefinition)scope).Assembly.Name;
                default:
                    return null;
            }
        }
        
        public static IEnumerable<TypeDefinition> GetTypesDefinitions(IEnumerable<ModuleDefinition> modules)
        {
            return modules.SelectMany(module => GetTypesDefinitions(module));
        }
        
        private static IEnumerable<TypeDefinition> GetTypesDefinitions(ModuleDefinition module)
        {
            return module.Types.Union(module.Types.SelectMany(type => GetNestedTypes(type)));
        }
        
        private static IEnumerable<TypeDefinition> GetNestedTypes(TypeDefinition type)
        {
            if (type == null || !type.HasNestedTypes)
                yield break;
            foreach (var nestedType in type.NestedTypes)
            {
                yield return nestedType;
                foreach (var innerNestedType in GetNestedTypes(nestedType))
                {
                    yield return innerNestedType;
                }
            }
        }
        
    }
}