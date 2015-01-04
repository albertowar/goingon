using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace GoingOn.Areas.HelpPage.ModelDescriptions
{
    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public class KeyValuePairModelDescription : ModelDescription
    {
        public ModelDescription KeyModelDescription { get; set; }

        public ModelDescription ValueModelDescription { get; set; }
    }
}