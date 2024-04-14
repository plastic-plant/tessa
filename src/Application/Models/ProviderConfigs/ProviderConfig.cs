using System.Text.Json.Serialization;
using Tessa.Application.Enums;

namespace Tessa.Application.Models.ProviderConfigs;

public class ProviderConfig : IProviderConfig
{
	public string? Name { get; set; }
	public LlmProvider Provider { get; set; }
	public float Temperature { get; set; } = 0.7f;
	public int MaxTokens { get; set; } = -1;
	public uint? ContextSize { get; set; }

	public static List<IProviderConfig> GetExamples() => new()
	{
		new ProviderConfigJan() { Model = "tinyllama-1.1b", ContextSize = 4096, Temperature = 0.7f, MaxTokens = -1 },
		new ProviderConfigLMStudio() { ContextSize = 16385, Temperature = 0.7f, MaxTokens = -1 }, // gemma-2b-it-q8_0.gguf
		new ProviderConfigOpenAI() { Model = "gpt-3.5-turbo", ContextSize = 16385, Temperature = 0.7f, MaxTokens = -1, ApiKey = "YOUR_API_KEY" },
		new ProviderConfigLlamaGguf() { Model = "wizardLM-7B.Q8_0.gguf", ContextSize = 2048, Seed = 42, GpuLayerCount = 5, Temperature = 0.7f, MaxTokens = -1 },
		new ProviderConfigLlamaGguf() { Model = "llama-2-7b-chat.Q3_K_M.gguf", ContextSize = 4096, Seed = 42, GpuLayerCount = 5, Temperature = 0.7f, MaxTokens = -1  },
		new ProviderConfigLlamaGguf() { Model = "gemma-2b-it.gguf", ContextSize = 1024, Seed = 42, GpuLayerCount = 5, Temperature = 0.6f, MaxTokens = -1 },
		new ProviderConfigLlamaGguf() { Model = "geitje-7b-ultra.Q8_0.gguf", ContextSize = 32768, Seed = 42, GpuLayerCount = 5, Temperature = 0.6f, MaxTokens = -1 },
	};
}
