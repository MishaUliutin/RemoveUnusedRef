using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace RemoveUnusedRef.Extensions
{
    internal static class CecilTypesExtension
    {
        private sealed class TypeReferenceComparer : IEqualityComparer<TypeReference>
        {
            public bool Equals(TypeReference typeReference1, TypeReference typeReference2)
            {
                if (ReferenceEquals(typeReference1, typeReference2))
                    return true;
                if ((typeReference1 != null && typeReference2 == null) || 
                    (typeReference1 == null && typeReference2 != null))
                    return false; 
                return typeReference1.FullNameWithAssembly().Equals(
                    typeReference2.FullNameWithAssembly(), StringComparison.InvariantCultureIgnoreCase);
            }
            
            public int GetHashCode(TypeReference typeReference)
            {
                return (typeReference.FullNameWithAssembly().GetHashCode());
            }
        }
        
        public static string AssemblyFullName(this IMetadataScope scope)
        {
            var assemblyName = Helper.GetAssemblyNameReference(scope);
            return (assemblyName != null) ? assemblyName.FullName : scope.Name;
        }
        
        public static string FullNameWithAssembly(this TypeDefinition typeDefinition)
        {
            return 
                string.Format("{0}, {1}", typeDefinition.FullName, typeDefinition.Scope.AssemblyFullName());
        }
        
        public static string FullNameWithAssembly(this TypeReference typeReference)
        {
            return string.Format("{0}, {1}", typeReference.FullName, typeReference.Scope.AssemblyFullName());
        }        
        
        public static IEnumerable<TypeReference> GetInterfacesHierarchy(this TypeDefinition typeDefinition)
        {
            return GetTypeInterfacesHierarchy(typeDefinition).Distinct(new TypeReferenceComparer());
        }
        
        private static IEnumerable<TypeReference> GetTypeInterfacesHierarchy(TypeDefinition typeDefinition)
        {
            if (typeDefinition == null)
                yield break;
            foreach (var intf in typeDefinition.Interfaces)
            {
                yield return intf;
            }
            if (typeDefinition.BaseType != null)
            {
                var baseTypeDefinition = typeDefinition.BaseType.Resolve();
                foreach (var intf in GetTypeInterfacesHierarchy(baseTypeDefinition))
                    yield return intf;
            }
        }
        
        /*
        public static TypeDefinition ResolveWithForwardedType(
            this TypeReference typeReference, out IMetadataScope forwardedFrom)
        {
            forwardedFrom = null;
            var typeDefinition = typeReference.Resolve();
            if (typeDefinition != null)
                return typeDefinition;
            var assemblyResolver = typeReference.Module.AssemblyResolver;
            if (assemblyResolver == null)
                return null;
            var typeAssembly = assemblyResolver.Resolve(Helper.GetAssemblyNameReference(typeReference.Scope));
            if (typeAssembly != null)
            {
                var forwardedType = typeAssembly.Modules.SelectMany(module => module.ExportedTypes)
                    .Where(type => type.IsForwarder && typeReference.Name.Equals(type.Name, StringComparison.InvariantCultureIgnoreCase))
                    .SingleOrDefault();
                if (forwardedType != null)
                {
                    forwardedFrom = forwardedType.Scope;   
                    var forwardedFromAssembly = assemblyResolver.Resolve(Helper.GetAssemblyNameReference(forwardedType.Scope));
                    var type = forwardedFromAssembly.Modules.SelectMany(module => module.Types)
                        .Where(t => t.FullName.Equals(forwardedType.FullName, StringComparison.InvariantCultureIgnoreCase))
                        .SingleOrDefault();
                    return type ?? 
                        forwardedFromAssembly.Modules.SelectMany(module => module.Types).SelectMany(t => t.NestedTypes)
                        .Where(t => t.FullName.Equals(typeReference.FullName, StringComparison.InvariantCultureIgnoreCase))
                        .SingleOrDefault();
                }
            }
            return null;
        }*/
    }
}