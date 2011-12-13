using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using RemoveUnusedRef.Extensions;

namespace RemoveUnusedRef
{ 
    public interface IUsedReferencesAuditEntry
    {
        IEnumerable<ProjectReference> Execute(AuditEntryParameters parameters);
    }

    public class AuditEntryParameters
    {
        public IEnumerable<ProjectReference> References { get; set; }
        public ProjectMetadata ProjectMetadata { get; set; }
        public IList<string> CheckedTypes { get; set; }
        
        public ProjectReference FindProjectReference(IMetadataScope scope)
        {
            if (scope == null)
                return null;
            string assemblyFullName = scope.AssemblyFullName();
            return References.FirstOrDefault(
                item => string.
                    Equals(item.FullName, assemblyFullName, StringComparison.InvariantCultureIgnoreCase));
        }
        
        public ProjectReference FindProjectReference(TypeDefinition typeDefinition)
        {
            if (typeDefinition == null)
                return null;
            return FindProjectReference(typeDefinition.Scope);
        }
        
        public void AddToCheckedTypes(string fullNameWithAssembly)
        {
            if (!string.IsNullOrWhiteSpace(fullNameWithAssembly) && 
                !IsTypeChecked(fullNameWithAssembly))
                CheckedTypes.Add(fullNameWithAssembly);
        }
        
        public void AddToCheckedTypes(TypeDefinition typeDefinition)
        {
            if (typeDefinition != null)
                AddToCheckedTypes(typeDefinition.FullNameWithAssembly());
        }
        
        public void AddToCheckedTypes(TypeReference typeReference)
        {
            if (typeReference != null)
                AddToCheckedTypes(typeReference.FullNameWithAssembly());
        }
        
        public bool IsTypeChecked(string fullNameWithAssembly)
        {
            return CheckedTypes.Contains(fullNameWithAssembly);
        }
        
        public bool IsTypeChecked(TypeDefinition typeDefinition)
        {
            return (typeDefinition != null) && 
                IsTypeChecked(typeDefinition.FullNameWithAssembly());
        }
        
        public bool IsTypeChecked(TypeReference typeReference)
        {
            return (typeReference != null) && 
                IsTypeChecked(typeReference.FullNameWithAssembly());
        }
    }
}