# Plan de Implementación: Consulta de propiedad por id

**Branch**: `feat/007-properties-get-by-id` | **Fecha**: 2026-07-11 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación funcional desde `specs/007-properties-get-by-id/spec.md`

## Resumen

Exponer `GET /api/properties/{id}` como endpoint público de consulta unitaria para devolver una sola propiedad cuando existe, con contrato de campos alineado con la spec 004 y sin metadatos de paginación. El flujo debe:

1. Validar el formato de `id` como UUID (GUID RFC 4122) y devolver `400 Bad Request` con ProblemDetails cuando sea inválido.
2. Buscar la propiedad por `id`; devolver `404 Not Found` con ProblemDetails cuando no exista.
3. Devolver `200 OK` con los campos públicos `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters`, `imageUrl`.
4. Construir `imageUrl` absoluto y público cuando exista imagen usando esquema y host de la request: `http(s)://{host}/assets/properties/{fileName}`.
5. Devolver `imageUrl = null` cuando la propiedad no tenga imagen asociada.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET 11 (`net11.0`)

**Dependencias Primarias**: ASP.NET Core Minimal APIs, EF Core, Npgsql, FluentValidation, ProblemDetails

**Almacenamiento**: PostgreSQL (tabla de propiedades) + archivos públicos de imagen bajo `wwwroot/assets/properties`

**Pruebas**: xUnit en `app/backend/tests/RealtorApiTests` + validación manual HTTP en `RealtorApi.http`

**Plataforma Objetivo**: Backend ASP.NET Core en entorno local/CI (Windows/Linux)

**Tipo de Proyecto**: Web API backend en arquitectura Vertical Slice

**Objetivos de Rendimiento**: p95 de consulta unitaria < 300 ms en entorno local de validación funcional

**Restricciones**:

- Sin controllers; solo Minimal APIs.
- Auto-registro por `ISlice` e `IHandler`; sin mapeo manual en `Program.cs`.
- Contrato público explícito sin exponer entidades EF.
- Cancelación obligatoria con `CancellationToken`.
- Consistencia de contrato con spec 004 para campos públicos.
- Esta operación no incluye metadatos de paginación.

**Alcance/Volumen**: Un endpoint GET con rutas de éxito y error (`200`, `400`, `404`) y pruebas unitarias asociadas.

## Revisión de Constitución

*PUERTA: Debe aprobar antes de la Fase 0. Revalidar tras la Fase 1.*

- **Principio I, solución única**: PASS. Se amplía el backend existente sin ramas arquitectónicas paralelas.
- **Principio II, spec-driven**: PASS. El plan deriva exclusivamente de `spec.md` 007 y clarificaciones registradas.
- **Principio III, arquitectura canónica**: PASS. Se mantiene Vertical Slice + Minimal APIs + endpoint delgado.
- **Principio IV, stack obligatorio**: PASS. Se conserva .NET 11, EF Core/Npgsql, FluentValidation, ProblemDetails.
- **Principio V, calidad de dominio y contratos**: PASS. Contrato OpenAPI explícito, ProblemDetails en errores y sin exponer entidades EF.
- **Principio VI, idioma y documentación**: PASS. Artefactos Markdown en español.
- **Principio VII, gobernanza y divergencias**: PASS. Sin desviaciones operativas ni de estructura.

Resultado de puerta previa: **APROBADO**.

## Estructura del Proyecto

### Documentación de la feature

```text
specs/007-properties-get-by-id/
├── plan.md              ← este archivo
├── research.md          ← Fase 0
├── data-model.md        ← Fase 1
├── quickstart.md        ← Fase 1
├── contracts/
│   └── backend-properties-get-by-id.openapi.yaml
├── checklists/
│   └── requirements.md
└── tasks.md             ← siguiente fase (/speckit.tasks)
```

### Código fuente relevante

```text
app/backend/src/RealtorApi/
├── Program.cs
├── RealtorApi.http
├── Features/
│   └── Properties/
│       ├── ListProperties/                ← referencia de contrato 004
│       └── GetPropertyById/               ← nuevo slice
│           ├── GetPropertyByIdSlice.cs
│           ├── GetPropertyByIdContracts.cs
│           ├── GetPropertyByIdQuery.cs
│           ├── GetPropertyByIdHandler.cs
│           ├── GetPropertyByIdMapping.cs
│           └── GetPropertyByIdValidator.cs
└── wwwroot/
    └── assets/
        └── properties/

app/backend/tests/RealtorApiTests/
└── Features/
    └── Properties/
        └── GetPropertyById/
```

**Decisión de estructura**: Se implementa un slice nuevo `GetPropertyById` bajo `Features/Properties`, reutilizando patrones de mapeo y validación existentes, sin introducir infraestructura transversal nueva.

## Fase 0: Investigación

### Tareas de investigación ejecutadas

1. Contrato de salida alineado con 004 (mismos campos públicos, sin paginación). Ver [research.md](research.md), Decisión 1.
2. Estrategia de construcción de `imageUrl` absoluto por request (`scheme` + `host`). Ver Decisión 2.
3. Política de id inválido (`400`) e id inexistente (`404`). Ver Decisión 3.
4. Estrategia de consulta por PK con cancelación (`FindAsync` + `CancellationToken`). Ver Decisión 4.
5. Reglas de nullabilidad de `imageUrl` (null si no hay imagen). Ver Decisión 5.
6. Contrato OpenAPI y pruebas mínimas del endpoint. Ver Decisión 6.

Resultado: todas las clarificaciones cerradas; sin pendientes `NEEDS CLARIFICATION`.

## Fase 1: Diseño y Contratos

### Entregables completados

1. [data-model.md](data-model.md) — Modelo funcional de entrada, salida, errores y reglas de mapeo.
2. [contracts/backend-properties-get-by-id.openapi.yaml](contracts/backend-properties-get-by-id.openapi.yaml) — Contrato OpenAPI público del endpoint GET unitario.
3. [quickstart.md](quickstart.md) — Guía de validación manual y automatizable de extremo a extremo.

## Revalidación de Constitución (post-diseño)

- **Principio I**: PASS
- **Principio II**: PASS
- **Principio III**: PASS
- **Principio IV**: PASS
- **Principio V**: PASS
- **Principio VI**: PASS
- **Principio VII**: PASS

Resultado de puerta post-diseño: **APROBADO**.

## Seguimiento de Complejidad

No se identifican violaciones constitucionales que requieran justificación.
