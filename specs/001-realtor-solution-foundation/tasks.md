# Tasks: Fundación de la Solución Realtor

**Input**: Documentos de diseño desde `/specs/001-realtor-solution-foundation/`

**Prerequisites**: plan.md (requerido), spec.md (requerido), research.md, data-model.md, contracts/, quickstart.md

**Tests**: Se incluyen tareas de pruebas unitarias base porque la spec exige proyectos de test para backend y frontend (RF-003, RF-005) y verificación ejecutable de la solución base (CE-002).

**Organization**: Las tareas se agrupan por historia de usuario para permitir implementación y validación independiente.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos distintos, sin dependencia de tarea incompleta)
- **[Story]**: Historia asociada (`[US1]`, `[US2]`, `[US3]`)
- Cada tarea incluye ruta de archivo específica

## Phase 1: Setup (Inicialización)

**Purpose**: Crear base mínima del workspace y confirmar baseline de plataforma

- [X] T001 Verificar baseline de SDK en global.json
- [X] T002 Crear estructura de carpetas base en app/backend/src/RealtorApi/.gitkeep
- [X] T003 [P] Crear estructura de carpetas base en app/backend/tests/RealtorApiTests/.gitkeep
- [X] T004 [P] Crear estructura de carpetas base en app/frontend/src/RealtorWeb/.gitkeep
- [X] T005 [P] Crear estructura de carpetas base en app/frontend/test/RealtorWeb/.gitkeep
- [X] T006 Crear solución principal vacía en app/Realtor.sln

---

## Phase 2: Foundational (Prerequisitos Bloqueantes)

**Purpose**: Infraestructura común obligatoria antes de implementar historias de usuario

**⚠️ CRITICAL**: Ninguna historia debe comenzar antes de completar esta fase

- [X] T007 Inicializar proyecto backend base en app/backend/src/RealtorApi/RealtorApi.csproj
- [X] T008 Inicializar proyecto frontend base en app/frontend/src/RealtorWeb/RealtorWeb.csproj
- [X] T009 [P] Inicializar proyecto de pruebas backend en app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
- [X] T010 [P] Inicializar proyecto de pruebas frontend en app/frontend/test/RealtorWeb/RealtorWebTests.csproj
- [X] T011 Agregar proyecto backend y frontend a la solución en app/Realtor.sln
- [X] T012 Agregar proyectos de pruebas a la solución en app/Realtor.sln
- [X] T013 Configurar referencias mínimas de prueba en app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
- [X] T014 Configurar referencias mínimas de prueba en app/frontend/test/RealtorWeb/RealtorWebTests.csproj

**Checkpoint**: Base técnica lista para implementar historias

---

## Phase 3: User Story 1 - Crear la base única de solución (Priority: P1) 🎯 MVP

**Goal**: Dejar una solución única con backend, frontend y pruebas en rutas canónicas

**Independent Test**: La solución en app/Realtor.sln contiene exactamente 4 proyectos base y compila en una ejecución completa

### Implementation for User Story 1

- [X] T015 [US1] Crear archivo de arranque backend mínimo en app/backend/src/RealtorApi/Program.cs
- [X] T016 [P] [US1] Crear archivo de arranque frontend base en app/frontend/src/RealtorWeb/Program.cs
- [X] T017 [P] [US1] Crear prueba unitaria de humo backend en app/backend/tests/RealtorApiTests/FoundationSmokeTests.cs
- [X] T018 [P] [US1] Crear prueba unitaria de humo frontend en app/frontend/test/RealtorWeb/FoundationSmokeTests.cs
- [X] T019 [US1] Validar composición de solución y actualizar guía de verificación en specs/001-realtor-solution-foundation/quickstart.md
- [X] T020 [US1] Ejecutar build de verificación y registrar estado de compilación en specs/001-realtor-solution-foundation/data-model.md

**Checkpoint**: US1 funcional y verificable de forma independiente (MVP)

---

## Phase 4: User Story 2 - Asegurar arranque técnico mínimo sin negocio (Priority: P2)

**Goal**: Restringir backend a configuración base, middleware y endpoint técnico mínimo sin dominio

**Independent Test**: El backend solo contiene arranque técnico mínimo y prueba de disponibilidad sin lógica de negocio

### Implementation for User Story 2

- [X] T021 [US2] Limitar configuración de backend a servicios/middleware base en app/backend/src/RealtorApi/Program.cs
- [X] T022 [P] [US2] Implementar endpoint técnico GET /health en app/backend/src/RealtorApi/Program.cs
- [X] T023 [P] [US2] Configurar logging estructurado mínimo en app/backend/src/RealtorApi/appsettings.json
- [X] T024 [US2] Alinear contrato HTTP técnico con implementación en specs/001-realtor-solution-foundation/contracts/backend-bootstrap.openapi.yaml
- [X] T025 [US2] Crear prueba unitaria del endpoint técnico en app/backend/tests/RealtorApiTests/HealthEndpointTests.cs
- [X] T026 [US2] Documentar verificación de ausencia de lógica de negocio en specs/001-realtor-solution-foundation/research.md

