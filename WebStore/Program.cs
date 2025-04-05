using Microsoft.EntityFrameworkCore;
using WebStore.Entities;
using WebStore.Assignments;

namespace WebStore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<Oppimistehtävä3Context>()
                .UseNpgsql("Host=localhost;Port=5432;Database=Oppimistehtävä 3;Username=postgres;Password=!Kim2003")
                .Options;

            using var context = new Oppimistehtävä3Context(options);

            var Assignments = new LinqQueriesAssignment(context);

            await Assignments.Task01_ListAllCustomers();
            await Assignments.Task02_ListOrdersWithItemCount();
            await Assignments.Task03_ListProductsByDescendingPrice();
            await Assignments.Task04_ListPendingOrdersWithTotalPrice();
            await Assignments.Task05_OrderCountPerCustomer();
            await Assignments.Task06_Top3CustomersByOrderValue();
            await Assignments.Task07_RecentOrders();
            await Assignments.Task08_TotalSoldPerProduct();
            await Assignments.Task09_DiscountedOrders();
            await Assignments.Task10_AdvancedQueryExample();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
