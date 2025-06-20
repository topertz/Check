using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

namespace InvoiceSystem.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        [HttpPost]
        public IActionResult AddProduct([FromForm] string productName, [FromForm] int nettPrice, [FromForm] int vatKey)
        {
            if (vatKey < 0 || vatKey > 100)
            {
                return BadRequest("VAT key must be between 0 and 100.");
            }
            using (var connection = DatabaseConnector.CreateNewConnection())
            {
                string insertSql = "INSERT INTO Product (ProductName, NettPrice, VatKey) VALUES (@ProductName, @NettPrice, @VatKey)";
                using (var cmd = new SQLiteCommand(insertSql, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductName", productName);
                    cmd.Parameters.AddWithValue("@NettPrice", nettPrice);
                    cmd.Parameters.AddWithValue("@VatKey", vatKey);
                    cmd.ExecuteNonQuery();
                }
            }

            return Ok("Product added successfully");
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = new List<Products>();

            using (var connection = DatabaseConnector.CreateNewConnection())
            {
                string selectSql = "SELECT ProductID, ProductName FROM Product";
                using (var cmd = new SQLiteCommand(selectSql, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data = new Products
                            {
                                ProductID = reader.GetInt32(0),
                                ProductName = reader.GetString(1)
                            };
                            products.Add(data);
                        }
                    }
                }
            }

            return Ok(products);
        }
    }
}