using Microsoft.AspNetCore.Mvc;
using MVCWebApp.Models;
using MVCWebApp.Services;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVCWebApp.Controllers
{
    public class VisitorController : Controller
    {
        private readonly IMongoCollection<VisitorEntry> _visitorCollection;

        public VisitorController(MongoDbService mongoDbService, IConfiguration config)
        {
            string visitorCollectionName = config["MongoDB:VisitorDB:VisitorCollectionName"];
            _visitorCollection = mongoDbService.GetVisitorCollection<VisitorEntry>(visitorCollectionName);
        }

        // Log Visitor
        [HttpPost]
        public async Task<IActionResult> LogVisitor(VisitorEntry visitor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    visitor.VisitTime = DateTime.UtcNow; // Set visit time
                    await _visitorCollection.InsertOneAsync(visitor);
                    return Ok();
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while logging the visitor.");
            }
        }

        // Get Visitor Logs
        public async Task<IActionResult> GetVisitors()
        {
            try
            {
                var visitors = await _visitorCollection
                    .Find(_ => true)
                    .SortByDescending(v => v.VisitTime)
                    .ToListAsync();
                return View(visitors);
            }
            catch (Exception ex)
            {
                // Log the error
                return View("Error");
            }
        }
    }
}
