using Microsoft.EntityFrameworkCore;
using WebStore.Entities;

namespace WebStore.Assignments
{
    /// Additional tutorial materials https://dotnettutorials.net/lesson/linq-to-entities-in-entity-framework-core/

    /// <summary>
    /// This class demonstrates various LINQ query tasks 
    /// to practice querying an EF Core database.
    /// 
    /// ASSIGNMENT INSTRUCTIONS:
    ///   1. For each method labeled "TODO", write the necessary
    ///      LINQ query to return or display the required data.
    ///      
    ///   2. Print meaningful output to the console (or return
    ///      collections, as needed).
    ///      
    ///   3. Test each method by calling it from your Program.cs
    ///      or test harness.
    /// </summary>
    public class LinqQueriesAssignment
    {
        private readonly Oppimistehtävä3Context _dbContext;

        public LinqQueriesAssignment(Oppimistehtävä3Context context)
        {
            _dbContext = context;
        }


        /// <summary>
        /// 1. List all customers in the database:
        ///    - Print each customer's full name (First + Last) and Email.
        /// </summary>
        public async Task Task01_ListAllCustomers()
        {
            var customers = await _dbContext.Customers
            // .AsNoTracking() // optional for read-only
            .ToListAsync();

            Console.WriteLine("=== TASK 01: List All Customers ===");

            foreach (var c in customers)
            {
                Console.WriteLine($"{c.FirstName} {c.LastName} - {c.Email}");
            }
        }

        /// <summary>
        /// 2. Fetch all orders along with:
        ///    - Customer Name
        ///    - Order ID
        ///    - Order Status
        ///    - Number of items in each order (the sum of OrderItems.Quantity)
        /// </summary>
        public async Task Task02_ListOrdersWithItemCount()
        {
            var orders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Select(o => new
                {
                    OrderId = o.OrderId,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    Status = o.OrderStatus,
                    ItemCount = o.OrderItems.Sum(oi => oi.Quantity)
                })
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== TASK 02: List Orders With Item Count ===");

            foreach (var order in orders)
            {
                Console.WriteLine($"Order #{order.OrderId} - {order.CustomerName} - {order.Status} - Items: {order.ItemCount}");
            }
        }

        /// <summary>
        /// 3. List all products (ProductName, Price),
        ///    sorted by price descending (highest first).
        /// </summary>
        public async Task Task03_ListProductsByDescendingPrice()
        {
            var products = await _dbContext.Products
                .OrderByDescending(p => p.Price)
                .Select(p => new { p.ProductName, p.Price })
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 03: List Products By Descending Price ===");

            foreach (var product in products)
            {
                Console.WriteLine($"{product.ProductName} - {product.Price} €");
            }
        }

        /// <summary>
        /// 4. Find all "Pending" orders (order status = "Pending")
        ///    and display:
        ///      - Customer Name
        ///      - Order ID
        ///      - Order Date
        ///      - Total price (sum of unit_price * quantity - discount) for each order
        /// </summary>
        public async Task Task04_ListPendingOrdersWithTotalPrice()
        {
            var pendingOrders = await _dbContext.Orders
                .Where(o => o.OrderStatus == "Pending")
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    TotalPrice = o.OrderItems.Sum(oi => (oi.UnitPrice * oi.Quantity) - oi.Discount)
                })
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 04: List Pending Orders With Total Price ===");

