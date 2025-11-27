-- Seed data for Firmeza database

-- Insert Categories
INSERT INTO "Categories" ("Name", "Description", "IsActive", "CreatedAt", "UpdatedAt")
VALUES 
    ('Materiales de Construcción', 'Cemento, arena, gravilla, etc.', true, NOW(), NOW()),
    ('Herramientas', 'Herramientas manuales y eléctricas', true, NOW(), NOW()),
    ('Ferretería', 'Tornillos, clavos, bisagras, etc.', true, NOW(), NOW()),
    ('Pintura', 'Pinturas, brochas, rodillos', true, NOW(), NOW()),
    ('Plomería', 'Tuberías, llaves, accesorios', true, NOW(), NOW())
ON CONFLICT DO NOTHING;

-- Insert Products
INSERT INTO "Products" ("Code", "Name", "Description", "Price", "Stock", "Type", "CategoryId", "IsActive", "CreatedAt", "UpdatedAt")
SELECT 
    'PROD001', 'Cemento Portland 50kg', 'Cemento gris de alta resistencia', 25000, 100, 0,
    (SELECT "Id" FROM "Categories" WHERE "Name" = 'Materiales de Construcción' LIMIT 1),
    true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Products" WHERE "Code" = 'PROD001')
UNION ALL
SELECT 
    'PROD002', 'Arena Lavada m3', 'Arena lavada para construcción', 45000, 50, 0,
    (SELECT "Id" FROM "Categories" WHERE "Name" = 'Materiales de Construcción' LIMIT 1),
    true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Products" WHERE "Code" = 'PROD002')
UNION ALL
SELECT 
    'PROD003', 'Ladrillo Tolete x 1000', 'Ladrillo tolete de arcilla', 850000, 20, 0,
    (SELECT "Id" FROM "Categories" WHERE "Name" = 'Materiales de Construcción' LIMIT 1),
    true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Products" WHERE "Code" = 'PROD003')
UNION ALL
SELECT 
    'PROD004', 'Taladro Percutor 850W', 'Taladro eléctrico profesional', 320000, 15, 1,
    (SELECT "Id" FROM "Categories" WHERE "Name" = 'Herramientas' LIMIT 1),
    true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Products" WHERE "Code" = 'PROD004')
UNION ALL
SELECT 
    'PROD005', 'Pala Cuadrada', 'Pala de acero con mango de madera', 45000, 30, 1,
    (SELECT "Id" FROM "Categories" WHERE "Name" = 'Herramientas' LIMIT 1),
    true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Products" WHERE "Code" = 'PROD005')
UNION ALL
SELECT 
    'PROD006', 'Pintura Vinilo Blanco 1Gal', 'Pintura vinilo para interiores', 35000, 40, 0,
    (SELECT "Id" FROM "Categories" WHERE "Name" = 'Pintura' LIMIT 1),
    true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Products" WHERE "Code" = 'PROD006')
UNION ALL
SELECT 
    'PROD007', 'Llave de Paso 1/2"', 'Llave de paso de bronce', 18000, 25, 0,
    (SELECT "Id" FROM "Categories" WHERE "Name" = 'Plomería' LIMIT 1),
    true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Products" WHERE "Code" = 'PROD007')
UNION ALL
SELECT 
    'PROD008', 'Tornillos 2" x 100 unidades', 'Tornillos para madera', 12000, 60, 0,
    (SELECT "Id" FROM "Categories" WHERE "Name" = 'Ferretería' LIMIT 1),
    true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Products" WHERE "Code" = 'PROD008')
UNION ALL
SELECT 
    'PROD009', 'Nivel Láser', 'Nivel láser profesional', 450000, 8, 1,
    (SELECT "Id" FROM "Categories" WHERE "Name" = 'Herramientas' LIMIT 1),
    true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Products" WHERE "Code" = 'PROD009')
UNION ALL
SELECT 
    'PROD010', 'Escalera Aluminio 6m', 'Escalera extensible de aluminio', 280000, 12, 1,
    (SELECT "Id" FROM "Categories" WHERE "Name" = 'Herramientas' LIMIT 1),
    true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Products" WHERE "Code" = 'PROD010');

-- Insert Customers
INSERT INTO "Customers" ("FirstName", "LastName", "DocumentType", "DocumentNumber", "Email", "Phone", "Address", "City", "IsActive", "CreatedAt", "UpdatedAt")
SELECT 'Juan', 'Pérez', 'CC', '1234567890', 'juan.perez@email.com', '3001234567', 'Calle 123 #45-67', 'Bogotá', true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Customers" WHERE "DocumentNumber" = '1234567890')
UNION ALL
SELECT 'María', 'García', 'CC', '9876543210', 'maria.garcia@email.com', '3109876543', 'Carrera 45 #12-34', 'Medellín', true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Customers" WHERE "DocumentNumber" = '9876543210')
UNION ALL
SELECT 'Carlos', 'López', 'CC', '5555555555', 'carlos.lopez@email.com', '3205555555', 'Avenida 68 #23-45', 'Cali', true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Customers" WHERE "DocumentNumber" = '5555555555')
UNION ALL
SELECT 'Ana', 'Martínez', 'CC', '1111111111', 'ana.martinez@email.com', '3151111111', 'Calle 50 #30-20', 'Barranquilla', true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Customers" WHERE "DocumentNumber" = '1111111111')
UNION ALL
SELECT 'Luis', 'Rodríguez', 'CC', '2222222222', 'luis.rodriguez@email.com', '3162222222', 'Carrera 7 #80-90', 'Bogotá', true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "Customers" WHERE "DocumentNumber" = '2222222222');

-- Confirmation message
DO $$
BEGIN
    RAISE NOTICE 'Test data loaded successfully!';
    RAISE NOTICE 'Categories: %', (SELECT COUNT(*) FROM "Categories");
    RAISE NOTICE 'Products: %', (SELECT COUNT(*) FROM "Products");
    RAISE NOTICE 'Customers: %', (SELECT COUNT(*) FROM "Customers");
END $$;
