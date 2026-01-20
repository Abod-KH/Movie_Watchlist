namespace Movie_Watchlist.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveImage(IFormFile imageFile)
        {
            if (imageFile == null)
            {
                return "";
            }

            // 1. Create the path to the folder: wwwroot/images
            var contentPath = _environment.WebRootPath;
            var path = Path.Combine(contentPath, "images");

            // 2. Make sure the folder exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // 3. Create a unique filename (e.g., "guid-batman.jpg")
            var ext = Path.GetExtension(imageFile.FileName);
            var newFileName = Guid.NewGuid().ToString() + ext;
            var fileWithPath = Path.Combine(path, newFileName);

            // 4. Copy the file to the stream
            using (var stream = new FileStream(fileWithPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return newFileName;
        }
    }
}