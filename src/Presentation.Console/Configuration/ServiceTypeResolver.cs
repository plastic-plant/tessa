using Spectre.Console.Cli;

namespace Tessa.Presentation.Console.Configuration;

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
