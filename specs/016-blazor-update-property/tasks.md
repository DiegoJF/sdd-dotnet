# Tasks: 016-blazor-update-property

**Input**: Documentos de diseño desde `specs/016-blazor-update-property/`

**Prerequisites**: `plan.md` (requerido), `spec.md` (requerido), `research.md`, `data-model.md`, `contracts/frontend-property-update.contract.yaml`, `quickstart.md`

**Tests**: Esta spec no exige TDD ni pruebas automatizadas nuevas de forma obligatoria; se incluyen tareas de validación manual y de compilación según `quickstart.md`.

**Organización**: Las tareas se agrupan por historia de usuario para implementación y validación independiente.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Preparar artefactos base de la feature y el punto de entrada funcional en frontend.

- [X] T001 [GOV] Revisar y alinear `specs/016-blazor-update-property/spec.md`, `specs/016-blazor-update-property/plan.md` y `specs/016-blazor-update-property/contracts/frontend-property-update.contract.yaml` antes de tocar código, dejando trazabilidad de soporte para FR-016
- [X] T002 Crear la nueva página enrutable de edición en `app/frontend/src/RealtorWeb/Components/Pages/PropertyUpdatePage.razor`
- [X] T003 [P] Crear estado de pantalla de edición en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateState.razor`
- [X] T004 [P] Crear modelos base de edición en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateModels.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Habilitar infraestructura de integración y estilos comunes que bloquean todas las historias.

**⚠️ CRITICAL**: Ninguna historia de usuario debe comenzar antes de completar esta fase.

- [X] T005 Extender cliente API con operación `UpdateProperty` en `app/frontend/src/RealtorWeb/Services/Api/Properties/IPropertiesApi.cs`
- [X] T006 [P] Incorporar mapeo de errores de validación reutilizable para update en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateModels.cs`
- [X] T007 [P] Agregar estilos reutilizables de estados de carga/éxito/error para edición en `app/frontend/src/RealtorWeb/wwwroot/app.css`
- [X] T008 Crear componente principal de edición con carga inicial y estructura del formulario en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor`
- [X] T009 Implementar code-behind base (inyecciones, estado y ciclo de vida) en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`

**Checkpoint**: Infraestructura lista para implementar historias de usuario en orden de prioridad.

---

## Phase 3: User Story 1 - Iniciar edición desde el listado (Priority: P1) 🎯 MVP

**Goal**: Permitir que la persona operadora acceda a la edición del inmueble correcto desde cada tarjeta del listado.

**Independent Test**: Desde el listado, accionar Editar en una tarjeta y verificar navegación a `/properties/{id}/edit` con carga inicial del inmueble.

### Implementation for User Story 1

- [X] T010 [US1] Agregar acción Editar en la tarjeta de listado en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyCard.razor`
- [X] T011 [US1] Vincular ruta de edición con parámetro `Id` en `app/frontend/src/RealtorWeb/Components/Pages/PropertyUpdatePage.razor`
- [X] T012 [US1] Implementar lectura inicial del inmueble mediante `GetPropertyByIdAsync` en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`
- [X] T013 [US1] Renderizar estados visuales de carga y contenido listo para edición en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor`
- [X] T014 [US1] Implementar fallback de carga fallida con acciones Reintentar y Volver al listado en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor` y `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`

**Checkpoint**: US1 funcional y comprobable de forma independiente.

---

## Phase 4: User Story 2 - Guardar cambios con validación y feedback (Priority: P1)

**Goal**: Permitir edición de campos, validaciones claras y guardado exitoso con confirmación y redirección al detalle.

**Independent Test**: Editar datos válidos, guardar, observar confirmación y navegación automática a `/properties/{id}`.

### Implementation for User Story 2

- [X] T015 [P] [US2] Definir modelo de formulario de edición con DataAnnotations en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateModels.cs`
- [X] T016 [US2] Construir campos editables y mensajes por campo en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor`
- [X] T017 [US2] Implementar validación en tiempo real y bloqueo de envío con errores en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`
- [X] T018 [US2] Implementar envío `UpdateProperty` con body compatible a contrato en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`
- [X] T019 [US2] Implementar estado de guardado en curso y prevención de clics duplicados en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor` y `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`
- [X] T020 [US2] Implementar confirmación de éxito y redirección al detalle actualizado en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`

**Checkpoint**: US2 funcional y comprobable de forma independiente.

---

## Phase 5: User Story 3 - Manejo de errores y conservación de datos (Priority: P2)

**Goal**: Manejar errores de carga/guardado de forma comprensible manteniendo datos ingresados para reintento.

**Independent Test**: Forzar errores `400`, `404`, `413`, `415` y `500` y verificar mensajes adecuados + conservación de datos.

### Implementation for User Story 3

