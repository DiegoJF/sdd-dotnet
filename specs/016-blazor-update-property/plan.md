# Plan de Implementación: 016-blazor-update-property

**Branch**: `feat/016-blazor-update-property` | **Date**: 2026-07-25 | **Spec**: `specs/016-blazor-update-property/spec.md`

**Input**: Especificación funcional desde `specs/016-blazor-update-property/spec.md`

## Resumen

Implementar en `RealtorWeb` el flujo completo de edición de inmuebles existentes: acción de editar desde tarjeta de listado, pantalla de edición con carga inicial, validación funcional alineada al contrato `UpdateProperty`, guardado con prevención de envíos duplicados, conservación de datos ante error y navegación al detalle tras éxito.

La integración técnica se realizará reutilizando `IPropertiesApi` (Refit) y los patrones ya aplicados en alta (`Create`) y detalle (`Detail`).

## Contexto Técnico

**Language/Version**: C# con .NET 11 (`net11.0`, SDK `11.0.100-preview.5.26302.115`)

**Primary Dependencies**: Blazor Web App (Razor Components), Refit + `Refit.HttpClientFactory`, Lucide.Blazor, DataAnnotations para validación de formulario

**Storage**: N/A en frontend (consume API backend; sin cambios de persistencia en esta feature)

**Testing**: xUnit (`app/frontend/test/RealtorWeb/RealtorWebTests.csproj`) y validación manual guiada

**Target Platform**: ASP.NET Core Interactive Server + navegadores web

**Project Type**: Aplicación web (frontend Blazor con integración backend existente)

**Performance Goals**: En validación manual del flujo de edición: (1) cada cambio de campo inválido debe reflejar mensaje de validación en la misma interacción de edición, (2) el estado visual de guardado debe mostrarse durante toda solicitud en curso, y (3) cada intento de guardado debe generar como máximo una solicitud HTTP efectiva aun con múltiples clics.

**Constraints**:
- Respetar contrato OpenAPI `UpdateProperty` (`PUT /api/properties/{id}` con `multipart/form-data` o `application/x-www-form-urlencoded`)
- Mantener consistencia visual con `wwwroot/app.css` y componentes existentes
- Reutilizar patrones de manejo de error y servicios HTTP existentes

**Scale/Scope**: Una nueva ruta/página de edición + componentes de feature asociados + extensión del cliente API y navegación desde tarjeta de listado

## Constitution Check

*GATE: Debe aprobar antes de Fase 0 (investigación). Revalidar tras Fase 1 (diseño).*

### Pre-Fase 0

- **Principio I - Solución única y compartida**: PASS
  - La iniciativa extiende el frontend existente sin crear soluciones paralelas.
- **Principio II - Spec-Driven Development**: PASS
  - Existe `spec.md` en `specs/016-blazor-update-property/` y el plan se deriva únicamente de esa spec.
- **Principio III - Arquitectura canónica**: PASS
  - Cambios acotados a Blazor Web App y consumo de Minimal API existente.
- **Principio IV - Stack obligatorio**: PASS
  - Se mantiene .NET 11, Blazor, Refit y estilos propios.
- **Principio V - Calidad de dominio y contratos**: PASS
  - El plan respeta contrato OpenAPI como fuente de verdad para integración.
- **Principio VI - Idioma y documentación**: PASS
  - Artefactos en español.
- **Principio IX - Calidad/verificabilidad de especificaciones**: PASS
  - Requisitos y criterios de éxito observables en la spec.

### Post-Fase 1

- **Revalidación**: PASS
  - `research.md`, `data-model.md`, `contracts/` y `quickstart.md` mantienen separación funcional/técnica y no introducen alcance no aprobado.

## Estructura del Proyecto

### Documentación (esta feature)

```text
specs/016-blazor-update-property/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── frontend-property-update.contract.yaml
└── tasks.md
```

### Código fuente (raíz del repositorio)

```text
app/
├── backend/
│   └── src/RealtorApi/
│       └── wwwroot/openapi/v1.json
└── frontend/
    ├── src/RealtorWeb/
    │   ├── Components/Pages/
    │   ├── Features/Properties/
    │   │   ├── List/
    │   │   ├── Detail/
    │   │   ├── Create/
    │   │   └── Update/
    │   ├── Services/Api/Properties/
    │   └── wwwroot/app.css
    └── test/RealtorWeb/
```

**Decisión de estructura**: Se usa la estructura web existente (frontend + backend). La implementación de la feature 016 se concentra en `app/frontend/src/RealtorWeb` reutilizando los slices actuales de propiedades y el cliente Refit existente.

## Complejidad y Excepciones

No se identifican violaciones de constitución que requieran justificación.
