using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace RemoveUnusedRef.UsedReferencesAuditEntries.Workers
{
    public class MemberReferencesWorker
    {
        private readonly ClassTypeHierarchyWorker m_classTypeHierarchyWorker;
        private readonly InterfacesTypeWorker m_interfacesTypeWorker;
        
        public MemberReferencesWorker()
        {
            m_interfacesTypeWorker = new InterfacesTypeWorker();
            m_classTypeHierarchyWorker = new ClassTypeHierarchyWorker();
        }
        
        public IEnumerable<ProjectReference> Execute(
            TypeDefinition typeDefinition, AuditEntryParameters parameters)
        {
            foreach(var memberReference in GetTypeMemberReferences(typeDefinition, parameters))
            {
                foreach(var projectReference in GetMemberReferences(memberReference, parameters))
                    yield return projectReference;
            }
        }
        
        private IEnumerable<ProjectReference> GetMemberReferences(
            MemberReference memberReference, AuditEntryParameters parameters)
        {
            if (memberReference is MethodReference)
            {
                var methodReferences = GetOverloadedMethods(
                    memberReference.DeclaringType, (memberReference as MethodReference).Name);
                if (methodReferences.Count() > 1)
                    return GetOverloadedMethodReferences(methodReferences, parameters);
            }
            return GetMemberParametersReferences(memberReference, parameters);
        }
        
        private IEnumerable<MethodDefinition> GetOverloadedMethods(
            TypeReference typeReference, string methodName)
        {
            var typeDefinition = typeReference.Resolve();
            if (typeDefinition != null)
            {
                return 
                    from method in typeDefinition.Methods
                        where method.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase)
                    select method;
            }
            return new List<MethodDefinition>();
        }
        
        private IEnumerable<MemberReference> GetTypeMemberReferences(
            TypeReference typeReference, AuditEntryParameters parameters)
        {
            return parameters.ProjectMetadata.MemberReferences.Where(
                memberReference => memberReference.DeclaringType.FullName.Equals(
                    typeReference.Name, StringComparison.InvariantCultureIgnoreCase));
        }
        
        private IEnumerable<ProjectReference> GetMemberParametersReferences(
            MemberReference memberReference, AuditEntryParameters parameters)
        {
            if (memberReference is MethodReference)
            {
                return GetMethodParametersReferences(memberReference as MethodReference, parameters);
            }
            if (memberReference is PropertyReference)
            {
                return GetPropertyParametersReferences(memberReference as PropertyReference, parameters);
            }
            if (memberReference is FieldReference)
            {
                return GetFieldTypeReferences(memberReference as FieldReference, parameters);
            }
            if (memberReference is EventReference)
            {
                return GetEventTypeReferences(memberReference as EventReference, parameters);
            }
            throw new NotImplementedException();
        }

        private IEnumerable<ProjectReference> GetMethodParametersReferences(
            MethodReference methodReference, AuditEntryParameters parameters)
        {
            foreach(var projectReference in GetTypeReferences(methodReference.ReturnType, parameters))
                yield return projectReference;
            foreach (var paramDefinition in methodReference.Parameters)
            {
                foreach(var projectReference in 
                        GetTypeReferences(paramDefinition.ParameterType, parameters))
                    yield return projectReference;
            }
        }
        
        private IEnumerable<ProjectReference> GetOverloadedMethodReferences(
            IEnumerable<MethodReference> methodReferences, AuditEntryParameters parameters)
        {
            foreach(var methodReference in methodReferences)
            {
                foreach(var projectReference in GetMethodParametersReferences(methodReference, parameters))
                    yield return projectReference;
            }
        }
        
        private IEnumerable<ProjectReference> GetPropertyParametersReferences(
            PropertyReference propertyReference, AuditEntryParameters parameters)
        {
            foreach(var projectReference in GetTypeReferences(propertyReference.PropertyType, parameters))
                yield return projectReference;
            foreach (var paramDefinition in propertyReference.Parameters)
            {
                foreach(var projectReference in 
                        GetTypeReferences(paramDefinition.ParameterType, parameters))
                    yield return projectReference;
            }
        }
        
        private IEnumerable<ProjectReference> GetFieldTypeReferences(
            FieldReference fieldReference, AuditEntryParameters parameters)
        {
            foreach(var projectReference in GetTypeReferences(fieldReference.FieldType, parameters))
                yield return projectReference;
        }
        
        private IEnumerable<ProjectReference> GetEventTypeReferences(
            EventReference eventReference, AuditEntryParameters parameters)
        {
            foreach(var projectReference in GetTypeReferences(eventReference.EventType, parameters))
                yield return projectReference;
        }
        
        private IEnumerable<ProjectReference> GetTypeReferences(
            TypeReference typeReference, AuditEntryParameters parameters)
        {
            if ((typeReference == null) || (parameters.IsTypeChecked(typeReference)))
                yield break;
            var typeDefinition = typeReference.Resolve();
            if (typeDefinition != null)
            {
                var pr = parameters.FindProjectReference(typeDefinition.Scope);
                if (pr != null)
                    yield return pr;
                foreach(var projectReference in 
                        m_interfacesTypeWorker.Execute(typeDefinition, parameters))
                    yield return projectReference;
                foreach(var projectReference in
                        m_classTypeHierarchyWorker.Execute(typeDefinition, parameters))
                    yield return projectReference;
            }
            else
            {
                var pr = parameters.FindProjectReference(typeReference.Scope);
                if (pr != null)
                    yield return pr;
            }
            parameters.AddToCheckedTypes(typeReference);
        }
    }
}