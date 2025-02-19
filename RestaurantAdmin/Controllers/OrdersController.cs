using Microsoft.AspNetCore.Mvc;
using RestaurantAdmin.Models;
using RestaurantAdmin.Data;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // Enable CORS for React Frontend
        private void EnableCors()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        }

        // Get all orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            EnableCors();
            return await _context.Orders.ToListAsync();
        }

        // Get order by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            EnableCors();
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound(new { message = "Order not found" });
            return order;
        }

        // Create a new order
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            EnableCors();

            // Validate order data
            if (order == null || string.IsNullOrEmpty(order.CustomerName) || string.IsNullOrEmpty(order.FoodItem))
            {
                return BadRequest(new { message = "Invalid order data. Please provide CustomerName and FoodItem." });
            }

            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Order placed successfully!", orderId = order.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to place order", error = ex.Message });
            }
        }

        // Update an existing order
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            EnableCors();
            if (id != order.Id) return BadRequest(new { message = "ID mismatch" });

            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Order updated successfully!" });
        }

        // Delete an order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            EnableCors();
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound(new { message = "Order not found" });

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Order deleted successfully!" });
        }
    }
}































// using Microsoft.AspNetCore.Mvc;
// using RestaurantAdmin.Models;
// using RestaurantAdmin.Data;
// using Microsoft.EntityFrameworkCore;

// namespace RestaurantAdmin.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class OrdersController : ControllerBase
//     {
//         private readonly AppDbContext _context;

//         public OrdersController(AppDbContext context)
//         {
//             _context = context;
//         }

//         // Get all orders
//         [HttpGet]
//         public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
//         {
//             return await _context.Orders.ToListAsync();
//         }

//         // Get order by ID
//         [HttpGet("{id}")]
//         public async Task<ActionResult<Order>> GetOrder(int id)
//         {
//             var order = await _context.Orders.FindAsync(id);
//             if (order == null) return NotFound();
//             return order;
//         }

//         // Create a new order
//         [HttpPost]
//         public async Task<ActionResult<Order>> CreateOrder(Order order)
//         {
//             _context.Orders.Add(order);
//             await _context.SaveChangesAsync();
//             return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
//         }

//         // Update an existing order
//         [HttpPut("{id}")]
//         public async Task<IActionResult> UpdateOrder(int id, Order order)
//         {
//             if (id != order.Id) return BadRequest();
//             _context.Entry(order).State = EntityState.Modified;
//             await _context.SaveChangesAsync();
//             return NoContent();
//         }

//         // Delete an order
//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeleteOrder(int id)
//         {
//             var order = await _context.Orders.FindAsync(id);
//             if (order == null) return NotFound();
//             _context.Orders.Remove(order);
//             await _context.SaveChangesAsync();
//             return NoContent();
//         }
//     }
// }
