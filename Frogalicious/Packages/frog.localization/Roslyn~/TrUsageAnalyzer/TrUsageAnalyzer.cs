using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace TrUsageAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TrUsageAnalyzer : DiagnosticAnalyzer
    {
        private static DiagnosticDescriptor Rule(int id, string title, string msg)
        {
            return new DiagnosticDescriptor(
                id: $"TR{id:D2}",
                title: title,
                messageFormat: msg,
                category: "Localization",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true
            );
        }

        private static readonly DiagnosticDescriptor LiteralArgRule = Rule(
            1, "Non-literal string id", "The id should be a string literal (required for analyzing all used string ids)"
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            LiteralArgRule
        );

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(
                GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics
            );
            context.EnableConcurrentExecution();

            context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
        }

        private static void AnalyzeInvocation(OperationAnalysisContext ctx)
        {
            var invocation = (IInvocationOperation)ctx.Operation;

            if (invocation.TargetMethod.MethodKind != MethodKind.Ordinary)
                return;

            if (!invocation.TargetMethod.IsStatic)
                return;

            if (invocation.Arguments.Length < 1 || invocation.Arguments.Length > 2)
                return;

            if (invocation.TargetMethod.ContainingType?.Name != "Tr")
                return;

            if (invocation.TargetMethod.Name != "Msg" && invocation.TargetMethod.Name != "Plu")
                return;

            var firstArg = invocation.Arguments[0];
            if (firstArg.Syntax is ArgumentSyntax argSyn && argSyn.Expression is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression))
                return;

            ctx.ReportDiagnostic(Diagnostic.Create(LiteralArgRule, firstArg.Syntax.GetLocation()));
        }
    }
}