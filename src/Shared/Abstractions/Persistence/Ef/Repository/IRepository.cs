using Ardalis.Specification;

namespace Shared.Abstractions.Persistence.Ef.Repository;

// from Ardalis.Specification
public interface IRepository<T> : IRepositoryBase<T>
    where T : class;
