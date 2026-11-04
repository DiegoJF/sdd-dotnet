# Plan de Implementación: Fundación Backend Base

**Branch**: `main` | **Fecha**: 2026-07-08 | **Spec**: `specs/002-foundation-backend/spec.md`

**Entrada**: Especificación funcional desde `specs/002-foundation-backend/spec.md`

## Resumen

Implementar la base transversal interna de `RealtorApi` para Minimal APIs con arquitectura de Vertical Slice sin features de negocio: contrato `ISlice`, descubrimiento/mapeo centralizado por reflection, auto-registro de `IHandler`, validación automática con FluentValidation y `ValidationFilterFactory`, mapeo consistente de errores esperados (`Result` -> `ProblemDetails`), y migración de `/health` al patrón slice con exposición solo interna vía infraestructura.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET SDK `11.0.100-preview.5.26302.115` (derivado de `global.json`)

**Dependencias Primarias**: ASP.NET Core Minimal APIs, FluentValidation, `Microsoft.AspNetCore.Mvc.Testing`, xUnit

**Almacenamiento**: N/A en esta iniciativa (sin DbContext, sin migraciones, sin conexión a base de datos)

**Testing**: `dotnet test` sobre `app/backend/tests/RealtorApiTests` con foco en pruebas unitarias de infraestructura transversal

**Plataforma Objetivo**: Backend ASP.NET Core ejecutable en entorno local y CI (Windows/Linux)

**Tipo de Proyecto**: Aplicación web backend (`app/backend/src/RealtorApi`) con proyecto de pruebas dedicado

**Objetivos de Rendimiento**: Mantener overhead de infraestructura bajo y predecible; objetivo operativo de foundation: compilación limpia y suite de pruebas de infraestructura en verde (alineado con CE-001 y CE-003)

**Restricciones**: Sin lógica de negocio, sin entidades de dominio, sin controllers, sin EF Core operativo, sin registros manuales por endpoint/handler/validator, `Program.cs` mínimo

**Escala/Alcance**: 1 API backend existente + 1 proyecto de tests existente; solo capacidades transversales y endpoint `/health` como sonda técnica

## Constitution Check

*GATE: Debe aprobar antes de Fase 0. Revalidar tras Fase 1.*

### Validación Previa (antes de Fase 0)

- Principio I (solución única): PASS. No se crean soluciones paralelas ni proyectos adicionales.
- Principio II (spec-driven): PASS. El plan se limita a RF-001..RF-020 de la spec 002.
- Principio III (arquitectura): PASS. Se mantiene Minimal APIs + Vertical Slice sin controllers.
- Principio IV (stack obligatorio): PASS. Se conserva .NET 11 y se usa FluentValidation + ProblemDetails.
- Principio V (calidad y contratos): PASS. Se define manejo uniforme de validación/errores y logging estructurado.
- Principio VI (idioma): PASS. Artefactos markdown en español.
- Principio VII (gobernanza): PASS. No se detectan divergencias con rutas canónicas ni flujo Speckit.

Resultado de gate previo: **APROBADO**.

### Revalidación (después de Fase 1)

- `research.md`, `data-model.md`, `quickstart.md` y `contracts/` se crean sin extender alcance a dominio o persistencia.
- El contrato API documenta únicamente capacidades de infraestructura (`/health` y esquemas de error base).
- La política de exposición interna de `/health` queda capturada de forma explícita.

Resultado de gate posterior: **APROBADO**.

## Estructura del Proyecto

### Documentación (feature actual)

```text
specs/002-foundation-backend/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── backend-infrastructure.openapi.yaml
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
│   │       └── ...
│   └── tests/
│       └── RealtorApiTests/
│           ├── RealtorApiTests.csproj
│           └── ...
└── frontend/
    └── ...
```

**Decisión de estructura**: Se mantiene la estructura web fullstack de la spec 001 y esta iniciativa se concentra exclusivamente en `app/backend/src/RealtorApi` y `app/backend/tests/RealtorApiTests`, preservando separación por capa y trazabilidad de pruebas.

## Fase 0: Investigación

### Tareas de investigación ejecutadas

1. Definir mecanismo de descubrimiento/mapeo de slices por reflection sin registro manual.
2. Establecer patrón de auto-registro de handlers con `IHandler`.
3. Determinar estrategia de validación automática con pass-through cuando no exista validator.
4. Definir mapeo consistente de errores esperados a `ProblemDetails`.
5. Resolver política de exposición de `/health` y su implicación contractual.

Resultado: decisiones documentadas en `research.md` sin incógnitas técnicas pendientes.

## Fase 1: Diseño y Contratos

### Entregables

1. `data-model.md` con modelo conceptual de slices, validación, handlers y errores esperados.
2. `contracts/backend-infrastructure.openapi.yaml` con contrato de `/health` y esquemas base de error.
3. `quickstart.md` con validación ejecutable de compilación, pruebas y verificación funcional mínima.
4. Actualización de contexto de agente para referenciar este plan en `.github/copilot-instructions.md`.

## Seguimiento de Complejidad

Sin violaciones constitucionales que requieran justificación.

## Estrategia de Evolución para Siguientes Iniciativas

- Las features backend futuras deberán añadirse como slices sin modificar el cableado base de `Program.cs`.
- Cada caso de uso deberá apoyarse en la infraestructura transversal de validación y mapeo de errores definida en esta iniciativa.
- La incorporación de persistencia (EF Core/Npgsql), dominio y endpoints de negocio queda deferida a specs posteriores.

## Observaciones finales de implementación

- Se implementó infraestructura transversal en `app/backend/src/RealtorApi/Infrastructure/` para slices, handlers, validación y mapeo de errores esperados.
- `Program.cs` quedó reducido a bootstrap de servicios y mapeo centralizado de endpoints.
- El endpoint `/health` fue migrado al patrón `ISlice`.
- Se implementaron pruebas en `app/backend/tests/RealtorApiTests/Infrastructure/` para discovery de slices, validación automática, mapping `Result -> ProblemDetails` y health.
- El contrato `contracts/backend-infrastructure.openapi.yaml` fue revisado y se mantiene alineado con la implementación final.
- Validación técnica ejecutada con `dotnet build` y `dotnet test` en verde.
