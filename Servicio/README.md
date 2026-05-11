# Módulo Servicio — Pooking API

Catálogo general de servicios integrables en la plataforma Pooking.

## Responsable
Módulo: **Servicio**  
Equipo: Pooking

## Arquitectura

```
Microservicio.Pooking.Servicio.Api          ← Controladores, middleware, Swagger
Microservicio.Pooking.Servicio.Business     ← Lógica de negocio, DTOs, validadores
Microservicio.Pooking.Servicio.DataManagement ← Servicios de datos, UnitOfWork
Microservicio.Pooking.Servicio.DataAcces    ← Entidades, DbContext, repositorios
```

## Endpoints principales

| Verbo  | Ruta                                  | Descripción                      |
|--------|---------------------------------------|----------------------------------|
| GET    | /api/v1/tipos-servicio/activos        | Lista tipos activos              |
| GET    | /api/v1/tipos-servicio/{guid}         | Obtiene tipo por GUID            |
| POST   | /api/v1/tipos-servicio               | Crea tipo de servicio            |
| PUT    | /api/v1/tipos-servicio/{guid}         | Actualiza tipo                   |
| DELETE | /api/v1/tipos-servicio/{guid}         | Eliminación lógica               |
| GET    | /api/v1/servicios                     | Lista/busca servicios paginados  |
| GET    | /api/v1/servicios/{guid}              | Obtiene servicio por GUID        |
| GET    | /api/v1/servicios/{guid}/detalle      | Detalle completo del servicio    |
| POST   | /api/v1/servicios                     | Registra nuevo servicio          |
| PUT    | /api/v1/servicios/{guid}              | Actualiza servicio               |
| DELETE | /api/v1/servicios/{guid}              | Eliminación lógica               |

## Tipos de servicio válidos

- `Vuelos`
- `Alojamiento`
- `Atracciones`
- `Alquiler de Carros`

## Configuración local

1. Copiar `appsettings.example.json` → `appsettings.Development.json`
2. Reemplazar la connection string con los datos de tu BD PostgreSQL/Supabase
3. El archivo `appsettings.Development.json` está en `.gitignore` — no se sube al repo

## Migrations

```bash
dotnet ef migrations add InitServicioModule \
  --project Microservicio.Pooking.Servicio.DataAcces \
  --startup-project Microservicio.Pooking.Servicio.Api

dotnet ef database update \
  --project Microservicio.Pooking.Servicio.DataAcces \
  --startup-project Microservicio.Pooking.Servicio.Api
```

## Swagger

Disponible en: `http://localhost:5100/swagger`
