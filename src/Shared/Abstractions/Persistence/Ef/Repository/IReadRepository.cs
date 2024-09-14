using Ardalis.Specification;

namespace Shared.Abstractions.Persistence.Ef.Repository;

public interface IReadRepository<T> : IReadRepositoryBase<T>
    where T : class;
