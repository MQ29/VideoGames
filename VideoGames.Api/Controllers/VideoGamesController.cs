using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGames.Api.DTO;
using VideoGames.Core;
using VideoGames.Core.Models;
using VideoGames.Infrasctructure;

namespace VideoGames.Api.Controllers
{
    [Route("api/games")]
    [ApiController]
    public class VideoGamesController(VideoGamesContext _context) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<ActionResult<PagedResult<Game>>> GetPagedVideoGames(int pageNumber = 1, int pageSize = 1)
        {
            var videoGames = _context.Games.AsQueryable();

            var totalCount = await videoGames.CountAsync();
            var pagedMovies = await videoGames.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Game>
            {
                Items = pagedMovies,
                TotalCount = totalCount,
                PageSize = pageSize,
                PageNumber = pageNumber
            };
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByGenre([FromQuery]string genre)
        {
            var lowerGenre = genre.ToLower();

            var games = await _context.Games
                        .Include(x => x.Genre)
                        .Include(x => x.GamePublishers)
                            .ThenInclude(gp => gp.Publisher)
                        .Include(x => x.GamePublishers)
                            .ThenInclude(gp => gp.GamePlatforms)
                                .ThenInclude(gp => gp.Platform)
                        .Where(x => x.Genre.GenreName.ToLower() == lowerGenre)
                        .ToListAsync();

            if (games.Count() == 0)
            {
                return NotFound();
            }

            var gameDtos = games.Select(x => new GameDto
            {
                Id = x.Id,
                GameName = x.GameName,
                PublisherName = x.GamePublishers.FirstOrDefault()?.Publisher?.PublisherName,
                GamePlatform = x.GamePublishers.FirstOrDefault()?.GamePlatforms.FirstOrDefault()?.Platform?.PlatformName,
                ReleaseYear = x.GamePublishers.FirstOrDefault()?.GamePlatforms.FirstOrDefault()?.ReleaseYear
            });

            return Ok(gameDtos);
        }

        [HttpPost("{id}/sales")]
        public async Task<ActionResult> AddSales(int id, [FromBody] SalesDto salesDto)
        {
            var game = _context.Games.FirstOrDefault(x => x.Id == id);

            if (game is null)
            {
                return NotFound();
            }

            var regionSale = await _context.RegionSales
        .Where(rs => rs.RegionId == salesDto.RegionId && rs.GamePlatform.GamePublisher.GameId == id)
        .FirstOrDefaultAsync();

            if (regionSale == null)
            {
                return NotFound("Region sale not found.");
            }

            regionSale.NumSales += salesDto.Sales;
            await _context.SaveChangesAsync();

            return Ok(regionSale);
        }
    }
}

