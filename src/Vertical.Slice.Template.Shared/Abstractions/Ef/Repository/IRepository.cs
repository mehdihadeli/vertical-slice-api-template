using Ardalis.Specification;

namespace Vertical.Slice.Template.Shared.Abstractions.Ef.Repository;

// from Ardalis.Specification
public interface IRepository<T> : IRepositoryBase<T>
    where T : class { }
