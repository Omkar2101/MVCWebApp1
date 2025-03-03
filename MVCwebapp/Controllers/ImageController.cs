using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.GridFS;
using MVCWebApp.Models;
using MVCWebApp.Services;
using MongoDB.Driver;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MVCWebApp.Controllers
{
    public class ImageController : Controller
    {
        private readonly GridFSBucket _gridFSBucket;
        private readonly IMongoCollection<ImageEntry> _imageCollection;

        public ImageController(MongoDbService mongoDbService, IConfiguration config)
        {
            if (mongoDbService == null)
            {
                throw new ArgumentNullException(nameof(mongoDbService), "MongoDbService is null");
            }

            string imageCollectionName = config["MongoDB:ImageDB:ImageCollectionName"];
            if (string.IsNullOrEmpty(imageCollectionName))
            {
                throw new InvalidOperationException("ImageCollectionName is not configured in appsettings.json.");
            }

            _gridFSBucket = mongoDbService.GetGridFSBucket();
            if (_gridFSBucket == null)
            {
                throw new InvalidOperationException("GridFSBucket initialization failed.");
            }

            _imageCollection = mongoDbService.GetImageCollection<ImageEntry>(imageCollectionName);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file, string text)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a file.");
                return View();
            }

            using var stream = file.OpenReadStream();
            var fileId = await _gridFSBucket.UploadFromStreamAsync(file.FileName, stream);

            var imageEntry = new ImageEntry
            {
                FileName = file.FileName,
                Text = text,
                FileId = fileId.ToString()
            };

            await _imageCollection.InsertOneAsync(imageEntry);

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var images = await _imageCollection.Find(_ => true).ToListAsync();
            return View(images);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var imageEntry = await _imageCollection.Find(i => i.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
            if (imageEntry == null)
            {
                return NotFound();
            }
            return View(imageEntry);
        }


        [HttpPost]

        public async Task<IActionResult> Edit(string id, IFormFile file, string text)
        {
            var imageEntry = await _imageCollection.Find(i => i.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
            if (imageEntry == null)
            {
                return NotFound();
            }

            if (file != null && file.Length > 0) // If user uploads a new file
            {
                // Delete old image from GridFS
                await _gridFSBucket.DeleteAsync(ObjectId.Parse(imageEntry.FileId));

                // Upload new image
                using var stream = file.OpenReadStream();
                var newFileId = await _gridFSBucket.UploadFromStreamAsync(file.FileName, stream);

                // Update database with new file details
                imageEntry.FileId = newFileId.ToString();
                imageEntry.FileName = file.FileName;
            }

            // Update text
            imageEntry.Text = text;

            await _imageCollection.ReplaceOneAsync(i => i.Id == ObjectId.Parse(id), imageEntry);

            return RedirectToAction("List");
        }


        [HttpGet]
        //public async Task<IActionResult> ViewImage(string fileId)
        //{
        //    var stream = await _gridFSBucket.OpenDownloadStreamAsync(new MongoDB.Bson.ObjectId(fileId));
        //    return File(stream, "image/jpeg");
        //}
        public async Task<IActionResult> GetImage(string id)
        {
            try
            {
                var fileId = ObjectId.Parse(id);
                var stream = await _gridFSBucket.OpenDownloadStreamAsync(fileId);
                return File(stream, "image/jpeg"); // Change type if needed (e.g., "image/png")
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving image: {ex.Message}");
                return NotFound();
            }
        }

    }
}
