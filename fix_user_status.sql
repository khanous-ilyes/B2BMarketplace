-- Fix user statuses in the database
-- This script sets the correct status for all users

-- 1. Set Admin to Active (Status = 1)
UPDATE Users 
SET Status = 1 
WHERE UserType = 2; -- Admin

-- 2. Keep Suppliers as Pending (Status = 0) - they need validation
-- No change needed for suppliers

-- 3. Set Clients to Active (Status = 1) - they don't need validation
UPDATE Users 
SET Status = 1 
WHERE UserType = 1; -- Client

-- Verify the changes
SELECT Id, Email, UserType, Status, 
       CASE UserType 
           WHEN 0 THEN 'Supplier' 
           WHEN 1 THEN 'Client' 
           WHEN 2 THEN 'Admin' 
       END as UserTypeName,
       CASE Status 
           WHEN 0 THEN 'Pending' 
           WHEN 1 THEN 'Active' 
           WHEN 2 THEN 'Suspended' 
       END as StatusName
FROM Users;
