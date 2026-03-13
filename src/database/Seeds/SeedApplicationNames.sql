USE [UserManagement]
GO

-- Seed sample application names into ApplicationName table
INSERT INTO [dbo].[ApplicationName] (AppName, Description, CreatedAt) VALUES
('SAP_ERP',           'SAP Enterprise Resource Planning system',              GETDATE()),
('CRM_Portal',        'Customer Relationship Management portal',              GETDATE()),
('HR_Self_Service',   'Human Resources self-service application',             GETDATE()),
('Finance_Reporting', 'Finance and accounting reporting dashboard',           GETDATE()),
('IT_Helpdesk',       'IT support and helpdesk ticketing system',             GETDATE()),
('Payroll_System',    'Payroll processing and management system',             GETDATE()),
('Asset_Management',  'Corporate asset tracking and management application',  GETDATE()),
('Document_Manager',  'Document management and archiving system',             GETDATE());
GO
