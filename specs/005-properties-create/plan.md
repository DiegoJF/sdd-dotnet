# Plan de Implementación: Alta de propiedades con imagen opcional

**Branch**: `main` | **Fecha**: 2026-07-11 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación funcional desde `specs/005-properties-create/spec.md`

## Resumen

Exponer `POST /api/properties` como slice público de creación en backend para registrar propiedades con imagen opcional. El flujo debe permitir alta sin imagen (`imageUrl = null`) y alta con imagen válida (PNG/JPG <= 5 MB), almacenando el archivo en `wwwroot/assets/properties` con nombre único interno (`UUID + extensión`). La operación con imagen será atómica: si falla el guardado, no se persiste la propiedad. Los errores esperados quedan normalizados en `400` (campos de negocio), `415` (formato inválido, 0 bytes o contenido real inválido), `413` y `500`.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET 11 (`net11.0`)

**Dependencias Primarias**: ASP.NET Core Minimal APIs, EF Core, Npgsql, FluentValidation, ProblemDetails

**Almacenamiento**: PostgreSQL para datos de propiedades y sistema de archivos en `wwwroot/assets/properties` para imágenes

**Pruebas**: xUnit en `app/backend/tests/RealtorApiTests`, además de validación manual HTTP

**Plataforma Objetivo**: Backend ASP.NET Core ejecutado localmente y en CI (Windows/Linux)

**Tipo de Proyecto**: Aplicación web backend con arquitectura Vertical Slice

**Objetivos de Rendimiento**: En una muestra de 100 solicitudes válidas mixtas (con y sin imagen), el percentil 95 debe ser menor a 3 segundos

**Restricciones**: Sin controllers, sin registro manual de endpoints en `Program.cs`, mapping sin lógica de negocio, auto-registro por `IHandler` e `ISlice`, operación atómica ante fallo de guardado de imagen

**Alcance/Volumen**: Un endpoint público de creación, con escenarios de éxito sin imagen, éxito con imagen y errores de validación/infraestructura

## Revisión de Constitución

*PUERTA: Debe aprobar antes de la Fase 0. Revalidar tras la Fase 1.*

- Principio I, solución única: PASS. El alcance se integra en backend existente sin soluciones paralelas.
- Principio II, spec-driven: PASS. El plan deriva exclusivamente de `spec.md` 005 y clarificaciones registradas.
- Principio III, arquitectura canónica: PASS. Se mantiene Minimal API + Vertical Slice + endpoint delgado.
- Principio IV, stack obligatorio: PASS. Se conserva .NET 11, EF Core, Npgsql, FluentValidation y ProblemDetails.
- Principio V, calidad de dominio y contratos: PASS. Se define contrato OpenAPI y validación consistente con códigos de error explícitos.
- Principio VI, idioma y documentación: PASS. Artefactos Markdown de la feature en español.
- Principio VII, gobernanza y divergencias: PASS. No hay cambios estructurales fuera del flujo Speckit.

Resultado de puerta previa: **APROBADO**.

## Estructura del Proyecto

### Documentación de la feature

```text
specs/005-properties-create/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── backend-properties-create.openapi.yaml
└── tasks.md
```

### Código fuente relevante

```text
app/backend/src/RealtorApi/
├── Program.cs
├── RealtorApi.http
├── Features/
│   └── Properties/
│       └── CreateProperty/
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

**Decisión de estructura**: El caso de uso se encapsula en `Features/Properties/CreateProperty` para concentrar endpoint, handler, mapping y validator por slice, mientras `Infrastructure` mantiene responsabilidades transversales de discovery, persistencia y validación.

## Fase 0: Investigación

### Tareas de investigación ejecutadas

1. Definir estrategia de entrada con imagen opcional en una única operación pública.
2. Definir validación funcional de upload (PNG/JPG, max 5 MB) y semántica HTTP de errores.
3. Resolver atomicidad transaccional entre persistencia de imagen y creación de propiedad.
4. Definir estrategia de naming interno seguro para imágenes (UUID + extensión válida).
5. Asegurar compatibilidad de `imageUrl` con flujo de listado existente.
6. Formalizar contrato OpenAPI y guía de validación de extremo a extremo.

Resultado: decisiones consolidadas en `research.md`, sin NEEDS CLARIFICATION pendientes.

## Fase 1: Diseño y Contratos

### Entregables

1. `data-model.md` con entidades de entrada, persistencia, respuesta y errores esperados.
2. `contracts/backend-properties-create.openapi.yaml` con contrato público de `POST /api/properties`.
3. `quickstart.md` con escenarios ejecutables de validación funcional y técnica.
4. Actualización de contexto del agente en `.github/copilot-instructions.md` para apuntar al plan 005.

## Revalidación de Constitución (post-diseño)

- Principio I: PASS
- Principio II: PASS
- Principio III: PASS
- Principio IV: PASS
- Principio V: PASS
- Principio VI: PASS
- Principio VII: PASS

Resultado de puerta post-diseño: **APROBADO**.

## Seguimiento de Complejidad

No se identifican violaciones constitucionales que requieran justificación.
