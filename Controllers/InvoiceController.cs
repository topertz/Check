using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

namespace InvoiceSystem.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class InvoiceController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateInvoice([FromForm] string invoiceSeller, [FromForm] string invoiceCustomer, [FromForm] string invoiceDate)
        {
            int checkID;

            using (var connection = DatabaseConnector.CreateNewConnection())
            {
                // Insert the invoice into the Cheque table
                string insertSql = "INSERT INTO Cheque (SellerID, CustomerID, CheckDate, SumCheck, Delivered) VALUES (@SellerID, @CustomerID, @CheckDate, @SumCheck, @Delivered)";

                using (var cmd = new SQLiteCommand(insertSql, connection))
                {
                    cmd.Parameters.AddWithValue("@SellerID", invoiceSeller);
                    cmd.Parameters.AddWithValue("@CustomerID", invoiceCustomer);
                    cmd.Parameters.AddWithValue("@CheckDate", invoiceDate);
                    cmd.Parameters.AddWithValue("@SumCheck", 0);
                    cmd.Parameters.AddWithValue("@Delivered", 0);

                    cmd.ExecuteNonQuery();

                    // Get the ID of the newly created record
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    checkID = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Perform the update if SumCheck and Delivered are 0
                string updateSql = "UPDATE Cheque SET SumCheck = CASE WHEN SumCheck = 0 THEN 1 ELSE SumCheck END, Delivered = CASE WHEN Delivered = 0 THEN 1 ELSE Delivered END WHERE CheckID = @CheckID";

                using (var cmd = new SQLiteCommand(updateSql, connection))
                {
                    cmd.Parameters.AddWithValue("@CheckID", checkID);

                    cmd.ExecuteNonQuery();
                }
            }

            return Ok(new { checkID, message = "Invoice created and updated successfully" });
        }

        [HttpPost]
        public IActionResult AddProductToInvoice([FromForm] int invoiceProduct, [FromForm] int productQuantity)
        {
            using (var connection = DatabaseConnector.CreateNewConnection())
            {
                string productSql = "SELECT NettPrice, VatKey FROM Product WHERE ProductID = @ProductID";
                int grossPrice = 0;
                using (var cmd = new SQLiteCommand(productSql, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", invoiceProduct);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int nettPrice = reader.GetInt32(0);
                            int vatKey = reader.GetInt32(1);
                            grossPrice = Convert.ToInt32(nettPrice * (100 + vatKey) / 100);
                        }
                        else
                        {
                            return BadRequest("Product not found.");
                        }
                    }
                }

                // Insert the new item into the CheckItems table
                string insertSql = "INSERT INTO CheckItems (ProductID, Quantity, GrossPrice) VALUES (@ProductID, @Quantity, @GrossPrice)";
                using (var cmd = new SQLiteCommand(insertSql, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", invoiceProduct);
                    cmd.Parameters.AddWithValue("@Quantity", productQuantity);
                    cmd.Parameters.AddWithValue("@GrossPrice", grossPrice * productQuantity);
                    cmd.ExecuteNonQuery();
                }   
            }
            return Ok("Product added to invoice successfully");
        }

        [HttpGet]
        public IActionResult GetInvoices()
        {
            var invoices = new List<Invoices>();

            using (var connection = DatabaseConnector.CreateNewConnection())
            {
                // SQL query to fetch CheckID, CheckDate from Cheque, SellerID, SellerName from Seller, 
                // CustomerID, CustomerName from Customer, ProductID, ProductName, NettPrice, VatKey from Product, and GrossPrice from CheckItems
                string selectSql = @"
                SELECT c.CheckID, c.SellerID, s.SellerName, c.CustomerID, cu.CustomerName, p.ProductID, p.ProductName, p.NettPrice, p.VatKey, c.CheckDate, ci.GrossPrice
                FROM Cheque c
                JOIN Seller s ON c.SellerID = s.SellerID
                JOIN Customer cu ON c.CustomerID = cu.CustomerID
                JOIN CheckItems ci ON c.CheckID = ci.CheckID
                JOIN Product p ON ci.ProductID = p.ProductID";

                using (var cmd = new SQLiteCommand(selectSql, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var invoice = new Invoices
                            {
                                CheckID = reader.GetInt32(0),      // CheckID from Cheque
                                SellerID = reader.GetInt32(1),     // SellerID from Cheque
                                SellerName = reader.GetString(2),  // SellerName from Seller
                                CustomerID = reader.GetInt32(3),   // CustomerID from Cheque
                                CustomerName = reader.GetString(4), // CustomerName from Customer
                                ProductId = reader.GetInt32(5),    // ProductID from Product
                                ProductName = reader.GetString(6), // ProductName from Product
                                NettPrice = reader.GetInt32(7),    // NettPrice from Product
                                VatKey = reader.GetInt32(8),       // VatKey from Product
                                CheckDate = reader.GetString(9),   // CheckDate from Cheque
                                GrossPrice = reader.GetInt32(10)   // GrossPrice from CheckItems
                            };
                            invoices.Add(invoice);
                        }
                    }
                }
            }

            return Ok(invoices);
        }
    }
}