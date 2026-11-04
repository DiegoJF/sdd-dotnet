# Modelo de Datos: Consulta de propiedad por id

## Entidad de entrada: GetPropertyByIdRequest

Descripción: Entrada funcional de la operación de consulta unitaria.

Campos:
- `id` (UUID/GUID RFC 4122, requerido, en ruta)

Reglas:
- Si `id` no cumple formato UUID, la operación debe rechazar con `400 Bad Request`.
- Si `id` es UUID válido pero no existe en almacenamiento, la operación debe responder `404 Not Found`.

## Entidad de dominio consultada: Property

Descripción: Registro persistido de propiedad en el catálogo inmobiliario.

Campos relevantes para lectura:
- `Id`
- `Title`
- `Description`
- `Address`
- `Price`
- `Status`
- `BedroomCount`
- `BathroomCount`
- `AreaSquareMeters`
- `ImageUrl` (ruta relativa persistida o `null`)

Reglas de acceso:
- La consulta se resuelve por clave primaria (`Id`) con soporte de `CancellationToken`.
- La entidad EF no se expone directamente al contrato público.

## Entidad de salida: PropertyDetailResponse

Descripción: Contrato público de respuesta para consulta unitaria exitosa.

Campos:
- `id` (uuid)
- `title` (string)
- `description` (string)
- `address` (string)
- `price` (number)
- `status` (string)
- `bedroomCount` (integer)
- `bathroomCount` (integer)
- `areaSquareMeters` (number)
- `imageUrl` (string nullable)

Reglas de mapeo:
- Todos los campos mantienen semántica y nombres de exposición coherentes con spec 004.
- Si `ImageUrl` persistido existe, se transforma a URL absoluta pública basada en request actual.
- Si `ImageUrl` persistido es `null`, `imageUrl` de salida es `null`.
- No se incluyen metadatos de paginación.

## Entidad derivada: PublicImageUrl

Descripción: Valor calculado para exponer la imagen a cliente externo.

Composición:
- Entrada: `scheme` + `host` de la request actual y `fileName` derivado de la ruta persistida.
- Salida: `http(s)://{host}/assets/properties/{fileName}`.

Reglas:
- Nunca devolver rutas físicas del servidor.
- Nunca devolver URL relativa cuando exista imagen.
- Debe adaptarse a cambios de host/esquema por entorno.

## Entidad de error: QueryProblem

Descripción: ProblemDetails público para errores de consulta.

Estados esperados:
- `400 Bad Request` para id inválido (formato no UUID).
- `404 Not Found` para id válido no encontrado.

Reglas:
- En errores, no se devuelve cuerpo de propiedad.
- El formato de error sigue ProblemDetails del proyecto.

## Transiciones funcionales

1. `Request recibida` -> `Validación de id`.
2. Si id inválido -> `400 ProblemDetails`.
3. Si id válido -> `Búsqueda por PK`.
4. Si no existe -> `404 ProblemDetails`.
5. Si existe -> `Mapeo a contrato público`.
6. `Construcción imageUrl` (absoluta si hay imagen, null si no hay imagen).
7. `200 OK` con `PropertyDetailResponse`.

## Relaciones

- `GetPropertyByIdRequest.id` selecciona una única `Property`.
- `Property` se proyecta a `PropertyDetailResponse` por mapeo explícito del slice.
- `PublicImageUrl` depende de la request actual y del valor persistido de imagen.
- `QueryProblem` se activa en ramas de error de validación o no encontrado.
