namespace ToastifyAPI.GitHub
{
    public class RepoInfo
    {
        public string Name { get; }

        public string Owner { get; }

        public RepoInfo(string name, string owner)
        {
            this.Name = name;
            this.Owner = owner;
        }

        public string Format(string @string)
        {
            return @string.Replace(":owner", this.Owner).Replace("{owner}", this.Owner)
                          .Replace(":repo", this.Name).Replace("{repo}", this.Name);
        }

        protected bool Equals(RepoInfo other)
        {
            return string.Equals(this.Name, other.Name) &&
                   string.Equals(this.Owner, other.Owner);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == this.GetType() && this.Equals((RepoInfo)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Name != null ? this.Name.GetHashCode() : 0) * 397) ^
                       (this.Owner != null ? this.Owner.GetHashCode() : 0);
            }
        }
    }
}