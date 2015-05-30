using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SimplifyConditionalTernaryExpressionAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            NRefactoryDiagnosticIDs.SimplifyConditionalTernaryExpressionAnalyzerID,
            GettextCatalog.GetString("Conditional expression can be simplified"),
            GettextCatalog.GetString("Simplify conditional expression"),
            DiagnosticAnalyzerCategories.PracticesAndImprovements,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(NRefactoryDiagnosticIDs.SimplifyConditionalTernaryExpressionAnalyzerID)
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                (nodeContext) =>
                {
                    Diagnostic diagnostic;
                    if (TryGetDiagnostic(nodeContext, out diagnostic))
                        nodeContext.ReportDiagnostic(diagnostic);
                },
                SyntaxKind.ConditionalExpression
            );
        }

        static bool TryGetDiagnostic(SyntaxNodeAnalysisContext nodeContext, out Diagnostic diagnostic)
        {
            diagnostic = default(Diagnostic);
            if (nodeContext.IsFromGeneratedCode())
                return false;
            var node = nodeContext.Node as ConditionalExpressionSyntax;

            bool? trueBranch = SimplifyConditionalTernaryExpressionCodeFixProvider.GetBool(node.WhenTrue.SkipParens());
            bool? falseBranch = SimplifyConditionalTernaryExpressionCodeFixProvider.GetBool(node.WhenFalse.SkipParens());

            if (trueBranch == falseBranch ||
                trueBranch == true && falseBranch == false) // Handled by RedundantTernaryExpressionIssue
                return false;

            diagnostic = Diagnostic.Create(
                descriptor,
                node.GetLocation()
            );
            return true;
        }
    }
}