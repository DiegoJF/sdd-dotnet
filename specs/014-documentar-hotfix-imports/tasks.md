# Tasks: Documentacion retrospectiva del hotfix de imports en Features

**Input**: Documentos de diseno desde `specs/014-documentar-hotfix-imports/`

**Prerequisites**: `plan.md` (requerido), `spec.md` (requerido para historias), `research.md`, `data-model.md`, `contracts/`, `quickstart.md`

**Tests**: Se incluyen tareas de prueba porque la spec exige evidencia verificable de compilacion y pruebas automatizadas.

**Organization**: Tareas agrupadas por historia de usuario para mantener trazabilidad e independencia de validacion.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Inicializar la carpeta de trabajo de la feature retrospectiva y su base documental.

- [X] T001 Crear estructura inicial de la spec retrospectiva en specs/014-documentar-hotfix-imports/spec.md
- [X] T002 Crear checklist de calidad de requisitos en specs/014-documentar-hotfix-imports/checklists/requirements.md
- [X] T003 Actualizar feature activa para la iniciativa retrospectiva en .specify/feature.json

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Consolidar evidencia tecnica base antes de evaluar historias de usuario.

**⚠️ CRITICAL**: Ninguna historia se considera cerrada sin esta base documental y de evidencia.

- [X] T004 Documentar causa raiz, componentes afectados y solucion aplicada en specs/014-documentar-hotfix-imports/research.md
- [X] T005 [P] Modelar incidente, alcance, evidencia y riesgos residuales en specs/014-documentar-hotfix-imports/data-model.md
- [X] T006 [P] Definir contrato retrospectivo de comportamiento esperado en specs/014-documentar-hotfix-imports/contracts/frontend-hotfix-imports-retrospective.contract.yaml
- [X] T007 Registrar plan tecnico retrospectivo con decisiones y riesgos en specs/014-documentar-hotfix-imports/plan.md
- [X] T008 [P] Registrar guia de validacion y evidencia de comandos en specs/014-documentar-hotfix-imports/quickstart.md

**Checkpoint**: Base retrospectiva completa para evaluar historias y validaciones.

---

## Phase 3: User Story 1 - Continuidad de navegacion en propiedades (Priority: P1) 🎯 MVP

**Goal**: Reflejar y validar que el flujo principal de listado y detalle permanece estable con el hotfix aplicado.

**Independent Test**: Navegar manualmente listado -> detalle -> volver y verificar que no hay ruptura visual/funcional.

### Implementation for User Story 1

- [X] T009 [US1] Registrar escenarios principales de continuidad de navegacion en specs/014-documentar-hotfix-imports/spec.md
- [X] T010 [US1] Verificar presencia del hotfix de imports compartidos en app/frontend/src/RealtorWeb/Features/_Imports.razor
- [X] T011 [US1] Ejecutar validacion manual de listado y detalle y anexar evidencia explicita en specs/014-documentar-hotfix-imports/quickstart.md

**Checkpoint**: US1 documentada, validada manualmente y cerrada con evidencia.

---

## Phase 4: User Story 2 - Robustez frente a estados de carga y error (Priority: P2)

**Goal**: Confirmar de forma verificable que los estados de interfaz definidos en la spec se sostienen en uso real.

**Independent Test**: Forzar manualmente estados de carga, vacio y error y observar respuestas de interfaz sin ruptura.

### Implementation for User Story 2

- [X] T012 [US2] Documentar escenarios y casos limite de estados de interfaz en specs/014-documentar-hotfix-imports/spec.md
- [X] T013 [P] [US2] Validar manualmente estado de carga y estado vacio en flujo de propiedades y registrar evidencia en specs/014-documentar-hotfix-imports/quickstart.md
- [X] T014 [P] [US2] Validar manualmente estado de error con accion de recuperacion y registrar evidencia en specs/014-documentar-hotfix-imports/quickstart.md

**Checkpoint**: US2 validada manualmente y cerrada con evidencia de estados y recuperacion.

---

