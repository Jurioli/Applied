namespace System
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ApplyPropertyNameAttribute : Attribute
    {
        public string Name { get; }

        public ApplyPropertyNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
