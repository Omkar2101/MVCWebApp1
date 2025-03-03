// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using MongoDB.Driver;
// using MVCWebApp.Models;
// using System.Threading.Tasks;

// namespace MVCWebApp.Controllers
// {
//     public class PrivacyController : Controller
//     {
//         private readonly IMongoCollection<VisitorEntry> _visitorCollection;

//         public PrivacyController(IConfiguration config)
//         {
//             var client = new MongoClient(config["MongoDB:ConnectionString"]);
//             var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
//             _visitorCollection = database.GetCollection<VisitorEntry>(config["MongoDB:CollectionName"]);
//         }

//         public async Task<IActionResult> Index()
//         {
//             var latestVisitor = await _visitorCollection
//                 .Find(_ => true)
//                 .SortByDescending(v => v.VisitTime)
//                 .Limit(1)
//                 .FirstOrDefaultAsync();

//             return View(latestVisitor);
//         }
//     }
// }
using Microsoft.AspNetCore.Mvc;
using MVCWebApp.Models;
using MVCWebApp.Services;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace MVCWebApp.Controllers
{
    public class PrivacyController : Controller
    {
        private readonly IMongoCollection<VisitorEntry> _visitorCollection;

        public PrivacyController(MongoDbService mongoService, IConfiguration config)
        {
            string collectionName = config["MongoDB:VisitorDB:VisitorCollectionName"];
            _visitorCollection = mongoService.GetVisitorCollection<VisitorEntry>(collectionName);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var latestVisitor = await _visitorCollection
                    .Find(_ => true)
                    .SortByDescending(v => v.VisitTime)
                    .Limit(1)
                    .FirstOrDefaultAsync();

                return View(latestVisitor);
            }
            catch (Exception ex)
            {
                // You might want to log the error here
                return View("Error");
            }
        }
    }
}