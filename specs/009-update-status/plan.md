# Plan de Implementación: Actualización de estado de propiedades

**Branch**: `feat/009-update-status` | **Fecha**: 2026-07-21 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación funcional desde `specs/009-update-status/spec.md`

## Resumen

Exponer un endpoint público `PATCH /api/properties/{id}/status` para actualizar únicamente el estado de una propiedad existente. El slice debe buscar primero la propiedad por `id`, devolver `404 Not Found` si no existe, validar que el cuerpo incluya `status`, rechazar con `400 Bad Request` cuando falte o sea inválido, y responder `200 OK` con la propiedad actualizada conservando intactos los demás datos persistidos, incluido `imageUrl`. El contrato y la validación manual deben quedar documentados en OpenAPI, `quickstart.md` y el archivo `.http` existente del backend.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET 11 (`net11.0`)

**Dependencias Primarias**: ASP.NET Core Minimal APIs, EF Core, Npgsql, FluentValidation, ProblemDetails

**Almacenamiento**: PostgreSQL para `properties` y catálogo de estados existente; no aplica almacenamiento de archivos en esta iniciativa

**Pruebas**: xUnit en `app/backend/tests/RealtorApiTests` y verificación manual desde `app/backend/src/RealtorApi/RealtorApi.http`

**Plataforma Objetivo**: Backend ASP.NET Core en Windows y Linux

**Tipo de Proyecto**: Web API backend con arquitectura Vertical Slice

**Objetivos de Rendimiento**: La actualización debe seguir el mismo perfil de latencia del backend de propiedades ya validado; la verificación manual debe poder demostrarse en menos de 5 minutos

**Restricciones**:

- El endpoint es público y no requiere autenticación.
- Solo se permite modificar `status`; cualquier otro campo fuera de alcance debe rechazarse.
- La respuesta debe conservar `imageUrl` y el resto de datos persistidos.
- Los errores funcionales deben serializarse con ProblemDetails.
- No se introduce manejo de imagen ni subida de archivos.
- No se cambian la semántica ni el contrato de los endpoints existentes.

**Alcance/Volumen**: Un slice nuevo para un único endpoint `PATCH`, un contrato OpenAPI, una guía quickstart, y ejemplos de validación manual en `.http`.

## Revisión de Constitución

*PUERTA: Debe aprobar antes de la Fase 0. Revalidar tras la Fase 1.*

- **Principio I, solución única**: PASS. El cambio vive dentro de la solución actual y reutiliza la infraestructura existente.
- **Principio II, spec-driven**: PASS. El plan deriva únicamente de `specs/009-update-status/spec.md`.
- **Principio III, arquitectura canónica**: PASS. Se mantiene ASP.NET Core Minimal API con Vertical Slice; no se introducen controllers.
- **Principio IV, stack obligatorio**: PASS. Se conservan .NET 11, EF Core, Npgsql, FluentValidation y ProblemDetails.
- **Principio V, calidad de dominio y contratos**: PASS. El endpoint expone validación consistente y respuesta estructurada, sin lógica de negocio fuera del slice.
- **Principio VI, idioma y documentación**: PASS. Los artefactos Markdown del feature están escritos en español.
- **Principio VII, gobernanza y divergencias**: PASS. No hay desvíos de rutas ni estructura fuera de la convención.

Resultado de puerta previa: **APROBADO**.

## Estructura del Proyecto

### Documentación de la feature

```text
specs/009-update-status/
├── plan.md              ← este archivo
├── research.md          ← Fase 0
├── data-model.md        ← Fase 1
├── quickstart.md        ← Fase 1
├── contracts/
│   └── backend-properties-update-status.openapi.yaml
└── tasks.md             ← siguiente fase (/speckit.tasks)
```

### Código fuente relevante

```text
app/backend/src/RealtorApi/
├── RealtorApi.http                     ← agregar ejemplo PATCH de validación
├── Features/
│   └── Properties/
│       └── UpdateStatus/
│           ├── UpdateStatusSlice.cs
│           ├── UpdateStatusContracts.cs
│           ├── UpdateStatusCommand.cs
│           ├── UpdateStatusHandler.cs
│           ├── UpdateStatusMapping.cs
│           └── UpdateStatusValidator.cs
└── Domain/
    └── Properties/
        └── Property.cs                 ← contiene el enum `PropertyStatusValue` existente reutilizado

app/backend/tests/RealtorApiTests/
└── Features/
    └── Properties/
        └── UpdateStatus/
```

**Decisión de estructura**: El caso de uso se encapsula en `Features/Properties/UpdateStatus` siguiendo el patrón ya usado en el backend. El contrato manual se valida en el `.http` del proyecto, y el OpenAPI de la feature se documenta en `contracts/` para trazabilidad.

## Fase 0: Investigación

### Tareas de investigación ejecutadas

1. Reutilización del modelo de estado existente (`PropertyStatusValue`) sin crear un catálogo nuevo. Ver [research.md](research.md), Decisión 1.
2. Uso de `FindAsync` para localizar la propiedad por clave primaria antes de mutar `status`. Ver Decisión 2.
3. Contrato mínimo de request con un único campo `status` y respuesta conservando `imageUrl`. Ver Decisión 3.
4. Semántica de errores: `400` para status omitido/inválido y `404` para propiedad inexistente. Ver Decisión 4.
5. Validación manual en `.http` como evidencia del flujo público del endpoint. Ver Decisión 5.

Resultado: no quedan `NEEDS CLARIFICATION`; la investigación cierra el alcance técnico necesario para diseñar la feature.

## Fase 1: Diseño y Contratos

### Entregables completados

1. [data-model.md](data-model.md) — Modelo funcional del request, respuesta, error y reglas de persistencia.
2. [contracts/backend-properties-update-status.openapi.yaml](contracts/backend-properties-update-status.openapi.yaml) — Contrato objetivo del endpoint `PATCH /api/properties/{id}/status`.
3. [quickstart.md](quickstart.md) — Guía reproducible de validación manual y automatizada.

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
