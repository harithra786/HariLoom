CREATE DATABASE IF NOT EXISTS hariloom;
USE hariloom;

-- ========================================================
-- 1. mstUser
-- ========================================================
CREATE TABLE IF NOT EXISTS `mstUser` (
    `mstUserId` int NOT NULL AUTO_INCREMENT,
    `name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `phoneNumber` longtext CHARACTER SET utf8mb4 NOT NULL,
    `password` longtext CHARACTER SET utf8mb4 NOT NULL,
    `email` longtext CHARACTER SET utf8mb4 NULL,
    `address` longtext CHARACTER SET utf8mb4 NULL,
    `addressLine1` longtext CHARACTER SET utf8mb4 NULL,
    `addressLine2` longtext CHARACTER SET utf8mb4 NULL,
    `city` longtext CHARACTER SET utf8mb4 NULL,
    `state` longtext CHARACTER SET utf8mb4 NULL,
    `zipCode` longtext CHARACTER SET utf8mb4 NULL,
    `profileImageUrl` longtext CHARACTER SET utf8mb4 NULL,
    `accessLevel` int NOT NULL DEFAULT 1,
    `createdDate` datetime(6) NOT NULL,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `isPromotionalEmailOptIn` tinyint(1) NOT NULL DEFAULT 0,
    `ipAddress` longtext CHARACTER SET utf8mb4 NULL,
    `visitCount` int NOT NULL DEFAULT 0,
    CONSTRAINT `PK_mstUser` PRIMARY KEY (`mstUserId`)
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 2. mstProductMainCategory
-- ========================================================
CREATE TABLE IF NOT EXISTS `mstProductMainCategory` (
    `mstProductMainCategoryId` int NOT NULL AUTO_INCREMENT,
    `mainCategoryName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `mainCategoryImagePath` longtext CHARACTER SET utf8mb4 NULL,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdBy` int NOT NULL,
    `createdDate` datetime(6) NOT NULL,
    `updatedBy` int NULL,
    `updatedDate` datetime(6) NULL,
    CONSTRAINT `PK_mstProductMainCategory` PRIMARY KEY (`mstProductMainCategoryId`)
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 3. mstProductSubCategory
-- ========================================================
CREATE TABLE IF NOT EXISTS `mstProductSubCategory` (
    `mstProductSubCategoryId` int NOT NULL AUTO_INCREMENT,
    `mstProductMainCategoryId` int NOT NULL,
    `subCategoryName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `subCategoryImagePath` longtext CHARACTER SET utf8mb4 NULL,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdBy` int NOT NULL,
    `createdDate` datetime(6) NOT NULL,
    `updatedBy` int NULL,
    `updatedDate` datetime(6) NULL,
    CONSTRAINT `PK_mstProductSubCategory` PRIMARY KEY (`mstProductSubCategoryId`),
    CONSTRAINT `FK_mstProductSubCategory_mstProductMainCategory` FOREIGN KEY (`mstProductMainCategoryId`) REFERENCES `mstProductMainCategory` (`mstProductMainCategoryId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 4. mstProduct
-- ========================================================
CREATE TABLE IF NOT EXISTS `mstProduct` (
    `mstProductId` int NOT NULL AUTO_INCREMENT,
    `productName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `productDisplayId` longtext CHARACTER SET utf8mb4 NULL,
    `description` longtext CHARACTER SET utf8mb4 NULL,
    `coverImagePath` longtext CHARACTER SET utf8mb4 NULL,
    `basePrice` int NOT NULL,
    `discountedPrice` int NULL,
    `quantityAvailable` int NOT NULL,
    `productImages` longtext CHARACTER SET utf8mb4 NULL,
    `washCareInstructions` longtext CHARACTER SET utf8mb4 NULL,
    `mstProductGroupingIds` longtext CHARACTER SET utf8mb4 NOT NULL,
    `mstProductMainCategoryId` int NOT NULL,
    `mstProductSubCategoryId` int NOT NULL,
    `isAvailable` tinyint(1) NOT NULL DEFAULT 1,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdBy` int NOT NULL,
    `createdDate` datetime(6) NOT NULL,
    `updatedBy` int NULL,
    `updatedDate` datetime(6) NULL,
    `deletedBy` int NULL,
    `deletedDate` datetime(6) NULL,
    CONSTRAINT `PK_mstProduct` PRIMARY KEY (`mstProductId`)
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 5. trnPaymentLog
-- ========================================================
CREATE TABLE IF NOT EXISTS `trnPaymentLog` (
    `trnPaymentLogId` int NOT NULL AUTO_INCREMENT,
    `trnOrderId` int NULL,
    `mstUserId` int NULL,
    `razorpayOrderId` longtext CHARACTER SET utf8mb4 NULL,
    `razorpayPaymentId` longtext CHARACTER SET utf8mb4 NULL,
    `razorpaySignature` longtext CHARACTER SET utf8mb4 NULL,
    `paymentStatus` longtext CHARACTER SET utf8mb4 NULL,
    `failureReason` longtext CHARACTER SET utf8mb4 NULL,
    `amount` decimal(65,30) NULL,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdDate` datetime(6) NOT NULL,
    CONSTRAINT `PK_trnPaymentLog` PRIMARY KEY (`trnPaymentLogId`)
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 6. trnUserAddress
-- ========================================================
CREATE TABLE IF NOT EXISTS `trnUserAddress` (
    `trnUserAddressId` int NOT NULL AUTO_INCREMENT,
    `mstUserId` int NOT NULL,
    `label` longtext CHARACTER SET utf8mb4 NOT NULL,
    `addressLine1` longtext CHARACTER SET utf8mb4 NOT NULL,
    `addressLine2` longtext CHARACTER SET utf8mb4 NULL,
    `city` longtext CHARACTER SET utf8mb4 NOT NULL,
    `state` longtext CHARACTER SET utf8mb4 NOT NULL,
    `zipCode` longtext CHARACTER SET utf8mb4 NOT NULL,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdDate` datetime(6) NOT NULL,
    `updatedDate` datetime(6) NULL,
    CONSTRAINT `PK_trnUserAddress` PRIMARY KEY (`trnUserAddressId`),
    CONSTRAINT `FK_trnUserAddress_mstUser_mstUserId` FOREIGN KEY (`mstUserId`) REFERENCES `mstUser` (`mstUserId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 7. trnProductColor
-- ========================================================
CREATE TABLE IF NOT EXISTS `trnProductColor` (
    `trnProductColorId` int NOT NULL AUTO_INCREMENT,
    `mstProductId` int NOT NULL,
    `color` longtext CHARACTER SET utf8mb4 NOT NULL,
    `colorHex` longtext CHARACTER SET utf8mb4 NOT NULL,
    `colorImagePath` longtext CHARACTER SET utf8mb4 NOT NULL,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdBy` int NOT NULL,
    `createdDate` datetime(6) NOT NULL,
    `updatedBy` int NULL,
    `updatedDate` datetime(6) NULL,
    `deletedBy` int NULL,
    `deletedDate` datetime(6) NULL,
    CONSTRAINT `PK_trnProductColor` PRIMARY KEY (`trnProductColorId`),
    CONSTRAINT `FK_trnProductColor_mstProduct_mstProductId` FOREIGN KEY (`mstProductId`) REFERENCES `mstProduct` (`mstProductId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 8. trnProductSize
-- ========================================================
CREATE TABLE IF NOT EXISTS `trnProductSize` (
    `trnProductSizeId` int NOT NULL AUTO_INCREMENT,
    `mstProductId` int NOT NULL,
    `trnProductColorId` int NOT NULL,
    `size` longtext CHARACTER SET utf8mb4 NOT NULL,
    `quantityAvailable` int NOT NULL DEFAULT 0,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdBy` int NOT NULL,
    `createdDate` datetime(6) NOT NULL,
    `updatedBy` int NULL,
    `updatedDate` datetime(6) NULL,
    `deletedBy` int NULL,
    `deletedDate` datetime(6) NULL,
    CONSTRAINT `PK_trnProductSize` PRIMARY KEY (`trnProductSizeId`),
    CONSTRAINT `FK_trnProductSize_mstProduct_mstProductId` FOREIGN KEY (`mstProductId`) REFERENCES `mstProduct` (`mstProductId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 9. trnProductSpecification
-- ========================================================
CREATE TABLE IF NOT EXISTS `trnProductSpecification` (
    `trnProductSpecificationId` int NOT NULL AUTO_INCREMENT,
    `mstProductId` int NOT NULL,
    `title` longtext CHARACTER SET utf8mb4 NOT NULL,
    `value` longtext CHARACTER SET utf8mb4 NOT NULL,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdBy` int NOT NULL,
    `createdDate` datetime(6) NOT NULL,
    `updatedBy` int NULL,
    `updatedDate` datetime(6) NULL,
    `deletedBy` int NULL,
    `deletedDate` datetime(6) NULL,
    CONSTRAINT `PK_trnProductSpecification` PRIMARY KEY (`trnProductSpecificationId`),
    CONSTRAINT `FK_trnProductSpecification_mstProduct_mstProductId` FOREIGN KEY (`mstProductId`) REFERENCES `mstProduct` (`mstProductId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 10. trnProductTags
-- ========================================================
CREATE TABLE IF NOT EXISTS `trnProductTags` (
    `trnProductTagsId` int NOT NULL AUTO_INCREMENT,
    `mstProductId` int NOT NULL,
    `tag` longtext CHARACTER SET utf8mb4 NOT NULL,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdBy` int NOT NULL,
    `createdDate` datetime(6) NOT NULL,
    `updatedBy` int NULL,
    `updatedDate` datetime(6) NULL,
    `deletedBy` int NULL,
    `deletedDate` datetime(6) NULL,
    CONSTRAINT `PK_trnProductTags` PRIMARY KEY (`trnProductTagsId`),
    CONSTRAINT `FK_trnProductTags_mstProduct_mstProductId` FOREIGN KEY (`mstProductId`) REFERENCES `mstProduct` (`mstProductId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 11. trnCart
-- ========================================================
CREATE TABLE IF NOT EXISTS `trnCart` (
    `trnCartId` int NOT NULL AUTO_INCREMENT,
    `mstUserId` int NOT NULL,
    `mstProductId` int NOT NULL,
    `size` longtext CHARACTER SET utf8mb4 NOT NULL,
    `quantity` int NOT NULL,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdBy` int NOT NULL,
    `createdDate` datetime(6) NOT NULL,
    `updatedBy` int NULL,
    `updatedDate` datetime(6) NULL,
    `deletedBy` int NULL,
    `deletedDate` datetime(6) NULL,
    CONSTRAINT `PK_trnCart` PRIMARY KEY (`trnCartId`),
    CONSTRAINT `FK_trnCart_mstProduct_mstProductId` FOREIGN KEY (`mstProductId`) REFERENCES `mstProduct` (`mstProductId`) ON DELETE CASCADE,
    CONSTRAINT `FK_trnCart_mstUser_mstUserId` FOREIGN KEY (`mstUserId`) REFERENCES `mstUser` (`mstUserId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 12. trnOrder
-- ========================================================
CREATE TABLE IF NOT EXISTS `trnOrder` (
    `trnOrderId` int NOT NULL AUTO_INCREMENT,
    `mstUserId` int NOT NULL,
    `orderNumber` longtext CHARACTER SET utf8mb4 NOT NULL,
    `orderDate` datetime(6) NOT NULL,
    `totalAmount` int NOT NULL,
    `orderStatus` int NOT NULL,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdBy` int NOT NULL,
    `createdDate` datetime(6) NOT NULL,
    `updatedBy` int NULL,
    `updatedDate` datetime(6) NULL,
    `adminNotes` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_trnOrder` PRIMARY KEY (`trnOrderId`),
    CONSTRAINT `FK_trnOrder_mstUser_mstUserId` FOREIGN KEY (`mstUserId`) REFERENCES `mstUser` (`mstUserId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 13. trnWishlist
-- ========================================================
CREATE TABLE IF NOT EXISTS `trnWishlist` (
    `trnWishlistId` int NOT NULL AUTO_INCREMENT,
    `mstUserId` int NOT NULL,
    `mstProductId` int NOT NULL,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdBy` int NOT NULL,
    `createdDate` datetime(6) NOT NULL,
    `updatedBy` int NULL,
    `updatedDate` datetime(6) NULL,
    `deletedBy` int NULL,
    `deletedDate` datetime(6) NULL,
    CONSTRAINT `PK_trnWishlist` PRIMARY KEY (`trnWishlistId`),
    CONSTRAINT `FK_trnWishlist_mstProduct_mstProductId` FOREIGN KEY (`mstProductId`) REFERENCES `mstProduct` (`mstProductId`) ON DELETE CASCADE,
    CONSTRAINT `FK_trnWishlist_mstUser_mstUserId` FOREIGN KEY (`mstUserId`) REFERENCES `mstUser` (`mstUserId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

-- ========================================================
-- 14. trnOrderItems
-- ========================================================
CREATE TABLE IF NOT EXISTS `trnOrderItems` (
    `trnOrderItemsId` int NOT NULL AUTO_INCREMENT,
    `trnOrderId` int NOT NULL,
    `mstProductId` int NOT NULL,
    `quantity` int NOT NULL,
    `size` longtext CHARACTER SET utf8mb4 NOT NULL,
    `price` int NOT NULL,
    `isDelivered` tinyint(1) NOT NULL DEFAULT 0,
    `deliveredDate` datetime(6) NULL,
    `isActive` tinyint(1) NOT NULL DEFAULT 1,
    `createdBy` int NOT NULL,
    `createdDate` datetime(6) NOT NULL,
    `updatedBy` int NULL,
    `updatedDate` datetime(6) NULL,
    CONSTRAINT `PK_trnOrderItems` PRIMARY KEY (`trnOrderItemsId`),
    CONSTRAINT `FK_trnOrderItems_mstProduct_mstProductId` FOREIGN KEY (`mstProductId`) REFERENCES `mstProduct` (`mstProductId`) ON DELETE CASCADE,
    CONSTRAINT `FK_trnOrderItems_trnOrder_trnOrderId` FOREIGN KEY (`trnOrderId`) REFERENCES `trnOrder` (`trnOrderId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

-- ========================================================
-- INDEXES
-- ========================================================
CREATE INDEX `IX_mstProductSubCategory_mstProductMainCategoryId` ON `mstProductSubCategory` (`mstProductMainCategoryId`);
CREATE INDEX `IX_mstProduct_mstProductMainCategoryId` ON `mstProduct` (`mstProductMainCategoryId`);
CREATE INDEX `IX_mstProduct_mstProductSubCategoryId` ON `mstProduct` (`mstProductSubCategoryId`);
CREATE INDEX `IX_trnUserAddress_mstUserId` ON `trnUserAddress` (`mstUserId`);
CREATE INDEX `IX_trnCart_mstProductId` ON `trnCart` (`mstProductId`);
CREATE INDEX `IX_trnCart_mstUserId` ON `trnCart` (`mstUserId`);
CREATE INDEX `IX_trnOrder_mstUserId` ON `trnOrder` (`mstUserId`);
CREATE INDEX `IX_trnOrderItems_mstProductId` ON `trnOrderItems` (`mstProductId`);
CREATE INDEX `IX_trnOrderItems_trnOrderId` ON `trnOrderItems` (`trnOrderId`);
CREATE INDEX `IX_trnProductColor_mstProductId` ON `trnProductColor` (`mstProductId`);
CREATE INDEX `IX_trnProductSize_mstProductId` ON `trnProductSize` (`mstProductId`);
CREATE INDEX `IX_trnProductSize_trnProductColorId` ON `trnProductSize` (`trnProductColorId`);
CREATE INDEX `IX_trnProductSpecification_mstProductId` ON `trnProductSpecification` (`mstProductId`);
CREATE INDEX `IX_trnProductTags_mstProductId` ON `trnProductTags` (`mstProductId`);
CREATE INDEX `IX_trnWishlist_mstProductId` ON `trnWishlist` (`mstProductId`);
CREATE INDEX `IX_trnWishlist_mstUserId` ON `trnWishlist` (`mstUserId`);
