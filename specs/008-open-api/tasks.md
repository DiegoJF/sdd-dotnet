# Tareas: OpenAPI v1 público

**Entrada**: Documentos de diseño desde /specs/008-open-api/

**Prerrequisitos**: plan.md (requerido), spec.md (requerido para historias), research.md, data-model.md, contracts/

**Pruebas**: Se incluyen tareas de pruebas porque la spec exige verificación automatizada de drift con xUnit, validación Redocly y smoke test NSwag. Esta feature justifica explícitamente pruebas xUnit de contrato sobre superficie HTTP/runtime (FR-011, FR-012, FR-017).

**Organización**: Las tareas están agrupadas por historia de usuario para habilitar implementación y validación independiente.

## Formato: [ID] [P?] [Historia] Descripción

- [P]: Puede ejecutarse en paralelo (archivos distintos, sin dependencia directa)
- [Historia]: Historia de usuario objetivo (US1, US2, US3)
- Todas las tareas incluyen rutas exactas de archivo

## Fase 1: Preparación (infraestructura compartida)

**Propósito**: Preparar herramientas y artefactos base de validación contractual.

- [X] T001 Crear o ajustar configuración Redocly para API pública sin auth en .redocly.yaml
- [X] T002 Registrar o actualizar NSwag.ConsoleCore como local tool en dotnet-tools.json
- [X] T003 [P] Crear/asegurar carpeta de salida de smoke test en artifacts/openapi-client-smoke/.gitkeep
- [X] T004 [P] Crear configuración de generación NSwag para cliente smoke en support/scripts/openapi-v1.nswag.json

---

## Fase 2: Fundacional (prerrequisitos bloqueantes)

**Propósito**: Infraestructura bloqueante para generar, publicar y validar el contrato antes de cualquier historia.

**CRÍTICO**: Ninguna historia puede comenzar hasta completar esta fase.

- [X] T005 Configurar generación OpenAPI v1 desde backend real en app/backend/src/RealtorApi/Program.cs
- [X] T006 Configurar persistencia de documento versionado en app/backend/src/RealtorApi/RealtorApi.csproj
- [X] T007 Asegurar publicación estática de wwwroot/openapi/v1.json en app/backend/src/RealtorApi/Program.cs
- [X] T008 [P] Crear utilitario de normalización de rutas para drift ({id:guid} -> {id}) en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiRouteNormalizer.cs
- [X] T009 [P] Crear lector de documento OpenAPI para pruebas en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiDocumentReader.cs
- [X] T010 [P] Crear inventario runtime filtrado a endpoints públicos funcionales en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiEndpointInventory.cs

**Checkpoint**: Base lista para implementar historias y validar de forma repetible.

---

## Fase 3: Historia de usuario 1 - Publicar contrato OpenAPI v1 estable (Prioridad: P1) MVP

**Objetivo**: Publicar un único contrato v1 en ruta estable y con cobertura completa de endpoints en alcance.

**Prueba independiente**: GET /openapi/v1.json responde 200 JSON e incluye GET /health, GET/POST /api/properties, GET/PUT /api/properties/{id}.

### Pruebas de la Historia de usuario 1

- [X] T011 [P] [US1] Crear prueba xUnit de contrato para GET /openapi/v1.json con 200 y Content-Type JSON en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiPublicationTests.cs
- [X] T012 [P] [US1] Crear prueba xUnit de cobertura de los 5 endpoints en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiPublicationTests.cs
- [X] T013 [P] [US1] Crear prueba xUnit que valide en el JSON generado la presencia de components/schemas para PropertyStatus, ProblemDetails y HttpValidationProblemDetails en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiSchemaConsistencyTests.cs
- [X] T014 [P] [US1] Crear prueba xUnit que valide uso de $ref (sin duplicación inline) para ProblemDetails y HttpValidationProblemDetails en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiSchemaConsistencyTests.cs

### Implementación de la Historia de usuario 1

- [X] T015 [US1] Ajustar metadata del documento v1 (info/version) y operaciones incluidas en app/backend/src/RealtorApi/Program.cs
- [X] T016 [US1] Asegurar generación/actualización de app/backend/src/RealtorApi/wwwroot/openapi/v1.json desde build en app/backend/src/RealtorApi/RealtorApi.csproj
- [X] T017 [US1] Actualizar escenario manual de consulta del contrato en app/backend/src/RealtorApi/RealtorApi.http

**Checkpoint**: Contrato v1 publicado y consultable de forma estable.

---

## Fase 4: Historia de usuario 2 - Evitar deriva contrato-implementación (Prioridad: P2)

**Objetivo**: Detectar automáticamente desalineaciones entre endpoints runtime y contrato publicado.

**Prueba independiente**: Las pruebas de drift pasan cuando hay alineación y fallan al inducir desalineación.

### Pruebas de la Historia de usuario 2

- [X] T018 [P] [US2] Crear prueba xUnit de comparación runtime vs documento con normalización de constraints en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiDriftTests.cs
- [X] T019 [P] [US2] Crear prueba xUnit de validación Redocly lint y fallo explícito por prerrequisito faltante en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiToolingTests.cs
- [X] T020 [P] [US2] Crear prueba xUnit de smoke generation NSwag y fallo explícito por prerrequisito faltante en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiToolingTests.cs

