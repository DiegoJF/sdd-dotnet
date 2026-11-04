# Modelo Conceptual: Foundation Blazor Frontend

## Propósito

Definir las entidades conceptuales de la fundación frontend para `RealtorWeb`, sin introducir lógica de negocio de propiedades ni contratos funcionales CRUD.

## Entidades conceptuales

### 1. SistemaVisualBase

Representa el contrato visual único de la aplicación en `wwwroot/app.css`.

Campos/atributos clave:
- `TokenGroup` (string, requerido): categoría de tokens (`color`, `space`, `radius`, `shadow`, `layout`).
- `TokenName` (string, requerido): nombre del token CSS (ej. `--color-primary`).
- `TokenValue` (string, requerido): valor concreto del token.
- `Scope` (string, requerido): alcance (`:root`, `component`, `utility`).

Reglas:
- Debe existir en `wwwroot/app.css`.
- No se permiten valores hardcodeados en `.razor` o `.cs` para estilos visuales base.
- Cualquier clase de layout/base debe consumir tokens del sistema.

### 2. LayoutAplicacionBase

Representa la estructura principal desktop first de la app.

Campos/atributos clave:
- `ShellClass` (string, requerido): `.app-shell`.
- `SidebarClass` (string, requerido): `.app-sidebar`.
- `MainClass` (string, requerido): `.app-main`.
- `ContentClass` (string, requerido): `.app-content`.
- `ResponsiveMode` (string, requerido): `desktop`, `tablet`, `mobile`.

Reglas:
- En desktop debe existir sidebar izquierda estable + contenido principal amplio.
- En tablet debe existir modo compacto utilizable.
- En móvil debe existir navegación colapsada/drawer o equivalente funcional.

### 3. NavegacionSidebar

Representa la navegación vertical base de la app.

Campos/atributos clave:
- `Brand` (string, requerido): marca/título del shell.
- `NavItemKey` (string, requerido): identificador del item.
- `Route` (string, requerido): ruta objetivo.
- `State` (string, requerido): `default`, `hover`, `active`.
- `IconRef` (string, opcional): ícono Lucide asociado.

Reglas:
- Debe soportar estados visuales `hover` y `active`.
- Debe mantener legibilidad en desktop/tablet/móvil.

### 4. SistemaIconografiaBase

Representa la convención de uso de íconos con Lucide.

Campos/atributos clave:
- `Provider` (string, requerido): `Lucide.Blazor`.
- `IconName` (string, requerido): nombre del ícono Lucide.
- `SizeClass` (string, requerido): `.icon-sm`, `.icon-md`, `.icon-lg`.
- `ColorMode` (string, requerido): `currentColor`.

Reglas:
- Se prohíbe mezclar Bootstrap Icons u otras librerías como fuente principal.
- El tamaño y color deben gestionarse por clases CSS, no por estilos inline.

### 5. EstadoPantallaBase

Representa un patrón reutilizable de estado de UI.

Campos/atributos clave:
- `StateType` (string, requerido): `loading`, `empty`, `error`, `success`.
- `ContainerClass` (string, requerido): clase principal del estado.
- `Title` (string, opcional): texto de encabezado de estado.
- `Description` (string, opcional): mensaje de soporte.
- `ActionSlot` (bool, requerido): indica si soporta acción contextual.

Reglas:
- Debe renderizarse sin depender de estilos de terceros.
- Debe ser reusable en futuras pantallas sin duplicar CSS.

### 6. BaseComunicacionFrontendBackend

Representa el andamiaje de clientes tipados para integración futura.

Campos/atributos clave:
- `ClientName` (string, requerido): nombre del cliente Refit por módulo.
- `InterfaceType` (string, requerido): interfaz `I...Api`.
- `BaseAddress` (string, requerido): URL base de `RealtorApi`.
- `RegistrationMode` (string, requerido): `IHttpClientFactory + AddRefitClient`.

Reglas:
- No se permite `HttpClient` directo en componentes Razor.
- No se permite `RestService.For<T>()` fuera del registro DI.
- En esta spec no se definen endpoints de negocio; solo registro/andamiaje.

## Relaciones

- `LayoutAplicacionBase` usa `SistemaVisualBase` para tokens y clases.
- `NavegacionSidebar` se renderiza dentro de `LayoutAplicacionBase`.
- `SistemaIconografiaBase` se usa en `NavegacionSidebar` y componentes de acción base.
- `EstadoPantallaBase` depende de `SistemaVisualBase` para presentación homogénea.
- `BaseComunicacionFrontendBackend` habilita consumo futuro desde servicios frontend, no desde componentes UI.

## Notas de alcance

- No define entidades de dominio inmobiliario.
- No define operaciones CRUD ni contratos de negocio de propiedades.
- No requiere cambios backend en esta iniciativa.
