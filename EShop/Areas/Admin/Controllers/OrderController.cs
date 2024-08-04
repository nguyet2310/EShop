using EShop.Models;
using EShop.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin, Publisher, Author")]
	public class OrderController : Controller
    {
		private readonly DataContext _dataContext;

		public OrderController(DataContext dataContext)
		{
			_dataContext=dataContext;
		}
        //public async Task<IActionResult> Index()
        //{
        //	return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
        //}

        public async Task<IActionResult> Index(int pg = 1)
        {
            List<OrderModel> orders = await _dataContext.Orders.ToListAsync(); //33 datas

            const int pageSize = 10; //10 items/trang

            if (pg < 1) //page < 1;
            {
                pg = 1; //page ==1
            }
            int recsCount = orders.Count(); //33 items;

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize; //(3 - 1) * 10; 

            //category.Skip(20).Take(10).ToList()

            var data = orders.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }

        public async Task<IActionResult> ViewOrder(string orderCode)
        {
            //if (string.IsNullOrEmpty(orderCode))
            //{
            //    return BadRequest("Order code is required.");
            //}
            var detailsOrder = await _dataContext.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == orderCode).ToListAsync();
            return View(detailsOrder);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(string orderCode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Order status updated successfully" });
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the order status.");
            }
        }
    }
}
