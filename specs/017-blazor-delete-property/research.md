# Investigación: 017-blazor-delete-property

Este documento consolida las decisiones técnicas tomadas durante la Fase 0 a partir de `spec.md`, la constitución y el código existente.

## R-01 — Estrategia de eliminación

- **Decisión**: Eliminación física del registro mediante `dbContext.Properties.Remove(entity)` seguido de `SaveChangesAsync`.
- **Justificación**: RN-01 establece eliminación definitiva sin recuperación. La eliminación física cumple directamente RF-05 (no aparece en consultas) sin introducir filtros globales en `ListProperties` ni `GetPropertyById`, y evita una migración de esquema.
- **Alternativas consideradas**: Soft delete con columna `DeletedAt` y filtro global de consulta. Rechazada: exige migración, modifica slices existentes y agrega capacidad (recuperación) declarada fuera de alcance.

## R-02 — Concurrencia en eliminaciones simultáneas

- **Decisión**: Cargar la entidad con `FindAsync`; si no existe, devolver 404. Si `SaveChangesAsync` lanza `DbUpdateConcurrencyException` (la fila ya fue eliminada por otra solicitud entre la lectura y la escritura), traducirla a 404.
- **Justificación**: Garantiza el caso límite "exactamente una responde 204 y la otra 404" sin bloqueos explícitos. EF Core detecta 0 filas afectadas en `DELETE` y lanza esa excepción.
- **Alternativas consideradas**: `ExecuteDeleteAsync` evaluando filas afectadas. Válida y atómica, pero se descarta para mantener coherencia con los handlers existentes que operan sobre entidades rastreadas; se reserva como opción si surgen problemas de rendimiento.

## R-03 — Validación del identificador

- **Decisión**: Replicar el patrón de `GetPropertyById`: `DeletePropertyCommand.Create(rawId)` con `RawId` y `Id` parseado (`Guid.TryParseExact(..., "D")`), validado con `DeletePropertyValidator` (FluentValidation) que produce `ValidationProblemDetails` 400.
- **Justificación**: Consistencia con el contrato existente (`id debe ser un UUID canónico.`) y con el Principio V (validación consistente + ProblemDetails).
- **Alternativas consideradas**: Restricción de ruta `{id:guid}`. Rechazada porque produce 404 en lugar del 400 exigido por RF-04.

## R-04 — Imagen asociada

- **Decisión**: El handler no toca el archivo referenciado por `ImageUrl`.
- **Justificación**: RN-02 y fuera de alcance explícito.
- **Alternativas consideradas**: Borrado del archivo en `wwwroot/assets/properties`. Rechazada por alcance.

## R-05 — Publicación del contrato OpenAPI

- **Decisión**: Declarar el endpoint con `.WithName("DeleteProperty")`, `.Produces(204)`, `.ProducesValidationProblem()` y `.ProducesProblem(404)`, y regenerar `wwwroot/openapi/v1.json`.
- **Justificación**: Las pruebas `OpenApiDriftTests` comparan el documento publicado con la superficie real; sin regenerar el archivo fallaría la suite.
- **Alternativas consideradas**: Ninguna; es requisito del proyecto (spec 008).

## R-06 — Cliente Refit

- **Decisión**: Agregar `[Delete("/api/properties/{id}")] Task DeletePropertyAsync(string id, CancellationToken)` a `IPropertiesApi`. Los errores se capturan como `ApiException` filtrando por `StatusCode`, siguiendo el patrón de `PropertyUpdateForm`.
- **Justificación**: RF-12 y constitución (Refit + IHttpClientFactory). `id` como `string` mantiene consistencia con los métodos existentes.
- **Alternativas consideradas**: `Task<IApiResponse>`. Rechazada por inconsistencia con el resto del cliente.

## R-07 — Diálogo de confirmación reutilizable

- **Decisión**: Crear un componente `PropertyDeleteDialog.razor` en `Features/Properties/Delete/` que recibe el identificador y el título, invoca `IPropertiesApi.DeletePropertyAsync`, gestiona el estado `Idle/Deleting/Error` y emite `OnDeleted` y `OnNotFound` mediante `EventCallback`. Se usa el elemento HTML `<dialog>` con estilos en `wwwroot/app.css`, sin JavaScript adicional (renderizado condicional con superposición modal).
- **Justificación**: Un único componente cumple RF-08, RF-13, RF-15 y RF-16 en listado y detalle. El estado de solicitud en curso deshabilita ambos botones y evita solicitudes duplicadas.
- **Alternativas consideradas**: `window.confirm` vía JS interop (no permite mostrar error dentro ni deshabilitar botones); confirmación inline en la tarjeta (descartada en la spec).

## R-08 — Mensajes de resultado y navegación entre páginas

- **Decisión**: Servicio con ámbito `PropertyNoticeService` (scoped por circuito en Interactive Server) con `Set(notice)` y `Consume()` de un solo uso. El detalle establece el aviso y navega a `/`; el listado lo consume en `OnInitializedAsync` y lo muestra en un banner descartable. En eliminaciones desde el listado, el banner se establece directamente.
- **Justificación**: No existe mecanismo de mensajes previo. Un aviso de un solo uso evita que el mensaje reaparezca al recargar (problema de usar query string) y no requiere dependencias nuevas.
- **Alternativas consideradas**: Parámetro de query string `?notice=deleted` (persiste en URL y se repite al refrescar); librería de toasts (prohibida por la constitución sin justificación).

## R-09 — Recarga del listado paginado

- **Decisión**: Tras `OnDeleted` o `OnNotFound`, `PropertyList` invoca `LoadPageAsync(currentPage)`. Si la respuesta trae `Items` vacío y `currentPage > 1`, invoca `LoadPageAsync(currentPage - 1)`.
- **Justificación**: RF-14. Reutiliza el método existente sin cambios en el backend de listado.
- **Alternativas consideradas**: Eliminación local de la tarjeta (descartada en clarificación).

## R-10 — Logging estructurado

- **Decisión**: `LogWarning` para identificador inválido, `LogInformation` para no encontrado y para eliminación exitosa (`PropertyId`), siguiendo el formato en español de los slices existentes.
- **Justificación**: Principio V (logging estructurado obligatorio).

## R-11 — Estrategia de pruebas

- **Decisión**: Pruebas unitarias del validador y del handler sobre `AppDbContext` con el proveedor usado en las pruebas existentes, y pruebas de endpoint siguiendo `GetPropertyByIdEndpointTests` (ya presentes con `TestWebApplicationFactory`). En frontend, validación manual guiada en `quickstart.md`.
- **Justificación**: Principio V solo permite unit tests salvo justificación; las pruebas de endpoint ya existen como patrón aprobado en specs anteriores y se reutilizan para cubrir 204/400/404 sin introducir infraestructura nueva.
