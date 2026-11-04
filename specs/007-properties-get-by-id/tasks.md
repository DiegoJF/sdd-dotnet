# Tareas: Consulta de propiedad por id

**Entrada**: Documentos de diseño desde `specs/007-properties-get-by-id/`

**Prerrequisitos**: `plan.md` (requerido), `spec.md` (requerido para historias), `research.md`, `data-model.md`, `contracts/`

**Pruebas**: No se añaden tareas de pruebas automatizadas porque la spec no solicita explícitamente enfoque TDD. La validación se ejecuta con `quickstart.md` y comprobación manual en `RealtorApi.http`.

**Organización**: Las tareas están agrupadas por historia de usuario para implementación y validación independientes.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos distintos, sin dependencias directas)
- **[Story]**: Historia de usuario objetivo (`US1`, `US2`, `US3`)
- Todas las tareas incluyen ruta exacta de archivo

---

## Fase 1: Setup (Infraestructura compartida)

**Propósito**: Preparar estructura mínima del nuevo slice y escenarios manuales base.

- [X] T001 [P] Crear la estructura `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/` para el nuevo slice de consulta unitaria
- [X] T002 [P] Añadir escenarios manuales de `GET /api/properties/{id}` (200 con imagen, 200 sin imagen, 400 por id inválido, 404 por inexistente) en `app/backend/src/RealtorApi/RealtorApi.http`

---

## Fase 2: Fundacional (Prerrequisitos bloqueantes)

**Propósito**: Definir contratos internos y mapeos base antes de implementar historias.

**⚠️ CRÍTICO**: Ninguna historia puede comenzar hasta completar esta fase.

- [X] T003 [P] Definir `PropertyDetailResponse` en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdContracts.cs` con campos públicos alineados a spec 004 (`id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters`, `imageUrl`)
- [X] T004 [P] Definir `GetPropertyByIdQuery` con `RawId` (string) e `Id` (`Guid?`) en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdQuery.cs` para separar parseo y validación de ruta
- [X] T005 [P] Implementar `GetPropertyByIdValidator : AbstractValidator<GetPropertyByIdQuery>` en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdValidator.cs` validando `RawId` requerido y formato UUID canónico
- [X] T006 [P] Implementar mapeos `Property -> PropertyDetailResponse` y composición de `imageUrl` absoluto por request en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdMapping.cs`

**Checkpoint**: Base técnica lista para codificar cada historia sin ambigüedades de contrato.

---

## Fase 3: User Story 1 - Consultar propiedad existente por id (Prioridad: P1) 🎯 MVP

**Objetivo**: Resolver consulta unitaria exitosa con respuesta pública completa y sin metadatos de paginación.

**Prueba independiente**: `GET /api/properties/{idValidoExistente}` devuelve `200 OK` con todos los campos públicos y contrato alineado a 004.

### Implementación para User Story 1

- [X] T007 [US1] Implementar `GetPropertyByIdHandler` con búsqueda por PK (`FindAsync`) y `CancellationToken`, retornando `PropertyDetailResponse` en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdHandler.cs`
- [X] T008 [US1] Registrar handler mediante `IHandler` en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdHandler.cs`
- [X] T009 [US1] Implementar `GetPropertyByIdSlice` con endpoint `GET /api/properties/{id}` y auto-registro por `ISlice` en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdSlice.cs`
- [X] T010 [US1] Asegurar en slice/handler que la respuesta `200` no incluye metadatos de paginación y solo expone contrato público en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdSlice.cs`
- [X] T011 [US1] Integrar `GetPropertyByIdValidator` en el flujo del endpoint (invocación explícita en slice o filtro de validación del módulo) para garantizar `400 ProblemDetails` en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdSlice.cs`

**Checkpoint**: Consulta exitosa por id funcionando de extremo a extremo.

---

## Fase 4: User Story 2 - Manejar id inexistente (Prioridad: P2)

**Objetivo**: Responder de forma consistente con `404 ProblemDetails` cuando el recurso no exista.

**Prueba independiente**: `GET /api/properties/{uuidNoExistente}` devuelve `404 Not Found` con ProblemDetails.

### Implementación para User Story 2

- [X] T012 [US2] Extender handler para retornar rama `404 Not Found` con ProblemDetails cuando `FindAsync` no encuentra entidad en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdHandler.cs`
- [X] T013 [US2] Añadir logging estructurado para caso no encontrado en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdHandler.cs`
- [X] T014 [US2] Verificar y ajustar el ejemplo `404` ya agregado en `app/backend/src/RealtorApi/RealtorApi.http` para incluir payload ProblemDetails alineado al contrato

**Checkpoint**: Error de inexistencia cubierto y observable de forma independiente.

---

## Fase 5: User Story 3 - Manejar id inválido y host/esquema variable (Prioridad: P3)

**Objetivo**: Responder `400` para id inválido y garantizar `imageUrl` absoluto dependiente del entorno.

**Prueba independiente**: `GET /api/properties/not-a-uuid` devuelve `400`; para propiedad con imagen, `imageUrl` refleja esquema/host del request actual.

### Implementación para User Story 3

- [X] T015 [US3] Implementar manejo explícito de id inválido con `400 Bad Request` y ProblemDetails en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdSlice.cs`
- [X] T016 [US3] Ajustar mapping para cubrir host/esquema variable por entorno y caso `imageUrl = null` en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdMapping.cs`
- [X] T017 [US3] Añadir logging estructurado para validación inválida de id en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdSlice.cs`

