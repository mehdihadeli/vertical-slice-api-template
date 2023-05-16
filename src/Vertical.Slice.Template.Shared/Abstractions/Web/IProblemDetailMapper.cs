namespace Vertical.Slice.Template.Shared.Abstractions.Web;

public interface IProblemDetailMapper
{
    int GetMappedStatusCodes(Exception exception);
}
