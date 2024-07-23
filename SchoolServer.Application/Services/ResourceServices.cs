using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Linq;
using SchoolServer.Application.Exceptions;
namespace SchoolServer.Application.Services;

public class ResourceServices
{
    private static string ResourcesDirectory { get; } = Path.Combine(Directory.GetCurrentDirectory(), "Resources");

    private readonly ILogger<ResourceServices> logger;

    public ResourceServices(ILogger<ResourceServices> logger)
    {
        this.logger = logger;
    }

    public async Task CreateResource(string subDirectory, IFormFile file, string[] supportedTypes)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Invalid file.");
        bool check = false;
        foreach (var supportedType in supportedTypes)
        {
            if (file.FileName.Contains(supportedType))
            {
                check = true;
                break;
            }

        }
        if (!check)
            throw new NotSupportedFileType();
        var directoryPath = Path.Combine(ResourcesDirectory, subDirectory);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(directoryPath, file.FileName);

        if (File.Exists(filePath))
            throw new FileExistException();
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
    }
    public FileStream GetResource(string subDirectory, string fileName)
    {
        var filePath = Path.Combine(ResourcesDirectory, subDirectory, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found.");
        }
        return File.OpenRead(filePath);
    }

    public void DeleteResource(string subDirectory, string fileName)
    {
        var filePath = Path.Combine(ResourcesDirectory, subDirectory, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        else
        {
            throw new FileNotFoundException("File not found.");
        }
    }

    public List<string> GetAllFiles(string subDirectory)
    {
        return Directory
            .GetFiles(Path.Combine(ResourcesDirectory, subDirectory))
            .Select(f=> Path.GetFileName(f))
            .ToList();
    }
}
