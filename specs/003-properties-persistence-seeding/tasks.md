# Tasks: Persistencia y Seeding Inicial de Propiedades

**Input**: Documentos de diseño desde `/specs/003-properties-persistence-seeding/`

**Prerequisites**: `plan.md` (required), `spec.md` (required for user stories), `research.md`, `data-model.md`, `contracts/`, `quickstart.md`

**Tests**: La spec exige pruebas para configuración EF, migración, idempotencia, `UseAsyncSeeding`, lectura de manifiesto/JSON y consistencia de imágenes/`ImageUrl`.

**Organization**: Tareas agrupadas por historia de usuario para implementación y validación independiente.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Preparar base técnica y estructura mínima para persistencia/seeding.

- [X] T001 Agregar dependencias EF Core + Npgsql en app/backend/src/RealtorApi/RealtorApi.csproj
- [X] T002 Agregar paquetes de pruebas de EF Core/SQLite para validaciones de persistencia en app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
- [X] T003 Crear estructura de carpetas de persistencia y dominio en app/backend/src/RealtorApi/Infrastructure/Persistence/
- [X] T004 [P] Crear estructura de pruebas de persistencia en app/backend/tests/RealtorApiTests/Infrastructure/Persistence/
- [X] T005 Crear manifiesto central de seed en support/seed-data/seed-manifest.json

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Infraestructura bloqueante que debe existir antes de cualquier historia.

**CRITICAL**: Ninguna historia puede implementarse antes de completar esta fase.

- [X] T006 Crear entidad de dominio PropertyStatus en app/backend/src/RealtorApi/Domain/Properties/PropertyStatus.cs
- [X] T007 Crear entidad de dominio Property en app/backend/src/RealtorApi/Domain/Properties/Property.cs
- [X] T008 Crear AppDbContext con ApplyConfigurationsFromAssembly en app/backend/src/RealtorApi/Infrastructure/Persistence/AppDbContext.cs
- [X] T009 [P] Crear configuración EF de PropertyStatus en app/backend/src/RealtorApi/Infrastructure/Persistence/Configurations/PropertyStatusConfiguration.cs
- [X] T010 [P] Crear configuración EF de Property con conversión enum-string y restricciones en app/backend/src/RealtorApi/Infrastructure/Persistence/Configurations/PropertyConfiguration.cs
- [X] T011 Registrar AppDbContext con UseNpgsql, UseSeeding y UseAsyncSeeding en app/backend/src/RealtorApi/Infrastructure/DependencyInjection.cs
- [X] T012 Crear DatabaseSeeder con versiones sync y async equivalentes en app/backend/src/RealtorApi/Infrastructure/Persistence/DatabaseSeeder.cs
- [X] T013 Crear MigrationExtensions con MigrateAsync relacional/no relacional sin invocación manual de seeder en app/backend/src/RealtorApi/Infrastructure/Persistence/MigrationExtensions.cs
- [X] T014 Actualizar arranque para ejecutar await app.MigrateAsync() antes de app.Run() en app/backend/src/RealtorApi/Program.cs

**Checkpoint**: Base de persistencia y arranque automático lista para historias.

---

## Phase 3: User Story 1 - Carga Inicial Confiable (Priority: P1) 🎯 MVP

**Goal**: Inicializar esquema y datos base de propiedades/estados de forma automática e idempotente.

**Independent Test**: Ejecutar arranque sobre base vacía y reiniciar varias veces verificando migración única + seed sin duplicados.

### Tests for User Story 1

- [X] T015 [P] [US1] Crear prueba de configuración EF y conversión enum-string en app/backend/tests/RealtorApiTests/Infrastructure/Persistence/PropertyConfigurationTests.cs
- [X] T016 [P] [US1] Crear prueba de esquema esperado de migración (Up/Down/snapshot) en app/backend/tests/RealtorApiTests/Infrastructure/Persistence/PropertiesMigrationSchemaTests.cs
- [X] T017 [P] [US1] Crear prueba de idempotencia de seeding (sin duplicados) en app/backend/tests/RealtorApiTests/Infrastructure/Persistence/DatabaseSeederIdempotencyTests.cs
- [X] T018 [P] [US1] Crear prueba de ejecución de UseAsyncSeeding durante arranque en app/backend/tests/RealtorApiTests/Infrastructure/Persistence/StartupAsyncSeedingTests.cs

### Implementation for User Story 1

