using FluentValidation;

namespace Catalogs.UnitTests.Common;

public class FakeValidator<T> : AbstractValidator<T>
    where T : class { }
