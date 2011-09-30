using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using CapRaffle.Models;

namespace CapRaffle.Controllers
{
    [Authorize]
    public class DrawWinnerController : Controller
    {
        private IDrawingRepository drawingRepository;
        
        

        public DrawWinnerController(IDrawingRepository drawRepo)
        {
            drawingRepository = drawRepo;
        }

        [HttpPost]
        public PartialViewResult DrawWinner(int eventId, string view)
        {
            DrawWinnerViewModel model = GenerateDrawWinnerViewModel(eventId);
            return PartialView(view, model);
        }

        public ActionResult RemoveWinner(Winner winner)
        {
            drawingRepository.RemoveWinner(winner);
            return Redirect(Url.Action("Details", "Event", new { id = winner.EventId }));
        }
        
        private DrawWinnerViewModel GenerateDrawWinnerViewModel(int eventId)
        {
            drawingRepository.PerformDrawing(eventId);

            DrawWinnerViewModel viewModel = new DrawWinnerViewModel
            {
                Winners = drawingRepository.WinnersForEvent(eventId).ToList<Winner>(),
                NumberOfSpotsLeft = drawingRepository.NumberOfSpotsLeftForEvent(eventId)
            };
            return viewModel;
        }
    }
}
