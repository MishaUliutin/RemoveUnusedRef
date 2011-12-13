using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using RemoveUnusedRef.UsedReferencesAuditEntries.Workers;

namespace RemoveUnusedRef.UsedReferencesAuditEntries
{
    public class TypesAttributesEntry : IUsedReferencesAuditEntry
    {
        private readonly InterfacesTypeWorker m_interfacesTypeWorker;
        private readonly ClassTypeHierarchyWorker m_classTypeHierarchyWorker;
        
        public TypesAttributesEntry()
        {
            m_interfacesTypeWorker = new InterfacesTypeWorker();
            m_classTypeHierarchyWorker = new ClassTypeHierarchyWorker();
        }
             
        public IEnumerable<ProjectReference> Execute(AuditEntryParameters parameters)
        {
            var references = new List<ProjectReference>();
            foreach (var typeDefinition in parameters.ProjectMetadata.TypesDefinitions) 
            {
                references.Union(GetAttributesReferences(typeDefinition, parameters));
            }
            return references;
        }
        
        private IEnumerable<ProjectReference> GetAttributesReferences(
            TypeDefinition typeDefinition, AuditEntryParameters parameters)
        {
            var funcs = 
                new List<Func<TypeDefinition, AuditEntryParameters, IEnumerable<ProjectReference>>>
            {
                GetTypeAttributesReferences,
                GetMethodsAttributesReferences,
                GetPropertiesAttributesReferences,
                GetFieldsAttributesReferences,
                GetEventsAttributesReferences
            };
            var projectReferences = new List<ProjectReference>();
            foreach (var func in funcs) 
            {
                foreach(var projectReference in func(typeDefinition, parameters))
                {
                    if (!projectReferences.Contains(projectReference))
                        projectReferences.Add(projectReference);
                }
            }
            return projectReferences;
        }
        
        private IEnumerable<ProjectReference> GetTypeAttributesReferences(
            TypeDefinition typeDefinition, AuditEntryParameters parameters)
        {
            return GetMemberDefinitionAttributesReferences(typeDefinition, parameters);
        }
        
        private IEnumerable<ProjectReference> GetMethodsAttributesReferences(
            TypeDefinition typeDefinition, AuditEntryParameters parameters)
        {
            var methods = 
                typeDefinition.Methods.Where(IsMethodContainsAttribute);
            foreach (var method in methods)
            {
                foreach(var projectReference in GetMemberDefinitionAttributesReferences(method, parameters))
                    yield return projectReference;
                foreach(var projectReference in 
                        GetMethodReturnAttributesReferences(method.MethodReturnType, parameters))
                    yield return projectReference;
            }
        }
        
        private IEnumerable<ProjectReference> GetPropertiesAttributesReferences(
            TypeDefinition typeDefinition, AuditEntryParameters parameters)
        {
            var properties = typeDefinition.Properties.Where(property => property.HasCustomAttributes);
            return GetMembersDefinitionAttributesReferences(properties, parameters);
        }
        
        private IEnumerable<ProjectReference> GetFieldsAttributesReferences(
            TypeDefinition typeDefinition, AuditEntryParameters parameters)
        {
            var fields = 
                typeDefinition.Fields.Where(field => field.HasCustomAttributes || field.HasMarshalInfo);
            foreach (var field in fields)
            {
                foreach(var projectReference in GetMemberDefinitionAttributesReferences(field, parameters))
                    yield return projectReference;
                var pr = GetMarshalInfoReference(field.MarshalInfo, parameters);
                if (pr != null)
                    yield return pr;
            }
        }

        private IEnumerable<ProjectReference> GetEventsAttributesReferences(
            TypeDefinition typeDefinition, AuditEntryParameters parameters)
        {
            var events = typeDefinition.Events.Where(evnt => evnt.HasCustomAttributes);
            return GetMembersDefinitionAttributesReferences(events, parameters);
        }
        
        private IEnumerable<ProjectReference> GetMemberDefinitionAttributesReferences(
            IMemberDefinition memberDefinition, AuditEntryParameters parameters)
        {
            return
                memberDefinition.CustomAttributes.SelectMany(ca => GetCustomAttributeReferences(ca, parameters));
        }
        
