# Modelo de Datos: Actualización de propiedades con imagen opcional

## Entidad de entrada: UpdatePropertyRequest

Descripción: Solicitud pública de actualización de propiedad con datos editables del inmueble y archivo opcional.

Campos funcionales:
- `title` (string, requerido)
- `description` (string, requerido)
- `address` (string, requerido)
- `price` (decimal, requerido)
- `status` (string o enum serializado, requerido)
- `image` (archivo, opcional)

Campos excluidos deliberadamente (no editables):
- `bedroomCount` — atributo estructural inmutable tras creación (decisión de clarificación Q1)
- `bathroomCount` — atributo estructural inmutable tras creación (decisión de clarificación Q1)
- `areaSquareMeters` — atributo estructural inmutable tras creación (decisión de clarificación Q1)

Reglas de validación:
- Todos los campos excepto `image` son obligatorios; campo omitido o con valor inválido devuelve `400 Bad Request`.
- Si `image` se envía, debe ser formato PNG o JPG con contenido real correspondiente al formato declarado.
- Si `image` se envía, su tamaño debe ser menor o igual a 5 MB.
- Si `image` no se envía, `imageUrl` de la propiedad existente se conserva sin modificación.

## Entidad de dominio persistida: Property (actualización)

Descripción: Registro existente del catálogo inmobiliario que se actualiza con los datos del request.

Campos modificados por la operación:
- `Title` ← `request.title`
- `Description` ← `request.description`
- `Address` ← `request.address`
- `Price` ← `request.price`
- `Status` ← `request.status` (validado contra enum `PropertyStatusValue`)
- `ImageUrl` ← nueva URL pública si se envía imagen válida; valor previo si no se envía imagen
- `UpdatedAt` ← `DateTimeOffset.UtcNow` al momento de persistir

Campos preservados (no modificables):
- `Id` — identidad inmutable
- `BedroomCount` — atributo estructural inmutable
- `BathroomCount` — atributo estructural inmutable
- `AreaSquareMeters` — atributo estructural inmutable
- `CreatedAt` — metadato de auditoría inmutable

Reglas de persistencia:
- Antes de actualizar, el sistema localiza la propiedad por `Id` mediante `FindAsync`.
- Si la propiedad no existe, se interrumpe la operación y se devuelve `404 Not Found`.
- Si no se envía imagen, `ImageUrl` se conserva con el valor previo (incluyendo `null`).
- Si se envía imagen válida, `ImageUrl` se reemplaza con la nueva ruta pública persistida.
- Si falla el guardado de imagen, la operación completa es atómica: `SaveChangesAsync` no se ejecuta.

## Entidad derivada: PropertyImageAsset

Descripción: Recurso de imagen nuevo y opcional almacenado durante la actualización. Idéntico al de spec 005.

Campos funcionales:
- `storedFileName` (string; `UUID + extensión`)
- `relativePublicPath` (string; `/assets/properties/{storedFileName}`)
- `contentType` (string)
- `sizeBytes` (long)

Reglas:
- `storedFileName` debe ser único por operación (UUID generado por el sistema).
- El nombre original del archivo del cliente no gobierna la persistencia.
- La imagen anterior no se elimina físicamente al ser reemplazada (fuera de alcance de esta spec).

## Entidad de respuesta: UpdatePropertyResponse

Descripción: Contrato de salida para operación de actualización exitosa. Mismo shape que `CreatePropertyResponse` de spec 005.

Campos:
- `id` (Guid)
- `title` (string)
- `status` (string)
- `imageUrl` (string nullable)

Reglas:
- `imageUrl` refleja el valor resultante tras la operación: URL actualizada si se envió imagen, valor previo (incluyendo `null`) si no se envió.
- La respuesta se retorna con `200 OK`.

## Entidad de error: UpdatePropertyError

Descripción: Representación funcional de errores esperados del endpoint. Extiende los errores de spec 005 con el caso de recurso no encontrado.

Estados esperados:
- `400 Bad Request`: campos editables obligatorios inválidos, incompletos u omitidos.
- `404 Not Found`: la propiedad identificada por el `id` del path no existe.
- `413 Payload Too Large`: imagen excede 5 MB.
- `415 Unsupported Media Type`: formato de imagen no permitido, archivo vacío (0 bytes) o contenido real no corresponde al formato declarado.
- `500 Internal Server Error`: fallo interno durante almacenamiento de imagen.

Reglas:
- Ante `404`, no se ejecuta ninguna escritura.
- Ante cualquier error de validación (`400`, `413`, `415`), la propiedad conserva sus valores previos sin modificación.
- Ante `500`, la operación es atómica: ni datos de negocio ni referencia a imagen quedan actualizados.
- Las respuestas de error son consistentes con ProblemDetails del sistema.

## Transiciones de estado

1. `Solicitud recibida` → `Validación de imagen (si aplica)`.
2. Si imagen inválida: `Validación de imagen fallida` → `Rechazada 415/413` (sin búsqueda ni persistencia).
3. `Validación de imagen OK` → `Búsqueda de propiedad por Id`.
4. Si propiedad no encontrada: `Búsqueda fallida` → `Rechazada 404` (sin escritura).
5. `Propiedad encontrada` → `Validación de campos de negocio`.
6. Si campos inválidos: `Validación de negocio fallida` → `Rechazada 400` (sin escritura).
7. Si hay imagen válida: `Validación OK` → `Guardar imagen` → `Actualizar propiedad` → `200 OK`.
8. Si no hay imagen: `Validación OK` → `Actualizar propiedad (conservando imageUrl previo)` → `200 OK`.
9. Si falla guardado de imagen: `Fallo interno` → `TryDelete` → `Rechazada 500` (sin `SaveChangesAsync`).

## Relaciones

- `UpdatePropertyRequest` modifica una `Property` existente identificada por `id` de ruta.
- `PropertyImageAsset` existe solo cuando `image` se envía y es válida; reutiliza la infraestructura de `CreatePropertyImageStorage`.
- `UpdatePropertyResponse` expone el resumen consumible de `Property` tras la actualización.
- `UpdatePropertyError` representa rechazo sin efectos persistentes (excepto TryDelete de imagen parcial ante `500`).
