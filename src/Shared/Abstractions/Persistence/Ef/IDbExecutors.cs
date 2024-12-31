using Microsoft.Extensions.DependencyInjection;

namespace Shared.Abstractions.Persistence.Ef;

public interface IDbExecutors
{
    public void Register(IServiceCollection services);
}
