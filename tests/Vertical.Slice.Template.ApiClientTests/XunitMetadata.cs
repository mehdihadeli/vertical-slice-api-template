using Vertical.Slice.Template.TestsShared.XunitFramework;

[assembly: TestFramework(
    $"Vertical.Slice.Template.{nameof( Vertical.Slice.Template.TestsShared)}.{nameof(Vertical.Slice.Template.TestsShared.XunitFramework)}.{nameof
    (CustomTestFramework)}",
    $"Vertical.Slice.Template.{nameof(Vertical.Slice.Template.TestsShared)}"
)]
