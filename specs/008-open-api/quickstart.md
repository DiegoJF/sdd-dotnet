# Quickstart de Validación: OpenAPI v1 público

## Objetivo

Validar de extremo a extremo la generación, publicación y consistencia del contrato OpenAPI v1 de `RealtorApi`.

Se valida que:

- Existe un único documento en `app/backend/src/RealtorApi/wwwroot/openapi/v1.json`.
- `GET /openapi/v1.json` responde correctamente como JSON.
- Redocly lint pasa con `.redocly.yaml`.
- NSwag genera cliente tipado de smoke test sin correcciones manuales.
- La verificación de drift detecta desalineación entre runtime y contrato.

## Prerrequisitos obligatorios

- .NET SDK 11.
- Node.js 20+ y npm/npx disponibles.
- `dotnet-tools.json` con `NSwag.ConsoleCore`.
- `.redocly.yaml` presente en raíz.

Verificación previa:

```powershell
node --version
npm --version
npx @redocly/cli --version
dotnet tool restore
dotnet tool list
```

## Preparación

```powershell
dotnet restore app/Realtor.sln
dotnet build app/backend/src/RealtorApi/RealtorApi.csproj
dotnet build app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

## Referencias de diseño

- Contrato: [contracts/backend-openapi-v1.openapi.yaml](contracts/backend-openapi-v1.openapi.yaml)
- Modelo funcional: [data-model.md](data-model.md)

## Escenario 1: Regeneración y validación reproducible

Ejecutar:

```powershell
pwsh ./support/scripts/generate-openapi-v1.ps1
```

Validar:

- Se genera/actualiza `wwwroot/openapi/v1.json`.
- Redocly lint finaliza sin errores.
- NSwag genera cliente de smoke en `artifacts/openapi-client-smoke/`.
- El script termina en éxito con rutas reportadas.

## Escenario 2: Publicación HTTP del documento

1. Levantar API:

```powershell
dotnet run --project app/backend/src/RealtorApi/RealtorApi.csproj
```

2. Consultar:

```http
GET /openapi/v1.json
```

Validar:

- Estado `200 OK`.
- `Content-Type` JSON.
- El documento incluye operaciones para `GET /health` y `/api/properties*` en alcance.

## Escenario 3: Drift alineado (debe pasar)

Ejecutar pruebas de drift:

```powershell
dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj --filter "OpenApi|Drift"
```

Validar:

- Comparación runtime/documento pasa con normalización de constraints.
- Endpoints técnicos como `/openapi/v1.json` no participan en inventario contractual.

## Escenario 4: Drift inducido (debe fallar)

1. Inducir desalineación controlada (ejemplo: modificar temporalmente una ruta pública del backend o documento generado).
2. Re-ejecutar pruebas de drift.

Resultado esperado:

- Al menos una prueba falla reportando diferencia entre inventario runtime y documentado.

## Escenario 5: Prerrequisitos faltantes (debe fallar explícitamente)

Condición de prueba:

- Simular ausencia de Node, Redocly o NSwag en PATH/herramientas.

Resultado esperado:

- Script de regeneración falla explícitamente con mensaje de prerrequisito faltante.
- Pruebas de drift fallan en estado Failed (no Skip).

## Criterios de salida

- Documento v1 disponible y servible en ruta estable.
- Validaciones Redocly y NSwag completadas sin errores.
- Drift check pasa en estado alineado y falla en estado inducido.
- No existe UI interactiva de documentación en runtime.

## Evidencia de validación

Fecha de ejecución: 2026-07-19

1. Generación y publicación del documento

- Comando: `dotnet build app/backend/src/RealtorApi/RealtorApi.csproj --tl:off`
- Resultado: éxito, `GenerateOpenApiDocuments` escribió `app/backend/src/RealtorApi/wwwroot/openapi/v1.json`.

2. Script reproducible offline

- Comando: `pwsh ./support/scripts/generate-openapi-v1.ps1`
- Resultado: éxito en secuencia build + Redocly lint + NSwag smoke.
- Artefactos confirmados:
	- `app/backend/src/RealtorApi/wwwroot/openapi/v1.json`
	- `artifacts/openapi-client-smoke/RealtorApiClient.cs`

3. Drift alineado

- Comando: `dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj --filter "OpenApi|Drift" --tl:off`
- Resultado: `Passed`, 8/8 pruebas en verde.

4. Drift inducido (falla esperada)

- Comando ejecutado: alteración temporal de `v1.json` (`/health` -> `/health-broken`) + ejecución de prueba `RuntimeEndpoints_AreAlignedWithDocumentedOperations_UsingRouteNormalization`.
- Resultado: falla esperada con mensaje `Faltan operaciones en documento: GET /health`.
- Restauración: documento restaurado inmediatamente desde respaldo temporal y revalidado con suite `OpenApi|Drift` en verde.

## Matriz de trazabilidad FR y SC a tareas

### FR

- FR-001, FR-002, FR-003: T005, T006, T007, T016
- FR-004, FR-005: T011, T012, T015
- FR-006, FR-007: T013, T014, T015
- FR-008: T005, T006, T016
- FR-009, FR-010: T025, T026
- FR-011: T008, T009, T010, T018, T022
- FR-012: T019, T020, T021, T023
- FR-013: T024, T028
- FR-014, FR-015: T001
- FR-016: T027, T028
- FR-017: T011, T012, T018, T019, T020

### SC

- SC-001: T012, T018
- SC-002: T011, T017
- SC-003: T001, T019, T025, T026
- SC-004: T018, T031
- SC-005: T002, T004, T020, T025
- SC-006: T025, T026, T029, T030
- SC-007: T001, T029
- SC-008: T024, T028
