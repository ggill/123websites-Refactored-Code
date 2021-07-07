using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BikeDistributor.Domain;
using System.Net;

namespace BikeApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BikeOrderController : ControllerBase
    {
        // Order List
        private static readonly List<Order> orders = new List<Order>();
        
        private static void Setup() 
        {
            // Bikes
            Bike defyBike = new Bike("Giant", "Defy 1", Bike.OneThousand);
            Bike eliteBike = new Bike("Specialized", "Venge Elite", Bike.TwoThousand);
            Bike duraBike = new Bike("Specialized", "S-Works Venge Dura-Ace", Bike.FiveThousand);
            // Company
            string CompanyA = "CompanyA";
            string CompanyB = "CompanyB";

            // Order
            Order order1 = new Order(1, CompanyA);
            Order order2 = new Order(2, CompanyA);
            Order order3 = new Order(1, CompanyB);
            Order order4 = new Order(2, CompanyB);

            // Add order to the list
            order1.AddLine(new OrderLine(defyBike, 3));
            order2.AddLine(new OrderLine(eliteBike, 5));
            order3.AddLine(new OrderLine(defyBike, 2));
            order4.AddLine(new OrderLine(eliteBike, 4));
            orders.Add(order1);
            orders.Add(order2);
            orders.Add(order3);
            orders.Add(order4);

        }

        private readonly ILogger<BikeOrderController> _logger;

        public BikeOrderController(ILogger<BikeOrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("/orders")]
        // Route GET /bikeorder?company=CompanyA
        // If company is missing will retrieve all companies
        public IEnumerable<Order> Orders(string company)
        {
            orders.Clear();
            Setup();
            if (string.IsNullOrEmpty(company)) 
            { 
                return orders; 
            }
            else
            {
                return orders.FindAll(delegate (Order o)
                {
                    return o.Company == company;
                });
            }
        }
        
        [HttpGet]
        [Route("/receipt")]
        // Route GET /bikeorder?company=CompanyA
        // If company is missing will retrieve all companies
        public ContentResult Receipt(int id)
        {
            if (id > 0) 
            {
                Order foundOrder = orders.Find(delegate (Order o)
                {
                    return o.OrderId == id;
                });
                if (foundOrder != null) 
                {
                    string receipt = foundOrder.HtmlReceipt();
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.OK,
                        Content = receipt
                    };
                }
                else
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Content = $"<html><head><title>Order Not Found</title></head><body><h2>Order not found for this id!</h2></body></html>"
                    };
                }
            }
            else
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = $"<html><head><title>Order Receipt Bad Request</title></head><body><h2>Missing id parameter or id should be greater then zero!</h2></body></html>"
                };
            }
        }
    }
}