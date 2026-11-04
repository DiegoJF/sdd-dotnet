# Tareas: Corrección de interactividad Blazor en servidor

**Input**: Documentos de diseño desde specs/013-signalr-interactive-server/

**Prerequisites**: plan.md (requerido), spec.md (requerido para historias), research.md, data-model.md, contracts/, quickstart.md

**Tests**: La spec no solicita pruebas automatizadas; la validación requerida es compilación de frontend y verificación manual de escenarios de interacción.

**Organization**: Las tareas se agrupan por historia de usuario para permitir implementación y validación independientes por incremento.

## Formato: [ID] [P?] [Story] Descripción

- [P]: Puede ejecutarse en paralelo (archivos distintos, sin dependencia directa)
- [Story]: Etiqueta de historia (US1, US2, US3) solo en fases de historias
- Todas las tareas incluyen ruta de archivo explícita

## Fase 1: Setup (inicialización)

**Purpose**: Preparar contexto técnico y puntos de edición para el cambio mínimo

- [X] T001 Verificar configuración existente de Interactive Server en app/frontend/src/RealtorWeb/Program.cs antes de editar páginas host
- [X] T002 [P] Confirmar rutas y hosts objetivo en specs/013-signalr-interactive-server/contracts/frontend-interactive-server-render-mode.contract.yaml
- [X] T003 [P] Confirmar criterios de validación manual en specs/013-signalr-interactive-server/quickstart.md

---

## Fase 2: Foundational (prerrequisitos bloqueantes)

**Purpose**: Proteger el alcance para evitar activación global o regresiones en páginas estáticas

**CRITICAL**: Ninguna historia debe cerrarse sin completar esta fase

- [X] T004 Validar que app/frontend/src/RealtorWeb/Components/App.razor no habilite render mode interactivo global
- [X] T005 [P] Validar que app/frontend/src/RealtorWeb/Components/Routes.razor no habilite render mode interactivo global
- [X] T006 [P] Validar que app/frontend/src/RealtorWeb/Components/Pages/Error.razor permanezca en Static SSR sin @rendermode
- [X] T007 [P] Validar que app/frontend/src/RealtorWeb/Components/Pages/NotFound.razor permanezca en Static SSR sin @rendermode

**Checkpoint**: Alcance protegido y sin riesgo de sobre-activación global.

---

## Fase 3: User Story 1 - Paginación interactiva del listado (Priority: P1) 🎯 MVP

**Goal**: Activar interacción en la página de listado para que Anterior/Siguiente funcionen sin recarga completa

**Independent Test**: Abrir /, pulsar Siguiente y Anterior, verificar actualización de UI sin recarga y estado disabled correcto

- [X] T008 [US1] Declarar @rendermode InteractiveServer en app/frontend/src/RealtorWeb/Components/Pages/Home.razor
- [X] T009 [US1] Verificar que la lógica de CanGoPrevious y CanGoNext se conserve sin cambios funcionales en app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor.cs
- [X] T010 [US1] Registrar evidencia del escenario de paginación interactiva en specs/013-signalr-interactive-server/quickstart.md incluyendo verificación explícita de estado disabled para Anterior/Siguiente según CanGoPrevious/CanGoNext

**Checkpoint**: US1 funcional y validable de forma independiente.

---

## Fase 4: User Story 2 - Retorno desde detalle al listado (Priority: P2)

**Goal**: Activar interacción en detalle para que Volver al listado funcione en success, not-found y error

**Independent Test**: Abrir /properties/{id} en estados success/not-found/error y validar retorno a / en una acción

- [X] T011 [US2] Declarar @rendermode InteractiveServer en app/frontend/src/RealtorWeb/Components/Pages/PropertyDetailPage.razor
- [X] T012 [US2] Verificar que la acción Volver al listado siga disponible en estados success, not-found y error en app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetailState.razor
- [X] T013 [US2] Registrar evidencia del retorno al listado en specs/013-signalr-interactive-server/quickstart.md

**Checkpoint**: US2 funcional y validable de forma independiente.

---

## Fase 5: User Story 3 - Reintento funcional en estados de error (Priority: P3)

