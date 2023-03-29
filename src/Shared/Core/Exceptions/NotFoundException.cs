using System.Net;

namespace Shared.Core.Exceptions;

public class NotFoundException : CustomException
{
	public NotFoundException(string message)
		: base(message)
	{
		StatusCode = HttpStatusCode.NotFound;
	}
}