- [X] T021 [US3] Implementar mapeo de `400 BadRequest` a errores por campo en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`
- [X] T022 [US3] Implementar manejo de `404 NotFound` en guardado con mensaje comprensible y continuidad de edición en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`
- [X] T023 [US3] Implementar manejo de `413` y `415` con mensajes globales de archivo/medio en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`
- [X] T024 [US3] Implementar manejo de `500` y error inesperado con estado recuperable en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`
- [X] T025 [US3] Verificar preservación de valores del formulario tras errores en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor` y `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`

**Checkpoint**: US3 funcional y comprobable de forma independiente.

---

## Phase 6: User Story 4 - Conservación o reemplazo opcional de imagen (Priority: P2)

**Goal**: Conservar imagen actual si no se reemplaza y permitir reemplazo opcional válido durante la edición.

**Independent Test**: Guardar sin nueva imagen (conserva actual) y guardar con nueva imagen válida (reemplaza correctamente).

### Implementation for User Story 4

- [X] T026 [P] [US4] Agregar estado de imagen actual/reemplazo en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateModels.cs`
- [X] T027 [US4] Renderizar bloque de imagen con vista actual y selección opcional de reemplazo en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor`
- [X] T028 [US4] Implementar validación de archivo seleccionado y preview opcional en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`
- [X] T029 [US4] Ajustar envío para no incluir archivo cuando no haya reemplazo y conservar imagen existente en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`
- [X] T030 [US4] Implementar reemplazo de imagen al guardar con archivo válido en `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor.cs`

**Checkpoint**: US4 funcional y comprobable de forma independiente.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Cierre de documentación, validación integral y consistencia de artefactos.

- [X] T031 [P] Actualizar guía de validación con evidencias reales en `specs/016-blazor-update-property/quickstart.md`
- [X] T032 Ejecutar compilación de frontend para validar integración (`dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj`) y registrar resultado en `specs/016-blazor-update-property/quickstart.md`
- [X] T033 [P] Ejecutar pruebas frontend existentes (`dotnet test app/frontend/test/RealtorWeb/RealtorWebTests.csproj`) y registrar resultado en `specs/016-blazor-update-property/quickstart.md`
- [X] T034 Revisar trazabilidad final entre requisitos FR/RV/SC y tareas en `specs/016-blazor-update-property/spec.md` y `specs/016-blazor-update-property/tasks.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: inicia de inmediato.
- **Phase 2 (Foundational)**: depende de Phase 1 y bloquea todas las historias.
- **Phase 3-6 (User Stories)**: dependen de completar Phase 2.
- **Phase 7 (Polish)**: depende de completar las historias objetivo.

### User Story Dependencies

- **US1 (P1)**: inicia tras Foundational, sin dependencia de otras historias.
- **US2 (P1)**: inicia tras Foundational y usa la navegación/carga establecida en US1.
- **US3 (P2)**: inicia tras Foundational y depende del flujo de guardado implementado en US2.
- **US4 (P2)**: inicia tras Foundational y depende del flujo de guardado implementado en US2.

### Within Each User Story

- Definición de modelos/estado antes de handlers complejos.
- UI base antes de integración de guardado.
- Manejo de errores después de tener guardado funcional.
- Validación de story completa antes de pasar a la siguiente prioridad.

## Parallel Opportunities

- `T003` y `T004` pueden ejecutarse en paralelo.
- `T006` y `T007` pueden ejecutarse en paralelo.
- En US2, `T015` puede avanzar en paralelo con ajustes visuales previos a `T018`.
- En US4, `T026` puede avanzar en paralelo con estructura visual de `T027`.
- En fase de cierre, `T031` y `T033` pueden ejecutarse en paralelo.

## Parallel Example: User Story 2

```bash
# Trabajo paralelo recomendado en US2
T015 [P] [US2] Definir modelo de formulario de edición
T016 [US2] Construir campos editables y mensajes por campo

# Tras completar ambos, continuar con:
T017 [US2] Validación en tiempo real y bloqueo de envío
T018 [US2] Integración UpdateProperty
```

## Implementation Strategy

### MVP First (US1 + US2)

1. Completar Phase 1 (Setup).
2. Completar Phase 2 (Foundational).
3. Implementar US1.
4. Implementar US2.
5. Validar flujo end-to-end mínimo: editar desde listado, guardar válido, redirigir a detalle.

### Incremental Delivery

1. Entrega 1 (MVP): US1 + US2.
2. Entrega 2: US3 (robustez de errores y conservación de datos).
3. Entrega 3: US4 (imagen opcional y reemplazo controlado).
4. Entrega final: fase Polish con evidencias de quickstart, build y tests.

### Parallel Team Strategy

1. Equipo A: navegación/ruta/estructura base (US1).
2. Equipo B: validación y guardado (US2).
3. Equipo C: manejo de errores (US3) y comportamiento de imagen (US4) tras estabilizar US2.

## Notes

- Formato aplicado: `- [ ] T### [P?] [US?] Descripción con ruta de archivo`.
- No se incluyen tareas de backend ni regeneración OpenAPI porque esta spec no modifica endpoints.
- Toda tarea mantiene trazabilidad directa con requisitos FR/RV/SC de la spec 016.