            foreach (var order in pendingOrders)
            {
                Console.WriteLine($"Order #{order.OrderId} | {order.CustomerName} | {order.OrderDate:d} | Total: {order.TotalPrice:0.00} €");
            }
        }

        /// <summary>
        /// 5. List the total number of orders each customer has placed.
        ///    Output should show:
        ///      - Customer Full Name
        ///      - Number of Orders
        /// </summary>
        public async Task Task05_OrderCountPerCustomer()
        {
            var orderCounts = await _dbContext.Orders
                .GroupBy(o => new { o.CustomerId, o.Customer.FirstName, o.Customer.LastName })
                .Select(g => new
                {
                    CustomerName = g.Key.FirstName + " " + g.Key.LastName,
                    OrderCount = g.Count()
                })
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 05: Order Count Per Customer ===");

            foreach (var c in orderCounts)
            {
                Console.WriteLine($"{c.CustomerName} - Orders: {c.OrderCount}");
            }
        }

        /// <summary>
        /// 6. Show the top 3 customers who have placed the highest total order value overall.
        ///    - For each customer, calculate SUM of (OrderItems * Price).
        ///      Then pick the top 3.
        /// </summary>
        public async Task Task06_Top3CustomersByOrderValue()
        {
            var topCustomers = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .GroupBy(o => new { o.CustomerId, o.Customer.FirstName, o.Customer.LastName })
                .Select(g => new
                {
                    CustomerName = g.Key.FirstName + " " + g.Key.LastName,
                    TotalValue = g.Sum(o => o.OrderItems.Sum(oi => (oi.UnitPrice * oi.Quantity) - oi.Discount))
                })
                .OrderByDescending(c => c.TotalValue)
                .Take(3)
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 06: Top 3 Customers By Order Value ===");

            foreach (var c in topCustomers)
            {
                Console.WriteLine($"{c.CustomerName} - Total Spent: {c.TotalValue:0.00} €");
            }
        }

        /// <summary>
        /// 7. Show all orders placed in the last 30 days (relative to now).
        ///    - Display order ID, date, and customer name.
        /// </summary>
        public async Task Task07_RecentOrders()
        {
            var recentDate = DateTime.Now.AddDays(-30);

            var recentOrders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Where(o => o.OrderDate >= recentDate)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName
                })
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 07: Recent Orders ===");

            foreach (var order in recentOrders)
            {
                Console.WriteLine($"Order #{order.OrderId} | {order.OrderDate:d} | {order.CustomerName}");
            }
        }

        /// <summary>
        /// 8. For each product, display how many total items have been sold
        ///    across all orders.
        ///    - Product name, total sold quantity.
        ///    - Sort by total sold descending.
        /// </summary>
        public async Task Task08_TotalSoldPerProduct()
        {
            var totalSold = await _dbContext.OrderItems
                .GroupBy(oi => new { oi.ProductId, oi.Product.ProductName })
                .Select(g => new
                {
                    g.Key.ProductName,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 08: Total Sold Per Product ===");

            foreach (var product in totalSold)
            {
                Console.WriteLine($"{product.ProductName} - Total Sold: {product.TotalQuantity}");
            }
        }

        /// <summary>
        /// 9. List any orders that have at least one OrderItem with a Discount > 0.
        ///    - Show Order ID, Customer name, and which products were discounted.
        /// </summary>
        public async Task Task09_DiscountedOrders()
        {
            var discountedOrders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderItems.Any(oi => oi.Discount > 0))
                .Select(o => new
                {
                    o.OrderId,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    DiscountedProducts = o.OrderItems
                        .Where(oi => oi.Discount > 0)
                        .Select(oi => oi.Product.ProductName)
                        .ToList()
                })
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 09: Discounted Orders ===");

            foreach (var order in discountedOrders)
            {
                var products = string.Join(", ", order.DiscountedProducts);
                Console.WriteLine($"Order #{order.OrderId} | {order.CustomerName} | Discounted: {products}");
            }
        }

        /// <summary>
        /// 10. (Open-ended) Combine multiple joins or navigation properties
        ///     to retrieve a more complex set of data. For example:
        ///     - All orders that contain products in a certain category
        ///       (e.g., "Electronics"), including the store where each product
        ///       is stocked most. (Requires `Stocks`, `Store`, `ProductCategory`, etc.)
        ///     - Or any custom scenario that spans multiple tables.
        /// </summary>
        public async Task Task10_AdvancedQueryExample()
        {
            var electronicsOrders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Categories)
                .Where(o => o.OrderItems.Any(oi =>
                    oi.Product.Categories.Any(c => c.CategoryName == "Electronics")))
                .Select(o => new
                {
                    o.OrderId,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    ElectronicsProducts = o.OrderItems
                        .Where(oi => oi.Product.Categories.Any(c => c.CategoryName == "Electronics"))
                        .Select(oi => oi.Product.ProductName)
                        .Distinct()
                        .ToList()
                })
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 10: Advanced Query Example (Electronics) ===");

            foreach (var order in electronicsOrders)
            {
                var products = string.Join(", ", order.ElectronicsProducts);
                Console.WriteLine($"Order #{order.OrderId} | {order.CustomerName} | Electronics: {products}");
            }
        }
    }
}
