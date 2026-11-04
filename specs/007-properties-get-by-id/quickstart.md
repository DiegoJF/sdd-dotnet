# Quickstart de Validación: Consulta de propiedad por id

## Objetivo

Validar de extremo a extremo el endpoint `GET /api/properties/{id}` garantizando:

- Respuesta unitaria `200` con campos públicos alineados con spec 004.
- Error `404` para id válido no existente.
- Error `400` para id inválido.
- `imageUrl` absoluto y público cuando exista imagen.
- `imageUrl = null` cuando la propiedad no tenga imagen.

## Prerrequisitos

- .NET SDK 11 instalado.
- Base de datos de desarrollo disponible y con propiedades de prueba.
- Backend arrancando desde `app/backend/src/RealtorApi`.

## Preparación

1. Restaurar y compilar:

```powershell
dotnet restore app/Realtor.sln
dotnet build app/backend/src/RealtorApi/RealtorApi.csproj
dotnet build app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

2. Levantar API:

```powershell
dotnet run --project app/backend/src/RealtorApi/RealtorApi.csproj
```

3. Obtener un id válido existente (vía listado):

```http
GET /api/properties?page=1&pageSize=1
```

Guarda el `id` como `{existingId}` para los escenarios.

## Referencias del diseño

- Contrato API: [contracts/backend-properties-get-by-id.openapi.yaml](contracts/backend-properties-get-by-id.openapi.yaml)
- Modelo de datos: [data-model.md](data-model.md)

## Escenario 1: Consulta exitosa con imagen

1. Ejecutar `GET /api/properties/{existingId}` para una propiedad con imagen.
2. Validar:
   - Estado `200 OK`.
   - Campos presentes: `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters`, `imageUrl`.
   - `imageUrl` en formato absoluto `http(s)://{host}/assets/properties/{fileName}`.

## Escenario 2: Consulta exitosa sin imagen

1. Ejecutar `GET /api/properties/{existingIdWithoutImage}` para una propiedad sin imagen.
2. Validar:
   - Estado `200 OK`.
   - `imageUrl = null`.
   - Sin metadatos de paginación en la respuesta.

## Escenario 3: Id inexistente

1. Ejecutar `GET /api/properties/{nonExistingUuid}` con UUID válido no persistido.
2. Validar:
   - Estado `404 Not Found`.
   - Cuerpo `application/problem+json`.

## Escenario 4: Id inválido

1. Ejecutar `GET /api/properties/not-a-uuid`.
2. Validar:
   - Estado `400 Bad Request`.
   - Cuerpo `application/problem+json` con detalle de validación del id.

## Escenario 5: Host/esquema variable

1. Consultar una propiedad con imagen en dos contextos (ejemplo: `http://localhost:5023` y otro host/esquema equivalente del entorno).
2. Validar:
   - `imageUrl` cambia su prefijo de esquema/host según cada request.
   - El path final mantiene `/assets/properties/{fileName}`.

## Verificación automatizada sugerida

```powershell
dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj --filter "GetPropertyById"
```

Resultado esperado:
- Pruebas de consulta por id en verde para casos `200`, `400` y `404`.

## Criterios de salida

- Todos los escenarios anteriores cumplen el comportamiento esperado.
- El contrato de respuesta se mantiene consistente con 004.
- No hay metadatos de paginación en la operación unitaria.

## Evidencia de validación

- Fecha/hora de ejecución: 2026-07-11T10:12:42.6744178-04:00.
- Validación de escenarios 1-5: `dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj --filter "GetPropertyById"`.
- Resultado de pruebas focalizadas: 5 pruebas aprobadas, 0 fallas, 0 omitidas.
- Escenario 1 (`200` con imagen): validado por `GetById_Returns200_WithPublicContractWithoutPaginationMetadata`.
- Escenario 2 (`200` sin imagen): validado por `GetById_Returns200WithNullImageUrl_WhenPropertyHasNoImage`.
- Escenario 3 (`404` id inexistente): validado por `GetById_Returns404ProblemDetails_WhenPropertyDoesNotExist`.
- Escenario 4 (`400` id inválido): validado por `GetById_Returns400ProblemDetails_WhenIdIsInvalid`.
- Escenario 5 (host/esquema variable): validado por `GetById_BuildsAbsoluteImageUrl_WithRequestHostAndScheme` usando base address `https://qa.realtor.local`.
- Validación de suite backend completa: `dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj` sin regresiones funcionales.

## Evidencia de validación manual (.http)

- Se actualizaron y verificaron los escenarios manuales de `GET /api/properties/{id}` en `app/backend/src/RealtorApi/RealtorApi.http` para 200 con imagen, 200 sin imagen, 400 y 404.
- El escenario `404` incluye payload esperado de `ProblemDetails` alineado al contrato.
- El escenario `200` sin imagen deja variable explícita para ejecutar con un id real sin imagen generado en entorno.

## Checklist de cierre de alcance

- [x] Solo se modificó el slice `GetPropertyById` para lógica nueva de endpoint.
- [x] Se mantuvo el contrato OpenAPI de la spec sin desviaciones funcionales.
- [x] `quickstart.md` contiene evidencia de validación no vacía.
- [x] `RealtorApi.http` contiene escenarios manuales actualizados del endpoint.

### Archivos tocados en esta implementación

- `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdContracts.cs`
- `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdQuery.cs`
- `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdValidator.cs`
- `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdMapping.cs`
- `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdHandler.cs`
- `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdSlice.cs`
- `app/backend/src/RealtorApi/RealtorApi.http`
- `app/backend/tests/RealtorApiTests/Infrastructure/Endpoints/GetPropertyByIdEndpointTests.cs`
- `specs/007-properties-get-by-id/quickstart.md`
