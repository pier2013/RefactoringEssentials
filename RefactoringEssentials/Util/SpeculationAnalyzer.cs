using System;
using System.Threading;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace RefactoringEssentials
{
    [RoslynReflectionUsage(RoslynReflectionAllowedContext.CodeFixes)]
#if NR6
    public
#endif
    class SpeculationAnalyzer
    {
        readonly static Type typeInfo;
        readonly static MethodInfo symbolsForOriginalAndReplacedNodesAreCompatibleMethod;
        readonly static MethodInfo replacementChangesSemanticsMethod;
        readonly object instance;

        static SpeculationAnalyzer()
        {
            Type[] abstractSpeculationAnalyzerGenericParams = new[] {
                Type.GetType ("Microsoft.CodeAnalysis.SyntaxNode" + ReflectionNamespaces.CAAsmName, true),
                Type.GetType ("Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax" + ReflectionNamespaces.CACSharpAsmName, true),
                Type.GetType ("Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax" + ReflectionNamespaces.CACSharpAsmName, true),
                Type.GetType ("Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax" + ReflectionNamespaces.CACSharpAsmName, true),
                Type.GetType ("Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax" + ReflectionNamespaces.CACSharpAsmName, true),
                Type.GetType ("Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax" + ReflectionNamespaces.CACSharpAsmName, true),
                Type.GetType ("Microsoft.CodeAnalysis.CSharp.Syntax.ThrowStatementSyntax" + ReflectionNamespaces.CACSharpAsmName, true),
                Type.GetType ("Microsoft.CodeAnalysis.SemanticModel" + ReflectionNamespaces.CAAsmName, true)
            };
            typeInfo = Type.GetType("Microsoft.CodeAnalysis.Shared.Utilities.AbstractSpeculationAnalyzer`8" + ReflectionNamespaces.WorkspacesAsmName, true)
                .MakeGenericType(abstractSpeculationAnalyzerGenericParams);

            symbolsForOriginalAndReplacedNodesAreCompatibleMethod = typeInfo.GetMethod("SymbolsForOriginalAndReplacedNodesAreCompatible", BindingFlags.Public | BindingFlags.Instance);
            replacementChangesSemanticsMethod = typeInfo.GetMethod("ReplacementChangesSemantics", BindingFlags.Public | BindingFlags.Instance);
            symbolInfosAreCompatibleMethod = typeInfo.GetMethod("SymbolInfosAreCompatible", BindingFlags.Public | BindingFlags.Static);

            typeInfo = Type.GetType("Microsoft.CodeAnalysis.CSharp.Utilities.SpeculationAnalyzer" + ReflectionNamespaces.CSWorkspacesAsmName, true);
            createSpeculativeSemanticModelForNodeMethod = typeInfo.GetMethod("CreateSpeculativeSemanticModelForNode", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(SyntaxNode), typeof(SyntaxNode), typeof(SemanticModel) }, null);
        }

        public SpeculationAnalyzer(ExpressionSyntax expression, ExpressionSyntax newExpression, SemanticModel semanticModel, CancellationToken cancellationToken, bool skipVerificationForReplacedNode = false, bool failOnOverloadResolutionFailuresInOriginalCode = false)
        {
            instance = Activator.CreateInstance(typeInfo, new object[] {
                expression,
                newExpression,
                semanticModel,
                cancellationToken,
                skipVerificationForReplacedNode,
                failOnOverloadResolutionFailuresInOriginalCode
            });
        }

        static MethodInfo symbolInfosAreCompatibleMethod;

        public static bool SymbolInfosAreCompatible(SymbolInfo originalSymbolInfo, SymbolInfo newSymbolInfo, bool performEquivalenceCheck, bool requireNonNullSymbols = false)
        {
            try
            {
                return (bool)symbolInfosAreCompatibleMethod.Invoke(null, new object[] { originalSymbolInfo, newSymbolInfo, performEquivalenceCheck, requireNonNullSymbols });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return false;
            }
        }

        public bool SymbolsForOriginalAndReplacedNodesAreCompatible()
        {
            try
            {
                return (bool)symbolsForOriginalAndReplacedNodesAreCompatibleMethod.Invoke(instance, new object[0]);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return false;
            }
        }

        public bool ReplacementChangesSemantics()
        {
            try
            {
                return (bool)replacementChangesSemanticsMethod.Invoke(instance, new object[0]);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return false;
            }
        }

        readonly static MethodInfo createSpeculativeSemanticModelForNodeMethod;

        public static SemanticModel CreateSpeculativeSemanticModelForNode(SyntaxNode originalNode, SyntaxNode nodeToSpeculate, SemanticModel semanticModel)
        {
            return (SemanticModel)createSpeculativeSemanticModelForNodeMethod.Invoke(null, new object[] { originalNode, nodeToSpeculate, semanticModel });
        }

        public static bool CanSpeculateOnNode(SyntaxNode node)
        {
            return (node is StatementSyntax && node.Kind() != SyntaxKind.Block) ||
                node is TypeSyntax ||
                node is CrefSyntax ||
                node.Kind() == SyntaxKind.Attribute ||
                node.Kind() == SyntaxKind.ThisConstructorInitializer ||
                node.Kind() == SyntaxKind.BaseConstructorInitializer ||
                node.Kind() == SyntaxKind.EqualsValueClause ||
                node.Kind() == SyntaxKind.ArrowExpressionClause;
        }

    }

}
