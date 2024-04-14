using Tessa.Application.Enums;

namespace Tessa.Application.Models.ProviderConfigs
{
	public class ProviderConfigLlamaGguf : ProviderConfig
	{
		public string? Model { get; set; }
		public int GpuLayerCount { get; set; }
		public uint Seed { get; set; }

        public ProviderConfigLlamaGguf()
        {
            Name = "llama";
			Provider = LlmProvider.Llama;
			Seed = (uint)DateTime.Now.Ticks;
		}
    }
}
