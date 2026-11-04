# Tasks: Fundación Backend Base

**Input**: Documentos de diseño desde /specs/002-foundation-backend/

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Se incluyen tareas de pruebas porque la spec exige pruebas unitarias de infraestructura en app/backend/tests/RealtorApiTests.

**Organization**: Las tareas están agrupadas por historia de usuario para permitir implementación y validación independiente por incremento.

## Formato: [ID] [P?] [Story] Descripción

- [P]: Puede ejecutarse en paralelo (archivos distintos, sin dependencia de tareas incompletas)
- [Story]: Historia de usuario asociada (US1, US2, US3)
- Todas las tareas incluyen rutas de archivo concretas

## Fase 1: Setup (Infraestructura compartida)

**Purpose**: Preparar estructura base y dependencias técnicas para implementar slices, validación y errores consistentes.

- [X] T001 Crear estructura de carpetas base en app/backend/src/RealtorApi/Infrastructure/{Endpoints,Handlers,Slices,Validation,Results}
- [X] T002 Crear estructura de carpetas de pruebas en app/backend/tests/RealtorApiTests/Infrastructure/{Slices,Validation,Results,Endpoints}
- [X] T003 Agregar FluentValidation en app/backend/src/RealtorApi/RealtorApi.csproj

---

## Fase 2: Foundational (Prerequisitos bloqueantes)

**Purpose**: Cableado mínimo común que debe existir antes de implementar cualquier historia.

**CRITICAL**: Ninguna historia inicia hasta completar esta fase.

- [X] T004 Crear extensión de bootstrap de infraestructura en app/backend/src/RealtorApi/Infrastructure/DependencyInjection.cs
- [X] T005 Crear contrato base de slice en app/backend/src/RealtorApi/Infrastructure/Slices/ISlice.cs
- [X] T006 Crear marker interface de handlers en app/backend/src/RealtorApi/Infrastructure/Handlers/IHandler.cs
- [X] T007 Crear punto central de mapeo de endpoints en app/backend/src/RealtorApi/Infrastructure/Endpoints/EndpointMappingExtensions.cs
- [X] T008 Actualizar app/backend/src/RealtorApi/Program.cs para limitarlo a configuración de servicios, middleware y mapeo centralizado
- [X] T009 [P] Crear fixture reutilizable de pruebas de API en app/backend/tests/RealtorApiTests/Infrastructure/TestWebApplicationFactory.cs

**Checkpoint**: Foundation lista para implementar historias de usuario.

---

## Fase 3: Historia de Usuario 1 - Base transversal extensible sin tocar Program (Prioridad: P1) MVP

**Goal**: Habilitar descubrimiento y mapeo automático de slices y auto-registro de handlers sin registros manuales.

**Independent Test**: RegisterSlices descubre una clase ISlice de prueba definida en tests, la registra bajo ISlice y evita duplicados.

### Tests for User Story 1

- [X] T010 [P] [US1] Crear slices de prueba para discovery en app/backend/tests/RealtorApiTests/Infrastructure/Slices/RegisterSlicesTestFixtures.cs
- [X] T011 [US1] Implementar pruebas de descubrimiento y no duplicados en app/backend/tests/RealtorApiTests/Infrastructure/Slices/RegisterSlicesTests.cs

### Implementación User Story 1

- [X] T012 [US1] Implementar registro por assembly scanning de ISlice en app/backend/src/RealtorApi/Infrastructure/Slices/SliceRegistrationExtensions.cs
- [X] T013 [US1] Implementar mapeo centralizado de endpoints de slices en app/backend/src/RealtorApi/Infrastructure/Slices/SliceEndpointMappingExtensions.cs
- [X] T014 [US1] Implementar auto-registro por marker IHandler en app/backend/src/RealtorApi/Infrastructure/Handlers/HandlerRegistrationExtensions.cs
- [X] T015 [US1] Integrar registro de slices y handlers en app/backend/src/RealtorApi/Infrastructure/DependencyInjection.cs
- [X] T016 [US1] Ajustar app/backend/src/RealtorApi/Program.cs para no requerir cambios al agregar nuevas slices

**Checkpoint**: Nuevas slices y handlers pueden agregarse sin modificar Program.cs.

---

## Fase 4: Historia de Usuario 2 - Validación automática y manejo consistente de errores (Prioridad: P2)

**Goal**: Implementar validación por convención con pass-through sin validator y mapeo uniforme de errores esperados a ProblemDetails.

**Independent Test**: ValidationFilterFactory valida cuando existe IValidator<T>, pasa sin error cuando no existe, retorna ValidationProblemDetails 400 ante fallo y mapea Result a ProblemDetails correctamente.

### Tests for User Story 2

- [X] T017 [P] [US2] Crear dobles de request/validator para pruebas en app/backend/tests/RealtorApiTests/Infrastructure/Validation/ValidationFilterFactoryTestFixtures.cs
- [X] T018 [US2] Implementar pruebas de validación con y sin validator en app/backend/tests/RealtorApiTests/Infrastructure/Validation/ValidationFilterFactoryTests.cs
- [X] T019 [US2] Implementar pruebas de mapping Result -> ProblemDetails en app/backend/tests/RealtorApiTests/Infrastructure/Results/ResultProblemDetailsMapperTests.cs

### Implementación User Story 2

