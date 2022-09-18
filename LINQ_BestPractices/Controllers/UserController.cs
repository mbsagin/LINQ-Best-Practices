using LINQ_BestPractices.Infrastructure;
using LINQ_BestPractices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LINQ_BestPractices.Controllers
{
    public class UserController : Controller
    {
        private readonly LINQDbContext _dbContext;
        public UserController(LINQDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Only pull the needed columns in the Select clause
        // Instead of loading all the columns
        public async Task<ActionResult> GetActiveUsers()
        {
            var users_1 = await _dbContext.Users.AsQueryable().Where(u => u.IsActive).ToListAsync();

            // AsNoTracking() // ObjectTrackingEnabled config in DbContext
            // When we load records from a database via LINQ-to-Entities queries,
            // we will be processing them and updating them back to the database.
            // For this purpose, entities be tracked.
            // When we are performing only read operations, we won’t make any updates back to the database,
            // but entities will assume that we are going to make updates back to the database and will process accordingly.
            // So, we can use AsNoTracking() to restrict entities from assuming and processing, thus reducing the amount of memory that entities will track.

            // Break After Each Statement
            // For LINQ queries, break them before every dot.
            // This makes them way better readable and the logic gets more clear.
            // You can read code faster vertically, than horizontally. 

            var users_2 = await _dbContext.Users
            .AsQueryable()
            .AsNoTracking()
            .Where(u =>
                u.IsActive)
            .Select(u => new
            {
                Name = u.Name,
                u.Mail
            })
            .ToArrayAsync();

            return View();
        }

        // Each time we add a new entity, DetectChanges() from Data.Entity.Core will be triggered,
        // and query execution will become slower.
        // To overcome this, use AddRange, which best suits bulk inserts. 
        public async Task<ActionResult> CreateUsers(List<User> users)
        {
            foreach (var user in users)
            {
                _dbContext.Add<User>(user);
            }

            _dbContext.SaveChanges();

            // Use of async operations in entities to reduce the blocking of the UI thread. 

            _dbContext.AddRange(users);
            await _dbContext.SaveChangesAsync();

            return View();
        }

        // Extract Complicated Where Clauses
        // If there are two or more condition, your intention will fade.
        // The next developer will struggle to understand, what you want to filter here.
        // To fix this, just extract the lambda into a local function and give it a good name
        public async Task<ActionResult> GetActiveMaleUsersRegisteredToday()
        {
            var users_1 = await _dbContext.Users
                .AsQueryable()
                .AsNoTracking()
                .Where(u =>
                    u.IsActive &&
                    u.Gender == 0b01 &&
                    u.RegisterDate >= DateTime.Today)
                .ToListAsync();

            var users_2 = await _dbContext.Users
                .AsQueryable()
                .AsNoTracking()
                .Where(ActiveMaleUsersRegisteredToday)
                .ToListAsync();

            return View();
        }

        // Use Single Characters for Your Lambda Variables
        readonly Expression<Func<User, bool>> ActiveMaleUsersRegisteredToday = (User u) =>
            u.IsActive &&
            u.Gender == 0b01 &&
            u.RegisterDate >= DateTime.Today;

        readonly Expression<Func<User, bool>> ActiveMaleUsersRegisteredToday_2 = (User user) =>
            user.IsActive &&
            user.Gender == 0b01 &&
            user.RegisterDate >= DateTime.Today;

        // Use FirstOrDefaultAsync Instead Where
        // The iterator in the second case can stop as soon as it finds a match,
        // where the first one must find all that match, and then pick the first of those
        public async Task<ActionResult> GetUserById(int userId)
        {
            var user_1 = await _dbContext.Users
                  .AsQueryable()
                  .AsNoTracking()
                  .Where(u =>
                      u.UserId == userId)
                  .FirstOrDefaultAsync();

            var user_2 = await _dbContext.Users
                  .AsQueryable()
                  .AsNoTracking()
                  .FirstOrDefaultAsync(u =>
                      u.UserId == userId);

            return View();
        }

        public async Task<ActionResult> GetAllActiveUsers()
        {
            var users_1 = await _dbContext.Users
                  .AsQueryable()
                  .AsNoTracking()
                  .Where(u =>
                      u.IsActive)
                  .ToListAsync();

            var users_2 = _dbContext.Users
                  .AsQueryable()
                  .AsNoTracking()
                  .Where(u =>
                      u.IsActive);

            await users_2.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
