using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NotPortedYet]
    public class RedundantBaseConstructorCallAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            NRefactoryDiagnosticIDs.RedundantBaseConstructorCallAnalyzerID,
            GettextCatalog.GetString("This is generated by the compiler and can be safely removed"),
            GettextCatalog.GetString("Redundant base constructor call"),
            DiagnosticAnalyzerCategories.RedundanciesInDeclarations,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(NRefactoryDiagnosticIDs.RedundantBaseConstructorCallAnalyzerID),
            customTags: DiagnosticCustomTags.Unnecessary
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            //context.RegisterSyntaxNodeAction(
            //	(nodeContext) => {
            //		Diagnostic diagnostic;
            //		if (TryGetDiagnostic (nodeContext, out diagnostic)) {
            //			nodeContext.ReportDiagnostic(diagnostic);
            //		}
            //	}, 
            //	new SyntaxKind[] { SyntaxKind.None }
            //);
        }

        static bool TryGetDiagnostic(SyntaxNodeAnalysisContext nodeContext, out Diagnostic diagnostic)
        {
            diagnostic = default(Diagnostic);
            if (nodeContext.IsFromGeneratedCode())
                return false;
            //var node = nodeContext.Node as ;
            //diagnostic = Diagnostic.Create (descriptor, node.GetLocation ());
            //return true;
            return false;
        }

        //		class GatherVisitor : GatherVisitorBase<RedundantBaseConstructorCallAnalyzer>
        //		{
        //			public GatherVisitor(SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, CancellationToken cancellationToken)
        //				: base (semanticModel, addDiagnostic, cancellationToken)
        //			{
        //			}

        ////			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
        ////			{
        ////				base.VisitConstructorDeclaration(constructorDeclaration);
        ////
        ////				if (constructorDeclaration.Initializer.ConstructorInitializerType != ConstructorInitializerType.Base)
        ////					return;
        ////				if (constructorDeclaration.Initializer.IsNull)
        ////					return;
        ////				if (constructorDeclaration.Initializer.Arguments.Count != 0)
        ////					return;
        ////				AddDiagnosticAnalyzer(new CodeIssue(constructorDeclaration.Initializer.StartLocation, constructorDeclaration.Initializer.EndLocation,
        ////				         ctx.TranslateString(""),
        ////				         ctx.TranslateString(""),
        ////				         script => {
        ////					var clone = (ConstructorDeclaration)constructorDeclaration.Clone();
        ////					script.Replace(clone.ColonToken, CSharpTokenNode.Null.Clone());
        ////					script.Replace(constructorDeclaration.Initializer, ConstructorInitializer.Null.Clone());
        ////					}) { IssueMarker = IssueMarker.GrayOut });
        ////			}
        ////
        ////			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
        ////			{
        ////				//ignore properties
        ////			}
        ////
        ////			public override void  VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
        ////			{
        ////				//ignore fields
        ////			}
        ////
        ////			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
        ////			{
        ////				//ignore method declarations
        ////			}
        //		}
    }


}