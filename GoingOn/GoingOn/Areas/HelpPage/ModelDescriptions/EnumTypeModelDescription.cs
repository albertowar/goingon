using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace GoingOn.Areas.HelpPage.ModelDescriptions
{
    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public class EnumTypeModelDescription : ModelDescription
    {
        public EnumTypeModelDescription()
        {
            Values = new Collection<EnumValueDescription>();
        }

        public Collection<EnumValueDescription> Values { get; private set; }
    }
}