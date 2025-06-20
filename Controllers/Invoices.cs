using System.Security.Policy;

public class Invoices
{
    public int CheckID { get; set; }
    public int SellerID { get; set; }
    public string? SellerName { get; set; }
    public int CustomerID { get; set; }
    public string? CustomerName { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int NettPrice { get; set; }
    public int VatKey { get; set; }
    public int GrossPrice { get; set; }
    public string? CheckDate { get; set; }
}