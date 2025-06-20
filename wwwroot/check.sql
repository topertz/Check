PRAGMA foreign_keys = ON;

DROP TABLE IF EXISTS `Seller`;
CREATE TABLE `Seller` (
    `SellerID` INTEGER NOT NULL PRIMARY KEY,
    `SellerName` TEXT NOT NULL,
    `TaxNumber` TEXT NOT NULL,
    `SellerTitle` TEXT NOT NULL
);

DROP TABLE IF EXISTS `Customer`;
CREATE TABLE `Customer` (
    `CustomerID` INTEGER NOT NULL PRIMARY KEY,
    `CustomerName` TEXT NOT NULL,
    `TaxNumber` TEXT NOT NULL,
    `CustomerTitle` TEXT NOT NULL
);

DROP TABLE IF EXISTS `Cheque`;
CREATE TABLE `Cheque` (
    `CheckID` INTEGER NOT NULL PRIMARY KEY,
    `SellerID` INTEGER NOT NULL,
    `CustomerID` INTEGER NOT NULL,
    `CheckDate` TEXT NOT NULL,
    `SumCheck` INTEGER NOT NULL,
    `Delivered` INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (`SellerID`) REFERENCES Seller(`SellerID`),
    FOREIGN KEY (`CustomerID`) REFERENCES Customer(`CustomerID`)
);

DROP TABLE IF EXISTS `Product`;
CREATE TABLE `Product` (
    `ProductID` INTEGER NOT NULL PRIMARY KEY,
    `ProductName` TEXT NOT NULL,
    `NettPrice` INTEGER NOT NULL,
    `VatKey` INTEGER NOT NULL
);

DROP TABLE IF EXISTS `CheckItems`;
CREATE TABLE `CheckItems` (
    `CheckID` INTEGER NOT NULL,
    `ProductID` INTEGER NOT NULL,
    `Quantity` INTEGER NOT NULL,
    `GrossPrice` INTEGER NOT NULL,
    PRIMARY KEY (`CheckID`, `ProductID`),
    FOREIGN KEY (`CheckID`) REFERENCES `Cheque`(`CheckID`),
    FOREIGN KEY (`ProductID`) REFERENCES `Product`(`ProductID`)
);