namespace task11;
﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.Emit;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
public static class Calculator
{
    public static ICalculator CreateCalculator(string row)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(row);
        row = row.Replace("public class Calculator","public class Calculator: task11.ICalculator");

        SyntaxTree tree = CSharpSyntaxTree.ParseText(row);

        List<MetadataReference> references =
        [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
        ];

        CSharpCompilation compiler = CSharpCompilation.Create(
            assemblyName: $"Calculator_{Guid.NewGuid():N}",
            syntaxTrees: new[] { tree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using MemoryStream stream = new();

       EmitResult result = compiler.Emit(stream);

        if (!result.Success)
        {
            var errors = result.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.ToString());

            throw new InvalidOperationException("Ошибка компиляции:\n" + string.Join(Environment.NewLine, errors));
        }

        stream.Position = 0;

        Assembly assembly = Assembly.Load(stream.ToArray());

        Type? calculator = assembly.GetType("Calculator");

        if (calculator is null)
            throw new InvalidOperationException("Класс Calculator не найден.");

        return (ICalculator)Activator.CreateInstance(calculator)!;
    }
        
}
