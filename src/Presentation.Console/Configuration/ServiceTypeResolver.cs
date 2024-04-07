using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tessa.Presentation.Cli.Configuration;

public class ServiceTypeResolver : ITypeResolver, IDisposable
{
	private readonly IServiceProvider _provider;

	public ServiceTypeResolver(IServiceProvider provider)
	{
		_provider = provider ?? throw new ArgumentNullException(nameof(provider));
	}

	public object? Resolve(Type? type)
	{
		if (type == null)
		{
			return null;
		}

		return _provider.GetService(type);
	}

	public void Dispose()
	{
		if (_provider is IDisposable disposable)
		{
			disposable.Dispose();
		}
	}
}
