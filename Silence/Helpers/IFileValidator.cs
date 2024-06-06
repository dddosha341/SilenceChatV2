using Microsoft.AspNetCore.Http;

namespace Silence.Web.Helpers
{
    public interface IFileValidator
    {
        bool IsValid(IFormFile file);
    }
}
