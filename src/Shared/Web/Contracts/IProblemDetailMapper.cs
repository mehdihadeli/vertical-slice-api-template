namespace Shared.Web.Contracts;

public interface IProblemDetailMapper
{
    int GetMappedStatusCodes(Exception exception);
}
