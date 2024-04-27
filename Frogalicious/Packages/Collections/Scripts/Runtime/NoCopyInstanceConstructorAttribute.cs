using System;

namespace Frog.Collections
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NoCopyInstanceConstructorAttribute : Attribute
    {

    }
}