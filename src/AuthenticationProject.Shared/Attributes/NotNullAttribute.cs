namespace AuthenticationProject.Shared.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class NotNullAttribute : Attribute
    {
    }
}