- [X] T020 [US2] Implementar registro de validators por assembly scanning en app/backend/src/RealtorApi/Infrastructure/Validation/ValidationServiceCollectionExtensions.cs
- [X] T021 [US2] Implementar ValidationFilterFactory con pass-through cuando no exista validator en app/backend/src/RealtorApi/Infrastructure/Validation/ValidationFilterFactory.cs
- [X] T022 [US2] Implementar contrato Result para errores esperados en app/backend/src/RealtorApi/Infrastructure/Results/Result.cs
- [X] T023 [US2] Implementar mapper central Result -> ProblemDetails en app/backend/src/RealtorApi/Infrastructure/Results/ResultProblemDetailsMapper.cs
- [X] T024 [US2] Implementar extensiones HTTP para traducir Result en app/backend/src/RealtorApi/Infrastructure/Results/ResultHttpExtensions.cs
- [X] T025 [US2] Integrar validación y mapping de errores en app/backend/src/RealtorApi/Infrastructure/DependencyInjection.cs

**Checkpoint**: Validación y errores esperados quedan estandarizados para futuras features.

---

## Fase 5: Historia de Usuario 3 - Migrar health a slice como sonda E2E de infraestructura (Prioridad: P3)

**Goal**: Migrar /health a ISlice y verificar respuesta operativa manteniendo política de exposición interna.

**Independent Test**: El endpoint /health migrado responde correctamente vía mapeo centralizado y su contrato de infraestructura permanece consistente.

### Tests for User Story 3

- [X] T026 [US3] Implementar prueba del endpoint /health migrado en app/backend/tests/RealtorApiTests/Infrastructure/Endpoints/HealthSliceTests.cs

### Implementación User Story 3

- [X] T027 [US3] Implementar slice de health en app/backend/src/RealtorApi/Infrastructure/Endpoints/HealthSlice.cs
- [X] T028 [US3] Remover mapeo inline legado de /health y usar slice central en app/backend/src/RealtorApi/Program.cs
- [X] T029 [US3] Actualizar contrato de salud y errores si aplica en specs/002-foundation-backend/contracts/backend-infrastructure.openapi.yaml
- [X] T030 [US3] Documentar en quickstart la validación de política internal-only para /health en specs/002-foundation-backend/quickstart.md

**Checkpoint**: /health opera como slice y demuestra el flujo E2E de infraestructura transversal.

---

## Fase 6: Polish & Cross-Cutting Concerns

**Purpose**: Cierre integral de calidad y verificación de criterios de éxito.

- [X] T031 Ejecutar validación completa de quickstart en specs/002-foundation-backend/quickstart.md
- [X] T032 [P] Ejecutar dotnet build de backend desde app/backend/src/RealtorApi/RealtorApi.csproj
- [X] T033 [P] Ejecutar dotnet test de infraestructura desde app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
- [X] T034 Consolidar observaciones finales de implementación en specs/002-foundation-backend/plan.md

---

## Dependencies & Execution Order

### Dependencias entre fases

- Setup (Fase 1): inicia inmediatamente.
- Foundational (Fase 2): depende de Fase 1 y bloquea historias.
- Historias (Fases 3 a 5): dependen de completar Fase 2.
- Polish (Fase 6): depende de completar historias seleccionadas.

### Dependencias entre historias

- US1 (P1): inicia al cerrar Fase 2; no depende de otras historias.
- US2 (P2): inicia al cerrar Fase 2; reutiliza contratos base de infraestructura de Fase 2.
- US3 (P3): inicia al cerrar Fase 2; consume infraestructura de slices definida en US1.

### Orden recomendado dentro de cada historia

- Primero pruebas de historia (deben fallar inicialmente).
- Luego implementación de infraestructura de historia.
- Finalmente integración y ajuste de cableado.

### Grafo de precedencia

- Fase 1 -> Fase 2 -> US1 -> US3 -> Fase 6
- Fase 1 -> Fase 2 -> US2 -> Fase 6

---

## Parallel Opportunities

- T009 puede ejecutarse en paralelo con T004-T008.
- T010 puede ejecutarse en paralelo con preparación de T011.
- T017 puede ejecutarse en paralelo con diseño de T018-T019.
- T032 y T033 pueden ejecutarse en paralelo en la fase de cierre.

## Parallel Example: User Story 2

- Ejecutar en paralelo T017 y el esqueleto de T019, porque escriben archivos distintos.
- Tras T020, ejecutar en paralelo T022 y T023, porque se implementan en archivos distintos dentro de Results.

---

## Implementation Strategy

### MVP First (US1)

1. Completar Fase 1 y Fase 2.
2. Completar US1 (T010-T016).
3. Validar independencia de US1 con sus pruebas y compilación.
4. Demostrar extensibilidad sin cambios en Program.cs.

### Entrega incremental

1. Foundation lista (Fases 1-2).
2. Entregar US1 (descubrimiento/mapeo/handlers).
3. Entregar US2 (validación y errores consistentes).
4. Entregar US3 (health migrado a slice y verificación E2E).
5. Cierre con Fase 6.

### Estrategia para equipo en paralelo

1. Un subequipo finaliza Fases 1-2.
2. Luego:
   - Persona A: US1
   - Persona B: US2
   - Persona C: US3 (después de T012-T013 de US1)
3. Integración final y validaciones en Fase 6.

---

## Notes

- Las tareas con [P] no deben tocar el mismo archivo.
- Cada tarea de historia usa etiqueta [US1], [US2] o [US3] para trazabilidad.
- Se mantiene el alcance sin dominio, sin base de datos y sin features de producto.
- No usar controllers; mantener Minimal APIs con infraestructura transversal.
