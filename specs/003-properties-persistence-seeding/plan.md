# Plan de Implementación: Persistencia y Seeding Inicial de Propiedades

**Branch**: `feat/003-properties-persistence-seeding` | **Fecha**: 2026-07-09 | **Spec**: `specs/003-properties-persistence-seeding/spec.md`

**Entrada**: Especificación funcional desde `specs/003-properties-persistence-seeding/spec.md`

## Resumen

Implementar persistencia inicial para `Property` y `PropertyStatus` en `RealtorApi` con EF Core + PostgreSQL, agregando configuraciones `IEntityTypeConfiguration<T>`, una única migración funcional, seeding idempotente nativo (`UseSeeding` + `UseAsyncSeeding`) y resolución centralizada de fuentes externas mediante `support/seed-data/seed-manifest.json`. El flujo debe garantizar `app.MigrateAsync()` antes de `app.Run()`, fail-fast por faltantes críticos y persistencia de `ImageUrl` con ruta pública servible por la API.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET SDK `11.0.100-preview.5.26302.115` (`net11.0`)

**Dependencias Primarias**: ASP.NET Core Minimal APIs, EF Core 11, `Npgsql.EntityFrameworkCore.PostgreSQL`, FluentValidation

**Almacenamiento**: PostgreSQL (relacional) + archivos JSON e imágenes de seed bajo `support/seed-data`

**Testing**: xUnit + `Microsoft.AspNetCore.Mvc.Testing`; ejecución con `dotnet test` en `app/backend/tests/RealtorApiTests`

**Plataforma Objetivo**: Backend ASP.NET Core ejecutable en entorno local y CI (Windows/Linux)

**Tipo de Proyecto**: Aplicación web backend en `app/backend/src/RealtorApi` con proyecto de pruebas dedicado

**Objetivos de Rendimiento**: Arranque consistente sin duplicación de seed, y costo de inicialización estable en reinicios consecutivos

**Restricciones**: Sin controllers, sin `HasData`, sin seeder manual fuera de `UseSeeding/UseAsyncSeeding`, sin condicionar `MigrateAsync`, una sola migración coherente por spec

**Escala/Alcance**: Inicialización de catálogo de 3 estados y dataset base de propiedades desde archivos externos, con trazabilidad para futuras consultas con imagen

## Constitution Check

*GATE: Debe aprobar antes de Fase 0. Revalidar tras Fase 1.*

### Validación Previa (antes de Fase 0)

- Principio I (solución única): PASS. Cambios circunscritos al backend existente sin crear soluciones paralelas.
- Principio II (spec-driven): PASS. Alcance limitado a FR-001..FR-011 y clarificaciones de la spec 003.
- Principio III (arquitectura): PASS. Se mantiene Minimal APIs + Vertical Slice; persistencia bajo `Infrastructure/Persistence`.
- Principio IV (stack obligatorio): PASS. Se preserva .NET 11, EF Core + Npgsql + PostgreSQL.
- Principio V (calidad y contratos): PASS. Se define validación de persistencia, seeding y contrato de ruta pública de imágenes.
- Principio VI (idioma): PASS. Artefactos markdown en español.
- Principio VII (gobernanza): PASS. Sin divergencias de rutas canónicas ni cambios fuera del flujo Speckit.

Resultado de gate previo: **APROBADO**.

### Revalidación (después de Fase 1)

- `research.md` resuelve todas las decisiones de alto impacto (upsert, fail-fast y manejo de imágenes faltantes).
- `data-model.md` define entidades, reglas, relaciones y manifiesto de seed sin contradicciones con la spec.
- `contracts/backend-properties-persistence.openapi.yaml` documenta interfaz pública relevante (`/health` y convención `/assets/properties/{fileName}`).
- `quickstart.md` documenta validaciones ejecutables para migración, seeding, idempotencia y assets.

Resultado de gate posterior: **APROBADO**.

## Estructura del Proyecto

### Documentación (feature actual)

```text
specs/003-properties-persistence-seeding/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── backend-properties-persistence.openapi.yaml
└── tasks.md
```

### Código Fuente (raíz del repositorio)

```text
app/
├── Realtor.sln
├── backend/
│   ├── src/
│   │   └── RealtorApi/
│   │       ├── Program.cs
│   │       ├── RealtorApi.csproj
│   │       ├── Domain/
│   │       │   └── Properties/
│   │       └── Infrastructure/
│   │           ├── Persistence/
│   │           │   ├── AppDbContext.cs
│   │           │   ├── DatabaseSeeder.cs
│   │           │   ├── MigrationExtensions.cs
│   │           │   └── Configurations/
│   │           └── Endpoints/
│   └── tests/
│       └── RealtorApiTests/
│           └── Infrastructure/
└── frontend/
    └── ...

support/
└── seed-data/
    ├── seed-manifest.json
    ├── properties.json
    ├── properties-statuses.json
    └── images/properties/
```

**Decisión de estructura**: Se mantiene arquitectura web existente y se incorpora persistencia en `app/backend/src/RealtorApi/Infrastructure/Persistence` con dominio en `Domain/Properties`, preservando que endpoints de negocio continúen bajo `Features/Properties` en specs posteriores.

## Fase 0: Investigación

### Tareas de investigación ejecutadas

1. Definir estrategia de migración + seeding automática alineada a `database.instructions.md`.
2. Determinar política de idempotencia y resolución de conflictos de registros existentes.
3. Determinar política operativa para faltantes críticos de manifiesto/JSON.
4. Definir tratamiento de imágenes faltantes por registro y su impacto funcional.
5. Establecer contrato de ruta pública para `ImageUrl` y sincronización de assets.

Resultado: decisiones consolidadas en `research.md`, sin `NEEDS CLARIFICATION` pendientes.

## Fase 1: Diseño y Contratos

### Entregables

1. `data-model.md` con entidades `PropertyStatus`, `Property` y `SeedManifest`, reglas y relaciones.
2. `contracts/backend-properties-persistence.openapi.yaml` con contrato de exposición relevante para salud e imágenes públicas.
3. `quickstart.md` con escenarios de validación ejecutable de migración, seeding y consistencia de assets.
4. Actualización del contexto de agente en `.github/copilot-instructions.md` para referenciar este plan.

## Seguimiento de Complejidad

Sin violaciones constitucionales que requieran justificación.

## Observaciones de implementación previstas

- Crear `AppDbContext` con `ApplyConfigurationsFromAssembly`.
- Agregar configuraciones `PropertyConfiguration` y `PropertyStatusConfiguration` en `Infrastructure/Persistence/Configurations`.
- Generar una única migración EF Core para todo el cambio funcional de la spec 003.
- Implementar `DatabaseSeeder` sync/async equivalentes con idempotencia y upsert.
- Integrar `support/seed-data` y manifiesto al `RealtorApi.csproj` para build/publish.
- Asegurar persistencia de `ImageUrl` en formato público servible por API.
