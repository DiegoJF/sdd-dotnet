# Plan de Implementación: Alta de inmuebles en frontend Blazor

**Branch**: `015-blazor-create-property` | **Fecha**: 2026-07-25 | **Spec**: `specs/015-blazor-create-property/spec.md`

**Entrada**: Especificación funcional desde `specs/015-blazor-create-property/spec.md`

## Resumen

Implementar una página de alta de inmuebles en `RealtorWeb` con formulario validado en frontend y alineado a backend, imagen opcional con vista previa/reemplazo/eliminación, manejo diferenciado de errores (`400`, `413`, `415`, `500`) conservando datos para reintento, y redirección automática al detalle del inmueble creado tras `201` de `CreateProperty`.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET 11 (`net11.0`) + Razor Components (Blazor Web App)

**Dependencias Primarias**: ASP.NET Core Blazor Web App, Refit, Refit.HttpClientFactory, Lucide.Blazor, CSS central en `wwwroot/app.css`

**Storage**: N/A (sin cambios de persistencia)

**Pruebas**: Compilación de `RealtorWeb` + validación manual funcional de escenarios `201`, `400`, `413`, `415`, `500`

**Plataforma Objetivo**: Web app Blazor Interactive Server

**Tipo de Proyecto**: Frontend web en `app/frontend/src/RealtorWeb`

**Objetivos de Rendimiento**: Feedback inmediato de validación en formulario y transición de navegación fluida hacia detalle al completar alta exitosa

**Restricciones**:

- Prohibido usar `HttpClient` directo en componentes Razor.
- Prohibido usar `RestService.For<T>()` fuera de registro DI.
- Prohibido estilos inline, colores hardcodeados o framework CSS externo.
- Prohibido introducir cambios de backend, dominio o persistencia en esta iniciativa.
- Prohibido expandir alcance a edición, eliminación, alta masiva o múltiples imágenes.

**Escala/Alcance**: 1 ruta nueva en frontend para alta, 1 slice funcional `Features/Properties/Create`, ajustes en cliente API tipado para creación, y estilos puntuales en `wwwroot/app.css`.

## Revisión de Constitución

*PUERTA: Debe aprobar antes de Fase 0. Revalidar tras Fase 1.*

### Validación previa (antes de Fase 0)

- **Principio I, solución única y compartida**: PASS. La iniciativa modifica únicamente frontend dentro de la solución existente.
- **Principio II, spec-driven**: PASS. El plan deriva exclusivamente de `specs/015-blazor-create-property/spec.md`.
- **Principio III, arquitectura canónica**: PASS. Se mantiene Blazor Web App y estructura por feature sin controllers ni patrones fuera de estándar.
- **Principio IV, stack obligatorio**: PASS. Se respeta .NET 11, Refit, Lucide y CSS propio.
- **Principio V, calidad de dominio y contratos**: PASS. Solo se implementa lógica de presentación y mapeo de errores de contrato.
- **Principio VI, idioma**: PASS. Artefactos markdown en español.
- **Principio VII, gobernanza y divergencias**: PASS. Sin divergencias detectadas.
- **Principio IX, calidad y verificabilidad**: PASS. Requisitos y criterios definidos en términos observables y medibles.

Resultado de puerta previa: **APROBADO**.

## Estructura del Proyecto

### Documentación de la feature

```text
specs/015-blazor-create-property/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── frontend-property-create.contract.yaml
└── tasks.md
```

### Código fuente relevante

```text
app/frontend/src/RealtorWeb/
├── Components/
│   └── Pages/
│       └── PropertyCreatePage.razor              # Nueva ruta de alta
├── Features/
│   └── Properties/
│       └── Create/
│           ├── PropertyCreateForm.razor          # Nuevo
│           ├── PropertyCreateForm.razor.cs       # Nuevo
│           ├── PropertyCreateModels.cs           # Nuevo (contract-first)
│           └── PropertyCreateState.razor         # Nuevo
├── Services/
│   └── Api/
│       └── Properties/
│           └── IPropertiesApi.cs                 # Operación CreateProperty (si falta)
└── wwwroot/
    └── app.css                                   # Extensión de estilos de formulario/errores/preview
```

**Decisión de estructura**: Mantener la ruta enrutable en `Components/Pages` y concentrar experiencia de alta en `Features/Properties/Create`, reutilizando integración Refit por DI en `Services/Api/Properties`.

## Fase 0: Investigación

### Tareas de investigación ejecutadas

1. Definir forma canónica de integrar `CreateProperty` con Refit y contenido `multipart/form-data`.
2. Definir estrategia de validación en frontend equivalente a backend con feedback por campo y resumen.
3. Definir ciclo de imagen opcional: vista previa, validación temprana, quitar/reemplazar.
4. Definir mapeo funcional de errores por familia de respuesta (`400`, `413`, `415`, `500`) con conservación de datos.
5. Definir cierre de flujo exitoso con redirección automática al detalle del recurso creado.

Resultado: decisiones cerradas en `research.md` sin `NEEDS CLARIFICATION`.

## Fase 1: Diseño y Contratos

### Entregables completados

1. `data-model.md` con entidades conceptuales de formulario, validación, imagen opcional y resultado de alta.
2. `contracts/frontend-property-create.contract.yaml` con contrato de ruta, integración API, errores y reglas de UI.
3. `quickstart.md` con escenarios ejecutables para éxito y errores contractuales, incluyendo conservación de datos y redirección.
4. Actualización de contexto de agente en `.github/copilot-instructions.md` apuntando a esta iniciativa.

## Revalidación de Constitución (post-diseño)

- **Principio I**: PASS
- **Principio II**: PASS
- **Principio III**: PASS
- **Principio IV**: PASS
- **Principio V**: PASS
- **Principio VI**: PASS
- **Principio VII**: PASS
- **Principio IX**: PASS

Resultado de puerta post-diseño: **APROBADO**.

## Seguimiento de Complejidad

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Regla de "tres archivos mínimos" frente a artefactos operativos adicionales (`research.md`, `data-model.md`, `quickstart.md`, `contracts/`) | El flujo Speckit de planificación del repositorio exige trazabilidad de decisiones, modelo conceptual, contrato funcional y guía de validación | Omitir artefactos reduce verificabilidad y rompe la salida esperada de `/speckit.plan` |

## Estrategia para tareas de implementación (siguiente fase)

- Implementar ruta de alta y componentes de formulario dentro del slice `Create`.
- Integrar validación en tiempo real y resumen de envío con reglas equivalentes a backend.
- Implementar UX de imagen opcional con vista previa, reemplazo y eliminación.
- Mapear respuestas `201`, `400`, `413`, `415`, `500` a estados de UI definidos en contrato.
- Redirigir al detalle del inmueble creado tras alta exitosa.
- Verificar escenarios del `quickstart.md` antes de cerrar implementación.
