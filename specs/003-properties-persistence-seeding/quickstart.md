# Quickstart de Validación: Persistencia y Seeding Inicial

## Objetivo

Validar extremo a extremo que la spec 003 deja operativo el modelo persistente inicial (`Property`, `PropertyStatus`), migración única y seeding idempotente con assets disponibles por ruta pública.

## Prerrequisitos

- .NET SDK `11.0.100-preview.5.26302.115`
- PostgreSQL accesible para ejecución local
- Cadena de conexión válida en `app/backend/src/RealtorApi/appsettings.Development.json`

## Preparación

1. Restaurar solución:

```powershell
dotnet restore app/Realtor.sln
```

2. Compilar backend y pruebas:

```powershell
dotnet build app/backend/src/RealtorApi/RealtorApi.csproj
dotnet build app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

## Escenario 1: Migración y seeding en base vacía

1. Arrancar API:

```powershell
dotnet run --project app/backend/src/RealtorApi/RealtorApi.csproj
```

2. Resultado esperado:
- Se aplica la migración única de la spec 003.
- Se ejecuta `UseAsyncSeeding` durante el arranque.
- Se insertan estados base (`Available`, `Rented`, `Maintenance`).
- Se insertan propiedades iniciales válidas.

## Escenario 2: Idempotencia en arranques consecutivos

1. Detener y volver a iniciar la API al menos 2 veces.
2. Resultado esperado:
- No aparecen duplicados en `PropertyStatus` ni `Property`.
- Se mantienen conteos estables.
- Si cambia el seed de una propiedad existente, se actualizan campos permitidos (upsert) preservando `Id`.

## Escenario 3: Fail-fast por archivos críticos faltantes

1. Simular ausencia de `support/seed-data/seed-manifest.json` o JSON críticos.
2. Arrancar API.
3. Resultado esperado:
- El arranque falla explícitamente con error trazable.
- No se deja el inventario en estado parcial silencioso.

## Escenario 4: Imagen faltante por propiedad

1. Remover una imagen específica referenciada por una sola propiedad en el seed.
2. Arrancar API.
3. Resultado esperado:
- Se omite solo la propiedad afectada.
- Se registra error estructurado.
- El resto de propiedades válidas se procesa.

## Escenario 5: Disponibilidad de rutas públicas de imagen

1. Verificar que imágenes se copian/sincronizan a destino final de API.
2. Inspeccionar propiedades persistidas.
3. Resultado esperado:
- `ImageUrl` persiste con patrón público `/assets/properties/{archivo}`.
- No se persisten rutas físicas de `support`.
- Las imágenes son servibles por la API en la ruta pública.

## Ejecución de pruebas

```powershell
dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

Resultado esperado:
- Pruebas en verde para:
  - configuración EF y conversión enum-string;
  - esquema esperado de migración;
  - idempotencia del seed;
  - ejecución de `UseAsyncSeeding`;
  - lectura de JSON + manifiesto;
  - consistencia `ImageUrl` + disponibilidad de imágenes.

## Referencias

- Contrato de interfaz: `specs/003-properties-persistence-seeding/contracts/backend-properties-persistence.openapi.yaml`
- Modelo de datos: `specs/003-properties-persistence-seeding/data-model.md`
- Investigación técnica: `specs/003-properties-persistence-seeding/research.md`

## Evidencia de validación

- Fecha: 2026-07-09
- Comando ejecutado: `dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj --logger "trx;LogFileName=spec003.trx" --results-directory app/backend/tests/RealtorApiTests/TestResults`
- Resultado: `total: 20, failed: 0, succeeded: 20, skipped: 0`
- Artefacto generado: `app/backend/tests/RealtorApiTests/TestResults/spec003.trx`
- Verificaciones cubiertas en la ejecución:
  - configuración EF y conversión enum-string;
  - migración/snapshot y esquema persistente esperado;
  - seeding idempotente y upsert preservando `Id`;
  - fail-fast por manifiesto/JSON críticos faltantes;
  - omisión selectiva por imagen faltante;
  - omisión selectiva por estado fuera de catálogo;
  - sincronización de imágenes a `wwwroot/assets/properties`;
  - disponibilidad pública de `/assets/properties/{fileName}`;
  - arranque de API con inicialización automática de base de datos y seeding.
