# Skill: Desarrollo de Arquitectura Limpia y DDD en .NET

Esta "Skill" o instrucción de sistema define las reglas y lineamientos estrictos que el asistente IA debe seguir al generar o modificar código para este proyecto (o proyectos similares basados en esta arquitectura).

## 1. Patrones Arquitectónicos
- **Clean Architecture**: El código siempre debe dividirse en 4 capas estrictas mediante proyectos separados (`.csproj`):
  1. `Domain` (Núcleo)
  2. `Application` (Casos de Uso)
  3. `Infrastructure` (Acceso a Datos)
  4. `Api` (Presentación)
- **Domain-Driven Design (DDD)**: La lógica de negocio pesada debe residir en las Entidades del Dominio (`Entities`), no en servicios anémicos. Las validaciones de reglas de negocio siempre se lanzarán como `DomainException`.
- **Repository Pattern**: Todo acceso a datos debe abstraerse mediante interfaces en la capa de Dominio (`IRepository`) y su implementación estará estrictamente en la capa de Infraestructura (`Repository`).

## 2. Reglas de Dependencias
- `Domain`: **NO DEPENDE DE NADA**. No se pueden instalar paquetes como Entity Framework o ASP.NET en esta capa.
- `Application`: Depende únicamente de `Domain`. Aquí viven los DTOs y las Interfaces de los Servicios (`I...Service`).
- `Infrastructure`: Depende de `Application` y `Domain`. Aquí se instala EF Core, conexiones a PostgreSQL o RabbitMQ.
- `Api`: Depende de `Application` e `Infrastructure`. No puede tener lógica de negocio pesada. Solo orquesta peticiones usando Controladores RESTful.

## 3. Constantes y Magic Strings
- **Prohibido el uso de Magic Strings o Magic Numbers** en cualquier parte del código.
- Toda constante, tamaño máximo, mensaje de error, prefijo o restricción horaria DEBE estar centralizada en la clase estática `GlobalConstants` dentro del `Domain.Constants`.

## 4. Principios SOLID y Calidad
- **Single Responsibility Principle (SRP)**: Cada clase debe hacer una sola cosa. Los servicios de aplicación deben enfocarse en orquestar casos de uso específicos.
- **Dependency Inversion (DIP)**: Los controladores nunca instancian clases, todo entra por el constructor como interfaz.
- **Fail Fast**: El manejo de errores se hace lanzando excepciones específicas del dominio (`DomainException`) lo más rápido posible, y se interceptan de forma global usando un Middleware (`GlobalExceptionHandlerMiddleware`) en la API para retornar HTTP 400.

## 5. Pruebas Automatizadas
- Toda regla de negocio debe tener un test asociado usando **xUnit**.
- Se favorecen las Pruebas Unitarias de Dominio por su rapidez e independencia de frameworks.

---
**Instrucción Final para la IA**: Al interactuar con este proyecto, asume automáticamente estas reglas sin que el usuario te lo pida explícitamente.
