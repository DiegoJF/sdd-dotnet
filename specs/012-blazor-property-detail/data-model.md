# Modelo Conceptual: Detalle de propiedad en frontend Blazor

## Propósito

Definir los conceptos funcionales de la página de detalle de propiedad y su integración de navegación con el listado existente, sin introducir cambios de dominio ni persistencia.

## Entidades conceptuales

### 1. DetalleDePropiedadPublica

Representa la información pública completa que se muestra en la vista de detalle.

Atributos clave:
- `Id` (Guid, requerido): identificador único de la propiedad.
- `Title` (string, requerido): título público.
- `Description` (string, requerido): descripción completa.
- `Address` (string, requerido): dirección pública.
- `Price` (decimal, requerido): precio publicado.
- `Status` (string, requerido): estado actual de publicación/comercialización.
- `BedroomCount` (int, requerido): dormitorios.
- `BathroomCount` (int, requerido): baños.
- `AreaSquareMeters` (decimal, requerido): superficie en m².
- `ImageUrl` (string?, opcional): imagen principal, puede ser nula.

Reglas:
- Debe corresponder 1:1 al contrato OpenAPI `PropertyDetailResponse`.
- No debe agregar ni ocultar campos públicos requeridos por la spec.

### 2. EstadoDePantallaDetalle

Representa el estado visual y funcional de la vista de detalle.

Valores conceptuales:
- `loading`
- `success`
- `not-found`
- `error`

Atributos clave:
- `StateType` (string, requerido): tipo de estado.
- `Message` (string?, opcional): mensaje contextual.
- `CanRetry` (bool, requerido): habilita acción de reintento si aplica.
- `CanBackToList` (bool, requerido): habilita retorno al listado.

Reglas:
- `not-found` se usa exclusivamente para HTTP 404.
- `error` agrupa errores de validación, backend y red no-404.

### 3. SolicitudDeDetalleDePropiedad

Representa la consulta de detalle por id.

Atributos clave:
- `RouteId` (string, requerido): id tomado de la ruta `/properties/{id}`.
- `CancellationToken` (token, requerido): cancelación de carga activa.

Reglas:
- Debe ejecutarse por `IPropertiesApi` registrado por DI.
- No debe ejecutarse con clientes HTTP creados fuera del registro Refit.

### 4. NavegacionDetalleDesdeListado

Representa la transición de la tarjeta del listado al detalle.

Atributos clave:
- `SourceRoute` (string, requerido): `/`.
- `TargetRoute` (string, requerido): `/properties/{id}`.
- `PropertyId` (Guid, requerido): id de la propiedad seleccionada.
- `ActionLabel` (string, requerido): "Ver".

Reglas:
- La navegación debe ser declarativa con mecanismo de navegación de Blazor.
- No debe mezclar lógica de negocio en la tarjeta.

### 5. PlaceholderDeImagen

Representa el fallback visual cuando no hay imagen disponible.

Atributos clave:
- `UsesImageUrl` (bool, derivado): verdadero cuando `ImageUrl` no es nulo.
- `PlaceholderIcon` (string, requerido): icono Lucide definido por la UI.
- `PlaceholderClass` (string, requerido): clase CSS centralizada en `wwwroot/app.css`.

Reglas:
- Debe respetar proporción visual del bloque de imagen.
- Debe usar `currentColor` para iconografía.

## Relaciones

- `SolicitudDeDetalleDePropiedad` obtiene `DetalleDePropiedadPublica`.
- `EstadoDePantallaDetalle` gobierna cómo se renderiza `DetalleDePropiedadPublica`.
- `NavegacionDetalleDesdeListado` usa el `PropertyId` del listado para abrir el detalle.
- `PlaceholderDeImagen` depende de `DetalleDePropiedadPublica.ImageUrl`.

## Notas de alcance

- No incluye edición, eliminación ni cambio de estado de propiedad.
- No incluye cambios en backend ni en OpenAPI.
- No altera la arquitectura de feature existente fuera de la navegación "Ver".