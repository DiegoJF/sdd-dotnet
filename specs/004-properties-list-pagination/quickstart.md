# Quickstart de Validación: Listado Paginado de Propiedades

## Objetivo

Validar extremo a extremo que `GET /api/properties` devuelve el listado paginado de propiedades con orden correcto, metadatos coherentes, validación de parámetros y URLs absolutas públicas para las imágenes.

## Prerrequisitos

- .NET SDK `11.0.100-preview.5.26302.115`
- PostgreSQL accesible para el backend
- `app/backend/src/RealtorApi/appsettings.Development.json` configurado con la conexión de desarrollo

## Preparación

1. Restaurar dependencias:

```powershell
dotnet restore app/Realtor.sln
```

2. Compilar backend y pruebas:

```powershell
dotnet build app/backend/src/RealtorApi/RealtorApi.csproj
dotnet build app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

## Escenario 1: Consulta por defecto

1. Iniciar la API:

```powershell
dotnet run --project app/backend/src/RealtorApi/RealtorApi.csproj
```

2. Consultar:

```http
GET http://localhost:5023/api/properties
Accept: application/json
```

3. Resultado esperado:
- Estado `200`.
- `page = 1` y `pageSize = 6`.
- Orden por `title` ascendente.
- Cada elemento incluye `imageUrl` absoluta con patrón `http://localhost:5023/assets/properties/{fileName}`.

## Escenario 2: Paginación explícita

1. Consultar la segunda página:

```http
GET http://localhost:5023/api/properties?page=2&pageSize=6
Accept: application/json
```

2. Resultado esperado:
- Estado `200`.
- Metadatos coherentes con el total de propiedades.
- `hasNext` y `hasPrevious` reflejan correctamente la posición de la página.

## Escenario 3: Validación de parámetros

1. Consultar con valores inválidos:

```http
GET http://localhost:5023/api/properties?page=0&pageSize=6
Accept: application/json
```

2. Resultado esperado:
- Estado `400`.
- Respuesta de validación tipo `ProblemDetails` o `ValidationProblemDetails`.
- Mensaje claro sobre la invalidez de `page` y/o `pageSize`.

## Escenario 4: URLs públicas de imagen

1. Revisar la respuesta de la consulta por defecto.
2. Resultado esperado:
- `imageUrl` comienza con `http://localhost:5023/assets/properties/`.
- No aparecen rutas físicas de servidor ni rutas relativas en la respuesta.

## Escenario 5: Verificación automatizada

```powershell
dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

Resultado esperado:
- Las pruebas de integración del backend validan la consulta por defecto, la paginación explícita, la validación de parámetros y la construcción de `imageUrl` absoluta.

## Referencias

- Modelo de datos: [data-model.md](data-model.md)
- Contrato público: [contracts/backend-properties-list-pagination.openapi.yaml](contracts/backend-properties-list-pagination.openapi.yaml)
- Especificación: [spec.md](spec.md)

## Evidencia de validación

- Fecha: 2026-07-09
- Compilación verificada: `dotnet build app/backend/src/RealtorApi/RealtorApi.csproj`
- Pruebas verficadas: `dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj`
- Validación manual ejecutada contra `http://localhost:5023` con SQLite temporal porque la conexión remota de desarrollo no estaba disponible en el entorno local
- Resultado observado:
  - `page = 1`
  - `pageSize = 6`
  - `totalItems = 10`
  - `totalPages = 2`
  - `hasNext = true`
  - `hasPrevious = false`
  - `firstTitle = Casa Coral Way con jardín tropical`
  - `firstImageUrl = http://localhost:5023/assets/properties/1.png`
  - `page=0&pageSize=6` responde `400` con `application/problem+json`