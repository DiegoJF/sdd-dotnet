# Plan de Implementación: Actualización de propiedades con imagen opcional

**Branch**: `feat/006-properties-update` | **Fecha**: 2026-07-11 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación funcional desde `specs/006-properties-update/spec.md`

## Resumen

Exponer `PUT /api/properties/{id}` como slice público de actualización en backend para editar propiedades existentes con imagen opcional. El flujo debe:
1. Buscar la propiedad por `id`; devolver `404 Not Found` si no existe.
2. Actualizar los campos editables: `title`, `description`, `address`, `price`, `status`.
3. Si se envía imagen válida (PNG/JPG <= 5 MB): almacenar con nombre único interno (`UUID + extensión`) y reemplazar `imageUrl`.
4. Si no se envía imagen: conservar el `imageUrl` existente sin modificación.
5. Operación atómica con imagen: fallo de guardado no persiste ningún cambio.
6. Retornar `200 OK` con el mismo shape de respuesta que el alta (005): `{ id, title, status, imageUrl }`.

Los errores siguen la misma semántica que spec 005 más el caso `404`.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET 11 (`net11.0`)

**Dependencias Primarias**: ASP.NET Core Minimal APIs, EF Core, Npgsql, FluentValidation, ProblemDetails

**Almacenamiento**: PostgreSQL para datos de propiedades; sistema de archivos en `wwwroot/assets/properties` para imágenes (reutilizando `CreatePropertyImageStorage` de spec 005)

**Pruebas**: xUnit en `app/backend/tests/RealtorApiTests`; validación manual HTTP mediante `RealtorApi.http`

**Plataforma Objetivo**: Backend ASP.NET Core ejecutado localmente y en CI (Windows/Linux)

**Tipo de Proyecto**: Aplicación web backend con arquitectura Vertical Slice

**Objetivos de Rendimiento**: En una muestra de 100 solicitudes válidas mixtas (con y sin imagen), el percentil 95 debe ser menor a 3 segundos

**Restricciones**:
- Sin controllers; sin registro manual de endpoints en `Program.cs`
- Mapping sin lógica de negocio; auto-registro por `IHandler` e `ISlice`
- Operación atómica ante fallo de guardado de imagen
- Los campos `bedroomCount`, `bathroomCount` y `areaSquareMeters` no son editables en esta operación
- Concurrencia: last-write-wins (sin ETag ni `rowVersion`)
- Endpoint público (sin autenticación), consistente con spec 005

**Alcance/Volumen**: Un endpoint PUT con escenarios: éxito sin imagen, éxito con imagen, `404`, `400`, `413`, `415` y `500`

## Revisión de Constitución

*PUERTA: Debe aprobar antes de la Fase 0. Revalidar tras la Fase 1.*

- **Principio I, solución única**: PASS. El alcance se integra en el backend existente sin soluciones paralelas.
- **Principio II, spec-driven**: PASS. El plan deriva exclusivamente de `spec.md` 006 y clarificaciones registradas en sesión 2026-07-11.
- **Principio III, arquitectura canónica**: PASS. Se mantiene Minimal API + Vertical Slice + endpoint delgado; auto-registro por `ISlice` e `IHandler`.
- **Principio IV, stack obligatorio**: PASS. Se conserva .NET 11, EF Core, Npgsql, FluentValidation y ProblemDetails.
- **Principio V, calidad de dominio y contratos**: PASS. Contrato OpenAPI definido; validación consistente con códigos de error explícitos; logging estructurado incluido en handler.
- **Principio VI, idioma y documentación**: PASS. Artefactos Markdown de la feature en español.
- **Principio VII, gobernanza y divergencias**: PASS. No hay cambios estructurales fuera del flujo Speckit.

Resultado de puerta previa: **APROBADO**.

## Estructura del Proyecto

### Documentación de la feature

