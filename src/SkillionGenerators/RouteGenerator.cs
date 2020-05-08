using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SkillionGenerators
{
    [Generator]
    public class RouteGenerator : ISourceGenerator
    {
        public void Execute(SourceGeneratorContext context)
        {
            var sourceBuilder = new StringBuilder(@"
using Skillion.Services;
using System.Collections.Generic;
namespace Skillion.GeneratedServices
{
public static class RouteMap {
");

            if (!(context.SyntaxReceiver is RequestAttributeSyntaxReceiver receiver))
                return;
            
            sourceBuilder.Append("private static Dictionary<string, RouteData> _routes = new Dictionary<string, RouteData> {");
            bool itemAdded = false;
            foreach (var ra in receiver.RequestAttributes)
            {
                if (itemAdded)
                    sourceBuilder.Append(", ");

                sourceBuilder.Append($"{{\"{ra.Name}\", new RouteData(\"{ra.Controller}\", \"{ra.Action}\")}}");
                itemAdded = true;
            }

            sourceBuilder.Append("};");
            sourceBuilder.Append("public static Dictionary<string, RouteData> GetRoutes() { return _routes; }");
            sourceBuilder.Append("}}");
            
            context.AddSource("RouteMap", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        public void Initialize(InitializationContext context)
        {
            Console.WriteLine("Scanning for attributes");
            context.RegisterForSyntaxNotifications(() => new RequestAttributeSyntaxReceiver());
        }
    }

    public class RequestAttributeSyntaxReceiver : ISyntaxReceiver
    {
        public List<RequestRouting> RequestAttributes { get; }

        public RequestAttributeSyntaxReceiver()
        {
            RequestAttributes = new List<RequestRouting>();
        }

        public string _clazz = string.Empty;

        Func<AttributeSyntax, bool> isSkillionRequestAttribute = attr =>
        {
            return attr.Name.GetText().ToString() == "IntentRequest" ||
                attr.Name.GetText().ToString() == "FallbackIntentRequest" ||
                attr.Name.GetText().ToString() == "LaunchRequest";
        };

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
            {
                _clazz = classDeclarationSyntax.Identifier.ValueText;
            }

            if (syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax
                && methodDeclarationSyntax.AttributeLists.Any())
            {
                foreach (var attrList in methodDeclarationSyntax.AttributeLists.Where(a => a.Attributes.Any(isSkillionRequestAttribute)))
                {
                    foreach (var attr in attrList.Attributes)
                    {
                        string name = attr.Name.GetText().ToString();
                        
                        if (attr.ArgumentList?.Arguments != null)
                        {
                            
                            var attrArg = attr.ArgumentList.Arguments.First();
                            name = attrArg.Expression.GetText().ToString().Replace("\"", string.Empty);
                            
                        }
                        
                        Console.WriteLine(name);
                        
                        RequestAttributes.Add(
                                new RequestRouting(name,
                                    _clazz,
                                    methodDeclarationSyntax.Identifier.ValueText));
                    }
                }
            }
        }

        public class RequestRouting
        {
            public string Name { get; }

            public string Controller { get; }

            public string Action { get; }

            public RequestRouting(string name, string controller, string action)
            {
                Name = name;
                Controller = controller;
                Action = action;
            }

        }
    }
}