        private IEnumerable<ProjectReference> GetMembersDefinitionAttributesReferences(
            IEnumerable<IMemberDefinition> membersDefinition, AuditEntryParameters parameters)
        {
            foreach (var member in membersDefinition)
            {
                foreach(var projectReference in GetMemberDefinitionAttributesReferences(member, parameters))
                    yield return projectReference;
            }
        }
        
        private IEnumerable<ProjectReference> GetMethodReturnAttributesReferences(
            MethodReturnType methodReturnType, AuditEntryParameters parameters)
        {
            if (methodReturnType.HasCustomAttributes)
            {
                var references = methodReturnType.CustomAttributes.
                            SelectMany(ca => GetCustomAttributeReferences(ca, parameters));
                foreach(var projectReference in references)
                    yield return projectReference;
            }
            if (methodReturnType.HasMarshalInfo)
            {
                var projectReference = GetMarshalInfoReference(methodReturnType.MarshalInfo, parameters);
                if (projectReference != null)
                    yield return projectReference;
            }
            yield break;
        }
        
        private ProjectReference GetMarshalInfoReference(
            MarshalInfo marshalInfo, AuditEntryParameters parameters)
        {
            var customMarshalInfo = marshalInfo as CustomMarshalInfo;
            if (customMarshalInfo != null && customMarshalInfo.ManagedType != null)
            {
                var typeDefinition = customMarshalInfo.ManagedType.Resolve();
                if (typeDefinition != null)
                {
                    parameters.AddToCheckedTypes(typeDefinition);
                    return parameters.FindProjectReference(typeDefinition);
                }
            }
            return null;
        }
        
        private bool IsMethodContainsAttribute(MethodDefinition method)
        {
            var methodInfo = method.GetElementMethod();
            return method.HasCustomAttributes ||
                methodInfo.MethodReturnType.HasCustomAttributes ||
                methodInfo.MethodReturnType.HasFieldMarshal ||
                methodInfo.MethodReturnType.HasMarshalInfo;
        }
        
        private IEnumerable<ProjectReference> GetCustomAttributeReferences(
            CustomAttribute customAttribute, AuditEntryParameters parameters)
        {
            var typeDefinition = customAttribute.AttributeType.Resolve();
            if ((typeDefinition != null) && !parameters.IsTypeChecked(typeDefinition))
            {
                foreach(var projectReference in 
                        m_classTypeHierarchyWorker.Execute(typeDefinition, parameters))
                    yield return projectReference;
                foreach(var projectReference in m_interfacesTypeWorker.Execute(typeDefinition, parameters))
                    yield return projectReference;
                parameters.AddToCheckedTypes(typeDefinition);
            }
            var references = GetCustomAttributeArguments(customAttribute).
                SelectMany(caa => GetCustomAttributeArgumentReferences(caa, parameters));
            foreach(var projectReference in references)
                yield return projectReference;
        }
        
        private IEnumerable<CustomAttributeArgument> GetCustomAttributeArguments(
            CustomAttribute customAttribute)
        {
            var result = new List<CustomAttributeArgument>();
            if (customAttribute.HasConstructorArguments)
            {
                result.AddRange(customAttribute.ConstructorArguments);
            }
            if (customAttribute.HasProperties)
            {
                result.AddRange(customAttribute.Properties.Select(p => p.Argument));
            }
            if (customAttribute.HasFields)
            {
                result.AddRange(customAttribute.Fields.Select(f => f.Argument));
            }
            return result;
        }
        
        private IEnumerable<ProjectReference> GetCustomAttributeArgumentReferences(
            CustomAttributeArgument customAttributeArgument, AuditEntryParameters parameters)
        {
            TypeDefinition typeDefinition = customAttributeArgument.Value is TypeReference
                    ? (customAttributeArgument.Value as TypeReference).Resolve()
                    : customAttributeArgument.Type.Resolve();
            if (typeDefinition != null && !parameters.IsTypeChecked(typeDefinition))
            {
                ProjectReference projectReference = parameters.FindProjectReference(typeDefinition.Scope);
                if (projectReference != null)
                    yield return projectReference;
                foreach(var pr in 
                        m_interfacesTypeWorker.Execute(typeDefinition, parameters))
                    yield return pr;
                foreach(var pr in
                        m_classTypeHierarchyWorker.Execute(typeDefinition, parameters))
                    yield return pr;
                parameters.AddToCheckedTypes(typeDefinition);
            }
        }
    }
}