## Phase 5: User Story 3 - Confianza del equipo en la correccion aplicada (Priority: P3)

**Goal**: Aportar evidencia automatizada reproducible de estabilidad minima tras el hotfix.

**Independent Test**: Ejecutar build y test del frontend y confirmar resultados exitosos.

### Tests for User Story 3

- [X] T015 [P] [US3] Ejecutar compilacion del frontend con dotnet build en app/frontend/src/RealtorWeb/RealtorWeb.csproj
- [X] T016 [P] [US3] Ejecutar suite de pruebas frontend con dotnet test en app/frontend/test/RealtorWeb/RealtorWebTests.csproj

### Implementation for User Story 3

- [X] T017 [US3] Registrar resultados de build/test y advertencias observadas en specs/014-documentar-hotfix-imports/quickstart.md
- [X] T018 [US3] Consolidar evidencia de pruebas en el contrato retrospectivo specs/014-documentar-hotfix-imports/contracts/frontend-hotfix-imports-retrospective.contract.yaml

**Checkpoint**: US3 completada con evidencia automatizada reproducible.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Cierres de validacion y gobernanza para declarar la retrospective completa.

- [X] T019 Documentar validacion funcional manual completa de quickstart en specs/014-documentar-hotfix-imports/quickstart.md
- [X] T020 Documentar decision de seguimiento del riesgo NU1902 en specs/014-documentar-hotfix-imports/research.md
- [X] T021 Actualizar trazabilidad de estado tras aprobacion funcional en specs/014-documentar-hotfix-imports/spec.md

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: sin dependencias.
- **Foundational (Phase 2)**: depende de Setup.
- **User Stories (Phase 3+)**: dependen de Foundational.
- **Polish (Phase 6)**: depende de completar historias objetivo y consolidar evidencia final de cierre.

### User Story Dependencies

- **User Story 1 (P1)**: inicia tras Foundational; MVP documental y tecnico.
- **User Story 2 (P2)**: depende de evidencia base de US1 para validar estados sobre el mismo flujo.
- **User Story 3 (P3)**: puede ejecutarse tras Foundational y en paralelo con validaciones manuales de US1/US2.

### Within Each User Story

- Evidencia existente primero (`diff`, build, test).
- Validaciones manuales despues.
- Registro de evidencia en `quickstart.md` y contrato al cierre de cada historia.

### Parallel Opportunities

- T005, T006 y T008 pueden ejecutarse en paralelo (archivos distintos).
- T013 y T014 pueden ejecutarse en paralelo por ser validaciones manuales independientes.
- T015 y T016 pueden ejecutarse en paralelo si hay capacidad de entorno.

---

## Parallel Example: User Story 3

```bash
Task: "T015 [US3] Ejecutar compilacion del frontend con dotnet build en app/frontend/src/RealtorWeb/RealtorWeb.csproj"
Task: "T016 [US3] Ejecutar suite de pruebas frontend con dotnet test en app/frontend/test/RealtorWeb/RealtorWebTests.csproj"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Completar Setup y Foundational.
2. Confirmar US1 con T009 y T010.
3. Ejecutar T011 para cierre real de MVP con evidencia manual registrada.

### Incremental Delivery

1. Base retrospectiva completada (Fase 1 + Fase 2).
2. Cerrar US1 con evidencia manual registrada.
3. Cerrar US2 con evidencia manual de estados registrada.
4. Consolidar US3 (ya completada) y terminar Polish.

### Parallel Team Strategy

1. Persona A: T013 y T014 (validaciones manuales US2).
2. Persona B: T019 y T021 (cierre documental de trazabilidad).
3. Persona C: T020 (gestion de riesgo NU1902).

---

## Notes

- Se marco `[X]` solo en tareas verificables por diff, archivos existentes, build y test ejecutados.
- Las validaciones manuales fueron completadas y registradas retrospectivamente como evidencia de cierre.
- No se incluyen tareas de rediseno ni modificaciones de codigo fuera del alcance retrospectivo.
