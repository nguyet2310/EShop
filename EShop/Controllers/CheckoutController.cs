﻿using EShop.Models;
using EShop.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;

        public CheckoutController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login","Account");
            }
            else
            {
                var ordercode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = ordercode;
                orderItem.UserName = userEmail;
                orderItem.CreatedDate = DateTime.Now;
                orderItem.Status = 1;
                _dataContext.Add(orderItem);
                _dataContext.SaveChanges();
                TempData["success"] = "Đơn hàng đã được tạo.";

                // Lấy giỏ hàng từ session, nếu không có sẽ tạo mới
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var cart in cartItems)
                {
                    var orderDetails = new OrderDetails();
                    orderDetails.UserName = userEmail;
                    orderDetails.OrderCode = ordercode;
                    orderDetails.ProductId = cart.ProductId;
                    orderDetails.Price = cart.Price;
                    orderDetails.Quantity = cart.Quantity;
                    _dataContext.Add(orderDetails); 
                    _dataContext.SaveChanges();
                }
                HttpContext.Session.Remove("Cart");
                TempData["success"] = "Chekout thành công, vui lòng chờ duyệt đơn hàng!";
                return RedirectToAction("Index", "Cart");
            }
            return View();
        }
    }
}
