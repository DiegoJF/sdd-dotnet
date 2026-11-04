# Modelo de Datos: Actualización de estado de propiedades

## Entidad de entrada: UpdateStatusRequest

Descripción: Solicitud pública mínima para cambiar el estado de una propiedad existente.

Campos funcionales:

- `status` (requerido)

Reglas:

- El cuerpo debe incluir `status`; si falta, la operación responde `400 Bad Request`.
- El valor debe pertenecer al catálogo vigente del dominio.
- No se admiten otros campos editables en esta operación.
- No se admiten propiedades JSON inesperadas en el request.

## Entidad de dominio persistida: Property

Descripción: Registro existente del catálogo inmobiliario que se actualiza por `id`.

Campos que se modifican:

- `Status` ← nuevo valor solicitado
- `UpdatedAt` ← marca temporal de persistencia

Campos que deben conservarse:

- `Id`
- `Title`
- `Description`
- `Address`
- `Price`
- `BedroomCount`
- `BathroomCount`
- `AreaSquareMeters`
- `ImageUrl`
- `CreatedAt`

Reglas de persistencia:

- La propiedad se localiza por `id` antes de aplicar cualquier cambio.
- Si la propiedad no existe, la operación se corta con `404 Not Found`.
- Si el estado es válido, solo cambia `Status`; el resto de datos se preserva.

## Entidad de salida: UpdateStatusResponse

Descripción: Contrato de salida para la actualización exitosa.

Campos funcionales:

- `id`
- `title`
- `status`
- `imageUrl`

Reglas:

- La respuesta debe reflejar el estado ya persistido.
- `imageUrl` debe mantenerse idéntico al valor previo, ya que no se toca la imagen.
- La respuesta se retorna con `200 OK`.

## Entidad de error: UpdateStatusError

Descripción: Respuesta funcional para rechazos esperados del endpoint.

Estados esperados:

- `400 Bad Request`: estado omitido, inválido o cuerpo vacío.
- `404 Not Found`: la propiedad identificada por `id` no existe.

Reglas:

- Los errores deben exponerse como ProblemDetails.
- Ante errores funcionales, la propiedad no debe modificarse.

## Relaciones

- `UpdateStatusRequest` modifica una `Property` existente identificada por `id` de ruta.
- `UpdateStatusResponse` expone el resumen actualizado de `Property`.
- `UpdateStatusError` representa los rechazos sin efectos persistentes.