using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mafia.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ObserverInterfaceSyntaxAnalyzer : DiagnosticAnalyzer
{
    public const string CompanyName = "KTU";
    public const string DiagnosticId = "MG0002";

    private static readonly string Title = "Observer classes must contain observer method";

    private static readonly string MessageFormat = "Interface '{0}' should have at least 1 public observable method";
    private static readonly string Description = "Type names should have at least 1 public observable method.";

    private const string Category = "Design";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInterfaceSyntax, SyntaxKind.InterfaceDeclaration);
    }
    
    private void AnalyzeInterfaceSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InterfaceDeclarationSyntax declarationNode) return;
        
        var interfaceName = declarationNode.Identifier.Text;
        if (!interfaceName.EndsWith("Observer")) return;
        
        var hasPublicMethod = false;

        foreach (var member in declarationNode.Members)
        {
            if (member is MethodDeclarationSyntax method &&
                method.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                hasPublicMethod = true;
                break;
            }
        }
        
        if (!hasPublicMethod)
        {
            var diagnostic = Diagnostic.Create(Rule, declarationNode.GetLocation(), interfaceName);
            context.ReportDiagnostic(diagnostic);
        }
    }
}