//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using MongoDB.Driver;
//using System;
//using System.Net;
//using System.Threading.Tasks;
//using MVCWebApp.Models;

//namespace MVCWebApp.Controllers
//{
//    public class HomeController : Controller
//    {
//        private readonly IMongoCollection<VisitorEntry> _visitorCollection;

//        public HomeController(IConfiguration config)
//        {
//            var visitorConnectionString = config.GetValue<string>("MongoDB:VisitorDB:ConnectionString");
//            var visitorDatabaseName = config.GetValue<string>("MongoDB:VisitorDB:DatabaseName");
//            var visitorCollectionName = config.GetValue<string>("MongoDB:VisitorDB:VisitorCollectionName");

//            if (string.IsNullOrEmpty(visitorConnectionString))
//                throw new ArgumentNullException(nameof(visitorConnectionString), "VisitorDB Connection String is missing in appsettings.json");

//            if (string.IsNullOrEmpty(visitorDatabaseName))
//                throw new ArgumentNullException(nameof(visitorDatabaseName), "VisitorDB Database Name is missing in appsettings.json");

//            if (string.IsNullOrEmpty(visitorCollectionName))
//                throw new ArgumentNullException(nameof(visitorCollectionName), "VisitorDB Collection Name is missing in appsettings.json");

//            var client = new MongoClient(visitorConnectionString);
//            var database = client.GetDatabase(visitorDatabaseName);
//            _visitorCollection = database.GetCollection<VisitorEntry>(visitorCollectionName);
//        }





//        public async Task<IActionResult> Index()
//        {
//            var remoteIp = HttpContext.Connection.RemoteIpAddress;

//            string formattedIp = "Unknown";

//            if (remoteIp != null)
//            {
//                if (remoteIp.IsIPv4MappedToIPv6)
//                {
//                    // Convert IPv6-mapped IPv4 to a standard IPv4 address
//                    formattedIp = remoteIp.MapToIPv4().ToString();
//                }
//                else if (remoteIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // IPv4
//                {
//                    formattedIp = remoteIp.ToString();
//                }
//                else if (remoteIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) // IPv6
//                {
//                    if (remoteIp.ToString() == "::1")
//                    {
//                        formattedIp = "127.0.0.1";  // Convert localhost IPv6 (::1) to IPv4
//                    }
//                    else
//                    {
//                        formattedIp = remoteIp.ToString();
//                    }
//                }
//            }

//            var visitor = new VisitorEntry
//            {
//                IpAddress = formattedIp,
//                VisitTime = DateTime.UtcNow
//            };

//            await _visitorCollection.InsertOneAsync(visitor);

//            return View();
//        }


//    }
//}
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using MVCWebApp.Models;
using MVCWebApp.Services;

namespace MVCWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly MongoDbService _mongoDbService;

        // Constructor injection of MongoDbService
        public HomeController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService ?? throw new ArgumentNullException(nameof(mongoDbService));
        }

        public async Task<IActionResult> Index()
        {
            // Get the visitor's IP address
            var remoteIp = HttpContext.Connection.RemoteIpAddress;

            string formattedIp = "Unknown";

            if (remoteIp != null)
            {
                if (remoteIp.IsIPv4MappedToIPv6)
                {
                    // Convert IPv6-mapped IPv4 to a standard IPv4 address
                    formattedIp = remoteIp.MapToIPv4().ToString();
                }
                else if (remoteIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // IPv4
                {
                    formattedIp = remoteIp.ToString();
                }
                else if (remoteIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) // IPv6
                {
                    if (remoteIp.ToString() == "::1")
                    {
                        formattedIp = "127.0.0.1";  // Convert localhost IPv6 (::1) to IPv4
                    }
                    else
                    {
                        formattedIp = remoteIp.ToString();
                    }
                }
            }

            // Create a new VisitorEntry object
            var visitor = new VisitorEntry
            {
                IpAddress = formattedIp,
                VisitTime = DateTime.UtcNow
            };

            // Use the VisitorCollectionName property
            var visitorCollection = _mongoDbService.GetVisitorCollection<VisitorEntry>(_mongoDbService.VisitorCollectionName);
            await visitorCollection.InsertOneAsync(visitor);

            return View();
        }
    }
}
