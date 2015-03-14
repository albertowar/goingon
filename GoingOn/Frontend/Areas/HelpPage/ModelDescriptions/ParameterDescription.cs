namespace GoingOn.Frontend.Areas.HelpPage.ModelDescriptions
{
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public class ParameterDescription
    {
        public ParameterDescription()
        {
            this.Annotations = new Collection<ParameterAnnotation>();
        }

        public Collection<ParameterAnnotation> Annotations { get; private set; }

        public string Documentation { get; set; }

        public string Name { get; set; }

        public ModelDescription TypeDescription { get; set; }
    }
}