**Checkpoint**: Validación de entrada y comportamiento multi-entorno cerrados.

---

## Fase Final: Polish y aspectos transversales

**Propósito**: Cerrar consistencia de contrato y evidencia de validación manual.

- [X] T018 [P] Verificar consistencia final entre implementación y `specs/007-properties-get-by-id/contracts/backend-properties-get-by-id.openapi.yaml`
- [X] T019 [P] Ejecutar escenarios 1-5 de `specs/007-properties-get-by-id/quickstart.md` y registrar evidencia en el mismo archivo
- [X] T020 [P] Validar manualmente requests de `app/backend/src/RealtorApi/RealtorApi.http` y registrar resultados en `specs/007-properties-get-by-id/quickstart.md`
- [X] T021 Ejecutar checklist de cierre de alcance en `specs/007-properties-get-by-id/quickstart.md` validando que solo se modifican archivos del slice `GetPropertyById`, contrato OpenAPI, `quickstart.md` y `RealtorApi.http`, con lista explícita de archivos tocados

---

## Dependencias y orden de ejecución

### Dependencias por fase

- **Fase 1 (Setup)**: puede iniciar de inmediato
- **Fase 2 (Fundacional)**: depende de Fase 1 y bloquea historias
- **Fases 3-5 (Historias)**: dependen de Fase 2
- **Fase Final (Polish)**: depende de completar historias objetivo

### Dependencias entre historias

- **US1 (P1)**: inicia tras Fase 2; no depende de otras historias
- **US2 (P2)**: depende de US1 porque extiende la rama de salida del mismo handler
- **US3 (P3)**: depende de US1 y US2 para consolidar validación + contrato final

### Dentro de cada historia

- Contratos/query/validator/mapping antes del endpoint funcional
- Integración del validator (T011) después de implementar el slice (T009/T010)
- Handler antes del mapeo final de respuestas HTTP en el slice
- Story completa solo si su prueba independiente se cumple

### Oportunidades de paralelismo

- **Fase 1**: T001 y T002 en paralelo
- **Fase 2**: T003, T004, T005 y T006 en paralelo
- **Fase Final**: T018, T019 y T020 en paralelo

---

## Ejemplo de paralelismo por historia

### US1

```text
T007 GetPropertyByIdHandler.cs
T009 GetPropertyByIdSlice.cs
```

### US2

```text
T013 logging 404 en GetPropertyByIdHandler.cs
T014 ajuste de ejemplo 404 en RealtorApi.http
```

### US3

```text
T016 mapping imageUrl absoluto en GetPropertyByIdMapping.cs
T017 logging id inválido en GetPropertyByIdSlice.cs
```

---

## Estrategia de implementación

### MVP primero (solo US1)

1. Completar Fase 1 y Fase 2.
2. Completar Fase 3 (US1).
3. Validar prueba independiente de US1.

### Entrega incremental

1. Añadir US2 y validar `404` de forma independiente.
2. Añadir US3 y validar `400` + host/esquema variable.
3. Ejecutar Fase Final para evidencia de cierre.
