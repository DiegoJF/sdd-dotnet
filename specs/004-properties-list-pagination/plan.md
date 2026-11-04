# Plan de Implementación: Listado Paginado de Propiedades

**Rama**: `feat/004-properties-list-pagination` | **Fecha**: 2026-07-09 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación funcional desde `specs/004-properties-list-pagination/spec.md`

## Resumen

Exponer `GET /api/properties` como un slice público de solo lectura que devuelva propiedades paginadas, ordenadas por `title` ascendente y acompañadas por metadatos de paginación. La respuesta debe proyectarse desde EF Core con `AsNoTracking`, validar `page` y `pageSize`, y construir `imageUrl` como URL absoluta pública usando el esquema y host de la solicitud actual.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET 11 (`net11.0`)

**Dependencias Primarias**: ASP.NET Core Minimal APIs, EF Core, Npgsql, FluentValidation, ProblemDetails

**Almacenamiento**: PostgreSQL para propiedades persistentes y archivos estáticos servidos desde `wwwroot/assets/properties`

**Pruebas**: xUnit, `Microsoft.AspNetCore.Mvc.Testing` y validación manual con `RealtorApi.http`

**Plataforma Objetivo**: Servicio backend ASP.NET Core para ejecución local y CI en Windows/Linux

**Tipo de Proyecto**: Aplicación web backend con arquitectura Vertical Slice

**Objetivos de Rendimiento**: Consulta de listado con paginación realizada en servidor mediante `OrderBy` + `Skip` + `Take`, sin cargar entidades completas innecesariamente

**Restricciones**: Sin controllers, sin registro manual de endpoints en `Program.cs`, sin exponer entidades EF en la respuesta, con propagación de `CancellationToken` y respuestas de validación con `400`

**Alcance/Volumen**: Un único endpoint de lectura sobre el catálogo existente de propiedades y su imagen pública asociada

## Revisión de Constitución

*PUERTA: Debe aprobar antes de la Fase 0. Revalidar tras la Fase 1.*

- Principio I, solución única: PASS. El cambio se limita al backend ya existente.
- Principio II, spec-driven: PASS. El alcance está acotado a `spec.md` y a sus criterios de aceptación.
- Principio III, arquitectura canónica: PASS. Se mantiene Minimal APIs con Vertical Slice y `Program.cs` solo como orquestador.
- Principio IV, stack obligatorio: PASS. Se conserva .NET 11, EF Core, Npgsql, PostgreSQL, FluentValidation y ProblemDetails.
- Principio V, calidad y contratos: PASS. Se define un contrato público explícito y validación consistente.
- Principio VI, idioma y documentación: PASS. Los artefactos Markdown de la feature quedan en español.
- Principio VII, gobernanza: PASS. No se introducen rutas paralelas ni cambios estructurales fuera del flujo Speckit.

Resultado de la puerta previa: **APROBADO**.

## Estructura del Proyecto

### Documentación de la feature

```text
specs/004-properties-list-pagination/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── backend-properties-list-pagination.openapi.yaml
└── tasks.md
```

### Código fuente relevante

```text
app/backend/src/RealtorApi/
├── Program.cs
├── RealtorApi.http
├── Domain/
│   └── Properties/
├── Features/
│   └── Properties/
│       └── ListProperties/
├── Infrastructure/
│   ├── Endpoints/
│   ├── Handlers/
│   ├── Persistence/
│   ├── Results/
│   ├── Slices/
│   └── Validation/
└── wwwroot/
    └── assets/
        └── properties/

app/backend/tests/RealtorApiTests/
└── Infrastructure/
```

**Decisión de estructura**: Se introduce un slice de lectura en `Features/Properties/ListProperties` para concentrar endpoint, handler, mapeo y validación del caso de uso, mientras `Infrastructure` conserva el discovery de slices, acceso a datos y utilidades transversales ya existentes.

## Fase 0: Investigación

### Tareas de investigación ejecutadas

1. Confirmar el patrón de descubrimiento de `ISlice` y la ruta de integración del nuevo endpoint.
2. Definir la estrategia de consulta paginada con EF Core y proyección directa sin exponer entidades.
3. Determinar la forma de validar `page` y `pageSize` con respuesta `400` compatible con ProblemDetails.
4. Resolver cómo construir `imageUrl` absoluta pública a partir del esquema y host de la solicitud actual.
5. Documentar el contrato público del endpoint y sus escenarios de validación manual.

Resultado: decisiones consolidadas en `research.md`, sin aclaraciones pendientes.

## Fase 1: Diseño y Contratos

### Entregables

1. `data-model.md` con el modelo de lectura, metadatos de paginación y contrato de respuesta.
2. `contracts/backend-properties-list-pagination.openapi.yaml` con el contrato público del endpoint.
3. `quickstart.md` con pasos ejecutables para validar listado, paginación, validación y URLs absolutas.
4. Actualización del contexto del agente en `.github/copilot-instructions.md` para referenciar este plan.

## Seguimiento de Complejidad

No existen violaciones de la constitución que requieran justificación adicional.

## Observaciones de implementación previstas

- Crear `Features/Properties/ListProperties` como slice principal de negocio.
- Reutilizar `AppDbContext` con proyección `AsNoTracking` y orden por `Title` ascendente.
- Validar `page` y `pageSize` con FluentValidation y devolver `400` con detalle de validación cuando corresponda.
- Construir `imageUrl` absoluta combinando `Request.Scheme`, `Request.Host` y el archivo de imagen persistido.
- Añadir entradas manuales en `RealtorApi.http` para las consultas base y parametrizadas.
