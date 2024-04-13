using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Application.Enums;

namespace Tessa.Application.Models.ProviderConfigs;

public interface IProviderConfig
{
    public string? Name { get; set; }
    public LlmProvider Provider { get; set; }
    public float Temperature { get; set; }
    public int MaxTokens { get; set; }
    public uint? ContextSize { get; set; }
}