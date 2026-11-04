# Modelo de Datos: Listado Paginado de Propiedades

## Entidad de lectura: Property

Descripción: Registro persistente existente del inventario de propiedades que actúa como fuente para el listado público.

Campos relevantes para esta feature:
- `Id` (Guid, requerido)
- `Title` (string, requerido)
- `Description` (string, requerido)
- `Address` (string, requerido)
- `Price` (decimal, requerido)
- `Status` (enum de dominio o valor persistido equivalente, requerido)
- `BedroomCount` (int, requerido)
- `BathroomCount` (int, requerido)
- `AreaSquareMeters` (decimal, requerido)
- `ImageUrl` (string, requerido; referencia al archivo de imagen)

Reglas:
- La consulta debe leer sin seguimiento de cambios.
- El orden funcional del listado es `Title` ascendente.
- El valor persistido de `ImageUrl` se usa como referencia del archivo para construir la URL absoluta pública.

## Entidad de contrato: ListPropertiesQuery

Descripción: Parámetros de consulta del endpoint público.

Campos:
- `Page` (int, opcional en la entrada; por defecto `1`)
- `PageSize` (int, opcional en la entrada; por defecto `6`)

Reglas:
- `Page` debe ser mayor que cero.
- `PageSize` debe ser mayor que cero.
- Si no se envían valores, se aplican los valores por defecto.

## Entidad de contrato: PropertyListItem

Descripción: Representación pública de una propiedad dentro del listado paginado.

Campos:
- `id` (Guid)
- `title` (string)
- `description` (string)
- `address` (string)
- `price` (decimal)
- `status` (string o valor serializado equivalente)
- `bedroomCount` (int)
- `bathroomCount` (int)
- `areaSquareMeters` (decimal)
- `imageUrl` (string, URL absoluta pública)

Reglas:
- No expone detalles internos de persistencia.
- `imageUrl` debe comenzar con el esquema y host de la solicitud actual.

## Entidad de contrato: PaginationMetadata

Descripción: Metadatos que describen el estado de la paginación del resultado.

Campos:
- `page` (int)
- `pageSize` (int)
- `totalItems` (int)
- `totalPages` (int)
- `hasNext` (bool)
- `hasPrevious` (bool)

Reglas:
- `totalPages` debe calcularse a partir de `totalItems` y `pageSize`.
- `hasNext` y `hasPrevious` deben derivarse de la página solicitada y del total disponible.

## Entidad de contrato: PagedPropertyListResponse

Descripción: Envoltura de respuesta del endpoint `/api/properties`.

Campos:
- `items` (colección de `PropertyListItem`)
- `page` (int)
- `pageSize` (int)
- `totalItems` (int)
- `totalPages` (int)
- `hasNext` (bool)
- `hasPrevious` (bool)

Reglas:
- Debe devolver colección vacía cuando no existan elementos para la página solicitada.
- Debe conservar metadatos consistentes incluso cuando `items` esté vacío.

## Relaciones

- `ListPropertiesQuery` controla la lectura de `Property`.
- `PagedPropertyListResponse.items` contiene proyecciones derivadas de `Property`.
- `PaginationMetadata` forma parte de la misma respuesta pública.

## Validación funcional

- `page <= 0` no es válido.
- `pageSize <= 0` no es válido.
- `imageUrl` debe ser absoluto y público.
- No se admite devolver entidades EF directamente.