namespace Tessa.Application.Enums;

public enum LlmProvider
{
	Unknown,

	/// <summary>
	/// Jan API: https://jan.ai/api-reference
	/// </summary>
	Jan,

	/// <summary>
	/// LlaMA .GGUF: https://github.com/ggerganov/ggml/blob/master/docs/gguf.md
	/// </summary>
	Llama,

	/// <summary>
	/// LMStudio API: https://lmstudio.ai/docs/local-server
	/// </summary>
	LMStudio,

	/// <summary>
	/// OpenAI API: https://platform.openai.com/docs/api-reference
	/// </summary>
	OpenAI
}
