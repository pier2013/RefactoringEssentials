using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantEmptyDefaultSwitchBranchAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            NRefactoryDiagnosticIDs.RedundantEmptyDefaultSwitchBranchAnalyzerID,
            GettextCatalog.GetString("Redundant empty 'default' switch branch"),
            GettextCatalog.GetString("Redundant empty 'default' switch branch"),
            DiagnosticAnalyzerCategories.RedundanciesInCode,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(NRefactoryDiagnosticIDs.RedundantEmptyDefaultSwitchBranchAnalyzerID),
            customTags: DiagnosticCustomTags.Unnecessary
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                nodeContext =>
                {
                    Diagnostic diagnostic;
                    if (TryGetDiagnostic(nodeContext, out diagnostic))
                    {
                        nodeContext.ReportDiagnostic(diagnostic);
                    }
                },
                new SyntaxKind[] { SyntaxKind.SwitchStatement }
            );
        }

        static bool TryGetDiagnostic(SyntaxNodeAnalysisContext nodeContext, out Diagnostic diagnostic)
        {
            diagnostic = default(Diagnostic);
            if (nodeContext.IsFromGeneratedCode())
                return false;

            var node = nodeContext.Node as SwitchStatementSyntax;
            var defaultCase = node.Sections.FirstOrDefault(s => s.Labels.Any(l => l.IsKind(SyntaxKind.DefaultSwitchLabel)));
            if (defaultCase == null || defaultCase.Statements.Any(s => !s.IsKind(SyntaxKind.BreakStatement)))
                return false;
            var switchLabelSyntax = defaultCase.Labels.Last() as DefaultSwitchLabelSyntax;
            diagnostic = Diagnostic.Create(descriptor, switchLabelSyntax.Keyword.GetLocation());
            return true;
        }
    }
}
