﻿using EShop.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
    {
		private readonly DataContext _dataContext;

		public OrderController(DataContext dataContext)
		{
			_dataContext=dataContext;
		}
		public async Task<IActionResult> Index()
		{

			return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
		}
	}
}
