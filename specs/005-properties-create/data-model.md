# Modelo de Datos: Alta de propiedades con imagen opcional

## Entidad de entrada: CreatePropertyRequest

Descripción: Solicitud pública de alta de propiedad con datos del inmueble y archivo opcional.

Campos funcionales:
- `title` (string, requerido)
- `description` (string, requerido)
- `address` (string, requerido)
- `price` (decimal, requerido)
- `status` (string o enum serializado, requerido)
- `bedroomCount` (int, requerido)
- `bathroomCount` (int, requerido)
- `areaSquareMeters` (decimal, requerido)
- `image` (archivo, opcional)

Reglas de validación:
- Si `image` se envía, debe ser formato PNG o JPG.
- Si `image` se envía, su tamaño debe ser menor o igual a 5 MB.
- Si `image` no se envía, el alta sigue siendo válida.

## Entidad de dominio persistida: Property

Descripción: Registro principal del catálogo inmobiliario existente que se crea con los datos del request.

Campos relevantes:
- `Id` (Guid)
- `Title`
- `Description`
- `Address`
- `Price`
- `Status`
- `BedroomCount`
- `BathroomCount`
- `AreaSquareMeters`
- `ImageUrl` (string nullable)

Reglas de persistencia:
- Sin imagen: `ImageUrl = null`.
- Con imagen válida: `ImageUrl` contiene URL pública/relativa persistida.
- Si falla persistencia de imagen, no se debe crear `Property` (atomicidad).

## Entidad derivada: PropertyImageAsset

Descripción: Recurso de imagen opcional asociado a la propiedad.

Campos funcionales:
- `storedFileName` (string; `UUID + extensión`)
- `relativePublicPath` (string; p.ej. `/assets/properties/{storedFileName}`)
- `contentType` (string)
- `sizeBytes` (long)

Reglas:
- `storedFileName` debe ser único por operación.
- El nombre original del cliente no debe gobernar la persistencia.

## Entidad de respuesta: CreatePropertyResponse

Descripción: Contrato de salida para operación de creación exitosa.

Campos:
- `id` (Guid)
- `title` (string)
- `status` (string)
- `imageUrl` (string nullable)

Reglas:
- En altas sin imagen, `imageUrl` debe ser `null`.
- En altas con imagen válida, `imageUrl` debe ser no nulo y consumible por clientes.

## Entidad de error: CreatePropertyError

Descripción: Representación funcional de errores esperados del endpoint.

Estados esperados:
- `400 Bad Request`: campos de negocio inválidos o incompletos.
- `415 Unsupported Media Type`: formato de imagen no permitido.
- `413 Payload Too Large`: imagen excede 5 MB.
- `500 Internal Server Error`: fallo interno durante almacenamiento de imagen.

Reglas:
- Si los campos de negocio del alta son inválidos o incompletos, la solicitud debe rechazarse con `400`.
- Si la imagen está vacía (0 bytes) o su contenido real no corresponde al formato declarado, la solicitud debe rechazarse con `415`.
- Ante error de validación o error interno de imagen, no debe quedar propiedad creada.
- La respuesta debe ser consistente con ProblemDetails del sistema.

## Transiciones de estado

1. `Solicitud recibida` -> `Validación de request`.
2. Si hay imagen válida: `Validación OK` -> `Guardar imagen` -> `Crear propiedad` -> `Creada`.
3. Si no hay imagen: `Validación OK` -> `Crear propiedad` -> `Creada`.
4. Si falla validación de imagen: `Validación fallida` -> `Rechazada` (sin persistencia).
5. Si falla guardado de imagen: `Fallo interno` -> `Rechazada` (sin persistencia por atomicidad).

## Relaciones

- `CreatePropertyRequest` produce una nueva `Property`.
- `PropertyImageAsset` existe solo cuando `image` es enviada y válida.
- `CreatePropertyResponse` expone resumen consumible de `Property`.
- `CreatePropertyError` representa rechazo sin efectos persistentes.
