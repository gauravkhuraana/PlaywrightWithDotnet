using System.Text.Json.Nodes;
using Json.Schema;

namespace Framework.Api.Schema;

/// <summary>Validates JSON payloads against a JSON Schema document.</summary>
public sealed class JsonSchemaValidator
{
    /// <summary>Validates <paramref name="json"/> against the schema at <paramref name="schemaPath"/>.</summary>
    public static IReadOnlyList<string> Validate(string json, string schemaPath)
    {
        if (!File.Exists(schemaPath))
        {
            throw new FileNotFoundException("Schema file not found", schemaPath);
        }

        var schema = JsonSchema.FromFile(schemaPath);
        var node = JsonNode.Parse(json);
        var results = schema.Evaluate(node, new EvaluationOptions
        {
            OutputFormat = OutputFormat.List,
        });

        if (results.IsValid)
        {
            return Array.Empty<string>();
        }

        var errors = new List<string>();
        Collect(results, errors);
        return errors;
    }

    private static void Collect(EvaluationResults result, List<string> errors)
    {
        if (result.HasErrors && result.Errors is not null)
        {
            foreach (var (key, msg) in result.Errors)
            {
                errors.Add($"{result.InstanceLocation}: {key} -> {msg}");
            }
        }
        if (result.Details is null)
        {
            return;
        }
        foreach (var d in result.Details)
        {
            Collect(d, errors);
        }
    }
}