**Checkpoint**: US2 funcional y verificable de forma independiente

---

## Phase 5: User Story 3 - Dejar base lista para evolución por specs (Priority: P3)

**Goal**: Dejar explícitas reglas y validaciones para evolución incremental sin reestructuración

**Independent Test**: Una nueva iniciativa puede reutilizar app/Realtor.sln y sus rutas sin crear segunda solución

### Implementation for User Story 3

- [X] T027 [US3] Documentar estrategia de evolución incremental por specs en specs/001-realtor-solution-foundation/plan.md
- [X] T028 [P] [US3] Consolidar reglas de idempotencia y no sobreescritura en specs/001-realtor-solution-foundation/research.md
- [X] T029 [P] [US3] Confirmar trazabilidad de entidades estructurales en specs/001-realtor-solution-foundation/data-model.md
- [X] T030 [US3] Expandir checklist de verificación de transición a siguiente iniciativa en specs/001-realtor-solution-foundation/checklists/requirements.md

**Checkpoint**: US3 funcional y verificable de forma independiente

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Ajustes finales de consistencia transversal

- [X] T031 [P] Normalizar formato final de guía de validación en specs/001-realtor-solution-foundation/quickstart.md
- [X] T032 [P] Revisar consistencia final de contrato técnico en specs/001-realtor-solution-foundation/contracts/backend-bootstrap.openapi.yaml
- [X] T033 Ejecutar validación completa de escenarios y registrar resultados en specs/001-realtor-solution-foundation/quickstart.md

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Sin dependencias, inicia inmediatamente
- **Foundational (Phase 2)**: Depende de Phase 1, bloquea todas las historias
- **User Stories (Phase 3+)**: Dependen de completar Phase 2
- **Polish (Phase 6)**: Depende de completar historias seleccionadas

### User Story Dependencies

- **US1 (P1)**: Inicia tras Foundational, sin dependencia de US2/US3
- **US2 (P2)**: Inicia tras Foundational, independiente de US1 salvo artefactos base compartidos
- **US3 (P3)**: Inicia tras Foundational, puede ejecutarse en paralelo con US2 si US1 ya consolidó estructura

### Within Each User Story

- Estructura/arranque base antes de validaciones
- Contrato/documentación alineada después de implementación técnica
- Pruebas unitarias antes del cierre de la historia

---

## Parallel Opportunities

- **Setup**: T003, T004 y T005 en paralelo
- **Foundational**: T009 y T010 en paralelo; T013 y T014 en paralelo
- **US1**: T016, T017 y T018 en paralelo
- **US2**: T022 y T023 en paralelo
- **US3**: T028 y T029 en paralelo
- **Polish**: T031 y T032 en paralelo

---

## Parallel Example: User Story 1

```bash
# Ejecutar en paralelo dentro de US1:
T016 -> app/frontend/src/RealtorWeb/Program.cs
T017 -> app/backend/tests/RealtorApiTests/FoundationSmokeTests.cs
T018 -> app/frontend/test/RealtorWeb/FoundationSmokeTests.cs
```

## Parallel Example: User Story 2

```bash
# Ejecutar en paralelo dentro de US2:
T022 -> app/backend/src/RealtorApi/Program.cs
T023 -> app/backend/src/RealtorApi/appsettings.json
```

## Parallel Example: User Story 3

```bash
# Ejecutar en paralelo dentro de US3:
T028 -> specs/001-realtor-solution-foundation/research.md
T029 -> specs/001-realtor-solution-foundation/data-model.md
```

---

## Implementation Strategy

### MVP First (Solo US1)

1. Completar Phase 1 y Phase 2
2. Completar US1 (Phase 3)
3. Validar build y composición de solución
4. Detener para demo/aceptación MVP

### Incremental Delivery

1. Base técnica (Phase 1 + 2)
2. US1 (MVP estructural)
3. US2 (arranque mínimo sin negocio)
4. US3 (preparación para escalado por specs)
5. Polish final

### Parallel Team Strategy

1. Equipo completo en Phase 1 + 2
2. Luego distribuir historias por desarrollador:
   - Dev A: US1
   - Dev B: US2
   - Dev C: US3
3. Integración y cierre con Phase 6

---

## Notes

- Tareas con `[P]` no deben editar el mismo archivo en paralelo
- Cada historia mantiene criterio de prueba independiente
- Toda implementación debe respetar límites foundation: sin negocio, sin entidades de dominio
- Marcar cada tarea como `[X]` únicamente tras verificación real
