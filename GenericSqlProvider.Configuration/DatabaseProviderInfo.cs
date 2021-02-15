using System;

namespace GenericSqlProvider.Configuration
{
    public class DatabaseProviderInfo : IEquatable<DatabaseProviderInfo>
    {
        public string InvariantName { get; set; }
        public string DisplayName { get; set; }

        #region IEquatable Implementation
        // Adapted from https://msdn.microsoft.com/en-us/library/ms131190%28v=vs.110%29.aspx

        public bool Equals(DatabaseProviderInfo other)
        {
            if (other == null)
                return false;

            if (this.InvariantName != other.InvariantName)
                return false;
            else
                return true;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            DatabaseProviderInfo castedObj = obj as DatabaseProviderInfo;
            if (castedObj == null)
                return false;
            else
                return Equals(castedObj);
        }

        public override int GetHashCode()
        {
            return this.InvariantName.GetHashCode();
        }

        public static bool operator ==(DatabaseProviderInfo first, DatabaseProviderInfo second)
        {
            if (((object)first) == null || ((object)second) == null)
                return Object.Equals(first, second);

            return first.Equals(second);
        }

        public static bool operator !=(DatabaseProviderInfo first, DatabaseProviderInfo second)
        {
            if (((object)first) == null || ((object)second) == null)
                return !Object.Equals(first, second);

            return !(first.Equals(second));
        }
    }

    #endregion
}
