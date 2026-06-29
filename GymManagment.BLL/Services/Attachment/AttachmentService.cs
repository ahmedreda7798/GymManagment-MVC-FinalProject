using GymManagment.BLL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GymManagment.BLL.Services.Attachment;
public class AttachmentService : IAttachmentService
{
    private readonly long _maxFileSize = 5 * 1024 * 1024; //5 MB
    private readonly ILogger<AttachmentService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly string[] _allowedExtensions = [".jpeg", ".png", ".jpg"];

    public AttachmentService(ILogger<AttachmentService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public bool Delete(string fileName, string folderName)
    {
        var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);
        try
        {
            if (!File.Exists(fullPath)) return true; // file already gone — treat as success
            File.Delete(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed To Delete Attachment : {fileName}");
            return false;
        }
        return true;
    }

    public (Stream stream, string conteneType)? GetFile(string fileName, string folderName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName)) return null;
        var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);
        if (!File.Exists(fullPath)) return null;

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        var extension = Path.GetExtension(fullPath).ToLower();
        var contentType = extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream" //Binary Date
        };
        return (stream, contentType);
    }

    public async Task<Result<string>> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
    {
        //Check the file steam 
        if (fileStream == null || !fileStream.CanRead)
            return Result<string>.Validation("File stream is null or unreadable.");

        if (fileName.Length == 0) //file stream is empty [0 bytes]
            return Result<string>.Validation("File strem must not be empty.");

        //Check the size — reject anything over 5 MB.
        if (fileStream.Length > _maxFileSize)
        {
            _logger.LogError($"File Rejected : File Is Too Large : {fileStream.Length} Bytes");
            return Result<string>.Validation($"File is too large ({fileStream.Length} bytes). Maximum allowed size is 5 MB.");
        }

        //Check the extension — only .jpg .jpeg .png allowed.
        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) || !_allowedExtensions.Contains(extension))
        {
            _logger.LogError($"File Rejected : Extension {extension} Not Allowed");
            return Result<string>.Validation($"File extension '{extension}' is not allowed. Only .jpg, .jpeg, and .png are accepted.");
        }

        //Locate the folder & create it if missing.
        var uploadsFolder = Path.Combine(_env.ContentRootPath, folderName);
        Directory.CreateDirectory(uploadsFolder); //Ensure the folder exists . if not it will be created
        //E:\Route\08 MVC\08 MVC\Session 01\GymManagmentSolution\GymManagment\MembersPhoto\

        //Make the name unique using a GUID
        var storedFileName = $"{Guid.NewGuid()}{fileName}";

        //Build the full file path.
        var filePath = Path.Combine(uploadsFolder, storedFileName);
        //now we have --> E:\Route\08 MVC\08 MVC\Session 01\GymManagmentSolution\GymManagment\MembersPhoto\Test.Png

        try
        {
            //Open a file stream (an unmanaged resource).
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            //Copy the file into that stream.
            await fileStream.CopyToAsync(fs, ct); //Copy the uploaded file stream to the file stream we just opened.
            //Return the file name to store in the database.
            return Result<string>.OK(storedFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed To Upload File {fileName}");
            return Result<string>.Fail($"An unexpected error occurred while uploading '{fileName}'.");
        }
    }
}
