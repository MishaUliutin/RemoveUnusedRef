using System;
using System.Text;

namespace RemoveUnusedRef
{
    public sealed class ProjectReference : IEquatable<ProjectReference>
    {
        private const string VersionField = "Version";
        private const string CultureField = "Culture";
        private const string PublicKeyTokenField = "PublicKeyToken";
        private const string NeutralCulture = "neutral";
        private const string NullPublicKeyToken = "null";
        
        #region IEquatable Members
        
        public bool Equals(ProjectReference other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(other.FullName, FullName, StringComparison.InvariantCultureIgnoreCase);
        }
        
        public override bool Equals(Object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return (obj is ProjectReference) ? Equals(obj as ProjectReference) : false;
        }
        
        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }
        
        #endregion
        
        public string Aliases { get; set; }
        
        public string Name { get; set; }
        
        public string Location { get; set; }
        
        public Version Version { get; set; }
        
        public string Culture { get; set; }
        
        public string PublicKeyToken { get; set; }
        
        public string FullName
        {
            get
            {
                var sb = new StringBuilder(Name).Append(", ");
                sb.Append(VersionField).
                    Append("=").
                    Append(Version == null ? string.Empty : Version.ToString()).
                    Append(", ");
                sb.Append(CultureField).
                    Append("=").
                    Append(string.IsNullOrEmpty(Culture) ? NeutralCulture : Culture).
                    Append(", ");
                sb.Append(PublicKeyTokenField).
                    Append("=").
                    Append(string.IsNullOrEmpty(PublicKeyToken) ? 
                           NullPublicKeyToken 
                           : PublicKeyToken.ToLower());
                return sb.ToString();
            }
        }
    }
}