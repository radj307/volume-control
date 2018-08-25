using System;

namespace Toastify.DI
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyDependencyAttribute : Attribute
    {
    }
}