- [X] T019 [US1] Implementar modelos auxiliares de seed (DTOs) para estados y propiedades en app/backend/src/RealtorApi/Infrastructure/Persistence/SeedModels.cs
- [X] T020 [US1] Implementar lectura y validación de JSON de estados en app/backend/src/RealtorApi/Infrastructure/Persistence/DatabaseSeeder.cs
- [X] T021 [US1] Implementar lectura y validación de JSON de propiedades en app/backend/src/RealtorApi/Infrastructure/Persistence/DatabaseSeeder.cs
- [X] T022 [US1] Implementar seed idempotente de PropertyStatus en app/backend/src/RealtorApi/Infrastructure/Persistence/DatabaseSeeder.cs
- [X] T023 [US1] Implementar upsert idempotente de Property preservando Id en app/backend/src/RealtorApi/Infrastructure/Persistence/DatabaseSeeder.cs
- [X] T024 [US1] Generar migración única de la spec 003 en app/backend/src/RealtorApi/Infrastructure/Persistence/Migrations/
- [X] T025 [US1] Revisar y ajustar archivos de migración (Up/Down/snapshot) en app/backend/src/RealtorApi/Infrastructure/Persistence/Migrations/

**Checkpoint**: US1 funcional y validable de forma independiente.

---

## Phase 4: User Story 2 - Integridad de Recursos Externos (Priority: P2)

**Goal**: Garantizar que manifiesto, JSON e imágenes estén disponibles y consumibles en runtime/build/publish.

**Independent Test**: Ejecutar la API con recursos presentes y con faltantes críticos para validar disponibilidad y fail-fast.

### Tests for User Story 2

- [X] T026 [P] [US2] Crear prueba de lectura de seed-manifest y resolución de rutas en app/backend/tests/RealtorApiTests/Infrastructure/Persistence/SeedManifestResolutionTests.cs
- [X] T027 [P] [US2] Crear prueba de fail-fast cuando faltan manifiesto/JSON críticos en app/backend/tests/RealtorApiTests/Infrastructure/Persistence/SeedFailFastTests.cs
- [X] T028 [P] [US2] Crear prueba de copia/sincronización de imágenes al destino final en app/backend/tests/RealtorApiTests/Infrastructure/Persistence/SeedImageSyncTests.cs

### Implementation for User Story 2

- [X] T029 [US2] Implementar cargador de manifiesto central y resolución de rutas de seed en app/backend/src/RealtorApi/Infrastructure/Persistence/SeedManifestLoader.cs
- [X] T030 [US2] Integrar uso exclusivo de rutas desde manifiesto en DatabaseSeeder en app/backend/src/RealtorApi/Infrastructure/Persistence/DatabaseSeeder.cs
- [X] T031 [US2] Implementar fail-fast por faltantes críticos de manifiesto/JSON en app/backend/src/RealtorApi/Infrastructure/Persistence/DatabaseSeeder.cs
- [X] T032 [US2] Implementar sincronización de imágenes desde support/seed-data/images/properties a wwwroot/assets/properties en app/backend/src/RealtorApi/Infrastructure/Persistence/DatabaseSeeder.cs
- [X] T033 [US2] Incluir manifiesto, JSON e imágenes en build/publish de la API en app/backend/src/RealtorApi/RealtorApi.csproj

**Checkpoint**: US2 funcional y validable de forma independiente.

---

## Phase 5: User Story 3 - Referencias de Imagen Consistentes (Priority: P3)

**Goal**: Persistir y servir `ImageUrl` pública estable para consumo directo por clientes.

**Independent Test**: Validar que `ImageUrl` persiste con ruta pública, que la API sirve los archivos y que imágenes faltantes omiten solo el registro afectado.

### Tests for User Story 3

- [X] T034 [P] [US3] Crear prueba de consistencia de ImageUrl pública persistida en app/backend/tests/RealtorApiTests/Infrastructure/Persistence/PropertyImageUrlConsistencyTests.cs
- [X] T035 [P] [US3] Crear prueba de omisión selectiva de propiedad con imagen faltante en app/backend/tests/RealtorApiTests/Infrastructure/Persistence/MissingImageSelectiveSkipTests.cs
- [X] T036 [P] [US3] Crear prueba de disponibilidad pública de imágenes en `/assets/properties/{fileName}` en app/backend/tests/RealtorApiTests/Infrastructure/Endpoints/PropertyAssetsEndpointTests.cs

### Implementation for User Story 3

