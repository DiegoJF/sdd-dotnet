# Plan de Implementación: Foundation Blazor Frontend

**Branch**: `feat/010-foundation-blazor-frontend` | **Fecha**: 2026-07-22 | **Spec**: `specs/010-foundation-blazor-frontend/spec.md`

**Entrada**: Especificación funcional desde `specs/010-foundation-blazor-frontend/spec.md`

## Resumen

Establecer la fundación interna del frontend `RealtorWeb` eliminando Bootstrap y cualquier framework CSS externo, consolidando `wwwroot/app.css` como fuente única del sistema visual, reemplazando el layout de plantilla por un shell desktop first con sidebar izquierda estable, integrando Lucide como sistema principal de íconos y habilitando el andamiaje base de comunicación tipada con Refit (`IHttpClientFactory`) sin endpoints de negocio.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET 11 (`net11.0`) + Razor Components (Blazor Web App)

**Dependencias Primarias**: ASP.NET Core Blazor Web App, Refit, Lucide.Blazor, sistema CSS propio en `wwwroot/app.css`

**Almacenamiento**: N/A (sin cambios de persistencia en esta iniciativa)

**Pruebas**: Validación por compilación de `RealtorWeb` + validación manual visual/responsive y verificación de recursos cargados

**Plataforma Objetivo**: Web app Blazor Server/Interactive Server en entorno local y CI

**Tipo de Proyecto**: Frontend web (`app/frontend/src/RealtorWeb`) dentro de solución fullstack

**Objetivos de Rendimiento**: Mantener tiempos de arranque equivalentes o mejores tras retirar Bootstrap; navegación base estable en desktop/tablet/móvil sin degradación perceptible

**Restricciones**:

- Prohibido cargar frameworks CSS externos.
- Prohibido usar Bootstrap Icons/Font Awesome/Fluent como sistema principal.
- Prohibido usar estilos inline y colores hardcodeados en `.razor`/`.cs`.
- Prohibido usar `HttpClient` directo en componentes y `RestService.For<T>()` fuera del registro DI.
- Sin lógica de negocio de propiedades en esta spec.

**Escala/Alcance**: 1 proyecto frontend (`RealtorWeb`) con ajuste de layout, CSS base, navegación, íconos y bootstrap de cliente API tipado.

## Revisión de Constitución

*PUERTA: Debe aprobar antes de Fase 0. Revalidar tras Fase 1.*

### Validación previa (antes de Fase 0)

- **Principio I, solución única y compartida**: PASS. La iniciativa modifica únicamente la capa frontend dentro de la solución existente.
- **Principio II, spec-driven**: PASS. El plan se limita al alcance de `specs/010-foundation-blazor-frontend/spec.md`.
- **Principio III, arquitectura canónica**: PASS. Se mantiene Blazor Web App con Razor Components.
- **Principio IV, stack obligatorio**: PASS. Se respeta .NET 11, Refit, CSS propio y Lucide como sistema principal de íconos.
- **Principio V, calidad y contratos**: PASS. Se define base de contratos frontend para consistencia y futura extensibilidad.
- **Principio VI, idioma**: PASS. Artefactos markdown en español.
- **Principio VII, gobernanza**: PASS. Sin divergencias de ruta ni estructura.

Resultado de puerta previa: **APROBADO**.

## Estructura del Proyecto

### Documentación de la feature

```text
specs/010-foundation-blazor-frontend/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── frontend-foundation.contract.yaml
└── tasks.md
```

### Código fuente relevante

```text
app/frontend/src/RealtorWeb/
├── Program.cs
├── Components/
│   ├── App.razor
│   ├── Routes.razor
│   └── Layout/
│       ├── MainLayout.razor
│       ├── MainLayout.razor.css
│       ├── NavMenu.razor
│       └── NavMenu.razor.css
├── wwwroot/
│   ├── app.css
│   └── lib/
│       └── bootstrap/            # objetivo: eliminar en esta spec
└── RealtorWeb.csproj
```

**Decisión de estructura**: Mantener la estructura de proyecto Blazor existente y concentrar la fundación en `Components/Layout`, `wwwroot/app.css`, `Program.cs` y dependencias de `RealtorWeb.csproj`, sin crear proyectos nuevos ni acoplarse a features de negocio.

## Fase 0: Investigación

### Tareas de investigación ejecutadas

1. Estrategia de retirada total de Bootstrap (`App.razor` + `wwwroot/lib/bootstrap`).
2. Estandarización de `wwwroot/app.css` como contrato visual único.
3. Diseño de shell desktop first (sidebar izquierda + main content) y adaptación responsive.
4. Selección e integración de `Lucide.Blazor` como sistema principal de íconos.
5. Estrategia de registro base Refit por módulo con `IHttpClientFactory`.
6. Definición de patrones reutilizables de estado de pantalla.

Resultado: decisiones cerradas en `research.md` sin `NEEDS CLARIFICATION`.

## Fase 1: Diseño y Contratos

### Entregables completados

1. `data-model.md` con entidades conceptuales del sistema visual, layout, navegación, íconos, estados y bootstrap de comunicación.
2. `contracts/frontend-foundation.contract.yaml` con el contrato de fundación visual y de integración frontend.
3. `quickstart.md` con validaciones ejecutables para compilación, recursos CSS, layout responsive, Lucide y Refit base.
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

- Implementar en orden: retiro de Bootstrap -> base CSS/tokens -> reemplazo layout/nav -> integración Lucide -> bootstrap Refit -> limpieza final y validación.
- Mantener trazabilidad estricta en `tasks.md` por cada cambio de archivo.
- Validar compilación y evidencias quickstart antes de cerrar estado del spec.
