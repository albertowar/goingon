namespace GoingOn.FrontendWebRole.Areas.HelpPage.SampleGeneration
{
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This represents an invalid sample on the help page. There's a display template named InvalidSample associated with this class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public class InvalidSample
    {
        public InvalidSample(string errorMessage)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException("errorMessage");
            }
            this.ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; private set; }

        public override bool Equals(object obj)
        {
            InvalidSample other = obj as InvalidSample;
            return other != null && this.ErrorMessage == other.ErrorMessage;
        }

        public override int GetHashCode()
        {
            return this.ErrorMessage.GetHashCode();
        }

        public override string ToString()
        {
            return this.ErrorMessage;
        }
    }
}