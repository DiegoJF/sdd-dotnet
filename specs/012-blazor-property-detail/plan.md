# Plan de Implementación: Detalle de propiedad en frontend Blazor

**Branch**: `feat/012-blazor-property-detail` | **Fecha**: 2026-07-22 | **Spec**: `specs/012-blazor-property-detail/spec.md`

**Entrada**: Especificación funcional desde `specs/012-blazor-property-detail/spec.md`

## Resumen

Agregar una nueva ruta de detalle de propiedad en `RealtorWeb` (`/properties/{id}`), consumiendo `GET /api/properties/{id}` mediante Refit en `IPropertiesApi`, con estados explícitos de carga/contenido/error y diferenciación específica de no encontrado (404), más navegación desde cada tarjeta del listado mediante botón "Ver" y retorno al listado.

## Contexto Técnico

**Lenguaje/Versión**: C# con .NET 11 (`net11.0`) + Razor Components (Blazor Web App)

**Dependencias Primarias**: ASP.NET Core Blazor Web App, Refit, Lucide.Blazor, sistema CSS en `wwwroot/app.css`

**Storage**: N/A (sin cambios de persistencia)

**Pruebas**: Compilación de `RealtorWeb` + validaciones manuales funcionales y visuales de navegación, estados y responsive

**Plataforma Objetivo**: Web app Blazor Interactive Server

**Tipo de Proyecto**: Frontend web en `app/frontend/src/RealtorWeb`

**Objetivos de Rendimiento**: Respuesta visual estable durante carga, sin parpadeos innecesarios entre estados, y transición de navegación fluida listado → detalle → listado

**Restricciones**:

- Prohibido usar `HttpClient` directo en componentes Razor.
- Prohibido usar `RestService.For<T>()` fuera de registro DI.
- Prohibido estilos inline, colores hardcodeados o framework CSS externo.
- Prohibido modificar backend o contrato OpenAPI.
- El único cambio permitido en feature 011 es agregar navegación por botón "Ver".

**Escala/Alcance**: 1 ruta nueva en frontend, 1 slice de feature nuevo `Features/Properties/Detail`, ajustes puntuales a `Features/Properties/List` e `IPropertiesApi`, más estilos necesarios en `wwwroot/app.css`.

## Revisión de Constitución

*PUERTA: Debe aprobar antes de Fase 0. Revalidar tras Fase 1.*

### Validación previa (antes de Fase 0)

- **Principio I, solución única y compartida**: PASS. Se modifica solo frontend dentro de la solución existente.
- **Principio II, spec-driven**: PASS. El plan sigue únicamente `specs/012-blazor-property-detail/spec.md`.
- **Principio III, arquitectura canónica**: PASS. Se mantiene Blazor Web App con Razor Components y estructura por feature.
- **Principio IV, stack obligatorio**: PASS. Se mantiene .NET 11, Refit, CSS centralizado y Lucide.
- **Principio V, calidad de dominio y contratos**: PASS. Solo lógica de presentación y consumo tipado del contrato existente.
- **Principio VI, idioma**: PASS. Artefactos markdown en español.
- **Principio VII, gobernanza**: PASS. Sin divergencias de rutas o flujo.

Resultado de puerta previa: **APROBADO**.

## Estructura del Proyecto

### Documentación de la feature

```text
specs/012-blazor-property-detail/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── frontend-property-detail.contract.yaml
└── tasks.md
```

### Código fuente relevante

```text
app/frontend/src/RealtorWeb/
├── Components/
│   └── Pages/
│       ├── Home.razor
│       └── PropertyDetailPage.razor                # Nuevo enrutable /properties/{id}
├── Features/
│   └── Properties/
│       ├── List/
│       │   ├── PropertyCard.razor                  # Agrega botón "Ver"
│       │   └── PropertyListModels.cs               # Incluye Id para navegación
│       └── Detail/
│           ├── PropertyDetail.razor                # Nuevo
│           ├── PropertyDetail.razor.cs             # Nuevo
│           ├── PropertyDetailState.razor           # Nuevo
│           └── PropertyDetailModels.cs             # Nuevo (contract-first)
├── Services/
│   └── Api/
│       └── Properties/
│           └── IPropertiesApi.cs                   # Agrega GetPropertyByIdAsync
└── wwwroot/
    └── app.css                                     # Extiende estilos de detalle/placeholder/estados
```

**Decisión de estructura**: Mantener la ruta pública en `Components/Pages` y encapsular la experiencia en un slice dedicado `Features/Properties/Detail`, reutilizando la integración Refit existente de `Services/Api/Properties`.

## Fase 0: Investigación

### Tareas de investigación ejecutadas

1. Definir contrato frontend para `GetPropertyById` alineado a OpenAPI (`PropertyDetailResponse`, `400`, `404`).
2. Definir estrategia de mapeo de errores Refit para distinguir `404` de error general.
3. Definir patrón de placeholder para `imageUrl = null` con clases de estado existentes.
4. Definir navegación declarativa desde tarjeta (`Ver`) y acción de volver al listado.
5. Validar ajuste mínimo de feature 011 para soportar navegación por `id`.

Resultado: decisiones cerradas en `research.md` sin `NEEDS CLARIFICATION`.

## Fase 1: Diseño y Contratos

### Entregables completados

1. `data-model.md` con entidades conceptuales de detalle, estados de vista y navegación.
2. `contracts/frontend-property-detail.contract.yaml` con contrato de ruta, integración API, errores y reglas visuales.
3. `quickstart.md` con escenarios ejecutables de compilación, navegación, carga, 404, error general e imagen nula.
4. Actualización de contexto de agente en `.github/copilot-instructions.md` apuntando a esta iniciativa.

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
| Regla constitucional de "exactamente tres archivos" por iniciativa vs artefactos operativos de diseño (`research.md`, `data-model.md`, `quickstart.md`, `contracts/`) | El flujo operativo vigente de `speckit.plan` en este repositorio exige estos artefactos para trazabilidad de decisiones, modelo conceptual, contrato de interacción y protocolo de validación | Omitir artefactos de diseño rompe el flujo Speckit efectivo del repositorio y reduce la verificabilidad de requisitos y criterios de éxito |

## Estrategia para tareas de implementación (siguiente fase)

- Extender contratos Refit y modelos front contract-first (sin duplicaciones ad hoc).
- Implementar slice `Detail` con estados `loading`, `not-found`, `error`, `success`.
- Agregar botón "Ver" en tarjeta con navegación a `/properties/{id}`.
- Agregar acción de volver al listado desde detalle.
- Consolidar estilos en `wwwroot/app.css` con placeholder e integración responsive.
- Validar compilación y escenarios del quickstart antes de cerrar la implementación.
