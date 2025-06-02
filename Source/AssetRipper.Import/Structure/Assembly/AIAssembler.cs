using AssetRipper.Import.Logging;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.PE.DotNet.Cil;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace AssetRipper.Import.Structure.Assembly;

public static class AIAssembler
{
    public static void AssembleScripts(IEnumerable<AssemblyDefinition> assemblies)
    {
        string? openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        string? claudeKey = Environment.GetEnvironmentVariable("CLAUDE_API_KEY");
        if (string.IsNullOrEmpty(openAiKey) && string.IsNullOrEmpty(claudeKey))
        {
            Logger.Warning(LogCategory.Import, "No AI API key provided. Skipping level 4 assembly.");
            return;
        }

        Logger.Info(LogCategory.Import, "Assembling scripts using AI service...");

        foreach (AssemblyDefinition assembly in assemblies)
        {
            foreach (TypeDefinition type in assembly.Modules.SelectMany(m => m.GetAllTypes()))
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    if (method.CilMethodBody is null && !method.IsAbstract)
                    {
                        string prompt = CreatePrompt(type, method);
                        string? source = openAiKey is not null
                                ? QueryOpenAi(openAiKey, prompt)
                                : QueryAnthropic(claudeKey!, prompt);

                        if (!string.IsNullOrEmpty(source))
                        {
                            method.CilMethodBody = new CilMethodBody(method);
                            method.CilMethodBody.Instructions.Add(CilOpCodes.Ldstr, source);
                            method.CilMethodBody.Instructions.Add(CilOpCodes.Ret);
                        }
                    }
                }
            }
        }
    }

    private static string CreatePrompt(TypeDefinition type, MethodDefinition method)
    {
        return $"Decompile the following IL method into valid C# code. Return only the code.\nType: {type.FullName}\nMethod: {method.Name}";
    }

    private static string? QueryOpenAi(string apiKey, string prompt)
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var request = new
        {
            model = "gpt-4",
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0
        };

        System.Text.Json.JsonSerializerOptions options = new()
        {
            TypeInfoResolver = System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver.Instance
        };
        string json = System.Text.Json.JsonSerializer.Serialize(request, options);
        using StringContent content = new(json, System.Text.Encoding.UTF8, "application/json");
        HttpResponseMessage response = client.PostAsync("https://api.openai.com/v1/chat/completions", content).GetAwaiter().GetResult();
        if (!response.IsSuccessStatusCode)
        {
            Logger.Warning(LogCategory.Import, $"OpenAI request failed: {response.StatusCode}");
            return null;
        }

        using var stream = response.Content.ReadAsStream();
        using var doc = System.Text.Json.JsonDocument.Parse(stream);
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
    }

    private static string? QueryAnthropic(string apiKey, string prompt)
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);

        var request = new
        {
            model = "claude-3-opus-20240229",
            max_tokens = 2048,
            messages = new[] { new { role = "user", content = prompt } }
        };

        System.Text.Json.JsonSerializerOptions options = new()
        {
            TypeInfoResolver = System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver.Instance
        };
        string json = System.Text.Json.JsonSerializer.Serialize(request, options);
        using StringContent content = new(json, System.Text.Encoding.UTF8, "application/json");
        HttpResponseMessage response = client.PostAsync("https://api.anthropic.com/v1/messages", content).GetAwaiter().GetResult();
        if (!response.IsSuccessStatusCode)
        {
            Logger.Warning(LogCategory.Import, $"Anthropic request failed: {response.StatusCode}");
            return null;
        }

        using var stream = response.Content.ReadAsStream();
        using var doc = System.Text.Json.JsonDocument.Parse(stream);
        return doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString();
    }
}
