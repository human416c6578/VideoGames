using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using VideoGames.data;
using VideoGames.Models;

namespace VideoGames.Controllers
{
    public class GamesController : Controller
    {
        private readonly MvcGamesDbContext _context;
        public GamesController(MvcGamesDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            ViewData["TitleSortParam"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            var games = await _context.Games.ToListAsync();
 

            if (!String.IsNullOrEmpty(searchString))
            {
                games = games.Where(g => g.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            switch (sortOrder)
            {
                case "title_desc":
                    games = games.OrderByDescending(g => g.Title).ToList();
                    break;
                case "date":
                    games = games.OrderBy(g => g.ReleaseDate).ToList();
                    break;
                case "date_desc":
                    games = games.OrderByDescending(g => g.ReleaseDate).ToList();
                    break;
                default:
                    games = games.OrderBy(g => g.Title).ToList();
                    break;
            }

            int pageSize = 3;
            ViewData["Games"] = PaginatedList<Game>.Create(games, pageNumber ?? 1, pageSize);
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Game addGameRequest)
        {
            if (String.IsNullOrEmpty(addGameRequest.Title))
                return RedirectToAction("Index");

            var game = new Game()
            {
                Id = Guid.NewGuid().ToString(),
                Genre = addGameRequest.Genre,
                ReleaseDate = addGameRequest.ReleaseDate,
                Title = addGameRequest.Title,
                Description = addGameRequest.Description,

            };
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id, string sortOrder, int? pageNumber)
        {
            if (id == null)
                return NotFound();

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            var reviews = await _context.Reviews.Where(r => r.GameId == game.Id).OrderBy(r => r.Created).ToListAsync();

            ViewData["RatingSortParam"] = sortOrder == "rating_asc" ? "rating_desc" : "rating_asc";
            ViewData["DateSortParam"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewData["CurrentSort"] = sortOrder;

            switch (sortOrder)
            {
                case "rating_desc":
                    reviews = reviews.OrderByDescending(r => r.Rating).ToList();
                    break;
                case "rating_asc":
                    reviews = reviews.OrderBy(r => r.Rating).ToList();
                    break;
                case "date_asc":
                    reviews = reviews.OrderBy(r => r.Created).ToList();
                    break;
                default:
                    reviews = reviews.OrderByDescending(r => r.Created).ToList();
                    break;
            }

            game.Rating = reviews.Sum(r=> r.Rating) / (float)reviews.Count;

            ViewData["Game"] = game;
            ViewData["Reviews"] = reviews;
            int pageSize = 3;
            ViewData["Reviews"] = PaginatedList<Review>.Create(reviews, pageNumber ?? 1, pageSize);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
                return NotFound();

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            return View(game);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Game editGameRequest)
        {
            if (editGameRequest == null)
                return NotFound();

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == editGameRequest.Id);
            if(game == null) return NotFound();
            _context.Games.Remove(game);
            await _context.Games.AddAsync(editGameRequest);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index"); ;
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Game deleteGameRequest)
        {
            if (deleteGameRequest == null)
                return NotFound();

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == deleteGameRequest.Id);
            if (game == null) return NotFound();
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index"); ;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(Review addReviewRequest)
        {
            if (String.IsNullOrEmpty(addReviewRequest.Author))
                return RedirectToAction("Details", new { id = addReviewRequest.GameId });

            var review = new Review()
            {
                Id = Guid.NewGuid().ToString(),
                Author = addReviewRequest.Author,
                Created = DateTime.UtcNow,
                Comment = addReviewRequest.Comment,
                GameId = addReviewRequest.GameId,
                Rating = addReviewRequest.Rating,

            };

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = addReviewRequest.GameId });

        }
    }
}
