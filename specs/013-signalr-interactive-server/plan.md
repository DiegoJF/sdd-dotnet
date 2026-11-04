# Plan de Implementación: Interactividad de servidor en páginas host Blazor

**Branch**: `feat/013-signalr-interactive-server` | **Fecha**: 2026-07-23 | **Spec**: `specs/013-signalr-interactive-server/spec.md`

**Entrada**: Especificación funcional desde `specs/013-signalr-interactive-server/spec.md`

## Resumen

Habilitar render interactivo de servidor de forma granular en las páginas host `Home.razor` y `PropertyDetailPage.razor` para que se ejecuten los handlers `@onclick` ya existentes en los componentes `PropertyList` y `PropertyDetail`, manteniendo `Error.razor` y `NotFound.razor` en Static SSR y sin alterar backend, contratos Refit ni lógica funcional de code-behind.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET 11 (`net11.0`) + Razor Components (Blazor Web App)

**Dependencias Primarias**: ASP.NET Core Blazor Web App, Refit, configuración existente de Interactive Server (`AddInteractiveServerComponents`, `AddInteractiveServerRenderMode`)

**Storage**: N/A (sin cambios de persistencia)

**Pruebas**: Build de frontend (`dotnet build`) + validación manual end-to-end de navegación y reintentos

**Plataforma Objetivo**: Web app Blazor en servidor con circuito SignalR

**Tipo de Proyecto**: Frontend web en `app/frontend/src/RealtorWeb`

**Objetivos de Rendimiento**: Navegación de paginación y retorno de detalle sin recarga completa del navegador en interacciones de UI

**Restricciones**:

- No habilitar interactividad global en `App.razor` ni `Routes`.
- Mantener `Error.razor` y `NotFound.razor` en modo estático.
- No modificar backend, contratos Refit ni lógica de negocio existente.
- Limitar cambios a páginas host y validación funcional asociada.

**Escala/Alcance**: 2 archivos de páginas host con `@rendermode InteractiveServer`, más documentación y validación de la feature.

## Revisión de Constitución

*PUERTA: Debe aprobar antes de Fase 0. Revalidar tras Fase 1.*

### Validación previa (antes de Fase 0)

- **Principio I, solución única y compartida**: PASS. Se ajusta comportamiento UI dentro de la solución existente.
- **Principio II, spec-driven**: PASS. Este plan deriva únicamente de la spec 013 aprobada en clarificación.
- **Principio III, arquitectura canónica**: PASS. Se mantiene Blazor Web App con componentes Razor.
- **Principio IV, stack obligatorio**: PASS. Se conserva .NET 11, Refit y CSS propio sin nuevos frameworks.
- **Principio V, calidad de dominio y contratos**: PASS. No se mueve lógica de dominio ni se alteran contratos de datos.
- **Principio VI, idioma**: PASS. Artefactos markdown de la feature en español.
- **Principio VII, gobernanza**: PASS. Sin conflicto de rutas ni desviaciones del flujo Speckit.

Resultado de puerta previa: **APROBADO**.

## Estructura del Proyecto

### Documentación de la feature

```text
specs/013-signalr-interactive-server/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── frontend-interactive-server-render-mode.contract.yaml
└── tasks.md
```

### Código fuente relevante

```text
app/frontend/src/RealtorWeb/
├── Components/
│   └── Pages/
│       ├── Home.razor
│       ├── PropertyDetailPage.razor
│       ├── Error.razor
│       └── NotFound.razor
└── Features/
    └── Properties/
        ├── List/
        │   └── PropertyList.razor.cs
        └── Detail/
            └── PropertyDetail.razor.cs
```

**Decisión de estructura**: Mantener la arquitectura actual y aplicar cambio mínimo en páginas host enroutables que contienen componentes interactivos, preservando páginas informativas en Static SSR.

## Fase 0: Investigación

### Tareas de investigación ejecutadas

1. Confirmar el patrón recomendado para activar interactividad de servidor de forma granular por página (`@rendermode InteractiveServer`).
2. Verificar que la configuración global de servicios/render mode ya existe en `Program.cs` y que no requiere cambios adicionales.
3. Evaluar el riesgo de habilitación global y validar su exclusión por alcance.
4. Confirmar que `Error.razor` y `NotFound.razor` deben permanecer estáticas por decisión de clarificación.

Resultado: decisiones cerradas en `research.md` sin `NEEDS CLARIFICATION` pendientes.

## Fase 1: Diseño y Contratos

### Entregables completados

1. `data-model.md` con modelo conceptual de superficies host, estado de interactividad y reglas de navegación afectadas.
2. `contracts/frontend-interactive-server-render-mode.contract.yaml` con contrato funcional de alcance, rutas y exclusiones.
3. `quickstart.md` con validación manual de paginación, retorno al listado, reintento y comprobación de páginas estáticas.
4. Actualización de contexto de agente en `.github/copilot-instructions.md` para apuntar a la iniciativa 013.

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

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Regla constitucional de "exactamente tres archivos" por iniciativa vs artefactos operativos de `speckit.plan` (`research.md`, `data-model.md`, `quickstart.md`, `contracts/`) | El flujo Speckit del repositorio exige estos artefactos para investigación, diseño, contrato y validación previa a tasks | Omitir artefactos rompe trazabilidad y deja sin evidencia verificable las decisiones de fase 0/1 |
