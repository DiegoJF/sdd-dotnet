# Quickstart de Validación: Fundación Backend Base

## Objetivo

Validar de extremo a extremo que la infraestructura transversal de backend quedó operativa según la spec 002, sin introducir lógica de negocio.

## Prerrequisitos

- .NET SDK `11.0.100-preview.5.26302.115` disponible (o compatible según `global.json`).
- Restauración de paquetes con acceso a NuGet.
- Repositorio en la raíz `c:\sdd`.

## Escenario 1: Compilación de backend

1. Ejecutar:

```powershell
dotnet build app/backend/src/RealtorApi/RealtorApi.csproj
```

2. Resultado esperado:
- Build exit code `0`.
- Sin errores de compilación.

## Escenario 2: Ejecución de pruebas de infraestructura transversal

1. Ejecutar:

```powershell
dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

2. Resultado esperado:
- Todas las pruebas en verde para:
  - descubrimiento `ISlice` sin duplicados,
  - `ValidationFilterFactory` con y sin validator,
  - respuesta 400 con `ValidationProblemDetails`,
  - mapeo de error esperado (`Result` -> `ProblemDetails`),
  - endpoint `/health` migrado a slice.

## Escenario 3: Verificación manual del endpoint `/health`

1. Iniciar API:

```powershell
dotnet run --project app/backend/src/RealtorApi/RealtorApi.csproj
```

2. Consultar la sonda:

```powershell
curl http://localhost:5000/health
```

o (PowerShell):

```powershell
Invoke-RestMethod http://localhost:5000/health
```

3. Resultado esperado:
- HTTP `200` con payload equivalente a:

```json
{ "status": "ok" }
```

## Escenario 4: Verificación de política de exposición interna para `/health`

1. Confirmar que la aplicación no exige autenticación de aplicación en `/health`.
2. Confirmar que el despliegue restringe exposición pública de `/health` en ingress/reverse proxy.

Resultado esperado:
- `/health` es consumible para monitoreo interno.
- `/health` no queda expuesto públicamente fuera de la red/origen autorizado.

## Referencias de diseño

- Modelo conceptual: `data-model.md`.
- Contrato API de infraestructura: `contracts/backend-infrastructure.openapi.yaml`.
- Decisiones técnicas: `research.md`.

## Evidencia de validación

- Fecha: 2026-07-08.
- `dotnet build app/backend/src/RealtorApi/RealtorApi.csproj`: éxito, sin errores.
- `dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj`: 8/8 pruebas exitosas.
- Cobertura funcional validada:
  - descubrimiento de `ISlice` sin duplicados,
  - validación automática con y sin validator,
  - respuesta 400 con `ValidationProblemDetails`,
  - mapeo de `Result` a `ProblemDetails`,
  - endpoint `/health` migrado a slice respondiendo `200` con `status: ok`.
- Política de exposición de `/health`: mantenida como internal-only vía ingress/reverse proxy (sin autenticación de aplicación).
