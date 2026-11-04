# Plan de Implementación: OpenAPI v1 público

**Branch**: `feat/008-open-api` | **Fecha**: 2026-07-19 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación funcional desde `specs/008-open-api/spec.md`

## Resumen

Publicar un único contrato OpenAPI v1 generado desde la implementación real de `RealtorApi` y exponerlo en `GET /openapi/v1.json`, sin UI interactiva de documentación. La implementación debe incluir pipeline reproducible de regeneración y validación (`dotnet build` + Redocly lint + smoke test NSwag), junto con verificación automatizada de drift que compare inventario runtime de endpoints públicos funcionales (`GET /health` y `/api/properties*`) contra el documento versionado, aplicando normalización de constraints y fallando explícitamente cuando falten prerrequisitos externos.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET 11 (`net11.0`) + PowerShell 7 para automatización + OpenAPI 3.x JSON

**Dependencias Primarias**: ASP.NET Core Minimal APIs, Microsoft.AspNetCore.OpenApi, xUnit, `@redocly/cli` (vía npx), `NSwag.ConsoleCore` (dotnet local tool)

**Almacenamiento**: Archivo versionado `app/backend/src/RealtorApi/wwwroot/openapi/v1.json` + artefacto smoke en `artifacts/openapi-client-smoke/`

**Pruebas**: xUnit (`app/backend/tests/RealtorApiTests`) + ejecución de CLI externas (Redocly y NSwag) desde pruebas de drift

**Plataforma Objetivo**: Backend ASP.NET Core en Windows/Linux para desarrollo y CI

**Tipo de Proyecto**: Web API backend (Minimal APIs) en solución .NET con arquitectura Vertical Slice

**Objetivos de Rendimiento**: Validación y generación reproducible en una sola ejecución local sin pasos manuales intermedios; endpoint estático `GET /openapi/v1.json` con respuesta inmediata de archivo

**Restricciones**:

- Prohibido Swagger UI/ReDoc UI u otra interfaz interactiva en runtime.
- El documento OpenAPI se genera desde código real del backend; no se edita manualmente.
- La prueba de drift compara solo endpoints públicos funcionales y normaliza constraints (`{id:guid}` == `{id}`).
- Si faltan Node/Redocly/NSwag en entorno de pruebas, drift debe fallar explícitamente (no Skip).
- `.redocly.yaml` es obligatorio para ajustar reglas de lint de API pública sin autenticación.
- La verificación contractual requiere pruebas xUnit sobre superficie HTTP/runtime (tipo integración de contrato); su uso queda justificado explícitamente por FR-011/FR-012/FR-017.

**Alcance/Volumen**: 5 endpoints públicos en alcance (`GET /health`, `GET/POST/PUT /api/properties...`, `GET /api/properties/{id}`), 1 documento versionado (`v1.json`), 1 script de regeneración y 1 suite de drift contractual.

## Revisión de Constitución

*PUERTA: Debe aprobar antes de la Fase 0. Revalidar tras la Fase 1.*

- **Principio I, solución única**: PASS. El alcance se implementa dentro de la solución actual (`app/backend`, `tests`, `specs`) sin ramificaciones.
- **Principio II, spec-driven**: PASS. Este plan deriva únicamente de `specs/008-open-api/spec.md` y clarificaciones registradas.
- **Principio III, arquitectura canónica**: PASS. Se mantiene backend Minimal APIs con slices existentes; no se introducen controllers.
- **Principio IV, stack obligatorio**: PASS. Se preserva .NET 11, ASP.NET Core Minimal APIs y herramientas permitidas de validación offline.
- **Principio V, calidad de dominio y contratos**: PASS. Se fortalece el contrato público y consistencia ProblemDetails; sin lógica de dominio en UI/endpoints. Se justifica explícitamente el uso de pruebas xUnit de contrato sobre runtime por exigencia de drift y validación de publicación OpenAPI.
- **Principio VI, idioma y documentación**: PASS. Artefactos `.md` en español.
- **Principio VII, gobernanza y divergencias**: PASS. Sin desvíos de estructura ni cambios a rutas canónicas.

Resultado de puerta previa: **APROBADO**.

## Estructura del Proyecto

### Documentación de la feature

```text
specs/008-open-api/
├── plan.md              ← este archivo
├── research.md          ← Fase 0
├── data-model.md        ← Fase 1
├── quickstart.md        ← Fase 1
├── contracts/
│   └── backend-openapi-v1.openapi.yaml
└── tasks.md             ← siguiente fase (/speckit.tasks)
```

### Código fuente relevante

```text
app/backend/src/RealtorApi/
├── Program.cs
├── RealtorApi.csproj
├── wwwroot/
│   └── openapi/
│       └── v1.json
└── Features/
    └── Properties/

app/backend/tests/RealtorApiTests/
└── Infrastructure/
    └── OpenApi/

support/scripts/
└── generate-openapi-v1.ps1

artifacts/
└── openapi-client-smoke/
```

**Decisión de estructura**: La feature se concentra en backend + pruebas + script de soporte, sin afectar frontend. El contrato publicado vive en `wwwroot/openapi/v1.json` y su fuente de verdad es la implementación runtime del backend.

## Fase 0: Investigación

### Tareas de investigación ejecutadas

1. Estrategia de generación OpenAPI desde implementación real sin edición manual. Ver [research.md](research.md), Decisión 1.
2. Publicación como archivo estático estable en `/openapi/v1.json` sin UI interactiva. Ver Decisión 2.
3. Reglas Redocly para API pública sin autenticación (`security-defined` off, `operation-4xx-response` warning). Ver Decisión 3.
4. Smoke test de consumibilidad mediante NSwag CLI local tool. Ver Decisión 4.
5. Diseño de prueba de drift con `EndpointDataSource`, normalización de constraints y filtro de endpoints públicos funcionales. Ver Decisión 5.
6. Política de fallo explícito ante prerrequisitos externos faltantes en pruebas de drift y script reproducible. Ver Decisión 6.

Resultado: todas las clarificaciones cerradas; sin pendientes `NEEDS CLARIFICATION`.

## Fase 1: Diseño y Contratos

### Entregables completados

1. [data-model.md](data-model.md) — Modelo funcional del documento OpenAPI, inventarios comparados y reglas de validación.
2. [contracts/backend-openapi-v1.openapi.yaml](contracts/backend-openapi-v1.openapi.yaml) — Contrato objetivo del documento publicado en v1.
3. [quickstart.md](quickstart.md) — Guía reproducible de validación end-to-end del contrato, drift y script.

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
