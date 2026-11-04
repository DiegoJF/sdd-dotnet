# Investigación técnica: Interactividad de servidor en páginas host Blazor

## Contexto

La feature 013 busca habilitar interactividad en botones ya implementados en componentes de listado y detalle. El problema actual es de render mode (Static SSR), no de backend ni de contratos Refit.

## Decisiones

### Decisión 1: Activación granular por página host

- Decisión: Declarar `@rendermode InteractiveServer` solo en `Components/Pages/Home.razor` y `Components/Pages/PropertyDetailPage.razor`.
- Rationale: Es el punto mínimo y explícito donde se habilita circuito interactivo para capturar `@onclick` sin ampliar alcance.
- Alternatives considered:
  - Activar interactividad global en `App.razor` o `Routes`: descartado por violar alcance y criterio funcional de mantener páginas estáticas.
  - Reescribir handlers o reemplazar navegación por enlaces completos: descartado porque la lógica existente es correcta y el fallo es de render mode.

### Decisión 2: Mantener páginas estáticas explícitas

- Decisión: Mantener `Error.razor` y `NotFound.razor` en Static SSR sin `@rendermode InteractiveServer`.
- Rationale: Respuesta directa a clarificación de la spec y al principio de cambio mínimo por superficie interactiva real.
- Alternatives considered:
  - Hacer interactivas todas las páginas por consistencia: descartado por sobrealcance.
  - Definir regla dinámica de interactividad por presencia de botones: descartado por ambigüedad y baja verificabilidad.

### Decisión 3: Sin cambios de backend ni contratos

- Decisión: No modificar backend, endpoints API, contratos Refit ni modelos de datos.
- Rationale: El defecto está en la capa de presentación y el circuito SignalR; la integración API no es la causa.
- Alternatives considered:
  - Agregar endpoints o retry server-side: descartado por fuera de alcance y sin necesidad técnica.

### Decisión 4: Validación orientada a experiencia de usuario

- Decisión: Validar con escenarios end-to-end manuales para paginación, retorno y reintento sin recarga completa.
- Rationale: El comportamiento buscado es de interacción en navegador y estado visual, no de transformación de datos.
- Alternatives considered:
  - Limitar validación a compilación: descartado porque no prueba interactividad real.
  - Crear nuevas pruebas de integración backend: descartado por no aportar señal sobre el problema.

## Resultado de investigación

No quedan temas con `NEEDS CLARIFICATION`. La implementación puede avanzar con cambios puntuales en páginas host y validación funcional de UI.
