namespace GoingOn.Frontend.Areas.HelpPage.ModelDescriptions
{
    using System;
    using System.CodeDom.Compiler;
    using System.Reflection;

    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}