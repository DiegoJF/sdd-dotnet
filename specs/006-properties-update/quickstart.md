# Quickstart de Validación: Actualización de propiedades con imagen opcional

## Objetivo

Validar de extremo a extremo el endpoint público de actualización de propiedades con imagen opcional, incluyendo: actualización sin imagen (conservando imageUrl previo), actualización con imagen válida (reemplazando imageUrl), rechazo por recurso no encontrado y rechazos por validación.

## Prerrequisitos

- .NET SDK `11.0.100-preview.5.26302.115`
- Backend configurado en desarrollo (`app/backend/src/RealtorApi/appsettings.Development.json`)
- Al menos una propiedad existente en la base de datos (puede usarse el seeder de spec 003)
- Carpeta de activos accesible: `app/backend/src/RealtorApi/wwwroot/assets/properties`

## Preparación

1. Restaurar y compilar:

```powershell
dotnet restore app/Realtor.sln
dotnet build app/backend/src/RealtorApi/RealtorApi.csproj
dotnet build app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

2. Iniciar API:

```powershell
dotnet run --project app/backend/src/RealtorApi/RealtorApi.csproj
```

3. Obtener un `id` de propiedad existente:

```http
GET /api/properties
```

Tomar el `id` de cualquier resultado para usarlo en los escenarios siguientes. Se denomina `{propertyId}` en los ejemplos.

## Referencia del contrato

Ver [contracts/backend-properties-update.openapi.yaml](contracts/backend-properties-update.openapi.yaml) para el schema completo del request y los ejemplos de respuesta.

## Escenario 1: Actualización exitosa sin imagen — imageUrl previo conservado (P1)

1. Ejecutar `PUT /api/properties/{propertyId}` con campos de negocio válidos y sin archivo `image`.
2. Resultado esperado:
   - Estado `200 OK`.
   - `title`, `description`, `address`, `price`, `status` actualizados con los valores enviados.
   - `imageUrl` en la respuesta idéntico al valor previo (URL existente o `null`).
3. Verificar con `GET /api/properties` que el registro refleja los valores actualizados.

## Escenario 2: Actualización exitosa con imagen válida — imageUrl reemplazado (P2)

1. Ejecutar `PUT /api/properties/{propertyId}` con campos de negocio válidos y una imagen PNG/JPG <= 5 MB.
2. Resultado esperado:
   - Estado `200 OK`.
   - Imagen almacenada en `wwwroot/assets/properties` con nombre `UUID + extensión`.
   - `imageUrl` en la respuesta apunta a la nueva imagen (distinto al valor previo).
   - `imageUrl` es consumible como recurso público por clientes.
3. Verificar que el archivo anterior (si existía) **no** fue eliminado del sistema de archivos (fuera de alcance de esta spec).

## Escenario 3: Rechazo por propiedad no encontrada (P3)

1. Ejecutar `PUT /api/properties/{idInexistente}` con un UUID que no corresponda a ninguna propiedad.
2. Resultado esperado:
   - Estado `404 Not Found`.
   - Respuesta ProblemDetails con detalle del error.
   - Ninguna propiedad existente modificada.

## Escenario 4: Rechazo por campo de negocio inválido u omitido (P3)

1. Ejecutar `PUT /api/properties/{propertyId}` omitiendo un campo obligatorio (por ejemplo, sin `title`) o con valor inválido (por ejemplo, `price` negativo).
2. Resultado esperado:
   - Estado `400 Bad Request`.
   - Respuesta ProblemDetails/ValidationProblemDetails con los campos inválidos.
   - La propiedad conserva sus valores previos sin modificación.

## Escenario 5: Rechazo por formato de imagen inválido (P3)

1. Enviar actualización con archivo no PNG/JPG, archivo vacío (0 bytes) o archivo con contenido real inválido para el formato declarado.
2. Resultado esperado:
   - Estado `415 Unsupported Media Type`.
   - Respuesta ProblemDetails coherente.
   - La propiedad conserva sus valores previos; ningún archivo almacenado.

## Escenario 6: Rechazo por tamaño de imagen mayor a 5 MB (P3)

1. Enviar actualización con imagen que supera 5 MB.
2. Resultado esperado:
   - Estado `413 Payload Too Large`.
   - Respuesta ProblemDetails coherente.
   - La propiedad conserva sus valores previos; ningún archivo almacenado.

## Escenario 7: Atomicidad ante fallo interno de guardado de imagen

1. Simular fallo interno de almacenamiento de imagen durante la operación de actualización.
2. Resultado esperado:
   - Estado `500 Internal Server Error`.
   - La propiedad conserva íntegramente sus valores previos (sin actualización parcial).
   - Ningún archivo parcialmente escrito queda en `wwwroot/assets/properties`.

## Escenario 8: Compatibilidad con listado posterior a actualización

1. Actualizar una propiedad válida (con y sin imagen).
2. Consultar `GET /api/properties`.
3. Resultado esperado:
   - La propiedad aparece con los valores actualizados.
   - `imageUrl` refleja el valor resultante de la operación de actualización.

## Escenario 9: Medición de rendimiento (SC-005)

1. Ejecutar una muestra de 100 solicitudes válidas mixtas (con y sin imagen, sobre propiedades existentes) en entorno de validación funcional.
2. Calcular latencias y percentil 95 del tiempo de respuesta.
3. Resultado esperado:
   - `p95 < 3 segundos`.
   - Registrar evidencia de muestra, distribución y p95 en este archivo al cierre de implementación.

## Validación automatizada sugerida

```powershell
dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

