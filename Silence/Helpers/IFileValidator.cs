using Microsoft.AspNetCore.Http;

namespace TaskManagement.Infrastructure.Helpers
{
    public interface IFileValidator
    {
        bool IsValid(IFormFile file);
    }
}
