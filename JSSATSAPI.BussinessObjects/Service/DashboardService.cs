using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels.DashboardResponse;
using JSSATSAPI.DataAccess.IRepository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderSellRepository _orderSellRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAccountRepository _accountRepository;

        public DashboardService(IOrderSellRepository orderSellRepository, ICategoryRepository categoryRepository, IAccountRepository accountRepository)
        {
            _orderSellRepository = orderSellRepository;
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
        }

        public async Task<DashboardResponse> GetDashboardDataAsync()
        {
            var totalRevenue = await _orderSellRepository.GetTotalRevenueAsync();
            var revenueByCategory = await GetRevenueByCategoryAsync();
            var orders = await GetOrderSummariesAsync();

            return new DashboardResponse
            {
                TotalRevenue = totalRevenue,
                RevenueByCategory = revenueByCategory,
                Orders = orders
            };
        }

        private async Task<List<CategoryRevenue>> GetRevenueByCategoryAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var revenueByCategory = new List<CategoryRevenue>();

            foreach (var category in categories)
            {
                var revenue = await _orderSellRepository.GetRevenueByCategoryIdAsync(category.CategoryId);
                revenueByCategory.Add(new CategoryRevenue
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Revenue = revenue
                });
            }

            return revenueByCategory;
        }

        private async Task<List<OrderSummary>> GetOrderSummariesAsync()
        {
            var orders = await _orderSellRepository.GetRecentOrdersAsync();
            return orders.Select(o => new OrderSummary
            {
                OrderSellId = o.OrderSellId,
                CustomerName = o.Customer?.Name,
                OrderDate = o.OrderDate ?? DateTime.MinValue,
                TotalAmount = o.FinalAmount ?? 0,
                Status = o.Status
            }).ToList();
        }

        public async Task<List<MonthlyRevenueResponse>> GetMonthlyRevenueAsync(int year, int month)
        {
            var orderSells = await _orderSellRepository.GetOrdersByMonthAsync(year, month);
            var accounts = await _accountRepository.GetAllAsync();

            var groupedOrders = orderSells
                .GroupBy(o => o.SellerId)
                .Select(g => new
                {
                    SellerId = g.Key,
                    Revenue = g.Sum(o => o.FinalAmount ?? 0)
                })
                .ToList();

            var response = groupedOrders.Select(g => new MonthlyRevenueResponse
            {
                AccountId = g.SellerId ?? 0,
                FullName = accounts.FirstOrDefault(a => a.AccountId == g.SellerId)?.FirstName + " " + accounts.FirstOrDefault(a => a.AccountId == g.SellerId)?.LastName,
                Month = month,
                Year = year,
                Revenue = g.Revenue
            }).ToList();

            return response;
        }

        public async Task<SalesDashboardResponse> GetSalesDataAsync()
        {
            var orderSells = await _orderSellRepository.GetAllAsync();

            var dailySales = orderSells
                .GroupBy(o => o.OrderDate.Value.Date)
                .Select(g => new ProductSalesResponse
                {
                    Period = g.Key.ToString("yyyy-MM-dd"),
                    TotalProductsSold = g.Sum(o => o.OrderSellDetails.Sum(d => d.Quantity ?? 0))
                }).ToList();

            var weeklySales = orderSells
                .GroupBy(o => new { Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(o.OrderDate.Value, CalendarWeekRule.FirstDay, DayOfWeek.Monday), Year = o.OrderDate.Value.Year })
                .Select(g => new ProductSalesResponse
                {
                    Period = $"Week {g.Key.Week}, {g.Key.Year}",
                    TotalProductsSold = g.Sum(o => o.OrderSellDetails.Sum(d => d.Quantity ?? 0))
                }).ToList();

            var monthlySales = orderSells
                .GroupBy(o => new { o.OrderDate.Value.Year, o.OrderDate.Value.Month })
                .Select(g => new ProductSalesResponse
                {
                    Period = $"{g.Key.Month}-{g.Key.Year}",
                    TotalProductsSold = g.Sum(o => o.OrderSellDetails.Sum(d => d.Quantity ?? 0))
                }).ToList();

            var yearlySales = orderSells
                .GroupBy(o => o.OrderDate.Value.Year)
                .Select(g => new ProductSalesResponse
                {
                    Period = g.Key.ToString(),
                    TotalProductsSold = g.Sum(o => o.OrderSellDetails.Sum(d => d.Quantity ?? 0))
                }).ToList();

            return new SalesDashboardResponse
            {
                DailySales = dailySales,
                WeeklySales = weeklySales,
                MonthlySales = monthlySales,
                YearlySales = yearlySales
            };
        }
    }

}
