using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using thegame.Infrastructure;
using thegame.Models;

namespace thegame.Controllers
{
    [Route("api/games/{gameId}/moves")]
    public class MovesController : Controller
    {
        readonly IMemoryCache cache;

        public MovesController(IMemoryCache cache) => this.cache = cache;

        [HttpPost]
        public IActionResult Moves(Guid gameId, [FromBody] UserInputForMovesPost userInput)
        {
            var game = cache.Get<Game>(gameId);

            if (game == null)
                return new BadRequestResult();

            var direction = userInput.GetDirection();
            game.MovePlayer(direction);
            game.CheckForLevelIsFinished();

            return game.ToResponse();
        }
    }
}