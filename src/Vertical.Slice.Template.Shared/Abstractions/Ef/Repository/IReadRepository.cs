using Ardalis.Specification;

namespace Vertical.Slice.Template.Shared.Abstractions.Ef.Repository;

public interface IReadRepository<T> : IReadRepositoryBase<T>
    where T : class { }
