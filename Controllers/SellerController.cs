using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;
using System.Diagnostics; // For debugging
using System.Collections.Generic;

namespace CheckApplication.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SellerController : Controller
    {
        [HttpPost]
        public IActionResult AddSeller([FromForm] string sellerName, [FromForm] string sellerTaxNumber, [FromForm] string sellerTitle)
        {

            try
            {
                using (var connection = DatabaseConnector.CreateNewConnection())
                {

                    string insertSql = "INSERT INTO Seller (SellerName, TaxNumber, SellerTitle) VALUES (@SellerName, @TaxNumber, @SellerTitle)";
                    using (var cmd = new SQLiteCommand(insertSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@SellerName", sellerName);
                        cmd.Parameters.AddWithValue("@TaxNumber", sellerTaxNumber);
                        cmd.Parameters.AddWithValue("@SellerTitle", sellerTitle);
                        cmd.ExecuteNonQuery();
                    }
                }

                return Ok("Seller added successfully");
            }
            catch (SQLiteException sqlEx)
            {
                Debug.WriteLine($"Database error: {sqlEx.Message}");
                return StatusCode(500, $"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An unexpected error occurred: {ex.Message}");
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult GetSellers()
        {
            var sellers = new List<Sellers>();

            try
            {
                using (var connection = DatabaseConnector.CreateNewConnection())
                {

                    string selectSql = "SELECT SellerID, SellerName FROM Seller";
                    using (var cmd = new SQLiteCommand(selectSql, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var data = new Sellers
                                {
                                    SellerID = reader.GetInt32(0),
                                    SellerName = reader.GetString(1)
                                };
                                sellers.Add(data);
                            }
                        }
                    }
                }

                return Ok(sellers);
            }
            catch (SQLiteException sqlEx)
            {
                // Log the SQL exception
                Debug.WriteLine($"Database error: {sqlEx.Message}");
                return StatusCode(500, $"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Log the general exception
                Debug.WriteLine($"An unexpected error occurred: {ex.Message}");
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