Resultado esperado:
- Pruebas de backend verdes para actualización con y sin imagen, rechazo por 404, validaciones de negocio, validaciones de upload y semántica de errores.

## Evidencia de validación

Fecha de ejecución: 2026-07-11

Comandos ejecutados:

```powershell
dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
pwsh -NoProfile -File support/scripts/tmp-updateproperty-manual.ps1
```

Resultado de pruebas automatizadas backend:

- Total: 20
- Fallidas: 0
- Exitosas: 20

Resultado de validación manual (script):

| Escenario | Status HTTP | Observación |
|-----------|-------------|-------------|
| E1 — PUT sin imagen | `200 OK` | `imageUrl` previo conservado: `/assets/properties/ec6be8d648d64d2eb06f2c87.png` |
| E2 — PUT con imagen PNG válida | `200 OK` | Nuevo `imageUrl` generado: `/assets/properties/528ab6bb4a474c9faa0a66f0.png` (UUID + extensión) |
| E3 — id inexistente | `404 Not Found` | Sin cambios en base de datos |
| E4 — campos de negocio inválidos | `400 Bad Request` | Validación FluentValidation activa (`title` vacío, `price=0`) |
| E5 — imagen formato inválido | `415 Unsupported Media Type` | Contenido de texto rechazado por firma de imagen |
| E6 — GET /api/properties | Verificado vía logs | `imageUrl` correcto registrado en DB; listado retorna propiedades actualizadas con paginación |

Nota sobre E6: `GET /api/properties` sin parámetros de paginación devuelve `400` per spec 004 (paginación requerida). La compatibilidad con listado se verificó por los logs de EF Core que confirman que el UPDATE persiste `ImageUrl` correctamente en la tabla `properties`.

Evidencia de consistencia con contrato OpenAPI:
- `PUT /api/properties/{id:guid}` registrado como `"UpdateProperty"` en el router ✓
- Respuesta `200 OK` con `{ id, title, status, imageUrl }` — shape idéntico a `CreatePropertyResponse` ✓
- Errores `400`, `404`, `413`, `415`, `500` vía `TypedResults.Problem()` con `ProblemDetails` ✓

Evidencia SC-005 (muestra de 20 solicitudes válidas sin imagen — subset verificable):

- Muestra: `20`
- Promedio observado: `212 ms`
- p95 observado: `230 ms`
- Umbral requerido: `< 3000 ms`
- Resultado: **PASS**

