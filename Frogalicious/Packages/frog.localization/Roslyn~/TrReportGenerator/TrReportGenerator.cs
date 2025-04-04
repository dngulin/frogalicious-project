using System.Text;
using Microsoft.CodeAnalysis;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TrReportGenerator
{
    [Generator(LanguageNames.CSharp)]
    public class TrReportGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initCtx)
        {
            var query = initCtx.SyntaxProvider.CreateSyntaxProvider(
                predicate: (SyntaxNode node, CancellationToken _) =>
                {
                    if (!(node is InvocationExpressionSyntax invocationSyntax))
                        return false;

                    if (!(invocationSyntax.Expression is MemberAccessExpressionSyntax memberAccessSyntax))
                        return false;

                    if (!(memberAccessSyntax.Expression is IdentifierNameSyntax identNameSyntax))
                        return false;

                    if (identNameSyntax.Identifier.Text != "Tr")
                        return false;

                    var argsSyntax = invocationSyntax.ArgumentList.Arguments;
                    if (argsSyntax.Count < 1 || argsSyntax.Count > 2)
                        return false;

                    if (!(argsSyntax[0].Expression is LiteralExpressionSyntax firstArgSyntax))
                        return false;

                    if (!firstArgSyntax.Token.IsKind(SyntaxKind.StringLiteralToken))
                        return false;

                    switch (memberAccessSyntax.Name.Identifier.Text)
                    {
                        case "Msg": return argsSyntax.Count == 1;
                        case "Plu": return argsSyntax.Count == 2;
                        default: return false;
                    }

                },
                transform: (GeneratorSyntaxContext ctx, CancellationToken _) =>
                {
                    var invocationSyntax = (InvocationExpressionSyntax)ctx.Node;
                    var memberAccessSyntax = (MemberAccessExpressionSyntax)invocationSyntax.Expression;
                    var argsSyntax = invocationSyntax.ArgumentList.Arguments;
                    var firstArgSyntax = (LiteralExpressionSyntax)argsSyntax[0].Expression;

                    var span = ctx.Node.GetLocation().GetLineSpan();

                    var entry = new Entry
                    {
                        Path = SyntaxFactory.Literal(span.Path),
                        LineNumber = SyntaxFactory.Literal(span.StartLinePosition.Line + 1),
                        MsgId = firstArgSyntax.Token,
                        IsPlural = memberAccessSyntax.Name.Identifier.Text == "Plu"
                    };

                    return entry;
                }
            );

            var collected = query.Collect();

            initCtx.RegisterSourceOutput(collected, (ctx, entries) =>
            {
                if (entries.Length == 0)
                    return;

                var sb = new StringBuilder();

                sb.AppendLine("#if UNITY_EDITOR");
                sb.AppendLine("using Frog.Localization;");
                sb.AppendLine();
                sb.AppendLine("internal sealed class TrReport : AbstractTrReport");
                sb.AppendLine("{");
                sb.AppendLine("    public override TrReportEntry[] Entries { get; } =");
                sb.AppendLine("    {");
                foreach (var e in entries)
                {
                    var pluralExprKind = e.IsPlural
                        ? SyntaxKind.TrueLiteralExpression
                        : SyntaxKind.FalseLiteralExpression;
                    var isPlural = SyntaxFactory.LiteralExpression(pluralExprKind);
                    sb.AppendLine($"        new TrReportEntry({e.Path}, {e.LineNumber}, {e.MsgId}, {isPlural}),");
                }
                sb.AppendLine("    };");
                sb.AppendLine("}");
                sb.AppendLine("#endif");

                ctx.AddSource("TrReport.g.cs", sb.ToString());
            });
        }
    }

    internal struct Entry
    {
        public SyntaxToken Path;
        public SyntaxToken LineNumber;
        public SyntaxToken MsgId;
        public bool IsPlural;
    }
}