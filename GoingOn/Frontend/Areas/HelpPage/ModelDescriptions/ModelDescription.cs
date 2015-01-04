using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace GoingOn.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// Describes a type model.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public abstract class ModelDescription
    {
        public string Documentation { get; set; }

        public Type ModelType { get; set; }

        public string Name { get; set; }
    }
}