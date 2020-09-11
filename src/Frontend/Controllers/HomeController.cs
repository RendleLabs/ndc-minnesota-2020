﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Frontend.Models;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly Toppings.ToppingsClient _toppingsClient;
        private readonly ILogger<HomeController> _log;

        public HomeController(Toppings.ToppingsClient toppingsClient,
            ILogger<HomeController> log)
        {
            _toppingsClient = toppingsClient;
            _log = log;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var response = _toppingsClient.GetAvailableAsync(new AvailableRequest());

            var headers = await response.ResponseHeadersAsync;
            var serviceIdHeader = headers.Get("service-id");
            ViewData["ServiceId"] = serviceIdHeader?.Value;

            var available = await response.ResponseAsync;

            var toppings = available.Toppings
                .Select(t => new ToppingViewModel(t.Topping.Id, t.Topping.Name, t.Topping.Price))
                .ToList();

            var viewModel = new HomeViewModel(toppings);
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