**Goal**: Confirmar que Reintentar relanza carga de datos en listado y detalle usando circuito interactivo

**Independent Test**: Forzar error en listado y detalle, pulsar Reintentar y verificar nueva ejecución de carga

- [X] T014 [P] [US3] Verificar reintento de listado sobre app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor.cs con host interactivo en app/frontend/src/RealtorWeb/Components/Pages/Home.razor
- [X] T015 [P] [US3] Verificar reintento de detalle sobre app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor.cs con host interactivo en app/frontend/src/RealtorWeb/Components/Pages/PropertyDetailPage.razor
- [X] T016 [US3] Registrar evidencia de reintentos en specs/013-signalr-interactive-server/quickstart.md

**Checkpoint**: US3 funcional y validable de forma independiente.

---

## Fase 6: Polish & Cross-Cutting

**Purpose**: Cerrar validación integral y no regresión de alcance

- [X] T017 [P] Ejecutar compilación de verificación en app/frontend/src/RealtorWeb/RealtorWeb.csproj mediante dotnet build
- [X] T018 [P] Confirmar no cambios funcionales en backend revisando app/backend/src/ y registrar resultado en specs/013-signalr-interactive-server/quickstart.md
- [X] T019 [P] Confirmar no cambios en contrato Refit revisando app/frontend/src/RealtorWeb/Services/Api/Properties/IPropertiesApi.cs y registrar resultado en specs/013-signalr-interactive-server/quickstart.md
- [X] T020 Revalidación final de cierre (no duplicada): confirmar nuevamente que no hubo activación interactiva global tras completar todas las historias, verificando app/frontend/src/RealtorWeb/Components/App.razor y app/frontend/src/RealtorWeb/Components/Routes.razor, y actualizar la traza de validación final en specs/013-signalr-interactive-server/quickstart.md con fecha, comandos y resultados

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup (Fase 1): Sin dependencias
- Foundational (Fase 2): Depende de Setup y bloquea historias
- User Stories (Fases 3-5): Dependen de Foundational
- Polish (Fase 6): Depende de historias completadas

### User Story Dependencies

- US1 (P1): Puede iniciar tras Foundational, entrega MVP
- US2 (P2): Puede iniciar tras Foundational, independiente de US1 a nivel de implementación
- US3 (P3): Depende de que Home y PropertyDetailPage ya estén en InteractiveServer (resultado de US1 y US2)

### Dependency Graph

- US1 -> US3
- US2 -> US3

---

## Parallel Opportunities

- Setup: T002 y T003
- Foundational: T005, T006 y T007
- US1: Sin paralelismo recomendado (tareas secuenciales para evitar validación prematura)
- US2: Sin paralelismo recomendado (validación tras activar host)
- US3: T014 y T015
- Polish: T017, T018 y T019

## Parallel Example: User Story 1

- Ejecutar en secuencia T008 -> T009 -> T010 sobre app/frontend/src/RealtorWeb/Components/Pages/Home.razor y app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor.cs

## Parallel Example: User Story 2

- Ejecutar en secuencia T011 -> T012 -> T013 sobre app/frontend/src/RealtorWeb/Components/Pages/PropertyDetailPage.razor y app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetailState.razor

## Parallel Example: User Story 3

- Ejecutar en paralelo T014 y T015, luego cerrar con T016 en specs/013-signalr-interactive-server/quickstart.md

---

## Implementation Strategy

### MVP First (US1 only)

1. Completar Fase 1
2. Completar Fase 2
3. Completar Fase 3 (US1)
4. Validar criterio independiente de US1

### Incremental Delivery

1. Foundation completa (Fases 1-2)
2. Entregar US1 (MVP)
3. Entregar US2
4. Entregar US3
5. Cierre con Fase 6

### Team Parallel Strategy

1. Un frente valida Foundational (T004-T007)
2. Otro frente prepara US1 y US2
3. Tras activar ambos hosts, cerrar US3 y Polish

---

## Notes

- Cada tarea sigue formato checklist estricto con ID secuencial
- No se incluyen pruebas automáticas porque no fueron solicitadas por la spec
- El alcance debe permanecer en frontend host pages sin cambios backend ni global render mode
