using System.Data.SQLite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseStaticFiles();

app.MapGet("/", () => Results.Redirect("/index.html"));
SQLiteConnection connection = DatabaseConnector.Db();
SQLiteCommand command = connection.CreateCommand();
command.CommandText = "PRAGMA foreign_keys = ON;" +
    "CREATE TABLE if not Exists `Seller` " +
    "(`SellerID` integer NOT NULL PRIMARY KEY, `SellerName` text not NULL, `TaxNumber` text not NULL, `SellerTitle` text not NULL); " +
    "CREATE TABLE if not Exists `Customer`" +
    " (`CustomerID` integer NOT NULL PRIMARY KEY,  `CustomerName` text NOT NULL, " +
    " `TaxNumber` text not null, `CustomerTitle` text not null);" +
    "CREATE TABLE if not Exists `Cheque` (`CheckID` integer NOT NULL PRIMARY KEY, " +
    "`SellerID` integer NOT NULL, `CustomerID` integer NOT NULL, " +
    "`CheckDate` text NOT NULL, `SumCheck` integer NOT NULL DEFAULT 0, " +
    "FOREIGN KEY (`SellerID`) REFERENCES `Seller`(`SellerID`), FOREIGN KEY (`CustomerID`) REFERENCES `Customer`(`CustomerID`));" +
    "CREATE TABLE if not Exists `Product` (`ProductID` integer NOT NULL PRIMARY KEY, " +
    "`ProductName` text NOT NULL, `NettPrice` integer NOT NULL, " +
    "`VatKey` integer NOT NULL); " +
    "CREATE TABLE if not Exists `CheckItems` (`CheckID` integer NOT NULL PRIMARY KEY, " +
    "`ProductID` integer NOT NULL, `Quantity` integer NOT NULL, " +
    "`GrossPrice` integer NOT NULL, " +
    "FOREIGN KEY (`CheckID`) REFERENCES `Cheque`(`CheckID`), FOREIGN KEY (`ProductID`) REFERENCES `Product`(`ProductID`));";
command.ExecuteNonQuery();
command.Dispose();
app.Run();