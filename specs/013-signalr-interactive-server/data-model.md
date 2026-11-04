# Modelo conceptual: Interactividad de servidor en páginas host

## Resumen

Esta iniciativa no introduce entidades de dominio nuevas. El modelo se centra en superficies de UI, estados de interactividad y transiciones de navegación que dependen de eventos `@onclick`.

## Entidades conceptuales

## 1) PaginaHost

- Descripción: Página enroutable de Blazor que aloja componentes visuales.
- Atributos:
  - `nombre` (string): Identificador lógico de la página.
  - `ruta` (string): Ruta pública (`/`, `/properties/{id}`, `/Error`, `/not-found`).
  - `requiereInteractividad` (bool): Indica si necesita circuito SignalR para manejar eventos.
  - `renderModeEsperado` (enum): `InteractiveServer` o `StaticSSR`.
- Reglas:
  - Si `requiereInteractividad = true`, `renderModeEsperado` debe ser `InteractiveServer`.
  - Si `requiereInteractividad = false`, `renderModeEsperado` debe ser `StaticSSR`.

## 2) AccionInteractiva

- Descripción: Acción de UI disparada por botones existentes en componentes.
- Atributos:
  - `nombre` (string): `Anterior`, `Siguiente`, `VolverAlListado`, `Reintentar`.
  - `paginaHost` (string): Página donde vive la acción.
  - `estadoHabilitacion` (enum): `Enabled` o `Disabled` (solo aplica a paginación).
  - `resultadoEsperado` (string): Cambio de estado/navegación sin recarga completa.
- Reglas:
  - `Anterior` y `Siguiente` respetan `CanGoPrevious` y `CanGoNext`.
  - `VolverAlListado` siempre navega a `/` desde estados cargado, no encontrado y error.
  - `Reintentar` relanza carga de datos en estados de error.

## 3) EstadoVista

- Descripción: Estado de visualización del componente alojado en la página host.
- Atributos:
  - `tipo` (enum): `Loading`, `Success`, `NotFound`, `Error`.
  - `permiteReintento` (bool): verdadero cuando `tipo = Error`.
  - `permiteVolver` (bool): verdadero en `Success`, `NotFound`, `Error` para detalle.
- Reglas:
  - La transición a `Loading` puede originarse por carga inicial o reintento.
  - En detalle, `NotFound` y `Error` mantienen la acción de retorno al listado.

## Relaciones

- `PaginaHost` 1..N `AccionInteractiva`.
- `PaginaHost` 1..N `EstadoVista`.
- `AccionInteractiva` puede producir transición entre `EstadoVista`.

## Mapeo a superficies reales

- `Home.razor`:
  - `requiereInteractividad = true`
  - Acciones: `Anterior`, `Siguiente`, `Reintentar`
- `PropertyDetailPage.razor`:
  - `requiereInteractividad = true`
  - Acciones: `VolverAlListado`, `Reintentar`
- `Error.razor`:
  - `requiereInteractividad = false`
- `NotFound.razor`:
  - `requiereInteractividad = false`
