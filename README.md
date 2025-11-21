# 🏗️ Firmeza - Sistema de Gestión de Materiales de Construcción y Renta de Vehículos

Sistema administrativo completo para la venta de insumos de construcción y renta de vehículos industriales, desarrollado con **ASP.NET Core 8** siguiendo principios de **Clean Architecture**.

## 📋 Descripción del Proyecto

Firmeza es una solución empresarial que permite administrar operaciones de venta y renta en el sector de la construcción, incluyendo:

- 📦 Gestión de productos (materiales y herramientas)
- 👥 Administración de clientes
- 💰 Sistema de ventas con generación de comprobantes PDF
- 🚜 Renta de vehículos industriales
- 📊 Dashboard administrativo con métricas
- 📄 Importación/Exportación masiva vía Excel
- ✉️ Notificaciones por correo electrónico

## 🏗️ Arquitectura

El proyecto sigue **Clean Architecture** con separación clara de responsabilidades:

```
Firmeza/
├── src/
│   ├── Core/
│   │   ├── Firmeza.Domain/              # Entidades, Enums, Interfaces
│   │   └── Firmeza.Application/         # Casos de uso, DTOs, Servicios
│   ├── Infrastructure/
│   │   └── Firmeza.Infrastructure/      # EF Core, Repositorios, Servicios externos
│   └── Presentation/
│       ├── Firmeza.Web.Mvc/            # Panel Admin (Razor MVC)
│       ├── Firmeza.Web.Api/            # API RESTful
│       └── Firmeza.Web.Client/         # Frontend SPA (Futuro)
└── tests/
    └── Firmeza.Tests/                  # Pruebas unitarias (xUnit)
```

## 🛠️ Stack Tecnológico

### Backend
- **Framework:** ASP.NET Core 8.0
- **ORM:** Entity Framework Core 8.0
- **Base de datos:** PostgreSQL 15+
- **Autenticación:** ASP.NET Core Identity + JWT
- **Validación:** FluentValidation 11.0
- **Mapeo:** AutoMapper 13.0

### Servicios
- **Excel:** EPPlus 7.5
- **PDF:** QuestPDF 2024.12
- **Email:** MailKit 4.14

### Testing
- **Framework:** xUnit
- **Mocking:** Moq

### DevOps
- **Containerización:** Docker + Docker Compose
- **CI/CD:** (Pendiente)

## 📊 Modelo de Datos

### Entidades Principales

1. **Category** - Categorías de productos
2. **Product** - Materiales y herramientas de construcción
3. **Customer** - Clientes del sistema
4. **Sale** - Ventas de productos
5. **SaleDetail** - Líneas de detalle de ventas
6. **Vehicle** - Vehículos industriales para renta
7. **VehicleRental** - Transacciones de alquiler

## 🚀 Configuración Inicial

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/)

### Instalación

1. **Clonar el repositorio**
```bash
git clone https://github.com/tu-usuario/firmeza.git
cd firmeza
```

2. **Configurar la base de datos**

Edita `src/Presentation/Firmeza.Web.Mvc/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=FirmezaDB;Username=postgres;Password=TU_PASSWORD"
  }
}
```

3. **Aplicar migraciones**
```bash
cd src/Presentation/Firmeza.Web.Mvc
dotnet ef database update --project ../../Infrastructure/Firmeza.Infrastructure
```

4. **Ejecutar el proyecto**
```bash
dotnet run
```

La aplicación estará disponible en: `https://localhost:5001`

## 📦 Paquetes NuGet Principales

```xml
<!-- Domain Layer -->
<PackageReference Include="AutoMapper" Version="13.0.1" />
<PackageReference Include="FluentValidation" Version="11.11.0" />

<!-- Infrastructure Layer -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.11" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.11" />
<PackageReference Include="EPPlus" Version="7.5.2" />
<PackageReference Include="QuestPDF" Version="2024.12.3" />
<PackageReference Include="MailKit" Version="4.14.1" />

<!-- API Layer -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.11" />
```

## 🔧 Comandos Útiles

### Entity Framework

```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion --project src/Infrastructure/Firmeza.Infrastructure --startup-project src/Presentation/Firmeza.Web.Mvc

# Aplicar migraciones
dotnet ef database update --project src/Infrastructure/Firmeza.Infrastructure --startup-project src/Presentation/Firmeza.Web.Mvc

# Revertir migración
dotnet ef migrations remove --project src/Infrastructure/Firmeza.Infrastructure --startup-project src/Presentation/Firmeza.Web.Mvc

# Generar script SQL
dotnet ef migrations script --project src/Infrastructure/Firmeza.Infrastructure --startup-project src/Presentation/Firmeza.Web.Mvc
```

### Build & Test

```bash
# Compilar solución completa
dotnet build

# Ejecutar pruebas
dotnet test

# Ejecutar con hot reload
dotnet watch run --project src/Presentation/Firmeza.Web.Mvc
```

## 📝 Funcionalidades Principales

### Módulo Administrativo (MVC)
- ✅ Dashboard con métricas
- ✅ CRUD de Productos
- ✅ CRUD de Clientes  
- ✅ Gestión de Ventas
- ✅ Importación masiva desde Excel (datos desnormalizados)
- ✅ Exportación a Excel/PDF
- ✅ Generación automática de recibos PDF

### API RESTful
- ✅ Endpoints CRUD completos
- ✅ Autenticación JWT
- ✅ Documentación Swagger
- ✅ DTOs y AutoMapper
- ✅ Validaciones con FluentValidation

### Sistema de Roles
- **Admin:** Acceso completo al panel MVC
- **Cliente:** Acceso solo a API y futura SPA

## 🔐 Autenticación y Seguridad

- **Identity:** Gestión de usuarios y roles
- **JWT:** Tokens para API
- **Cookies:** Sesiones en MVC
- **Policies:** Control de acceso basado en roles

## 📧 Configuración de Email (SMTP)

Edita `appsettings.json`:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderName": "Firmeza",
    "SenderEmail": "tu-email@gmail.com",
    "Password": "tu-app-password"
  }
}
```

**Nota:** Para Gmail, usa una [App Password](https://support.google.com/accounts/answer/185833).

## 🐳 Docker

```bash
# Build imagen
docker build -t firmeza:latest .

# Ejecutar con docker-compose
docker-compose up -d
```

## 🧪 Testing

```bash
# Ejecutar todas las pruebas
dotnet test

# Con cobertura
dotnet test /p:CollectCoverage=true
```

## 📚 Documentación Adicional

- [Diagrama ER](docs/ER-Diagram.png)
- [Diagrama de Clases](docs/Class-Diagram.png)
- [API Documentation](docs/API-Documentation.md)

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto es privado y está desarrollado como parte del curso de ASP.NET Core.

## 👤 Autor

**Tu Nombre**
- Email: tu-email@example.com
- GitHub: [@tu-usuario](https://github.com/tu-usuario)

## 🎯 Estado del Proyecto

- [x] Fundamentos (Domain, Infrastructure)
- [x] Migraciones de base de datos
- [x] Configuración de Identity
- [ ] Módulo Admin MVC
- [ ] API RESTful
- [ ] Frontend SPA
- [ ] Pruebas unitarias
- [ ] Documentación completa
- [ ] Docker deployment

---

**Firmeza** - Sistema de gestión empresarial para el sector de la construcción 🏗️
