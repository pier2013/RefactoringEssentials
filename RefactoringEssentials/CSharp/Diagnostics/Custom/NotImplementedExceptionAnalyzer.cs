using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    /// <summary>
    /// This inspector just shows that there is a not implemented exception. It doesn't offer a fix.
    /// Should only be shown in overview bar, no underlining.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NotImplementedExceptionAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            NRefactoryDiagnosticIDs.NotImplementedExceptionAnalyzerID,
            GettextCatalog.GetString("Shows NotImplementedException throws in the quick task bar"),
            GettextCatalog.GetString("Not implemented"),
            DiagnosticAnalyzerCategories.Notifications,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(NRefactoryDiagnosticIDs.NotImplementedExceptionAnalyzerID)
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                (nodeContext) =>
                {
                    Diagnostic diagnostic;
                    if (TryGetDiagnostic(nodeContext, out diagnostic))
                    {
                        nodeContext.ReportDiagnostic(diagnostic);
                    }
                },
                new SyntaxKind[] { SyntaxKind.ThrowStatement }
            );
        }

        static bool TryGetDiagnostic(SyntaxNodeAnalysisContext nodeContext, out Diagnostic diagnostic)
        {
            diagnostic = default(Diagnostic);
            if (nodeContext.IsFromGeneratedCode())
                return false;
            var node = nodeContext.Node as ThrowStatementSyntax;
            if ((node == null) || (node.Expression == null))
                return false;
            var result = nodeContext.SemanticModel.GetTypeInfo(node.Expression).Type;
            if (result == null || result.Name != "NotImplementedException" || result.ContainingNamespace.ToDisplayString() != "System")
                return false;
            diagnostic = Diagnostic.Create(descriptor, node.Expression.GetLocation());
            return true;
        }
    }
}