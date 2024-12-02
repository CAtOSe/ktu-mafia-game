using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mafia.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SampleSyntaxAnalyzer : DiagnosticAnalyzer
{
    public const string CompanyName = "KTU";
    public const string DiagnosticId = "MG0001";

    private static readonly string Title = "Observer classes must implement observer interfaces";

    private static readonly string MessageFormat = "Class '{0}' ends with 'Observer' but does not implement an interface with 'Observer' in its name";
    private static readonly string Description = "Type names should contain 'Observer'.";

    private const string Category = "Design";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
    }

    /// <summary>
    /// Executed for each Syntax Node with 'SyntaxKind' is 'ClassDeclaration'.
    /// </summary>
    /// <param name="context">Operation context.</param>
    private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclarationNode) return;
        
        var className = classDeclarationNode.Identifier.Text;
        if (!className.EndsWith("Observer")) return;
        
        var semanticModel = context.SemanticModel;
        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationNode);

        if (classSymbol == null)
            return;

        var implementedInterfaces = classSymbol.Interfaces;
        var implementsObserverInterface = implementedInterfaces.Any(i => i.Name.EndsWith("Observer"));

        if (implementsObserverInterface) return;
        
        var diagnostic = Diagnostic.Create(Rule, classDeclarationNode.Identifier.GetLocation(), className);
        context.ReportDiagnostic(diagnostic);
    }
}