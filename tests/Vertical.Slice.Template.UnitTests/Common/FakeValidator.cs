using FluentValidation;

namespace Vertical.Slice.Template.UnitTests.Common;

public class FakeValidator<T> : AbstractValidator<T>
    where T : class { }
