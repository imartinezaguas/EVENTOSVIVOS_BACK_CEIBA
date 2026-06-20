# EventosVivos - API Backend

EventosVivos es una plataforma para la gestión de reservas de eventos, conferencias y talleres. El objetivo de este backend es proveer un núcleo robusto y seguro para evitar sobreventa de tiquetes, conflictos de horarios en venues y ofrecer reglas de negocio claras sobre los tiempos de reserva y cancelación.

## 🏗 Arquitectura y Diseño de la Solución

El proyecto está construido bajo **.NET 9** utilizando los principios de **Clean Architecture** (Arquitectura Limpia) y enfoques tácticos de **Domain-Driven Design (DDD)**. Esto garantiza un bajo acoplamiento, alta cohesión y una estructura donde el negocio es el verdadero protagonista.

La solución se divide en las siguientes capas:
1. **Domain (`EventosVivos.Domain`)**: El corazón lógico (DDD). Utiliza **Modelos de Dominio Ricos** (*Rich Domain Models*), donde las entidades (Event, Booking) no son anémicas (solo getters/setters), sino que encapsulan métodos y validaciones de negocio (`ConfirmPayment`, `MarkAsCompleted`). También define las interfaces de repositorios y Excepciones de Dominio.
2. **Application (`EventosVivos.Application`)**: Coordina los flujos de negocio a través de **Servicios**. Implementa los DTOs (Data Transfer Objects) para evitar exponer la base de datos a la capa de presentación.
3. **Infrastructure (`EventosVivos.Infrastructure`)**: Implementa el patrón **Repository**. Se encarga de la comunicación directa con **PostgreSQL** mediante **Entity Framework Core**. Contiene configuraciones de mapeo y optimización de base de datos.
4. **Api (`EventosVivos.Api`)**: Punto de entrada de la aplicación. Contiene los Controladores RESTful. Se inyectan las dependencias, políticas CORS y manejo de middlewares.

## 💎 Calidad de Código y Principios
- **SOLID**: El código respeta principios de responsabilidad única y segregación de interfaces.
- **RESTful**: Uso estricto de verbos HTTP (`GET`, `POST`) y retornos adecuados como `201 Created` con encabezado `Location` y `400 Bad Request` en fallas de dominio.
- **Patrones de Diseño**: Uso extensivo de *Dependency Injection* y *Repository Pattern*.

## 🛡️ Manejo de Errores y Excepciones
El API no derrama _stack traces_ sensibles al frontend.
- Se implementó un **`GlobalExceptionHandlerMiddleware`** que atrapa errores de ejecución (Error 500) formateándolos limpiamente.
- Las validaciones de reglas de negocio arrojan una **`DomainException`**, que los controladores capturan y transforman en un código HTTP `400 Bad Request` con un mensaje claro y amigable al usuario.

## 🔐 Seguridad
- Configuración de políticas estrictas de **CORS** para aceptar orígenes únicamente del frontend.
- Protección inherente contra Inyección SQL mediante el uso de **EF Core** y consultas parametrizadas.
- Ocultamiento de la arquitectura de la base de datos mediante **DTOs**.

## 🧪 Cobertura de Pruebas (>80%)
El proyecto incluye `EventosVivos.Tests` utilizando **xUnit** y **Moq**.
- Las capas `Domain` y `Application` cuentan con una **cobertura que roza el 80%**, evaluando todas las restricciones críticas de negocio (fechas de fin de semana, límites de precio, cálculo de penalidades por cancelación tardía, entre otros).

## 🚀 Instalación y Ejecución

1. **Prerrequisitos**:
   - .NET 9 SDK
   - PostgreSQL
2. **Configuración de Base de Datos**:
   Ajusta la cadena de conexión en `EventosVivos.Api/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=eventosvivos_db;Username=postgres;Password=tu_password"
   }
   ```
3. **Migraciones**:
   Para crear las tablas en la base de datos, ejecuta desde el directorio de la API:
   ```bash
   dotnet ef database update
   ```
4. **Ejecutar API**:
   ```bash
   dotnet run --project EventosVivos.Api
   ```
5. **Ejecutar Pruebas**:
   ```bash
   dotnet test
   ```
