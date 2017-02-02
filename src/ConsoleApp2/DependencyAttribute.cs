using System;

namespace ConsoleApp2
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DependencyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyAttribute"/> class.
        /// </summary>
        /// <param name="baseInterface">The base interface.</param>
        public DependencyAttribute(Type baseInterface)
        {
            if(baseInterface is IOperation)
            {

            }
        }
    }
}
