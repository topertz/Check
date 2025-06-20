using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

namespace InvoiceSystem.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CustomerController : ControllerBase
    {
        [HttpPost]
        public IActionResult AddCustomer([FromForm] string customerName, [FromForm] string customerTaxNumber, [FromForm] string customerTitle)
        {
            using (var connection = DatabaseConnector.CreateNewConnection())
            {
                string insertSql = "INSERT INTO Customer (CustomerName, TaxNumber, CustomerTitle) VALUES (@CustomerName, @TaxNumber, @CustomerTitle)";
                using (var cmd = new SQLiteCommand(insertSql, connection))
                {
                    cmd.Parameters.AddWithValue("@CustomerName", customerName);
                    cmd.Parameters.AddWithValue("@TaxNumber", customerTaxNumber);
                    cmd.Parameters.AddWithValue("@CustomerTitle", customerTitle);
                    cmd.ExecuteNonQuery();
                }
            }

            return Ok("Customer added successfully");
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            var customers = new List<Customers>();

            using (var connection = DatabaseConnector.CreateNewConnection())
            {
                string selectSql = "SELECT CustomerID, CustomerName FROM Customer";
                using (var cmd = new SQLiteCommand(selectSql, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data = new Customers
                            {
                                CustomerID = reader.GetInt32(0),
                                CustomerName = reader.GetString(1)
                            };
                            customers.Add(data);
                        }
                    }
                }
            }

            return Ok(customers);
        }
    }
}