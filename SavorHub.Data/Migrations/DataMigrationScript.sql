-- Script to handle existing Review data before adding MenuItemId foreign key
-- Run this BEFORE applying the migration if you want to preserve existing reviews

-- Option 1: Delete all existing reviews (simplest)
DELETE FROM Reviews;

-- Option 2: Assign existing reviews to the first menu item (if you want to keep them)
-- First, get a valid MenuItemId
-- DECLARE @FirstMenuItemId INT = (SELECT TOP 1 Id FROM MenuItem ORDER BY Id);
-- UPDATE Reviews SET MenuItemId = @FirstMenuItemId WHERE MenuItemId = 0 OR MenuItemId IS NULL;
