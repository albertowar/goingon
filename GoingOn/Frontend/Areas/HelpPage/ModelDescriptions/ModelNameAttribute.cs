namespace GoingOn.Frontend.Areas.HelpPage.ModelDescriptions
{
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Use this attribute to change the name of the <see cref="ModelDescription"/> generated for a type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public sealed class ModelNameAttribute : Attribute
    {
        public ModelNameAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
    }
}