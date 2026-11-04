# Plan de Implementación: Fundación de la Solución Realtor

**Branch**: `main` | **Fecha**: 2026-07-07 | **Spec**: `specs/001-realtor-solution-foundation/spec.md`

**Entrada**: Especificación funcional desde `specs/001-realtor-solution-foundation/spec.md`

## Resumen

Construir la base estructural única de Realtor con una solución principal en `app/Realtor.sln`, cuatro proyectos iniciales (backend, frontend y sus tests), y arranque técnico mínimo sin lógica de negocio ni entidades de dominio. El enfoque técnico prioriza trazabilidad, compilación verificable y cumplimiento estricto de la constitución para habilitar las siguientes specs sin reestructuración.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET SDK 11.0.100-preview.5 (derivado de `global.json`)

**Dependencias Primarias**: ASP.NET Core Minimal APIs, Blazor Web App con Razor Components, xUnit para pruebas base

**Almacenamiento**: N/A en esta iniciativa (sin persistencia funcional); stack objetivo posterior definido por constitución: EF Core + Npgsql + PostgreSQL

**Testing**: `dotnet test` sobre proyectos base de pruebas, con foco en esqueleto unitario inicial

**Plataforma Objetivo**: Entorno de desarrollo y CI para .NET en repositorio único (Windows/Linux)

**Tipo de Proyecto**: Aplicación web fullstack en una sola solución con backend + frontend + pruebas separadas

**Objetivos de Rendimiento**: Compilación exitosa de la solución base en al menos una ejecución completa de verificación (CE-002)

**Restricciones**: Sin lógica de negocio, sin features funcionales, sin entidades de dominio, sin modificación de `global.json`, sin soluciones paralelas

**Escala/Alcance**: 1 solución (`app/Realtor.sln`) y 4 proyectos base en rutas canónicas definidas por RF-002 a RF-005

## Constitution Check

*GATE: Debe aprobar antes de Fase 0. Revalidar tras Fase 1.*

### Validación Previa (antes de Fase 0)

- Principio I (solución única): PASS. Se define una sola solución principal.
- Principio II (spec-driven): PASS. El alcance corresponde exactamente a RF-001..RF-010.
- Principio III (arquitectura): PASS. Backend orientado a Minimal APIs y frontend Blazor.
- Principio IV (stack obligatorio): PASS. Se mantiene .NET 11 y stack canonical como dirección técnica.
- Principio V (calidad y contratos): PASS. Se restringe a arranque técnico sin lógica de dominio.
- Principio VI (idioma): PASS. Artefactos de planificación en español.
- Principio VII (gobernanza): PASS. No se detectan divergencias operativas activas en esta fase.

Resultado de gate previo: **APROBADO**.

### Revalidación (después de Fase 1)

- Artefactos de diseño creados sin expandir alcance fuera de foundation.
- Contratos definidos solo para interfaz técnica mínima de arranque.
- No se introducen cambios arquitectónicos incompatibles.

Resultado de gate posterior: **APROBADO**.

## Estructura del Proyecto

### Documentación (feature actual)

```text
specs/001-realtor-solution-foundation/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── backend-bootstrap.openapi.yaml
└── tasks.md
```

### Código Fuente (raíz del repositorio)

```text
app/
├── Realtor.sln
├── backend/
│   ├── src/
│   │   └── RealtorApi/
│   └── tests/
│       └── RealtorApiTests/
└── frontend/
    ├── src/
    │   └── RealtorWeb/
    └── test/
        └── RealtorWeb/
```

**Decisión de Estructura**: Se adopta estructura web fullstack única con separación explícita de backend y frontend dentro de `app/`, manteniendo proyectos de prueba dedicados por capa para facilitar evolución incremental por specs.

## Fase 0: Investigación

### Tareas de Investigación Ejecutadas

1. Confirmar baseline de plataforma desde `global.json`.
2. Determinar patrón de arranque técnico mínimo para backend foundation sin negocio.
3. Definir forma de contrato externo mínima sin introducir features funcionales.
4. Establecer estrategia para manejo de preexistencia de rutas/proyectos sin sobreescritura destructiva.

Resultado: decisiones consolidadas en `research.md`.

## Fase 1: Diseño y Contratos

### Entregables

1. `data-model.md` con entidades conceptuales de foundation y reglas de validación estructural.
2. `contracts/backend-bootstrap.openapi.yaml` con contrato HTTP técnico mínimo de arranque.
3. `quickstart.md` con guía de validación end-to-end para comprobar criterios de éxito.
4. Contexto de agente actualizado para referenciar este plan entre marcadores SPECKIT.

## Seguimiento de Complejidad

Sin violaciones constitucionales que requieran justificación.

## Estrategia de Evolución para Siguientes Iniciativas

- Toda nueva iniciativa debe extender la solución `app/Realtor.sln` existente sin crear una segunda solución.
- Los nuevos casos de uso backend deben añadirse como slices en `app/backend/src/RealtorApi/Features/`.
- El frontend debe evolucionar sobre `app/frontend/src/RealtorWeb/` reutilizando layout y estilos base.
- Cualquier proyecto adicional debe justificarse en una spec posterior y trazarse en `tasks.md` de esa iniciativa.
