using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MVCWebApp.Models;
using MVCWebApp.Services;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace MVCWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IMongoCollection<UserEntry> _userCollection;

        public UserController(MongoDbService mongoDbService, IConfiguration config)
        {
            string userCollectionName = config["MongoDB:UserDB:UserCollectionName"];
            if (string.IsNullOrEmpty(userCollectionName))
            {
                throw new InvalidOperationException("UserCollectionName is not configured.");
            }

            _userCollection = mongoDbService.GetUserCollection<UserEntry>(userCollectionName);
            Console.WriteLine($"Initialized collection: {userCollectionName}");
        }

        // Display Form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserEntry user)
        {

            try
            {
                Console.WriteLine("=== DEBUG: User Creation Started ===");
                Console.WriteLine($"Received User: {user.Name}, {user.Location}");

                // Ensure user has a new ObjectId
                user.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                Console.WriteLine($"Assigned ID: {user.Id}");

                Console.WriteLine("Inserting user...");
                await _userCollection.InsertOneAsync(user);
                Console.WriteLine("User inserted successfully!");


                Console.WriteLine("=== DEBUG: User Inserted Successfully ===");
                return RedirectToAction("Details");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting user: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the user.");
            }

            return View(user);
        }



        // Display User List
        public async Task<IActionResult> Details()
        {
            try
            {
                var users = await _userCollection.Find(_ => true).ToListAsync();
                Console.WriteLine($"Retrieved {users.Count} users from MongoDB.");

                foreach (var user in users)
                {
                    Console.WriteLine($"User ID: {user.Id}, Name: {user.Name}, Email: {user.Location}");
                }

                return View(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving users: {ex.Message}");
                return View(new List<UserEntry>());
            }
        }
    }
}
