# Firmeza - Sistema de Gestión Empresarial

Sistema completo de gestión empresarial con panel de administración, API REST y frontend para clientes.

## 🚀 Inicio Rápido con Docker

### Requisitos Previos
- Docker Desktop instalado
- Puertos disponibles: 3001, 5100, 5242, 5433

### Levantar el Proyecto

```bash
# Clonar el repositorio
git clone <repository-url>
cd Firmeza

# Levantar todos los servicios
docker-compose up -d --build
```

Esto levantará automáticamente:
- ✅ Base de datos PostgreSQL (con migraciones aplicadas)
- ✅ API REST (.NET 8)
- ✅ Panel de Administración (ASP.NET MVC)
- ✅ Frontend Cliente (Next.js)
- ✅ Datos de prueba (categorías, productos, clientes)
- ✅ Usuario administrador

### Acceder a las Aplicaciones

| Servicio | URL | Credenciales |
|----------|-----|--------------|
| **Panel de Admin** | http://localhost:5100 | Email: `admin@firmeza.com`<br>Password: `Admin123!` |
| **Frontend Cliente** | http://localhost:3001 | Registrarse desde la interfaz |
| **API REST** | http://localhost:5242 | - |
| **PostgreSQL** | localhost:5433 | User: `postgres`<br>Password: `Qwe.123*` |

## 📦 Datos de Prueba

Al levantar el proyecto, se crean automáticamente:

- **Roles:** Admin, Customer
- **Usuario Admin:** admin@firmeza.com / Admin123!
- **5 Categorías:** Materiales de Construcción, Herramientas, Acabados, etc.
- **10+ Productos:** Con stock y precios
- **Clientes de prueba**

## 🛠️ Comandos Útiles

### Ver logs en tiempo real
```bash
docker-compose logs -f
```

### Ver logs de un servicio específico
```bash
docker-compose logs -f web-mvc    # Panel Admin
docker-compose logs -f web-api    # API
docker-compose logs -f client     # Frontend
```

### Detener servicios
```bash
docker-compose stop
```

### Reiniciar servicios
```bash
docker-compose restart
```

### Detener y eliminar contenedores (mantiene la base de datos)
```bash
docker-compose down
```

### Limpiar TODO (⚠️ elimina la base de datos)
```bash
docker-compose down -v
```

### Reconstruir y levantar
```bash
docker-compose down
docker-compose up -d --build
```

## 🏗️ Arquitectura

```
Firmeza/
├── src/
│   ├── Core/
│   │   ├── Firmeza.Domain/          # Entidades y enums
│   │   └── Firmeza.Application/     # DTOs e interfaces
│   ├── Infrastructure/
│   │   └── Firmeza.Infrastructure/  # EF Core, servicios
│   └── Presentation/
│       ├── Firmeza.Web.Api/         # API REST
│       └── Firmeza.Web.Mvc/         # Panel Admin
├── client/                          # Frontend Next.js
└── docker-compose.yml
```

## 🔧 Desarrollo Local (sin Docker)

### Requisitos
- .NET 8 SDK
- Node.js 20+
- PostgreSQL 16

### Base de Datos
```bash
# Crear base de datos
createdb -U postgres FirmezaDB

# Aplicar migraciones
cd src/Infrastructure/Firmeza.Infrastructure
dotnet ef database update --startup-project ../../Presentation/Firmeza.Web.Api
```

### API
```bash
cd src/Presentation/Firmeza.Web.Api
dotnet run
# Disponible en: http://localhost:5242
```

### Panel Admin
```bash
cd src/Presentation/Firmeza.Web.Mvc
dotnet run
# Disponible en: http://localhost:5100
```

### Frontend
```bash
cd client
npm install
npm run dev
# Disponible en: http://localhost:3001
```

## 📝 Funcionalidades Principales

### Panel de Administración
- ✅ Gestión de productos y categorías
- ✅ Gestión de clientes
- ✅ Registro de ventas
- ✅ Gestión de vehículos y alquileres
- ✅ Importación masiva desde Excel
- ✅ Generación de reportes PDF
- ✅ Soft delete con bloqueo de cuentas

### API REST
- ✅ Autenticación JWT
- ✅ Registro y login de usuarios
- ✅ CRUD de productos
- ✅ Procesamiento de ventas
- ✅ Generación de PDFs
- ✅ Envío de emails (bienvenida y confirmación de compra)
- ✅ Validación de cuentas activas

### Frontend Cliente
- ✅ Catálogo de productos
- ✅ Carrito de compras
- ✅ Registro e inicio de sesión
- ✅ Navbar dinámico con estado de sesión
- ✅ Proceso de checkout
- ✅ Confirmación por email con PDF

## 🔐 Seguridad

- Contraseñas hasheadas con ASP.NET Identity
- Tokens JWT para autenticación API
- Cookies seguras para panel admin
- Validación de cuentas activas en login
- Bloqueo automático de cuentas inactivas
- Protección CSRF en formularios

## 📧 Configuración de Email

El sistema envía emails automáticamente:
- Email de bienvenida al registrarse
- Confirmación de compra con PDF adjunto

Configurar en `appsettings.json`:
```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "Username": "tu-email@gmail.com",
  "Password": "tu-app-password",
  "SenderName": "Firmeza",
  "SenderEmail": "tu-email@gmail.com"
}
```

## 🧪 Testing

```bash
cd tests/Firmeza.Tests
dotnet test
```

## 📄 Licencia

Este proyecto es parte de un trabajo académico.

## 👥 Autor

Diego AG
