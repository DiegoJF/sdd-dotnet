# Modelo Conceptual: Listado de propiedades en la página principal

## Propósito

Definir los conceptos que gobiernan la página principal de listado paginado de propiedades en `RealtorWeb`, sin introducir implementación de negocio, persistencia ni contratos backend nuevos.

## Entidades conceptuales

### 1. ElementoDePropiedadPublica

Representa una propiedad visible en la grilla del frontend.

Atributos clave:
- `Title` (string, requerido): título visible de la propiedad.
- `ImageUrl` (string, opcional): imagen destacada pública de la propiedad.
- `Price` (decimal, requerido): precio mostrado al usuario.
- `Status` (string, requerido): estado visible del inventario.
- `Address` (string, requerido): dirección resumida o completa.
- `BedroomCount` (int, requerido): cantidad de dormitorios.
- `BathroomCount` (int, requerido): cantidad de baños.
- `AreaSquareMeters` (decimal, requerido): superficie en metros cuadrados.

Reglas:
- Debe renderizarse como una tarjeta compacta.
- No debe depender de estilos inline.
- No debe mostrar campos no descritos por el contrato del listado.

### 2. RespuestaPaginadaDePropiedades

Representa el resultado que el frontend recibe desde el backend para una página concreta.

Atributos clave:
- `Items` (colección de `ElementoDePropiedadPublica`, requerida): propiedades visibles.
- `Page` (int, requerido): página actual.
- `PageSize` (int, requerido): tamaño de página usado en la consulta.
- `TotalItems` (int, requerido): total de elementos disponibles.
- `TotalPages` (int, requerido): total de páginas calculado por el backend.
- `HasNext` (bool, requerido): indica si existe siguiente página.
- `HasPrevious` (bool, requerido): indica si existe página anterior.

Reglas:
- Debe consumirse por un cliente Refit tipado.
- Debe permitir construir controles de navegación coherentes.

### 3. EstadoDePaginacion

Representa el estado funcional de la navegación entre páginas.

Atributos clave:
- `CurrentPage` (int, requerido): página visible en pantalla.
- `TotalPages` (int, requerido): límite superior de navegación.
- `TotalItems` (int, requerido): total de resultados disponibles.
- `CanGoNext` (bool, requerido): derivado de `HasNext`.
- `CanGoPrevious` (bool, requerido): derivado de `HasPrevious`.

Reglas:
- Los controles de anterior y siguiente deben reflejar este estado.
- No debe permitir acciones imposibles.

### 4. EstadoDePantallaListado

Representa el momento visual del listado.

Valores conceptuales:
- `loading`
- `empty`
- `error`
- `success`

Atributos clave:
- `StateType` (string, requerido): tipo de estado.
- `Message` (string, opcional): mensaje visible para la persona usuaria.
- `ActionLabel` (string, opcional): texto de acción contextual si aplica.
- `HasContent` (bool, requerido): indica si se muestran tarjetas de propiedades.

Reglas:
- Debe usar patrones visuales reutilizables de `wwwroot/app.css`.
- Debe evitar mostrar datos viejos como si fueran vigentes cuando ocurre un error.

### 5. SolicitudDeListadoDePropiedades

Representa la consulta que dispara la página al backend.

Atributos clave:
- `Page` (int, requerido): página solicitada.
- `PageSize` (int, requerido): tamaño de página solicitado.
- `CancellationToken` (token, requerido): cancelación de la operación en curso.

Reglas:
- Debe pasar por el cliente `IPropertiesApi` registrado por DI.
- Debe cancelarse y reemplazarse cuando el usuario cambie de página.

### 6. VistaPrincipalDePropiedades

Representa la composición de la página enroutable `/`.

Atributos clave:
- `Route` (string, requerido): `/`.
- `FeatureFolder` (string, requerido): `Features/Properties/List`.
- `PageFolder` (string, requerido): `Components/Pages`.
- `DefaultPageSize` (int, requerido): `9`.
- `LayoutMode` (string, requerido): `desktop-first`.

Reglas:
- Debe permanecer enrutable desde `Components/Pages`.
- Debe delegar la lógica de presentación a una feature por carpeta.

## Relaciones

- `VistaPrincipalDePropiedades` usa `SolicitudDeListadoDePropiedades` para obtener datos.
- `SolicitudDeListadoDePropiedades` produce `RespuestaPaginadaDePropiedades`.
- `RespuestaPaginadaDePropiedades` contiene múltiples `ElementoDePropiedadPublica`.
- `EstadoDePaginacion` se deriva de `RespuestaPaginadaDePropiedades`.
- `EstadoDePantallaListado` gobierna la representación visual de `VistaPrincipalDePropiedades`.

## Notas de alcance

- No define entidades de dominio persistente.
- No define filtros, búsqueda ni ordenamiento complejo.
- No introduce endpoints backend nuevos.