### Implementación de la Historia de usuario 2

- [X] T021 [US2] Implementar runner de herramientas externas (node/npx/nswag) para pruebas en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiToolRunner.cs
- [X] T022 [US2] Implementar filtro de endpoints públicos funcionales (GET /health y /api/properties*) en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiEndpointInventory.cs
- [X] T023 [US2] Integrar suite de drift en el proyecto de pruebas en app/backend/tests/RealtorApiTests/RealtorApiTests.csproj

**Checkpoint**: Verificación automática de drift activa y determinística.

---

## Fase 5: Historia de usuario 3 - Excluir UI interactiva y mantener flujo offline (Prioridad: P3)

**Objetivo**: Garantizar superficie runtime sin Swagger UI/ReDoc UI y pipeline offline reproducible de validación.

**Prueba independiente**: No existen endpoints UI interactivos; generate-openapi-v1.ps1 ejecuta build+lint+smoke y falla explícitamente ante errores.

### Pruebas de la Historia de usuario 3

- [X] T024 [P] [US3] Crear prueba xUnit de contrato que confirme ausencia de endpoints UI interactivos en app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiSurfaceTests.cs

### Implementación de la Historia de usuario 3

- [X] T025 [US3] Implementar script reproducible de regeneración/validación en support/scripts/generate-openapi-v1.ps1
- [X] T026 [US3] Implementar reporte final de éxito y fallos explícitos del script en support/scripts/generate-openapi-v1.ps1
- [X] T027 [US3] Actualizar guía de ejecución reproducible y prerrequisitos en specs/008-open-api/quickstart.md
- [X] T028 [US3] Alinear contrato de diseño sin UI interactiva en specs/008-open-api/contracts/backend-openapi-v1.openapi.yaml

**Checkpoint**: Superficie runtime mínima y flujo offline cerrado.

---

## Fase 6: Pulido y transversal

**Propósito**: Cierre transversal de consistencia y evidencia.

- [X] T029 [P] Ejecutar validación funcional final de quickstart en specs/008-open-api/quickstart.md
- [X] T030 [P] Registrar evidencia de comando de script reproducible en specs/008-open-api/quickstart.md
- [X] T031 [P] Registrar evidencia de pruebas de drift (alineado e inducido) en specs/008-open-api/quickstart.md
- [X] T032 Ejecutar matriz de trazabilidad FR/SC->Tareas, verificando cobertura de FR-001..FR-017 y SC-001..SC-008, y registrar el resultado en specs/008-open-api/quickstart.md

---

## Dependencias y orden de ejecución

### Dependencias por fase

- Preparación (Fase 1): sin dependencias
- Fundacional (Fase 2): depende de Preparación y bloquea historias
- Historias (Fase 3+): dependen de Fundacional
- Pulido (Fase 6): depende de historias completadas

### Dependencias entre historias

- US1 (P1): inicia tras Fundacional, sin depender de otras historias
- US2 (P2): inicia tras Fundacional; depende funcionalmente de que exista documento v1 publicado por US1
- US3 (P3): inicia tras Fundacional; depende funcionalmente de US1 y se beneficia de tooling de US2

### Dentro de cada historia

- Pruebas primero (deben fallar antes de implementar)
- Utilitarios/modelos antes de integración
- Implementación principal antes de evidencia de cierre

### Oportunidades de paralelismo

- Preparación: T003-T004 en paralelo
- Fundacional: T008-T010 en paralelo
- US1: T011-T014 en paralelo
- US2: T018-T020 en paralelo
- Pulido: T029-T031 en paralelo

---

## Ejemplo de paralelismo: Historia de usuario 2

```bash
# Ejecutar en paralelo tareas de pruebas de drift y tooling:
T018 app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiDriftTests.cs
T019 app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiToolingTests.cs
T020 app/backend/tests/RealtorApiTests/Infrastructure/OpenApi/OpenApiToolingTests.cs
```

---

## Estrategia de implementación

### MVP primero (solo Historia de usuario 1)

1. Completar Fase 1 y Fase 2
2. Completar Fase 3 (US1)
3. Validar prueba independiente de publicación de contrato
4. Demostrar contrato estable para consumidores

### Entrega incremental

1. Base técnica (Preparación + Fundacional)
2. Publicación contractual (US1)
3. Drift automatizado (US2)
4. Endurecimiento de superficie + script reproducible (US3)
5. Evidencia final (Pulido)

### Estrategia para equipo en paralelo

1. Equipo A: Preparación + Fundacional
2. Equipo B: US1 cuando desbloquee Fundacional
3. Equipo C: US2 en paralelo tras documento publicado
4. Equipo D: US3 y cierre documental

---

## Notas

- Las tareas [P] evitan colisión de archivos o dependencia de resultados incompletos.
- Cada historia mantiene criterio de prueba independiente.
- No se permite cerrar la feature sin evidencia en quickstart y sin tareas marcadas.

