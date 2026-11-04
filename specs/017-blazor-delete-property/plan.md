# Plan de Implementación: 017-blazor-delete-property

**Branch**: `feat/017-blazor-delete-property` | **Date**: 2026-11-04 | **Spec**: `specs/017-blazor-delete-property/spec.md`

**Input**: Especificación funcional desde `specs/017-blazor-delete-property/spec.md`

## Resumen

Agregar la eliminación definitiva de propiedades: un nuevo slice `DeleteProperty` en `RealtorApi` (`DELETE /api/properties/{id}` → 204/400/404) y, en `RealtorWeb`, un diálogo de confirmación reutilizable integrado en la tarjeta del listado y en la página de detalle, con avisos de resultado, recarga paginada y navegación al listado.

## Contexto Técnico

**Language/Version**: C# con .NET 11 (`net11.0`)

**Primary Dependencies**: ASP.NET Core Minimal APIs, EF Core + Npgsql, FluentValidation, Blazor Web App (Interactive Server), Refit + `Refit.HttpClientFactory`, Lucide.Blazor

**Storage**: PostgreSQL; eliminación física sin cambios de esquema ni migraciones

**Testing**: xUnit en `app/backend/tests/RealtorApiTests` (validador, handler, endpoint, drift OpenAPI); validación manual guiada del frontend

**Target Platform**: ASP.NET Core en servidor + navegadores web

**Project Type**: Aplicación web (backend + frontend en la misma solución)

**Performance Goals**: Una única solicitud `DELETE` por confirmación; una consulta + un `DELETE` SQL por operación

**Constraints**:
- `400` (no `404`) para identificadores mal formados.
- Contrato OpenAPI publicado en `wwwroot/openapi/v1.json` sin drift.
- Estilos solo en `wwwroot/app.css`; íconos Lucide.
- No modificar el recurso de imagen.

**Scale/Scope**: 1 slice backend nuevo, 1 método Refit, 1 componente de diálogo, 1 servicio de avisos, cambios en `PropertyCard`, `PropertyList` y `PropertyDetail`

## Constitution Check

*GATE: Debe aprobar antes de Fase 0 (investigación). Revalidar tras Fase 1 (diseño).*

### Pre-Fase 0

- **Principio I - Solución única**: PASS. Extiende `RealtorApi` y `RealtorWeb` dentro de `app/Realtor.sln`.
- **Principio II - Spec-Driven**: PASS. `spec.md` existe y fue clarificada; el plan solo cubre RF-01 a RF-16.
- **Principio III - Arquitectura canónica**: PASS. Slice vertical `Features/Properties/DeleteProperty` con `ISlice` e `IHandler`; sin controllers; frontend en Razor Components.
- **Principio IV - Stack obligatorio**: PASS. Sin dependencias nuevas.
- **Principio V - Dominio y contratos**: PASS. Validación con FluentValidation, errores como `ProblemDetails`, logging estructurado; lógica fuera de componentes UI (el diálogo solo orquesta la llamada).
- **Principio VI - Idioma**: PASS. Artefactos en español.
- **Principio IX - Verificabilidad**: PASS. Sin aclaraciones pendientes.

### Post-Fase 1

- **Revalidación**: PASS. `research.md`, `data-model.md`, `contracts/` y `quickstart.md` no introducen alcance fuera de la spec ni migraciones. Las pruebas de endpoint reutilizan el patrón ya aprobado (`GetPropertyByIdEndpointTests`).

## Estructura del Proyecto

### Documentación (esta feature)

```text
specs/017-blazor-delete-property/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── delete-property.openapi.yaml
│   └── frontend-property-delete.contract.md
└── tasks.md            (speckit.tasks)
```

### Código fuente (raíz del repositorio)

```text
app/
├── backend/
│   ├── src/RealtorApi/
│   │   ├── Features/Properties/DeleteProperty/
│   │   │   ├── DeletePropertyCommand.cs
│   │   │   ├── DeletePropertyValidator.cs
│   │   │   ├── DeletePropertyHandler.cs
│   │   │   └── DeletePropertySlice.cs
│   │   └── wwwroot/openapi/v1.json          (regenerado)
│   └── tests/RealtorApiTests/
│       ├── Features/Properties/DeleteProperty/   (validador y handler)
│       └── Infrastructure/Endpoints/DeletePropertyEndpointTests.cs
└── frontend/src/RealtorWeb/
		├── Features/Properties/
		│   ├── Delete/
		│   │   ├── PropertyDeleteDialog.razor
		│   │   ├── PropertyDeleteDialog.razor.cs
		│   │   ├── PropertyNotice.cs
		│   │   └── PropertyNoticeService.cs
		│   ├── List/PropertyCard.razor           (acción Eliminar)
		│   ├── List/PropertyList.razor(.cs)      (diálogo, aviso, recarga)
		│   └── Detail/PropertyDetail.razor(.cs)  (diálogo, navegación)
		├── Services/Api/Properties/IPropertiesApi.cs   (DeletePropertyAsync)
		├── Program.cs                             (registro scoped de PropertyNoticeService)
		└── wwwroot/app.css                        (estilos de diálogo y aviso)
```

**Decisión de estructura**: Se mantiene la organización por features existente en ambos proyectos.

## Decisiones clave

Detalle completo en `research.md`.

| Tema | Decisión |
|---|---|
| Tipo de eliminación | Física (R-01) |
| Concurrencia | `DbUpdateConcurrencyException` → 404 (R-02) |
| Id inválido | Command + validador, 400 (R-03) |
| Imagen | No se modifica (R-04) |
| Confirmación | Componente modal reutilizable (R-07) |
| Avisos entre páginas | Servicio scoped de un solo uso (R-08) |
| Listado | Recarga de página actual; anterior si vacía (R-09) |

## Complejidad y Excepciones

No se identifican violaciones de constitución que requieran justificación.
