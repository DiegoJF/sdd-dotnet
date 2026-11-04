# Modelo de Datos: Persistencia Inicial de Propiedades

## Entidad: PropertyStatus

Descripción: Catálogo de estados disponibles para una propiedad.

Campos:
- `Value` (string, PK, requerido, longitud máxima 32)
- `Description` (string, requerido, longitud máxima 200)

Reglas:
- Valores iniciales obligatorios: `Available`, `Rented`, `Maintenance`.
- `Value` debe ser único y estable para referencia desde `Property.Status`.

## Entidad: Property

Descripción: Registro principal del inventario de propiedades.

Campos:
- `Id` (Guid, PK, requerido)
- `Title` (string, requerido, max 160)
- `Description` (string, requerido, max 1200)
- `Address` (string, requerido, max 240)
- `Price` (decimal(12,2), requerido)
- `Status` (enum de dominio persistido como string, requerido, max 32)
- `BedroomCount` (int, requerido, rango >= 0)
- `BathroomCount` (int, requerido, rango >= 0)
- `AreaSquareMeters` (decimal(10,2), requerido, rango > 0)
- `ImageUrl` (string, requerido, max 260; formato público `/assets/properties/{archivo}`)
- `CreatedAt` (DateTimeOffset, requerido)
- `UpdatedAt` (DateTimeOffset, nullable)

Reglas:
- `Status` debe pertenecer al catálogo funcional (`Available`, `Rented`, `Maintenance`).
- `ImageUrl` no puede contener rutas físicas locales ni rutas de `support`.
- Para upsert, `Id` se preserva y se actualizan campos funcionales permitidos.

## Entidad auxiliar: SeedManifest

Descripción: Documento central de resolución de rutas para seeding.

Campos esperados:
- `version` (string, requerido)
- `statusesFile` (string, requerido)
- `propertiesFile` (string, requerido)
- `imagesSourceDirectory` (string, requerido)
- `imagesTargetDirectory` (string, requerido)
- `publicImageBasePath` (string, requerido)

Reglas:
- Ausencia del manifiesto o JSON críticos implica fail-fast en arranque.
- Rutas del manifiesto son la única fuente de verdad para ubicar archivos de seed.

## Relaciones

- `Property.Status` referencia semánticamente a `PropertyStatus.Value`.
- Esta relación puede implementarse con FK explícita o validación de catálogo durante seeding, manteniendo consistencia funcional del dominio.

## Transiciones de estado

Ámbito de esta spec:
- Se inicializan estados y propiedades.
- No se define aún una máquina de transición de negocio para cambios de estado en runtime.

Estados válidos iniciales:
- `Available`
- `Rented`
- `Maintenance`