- [X] T037 [US3] Configurar archivos estáticos para servir `wwwroot/assets/properties` en app/backend/src/RealtorApi/Program.cs
- [X] T038 [US3] Persistir `ImageUrl` con base pública del manifiesto (`/assets/properties`) en app/backend/src/RealtorApi/Infrastructure/Persistence/DatabaseSeeder.cs
- [X] T039 [US3] Implementar regla de omisión selectiva y logging estructurado para imágenes faltantes en app/backend/src/RealtorApi/Infrastructure/Persistence/DatabaseSeeder.cs
- [X] T040 [US3] Alinear contrato OpenAPI de assets con comportamiento implementado en specs/003-properties-persistence-seeding/contracts/backend-properties-persistence.openapi.yaml

**Checkpoint**: US3 funcional y validable de forma independiente.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Cierre transversal, documentación y validación final de la feature.

- [X] T041 [P] Actualizar quickstart con comandos finales y evidencia de validación en specs/003-properties-persistence-seeding/quickstart.md
- [X] T042 Ejecutar suite de pruebas backend de la spec 003 en app/backend/tests/RealtorApiTests/
- [X] T043 Ejecutar validación de arranque end-to-end con migración + seeding en app/backend/src/RealtorApi/
- [X] T044 Revisar trazabilidad y marcar tareas completadas en specs/003-properties-persistence-seeding/tasks.md

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: Sin dependencias.
- **Phase 2 (Foundational)**: Depende de Phase 1 y bloquea todas las historias.
- **Phase 3 (US1)**: Depende de Phase 2.
- **Phase 4 (US2)**: Depende de Phase 2 y puede ejecutarse en paralelo con US1 si la base de persistencia ya existe.
- **Phase 5 (US3)**: Depende de Phase 2; usa resultados de US2 para paths públicos e imágenes.
- **Phase 6 (Polish)**: Depende de completar historias objetivo.

### User Story Dependencies

- **US1 (P1)**: Base MVP; no depende de otras historias tras Foundational.
- **US2 (P2)**: Requiere infraestructura de persistencia de Foundational; integra seeder y csproj.
- **US3 (P3)**: Requiere persistencia y sincronización de imágenes (US2) para validación completa de URL pública.

### Within Each User Story

- Pruebas primero (deben fallar antes de implementar).
- Configuraciones/modelos antes de migración.
- Seeding antes de validación de arranque.
- Historia cerrada antes de pasar a la siguiente en secuencial.

### Parallel Opportunities

- Tareas `[P]` de Setup y Foundational pueden correrse en paralelo por archivos distintos.
- Pruebas `[P]` de cada historia pueden implementarse en paralelo.
- US1 y US2 pueden solaparse tras Foundational si se coordina integración en `DatabaseSeeder`.
- Validaciones de Polish (`T041`, `T042`) pueden ejecutarse en paralelo antes de `T044`.

---

## Parallel Example: User Story 1

```bash
# Pruebas en paralelo (US1)
Task: "T015 [US1] PropertyConfigurationTests"
Task: "T016 [US1] PropertiesMigrationSchemaTests"
Task: "T017 [US1] DatabaseSeederIdempotencyTests"
Task: "T018 [US1] StartupAsyncSeedingTests"

# Implementación paralela inicial (US1)
Task: "T019 [US1] SeedModels"
Task: "T022 [US1] Seed idempotente de PropertyStatus"
```

---

## Parallel Example: User Story 2

```bash
# Pruebas en paralelo (US2)
Task: "T026 [US2] SeedManifestResolutionTests"
Task: "T027 [US2] SeedFailFastTests"
Task: "T028 [US2] SeedImageSyncTests"

# Implementación paralela (US2)
Task: "T029 [US2] SeedManifestLoader"
Task: "T033 [US2] Integración de assets en csproj"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Completar Setup (Phase 1).
2. Completar Foundational (Phase 2).
3. Completar US1 (Phase 3).
4. Validar arranque + idempotencia + migración única.
5. Demostrar valor base (inventario inicial persistido).

### Incremental Delivery

1. Setup + Foundational.
2. US1 (MVP persistente y seed automático).
3. US2 (integridad de recursos externos y fail-fast).
4. US3 (rutas públicas de imagen y consistencia final).
5. Polish y evidencia final en quickstart.

### Parallel Team Strategy

1. Equipo A: entidades/configuraciones/migración (US1).
2. Equipo B: manifiesto, csproj y sincronización de assets (US2).
3. Equipo C: validación de URL pública y tests de assets (US3).
4. Integración final y cierre transversal en Phase 6.

---

## Notes

- Todas las tareas siguen formato checklist obligatorio con `T###`, marcadores `[P]` y `[US#]` cuando corresponde.
- Cada tarea referencia ruta de archivo concreta para trazabilidad con la spec.
- No se incluyen controllers ni `HasData`, en cumplimiento de restricciones explícitas.
