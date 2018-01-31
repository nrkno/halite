using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.IO;
using System.Reflection;
using TestHelper;
using Xunit;

namespace Halite.Analyzer.Test
{
    
    public class PropertySetterTests : CodeFixVerifier
    {
        [Fact]
        public void Property_DiagnosticsNotAppliedOnNormalClass()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class TypeName 
        {   
            public HalLink SomeLink { get; set; }
        }
    }";

            VerifyCSharpDiagnostic(test, new DiagnosticResult[0]);
        }

        [Fact]
        public void Property_PublicSetterNotAllowed()
        {
            var test = @"
    using Halite;
    namespace ConsoleApplication1
    {
        public class TypeName : HalLinks
        {   
            public static void Main(string[] args)
            {
            }

            public TypeName(SelfLink self) : base(self)
            { }

            public Halite.HalLink SomeLink { get; set; }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "HalLinksPropertyNoPublicSetter",
                Message = String.Format("Property SomeLink in TypeName, a subclass of HalLinks, must not have externally accessible setter."),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 14, 51)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void Property_PrivateSetterAllowed()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName : Halite.HalLinks
        {   
            public HalLink SomeLink { get; private set; }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "HalLinksPropertyDeclaration",
                Message = String.Format("Property SomeLink in TypeName, a subclass of HalLinks, must not have externally accessible setter."),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 44)
                        }
            };

            VerifyCSharpDiagnostic(test, new DiagnosticResult[0]);
        }

        [Fact]
        public void Property_InternalSetterNotAllowed()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName : Halite.HalLinks
        {   
            public HalLink SomeLink { get; internal set; }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "HalLinksPropertyNoPublicSetter",
                Message = String.Format("Property SomeLink in TypeName, a subclass of HalLinks, must not have externally accessible setter."),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 44)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void Property_ProtectedSetterNotAllowed()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName : Halite.HalLinks
        {   
            public HalLink SomeLink { get; protected set; }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "HalLinksPropertyNoPublicSetter",
                Message = String.Format("Property SomeLink in TypeName, a subclass of HalLinks, must not have externally accessible setter."),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 44)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void Property_PrivateSetterFix_Applied()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class TypeName : Halite.HalLinks
        {   
            public Halite.HalLink SomeLink { get; set; }
        }
    }";

            var fixedTest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class TypeName : Halite.HalLinks
        {   
            public Halite.HalLink SomeLink { get; private set; }
        }
    }";
            VerifyCSharpFix(test, fixedTest);
        }

        [Fact]
        public void Property_InternalSetterFix_Applied()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class TypeName : Halite.HalLinks
        {   
            public Halite.HalLink SomeLink { get; internal set; }
        }
    }";

            var fixedTest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class TypeName : Halite.HalLinks
        {   
            public Halite.HalLink SomeLink { get; private set; }
        }
    }";
            VerifyCSharpFix(test, fixedTest);
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider() => new HalLinksChangePropertySetterToPrivateCodeFixProvider();

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new HalLinksSubclassPropertySetterAnalyzer();

        protected override MetadataReference[] GetReferences()
        {
            var path = AssemblyDirectory;

            return new[] {
                MetadataReference.CreateFromFile(Path.Combine(path,"Halite.dll")) };
        }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
