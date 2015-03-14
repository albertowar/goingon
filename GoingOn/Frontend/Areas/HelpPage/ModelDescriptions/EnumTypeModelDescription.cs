namespace GoingOn.Frontend.Areas.HelpPage.ModelDescriptions
{
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public class EnumTypeModelDescription : ModelDescription
    {
        public EnumTypeModelDescription()
        {
            this.Values = new Collection<EnumValueDescription>();
        }

        public Collection<EnumValueDescription> Values { get; private set; }
    }
}