```text
specs/006-properties-update/
├── plan.md              ← este archivo
├── research.md          ← Fase 0 completada
├── data-model.md        ← Fase 1 completada
├── quickstart.md        ← Fase 1 completada
├── contracts/
│   └── backend-properties-update.openapi.yaml   ← Fase 1 completada
├── checklists/
│   └── requirements.md
└── tasks.md             ← generado por /speckit.tasks (siguiente fase)
```

### Código fuente relevante

```text
app/backend/src/RealtorApi/
├── Program.cs                          ← sin cambios (auto-registro)
├── RealtorApi.http                     ← agregar request PUT de validación
├── Features/
│   └── Properties/
│       ├── CreateProperty/             ← existente; se reutiliza CreatePropertyImageStorage
│       └── UpdateProperty/             ← nuevo slice
│           ├── UpdatePropertySlice.cs
│           ├── UpdatePropertyContracts.cs
│           ├── UpdatePropertyCommand.cs
│           ├── UpdatePropertyHandler.cs
│           ├── UpdatePropertyMapping.cs
│           └── UpdatePropertyValidator.cs
└── wwwroot/
    └── assets/
        └── properties/                 ← sin cambios estructurales

app/backend/tests/RealtorApiTests/
└── Features/
    └── Properties/
        └── UpdateProperty/             ← pruebas unitarias del nuevo slice
```

**Decisión de estructura**: El caso de uso se encapsula en `Features/Properties/UpdateProperty` siguiendo el mismo patrón de `CreateProperty`. No se crea infraestructura transversal nueva. `CreatePropertyImageStorage` se reutiliza directamente desde el namespace `CreateProperty` (sin mover a infraestructura compartida — fuera del alcance de esta spec).

## Fase 0: Investigación

### Tareas de investigación ejecutadas

1. Reutilización de `CreatePropertyImageStorage` — sin duplicación. Ver [research.md](research.md) Decisión 1.
2. Estrategia de búsqueda por `Id` con `FindAsync` antes de actualizar. Ver Decisión 2.
3. Campos editables confirmados: `title`, `description`, `address`, `price`, `status`. Ver Decisión 3.
4. Semántica PUT estricta: todos los campos obligatorios, campo omitido → `400`. Ver Decisión 4.
5. Respuesta `200 OK` con shape idéntico a `CreatePropertyResponse`. Ver Decisión 5.
6. Atomicidad con imagen: misma estrategia `try/catch + TryDelete` de spec 005. Ver Decisión 6.
7. Conservación de `imageUrl` previo cuando no se envía imagen. Ver Decisión 7.
8. Concurrencia last-write-wins confirmada. Ver Decisión 8.
9. Endpoint público (sin autenticación) confirmado. Ver Decisión 9.
10. Nombre de campo `status` (no `statusId`). Ver Decisión 10.

Resultado: todas las NEEDS CLARIFICATION resueltas; `research.md` completo sin pendientes.

## Fase 1: Diseño y Contratos

### Entregables completados

1. [data-model.md](data-model.md) — Entidades de entrada, dominio actualizado, respuesta y errores esperados con transiciones de estado.
2. [contracts/backend-properties-update.openapi.yaml](contracts/backend-properties-update.openapi.yaml) — Contrato público `PUT /api/properties/{id}` con todos los casos de respuesta.
3. [quickstart.md](quickstart.md) — 9 escenarios ejecutables de validación funcional y técnica incluyendo medición de rendimiento SC-005.

## Revalidación de Constitución (post-diseño)

- **Principio I**: PASS
- **Principio II**: PASS
- **Principio III**: PASS — Vertical Slice, ISlice, IHandler confirmados en estructura de código.
- **Principio IV**: PASS — FluentValidation y ProblemDetails en contrato y data-model.
- **Principio V**: PASS — contrato OpenAPI completo, semántica de errores HTTP correcta.
- **Principio VI**: PASS
- **Principio VII**: PASS

Resultado de puerta post-diseño: **APROBADO**.

## Seguimiento de Complejidad

No se identifican violaciones constitucionales que requieran justificación.

La reutilización de `CreatePropertyImageStorage` es referencia directa al namespace existente, no una dependencia transversal nueva.
