# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]

**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

[Extract from feature spec: primary requirement + technical approach from research]

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  # Plan de Implementación: Listado de propiedades en la página principal

  **Branch**: `feat/011-blazor-list-properties` | **Fecha**: 2026-07-22 | **Spec**: `specs/011-blazor-list-properties/spec.md`

  **Entrada**: Especificación funcional desde `specs/011-blazor-list-properties/spec.md`

  ## Resumen

  Convertir la ruta principal `/` de `RealtorWeb` en una experiencia de listado paginado de propiedades consumida mediante `IPropertiesApi` y Refit, con navegación anterior/siguiente, estados de carga/vacío/error y una grilla responsive de 3/2/1 columnas apoyada en `wwwroot/app.css`.

  ## Contexto Técnico

  **Lenguaje/Versión**: C# con .NET 11 (`net11.0`) + Razor Components (Blazor Web App)

  **Dependencias Primarias**: ASP.NET Core Blazor Web App, Refit, Lucide.Blazor, sistema CSS propio en `wwwroot/app.css`

  **Almacenamiento**: N/A (sin cambios de persistencia en esta iniciativa)

  **Pruebas**: Validación por compilación de `RealtorWeb` + validación manual visual/responsive y verificación del consumo Refit desde la página principal

  **Plataforma Objetivo**: Web app Blazor Interactive Server en entorno local y CI

  **Tipo de Proyecto**: Frontend web (`app/frontend/src/RealtorWeb`) dentro de solución fullstack

  **Objetivos de Rendimiento**: Mantener una experiencia de navegación fluida entre páginas, sin sobrescribir resultados con respuestas obsoletas y sin degradar la composición visual en desktop/tablet/móvil

  **Restricciones**:

  - Prohibido usar `HttpClient` directo en componentes Razor.
  - Prohibido llamar `RestService.For<T>()` fuera del registro DI.
  - Prohibido introducir estilos inline o colores hardcodeados en `.razor`/`.cs`.
  - Prohibido usar frameworks CSS externos.
  - Prohibido introducir lógica de negocio de propiedades fuera del alcance visual de esta spec.

  **Escala/Alcance**: 1 proyecto frontend (`RealtorWeb`) con una página principal enrutable, feature interna de listado paginado, consumo Refit y ajustes visuales en `wwwroot/app.css`.

  ## Revisión de Constitución

  *PUERTA: Debe aprobar antes de Fase 0. Revalidar tras Fase 1.*

  ### Validación previa (antes de Fase 0)

  - **Principio I, solución única y compartida**: PASS. La iniciativa modifica únicamente la capa frontend dentro de la solución existente.
  - **Principio II, spec-driven**: PASS. El plan se limita al alcance de `specs/011-blazor-list-properties/spec.md`.
  - **Principio III, arquitectura canónica**: PASS. Se mantiene Blazor Web App con Razor Components.
  - **Principio IV, stack obligatorio**: PASS. Se respeta .NET 11, Refit, CSS propio y Lucide como sistema principal de íconos.
  - **Principio V, calidad y contratos**: PASS. La feature usa contrato explícito de listado, estados de pantalla y trazabilidad visual.
  - **Principio VI, idioma**: PASS. Artefactos markdown en español.
  - **Principio VII, gobernanza**: PASS. Sin divergencias de ruta ni estructura.

  Resultado de puerta previa: **APROBADO**.

  ## Estructura del Proyecto

  ### Documentación de la feature

  ```text
  specs/011-blazor-list-properties/
  ├── plan.md
  ├── research.md
  ├── data-model.md
  ├── quickstart.md
  ├── contracts/
  │   └── frontend-properties-list.contract.yaml
  └── tasks.md
  ```

  ### Código fuente relevante

  ```text
  app/frontend/src/RealtorWeb/
  ├── Program.cs
  ├── Components/
  │   ├── App.razor
  │   ├── Routes.razor
  │   └── Pages/
  │       └── Home.razor
  ├── Features/
  │   └── Properties/
  │       └── List/
  │           ├── PropertyList.razor
  │           ├── PropertyList.razor.cs
  │           ├── PropertyCard.razor
  │           ├── PropertyListState.razor
  │           └── PropertyListModels.cs
  ├── Services/
  │   └── Api/
  │       ├── Properties/
  │       │   └── IPropertiesApi.cs
  │       ├── ServiceCollectionExtensions.cs
  │       └── RealtorApiOptions.cs
  ├── wwwroot/
  │   └── app.css
  └── RealtorWeb.csproj
  ```

No se creará una hoja CSS por componente para esta feature; toda la presentación visual del listado vive en `wwwroot/app.css`.

  **Decisión de estructura**: Mantener la página enrutable en `Components/Pages/Home.razor` y mover la experiencia del listado a una feature dedicada bajo `Features/Properties/List`, usando la infraestructura Refit ya presente en `Services/Api` y sin crear una arquitectura paralela.

  ## Fase 0: Investigación

  ### Tareas de investigación ejecutadas

  1. Patrón de carga explícita con método único para paginación, cancelación y guardia contra respuestas obsoletas.
  2. Decisión de no persistir el estado prerenderizado para esta pantalla.
  3. Composición compacta de tarjetas con jerarquía visual clara para 3 columnas.
  4. Estados reutilizables de `loading`, `empty` y `error` apoyados en `wwwroot/app.css`.
  5. Responsive explícito 3/2/1 con breakpoints estables.
  6. Consumo Refit tipado por DI sin `HttpClient` directo ni `RestService.For<T>()` fuera de registro.

  Resultado: decisiones cerradas en `research.md` sin `NEEDS CLARIFICATION`.

  ## Fase 1: Diseño y Contratos

  ### Entregables completados

  1. `data-model.md` con entidades conceptuales del listado, paginación, estados de pantalla y vista principal.
  2. `contracts/frontend-properties-list.contract.yaml` con el contrato visual y de interacción de la feature.
  3. `quickstart.md` con validaciones ejecutables para compilación, carga inicial, paginación, estados de pantalla, composición visual y consumo Refit.
  4. Actualización de contexto del agente para apuntar este plan en `.github/copilot-instructions.md`.

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

  ## Estrategia para tareas de implementación (siguiente fase)

  - Implementar primero la base de `PropertyList` y sus modelos de pantalla.
  - Conectar la carga a `IPropertiesApi` mediante DI y controlar paginación con cancelación.
  - Ajustar `Home.razor` para delegar en la feature y mantener la ruta principal enrutable.
  - Consolidar la composición visual en `app.css` y validar responsive 3/2/1.
  - Mantener trazabilidad estricta en `tasks.md` por cada cambio de archivo.
  - Validar compilación y evidencias de quickstart antes de cerrar el estado del spec.
