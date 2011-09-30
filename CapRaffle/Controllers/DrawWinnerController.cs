﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using CapRaffle.Models;
using CapRaffle.ActionFilterAttributes;

namespace CapRaffle.Controllers
{
    [Authorize]
    [SetSelectedMenu]
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
