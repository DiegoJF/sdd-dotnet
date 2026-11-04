# Quickstart de Validación: Actualización de estado de propiedades

## Objetivo

Validar el endpoint público `PATCH /api/properties/{id}/status` para asegurar que actualiza solo el estado de una propiedad existente, conserva intactos los demás datos persistidos y responde con ProblemDetails cuando el request es inválido o el recurso no existe.

## Prerrequisitos

- .NET SDK 11.
- Backend configurado para desarrollo.
- Al menos una propiedad existente en la base de datos.
- El archivo [RealtorApi.http](../../app/backend/src/RealtorApi/RealtorApi.http) disponible para pruebas manuales.

## Preparación

1. Restaurar y compilar:

```powershell
dotnet restore app/Realtor.sln
dotnet build app/backend/src/RealtorApi/RealtorApi.csproj
dotnet build app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

2. Iniciar la API:

```powershell
dotnet run --project app/backend/src/RealtorApi/RealtorApi.csproj
```

3. Obtener un `id` válido de propiedad desde los datos existentes o desde los listados del backend.

## Referencias de diseño

- Contrato: [contracts/backend-properties-update-status.openapi.yaml](contracts/backend-properties-update-status.openapi.yaml)
- Modelo funcional: [data-model.md](data-model.md)

## Escenario 1: Actualización exitosa de estado

1. Ejecutar `PATCH /api/properties/{id}/status` con un cuerpo que contenga un `status` válido.
2. Resultado esperado:
   - `200 OK`.
   - La respuesta devuelve la propiedad actualizada.
   - Los demás datos, incluido `imageUrl`, permanecen idénticos.

## Escenario 2: Rechazo por propiedad no encontrada

1. Ejecutar `PATCH /api/properties/00000000-0000-0000-0000-000000000000/status` con un `status` válido.
2. Resultado esperado:
   - `404 Not Found`.
   - Respuesta ProblemDetails.
   - Ningún dato persistido se modifica.

## Escenario 3: Rechazo por `status` omitido o vacío

1. Ejecutar `PATCH /api/properties/{id}/status` sin enviar `status` o con cuerpo vacío.
2. Resultado esperado:
   - `400 Bad Request`.
   - Respuesta ProblemDetails.
   - La propiedad conserva sus valores previos.

## Escenario 4: Rechazo por `status` inválido

1. Ejecutar `PATCH /api/properties/{id}/status` con un valor fuera del catálogo vigente.
2. Resultado esperado:
   - `400 Bad Request`.
   - Respuesta ProblemDetails.
   - La propiedad conserva sus valores previos.

## Escenario 5: Rechazo por propiedades JSON inesperadas

1. Ejecutar `PATCH /api/properties/{id}/status` enviando `status` válido junto con una propiedad adicional no permitida.
2. Resultado esperado:
   - `400 Bad Request`.
   - Respuesta ProblemDetails.
   - El request no modifica ningún dato persistido.

## Escenario 6: Verificación manual desde `.http`

1. Abrir [RealtorApi.http](../../app/backend/src/RealtorApi/RealtorApi.http).
2. Ejecutar los ejemplos documentados para éxito, `404` y `400`.
3. Confirmar que el flujo público es reproducible sin usar autenticación ni imagen.

## Escenario 7: Verificación automatizada sugerida

```powershell
dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

Resultado esperado:

- Las pruebas del slice de actualización de estado pasan.
- Los rechazos por `400` y `404` quedan cubiertos.

## Criterios de salida

- El endpoint responde `200`, `400` y `404` según la condición del request.
- El cambio persiste solo `status`.
- `imageUrl` y el resto de campos permanecen intactos.

## Evidencia de validación

Fecha de ejecución: 2026-07-21

Estado del cierre: completado.

1. Compilación del backend

- Comando: `dotnet build app/backend/src/RealtorApi/RealtorApi.csproj`
- Resultado: éxito, con el contrato OpenAPI v1 regenerado y el nuevo endpoint `PATCH /api/properties/{id}/status` incluido.

2. Validación automatizada

- Comando: `dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj`
- Resultado: `33` pruebas exitosas, `0` fallidas, `0` omitidas.

3. Validación manual en `.http`

- Comando: ejecutar los ejemplos del archivo [RealtorApi.http](../../app/backend/src/RealtorApi/RealtorApi.http).
- Resultado esperado: éxito en `200`, `400` y `404` según el caso.

4. Rechazo por propiedades inesperadas

- Comando: ejecutar el escenario de propiedades JSON inesperadas documentado en este archivo.
- Resultado esperado: `400 Bad Request` con ProblemDetails.

5. Validación de contrato OpenAPI

- Comando: verificación del contrato generado en `app/backend/src/RealtorApi/wwwroot/openapi/v1.json`.
- Resultado: el path `PATCH /api/properties/{id}/status` quedó publicado con `UpdatePropertyStatus` y schema `UpdateStatusRequest`/`UpdateStatusResponse`.

6. Validación automatizada sugerida

- Comando: `dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj`
- Resultado esperado: suite verde para el slice de estado.
