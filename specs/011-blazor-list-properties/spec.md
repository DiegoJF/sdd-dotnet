# Especificación de la Feature: Listado de propiedades en la página principal

**Rama de feature**: `[011-blazor-list-properties]`

**Creado**: 2026-07-22

**Estado**: Implementada

**Entrada**: Descripción del usuario: "Quiero crear una nueva spec llamada 011-blazor-list-properties para convertir la página principal en un listado paginado de propiedades, usando el contrato de `/api/properties` y respetando las reglas de frontend del proyecto."

## Ciclo de estado del spec *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementación -> Implementada**
- Está prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Está prohibido cerrar en **Implementada** sin evidencia de validación en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- 2026-07-22 Borrador - Spec creado por `/speckit.specify`
- 2026-07-22 Aprobada -> En implementación - Transición automática de inicio de implementación
- 2026-07-22 En implementación -> Implementada - Cierre automático validado con `tasks.md` sin pendientes y evidencia de validación en `quickstart.md`

## Clarificaciones

### Sesión 2026-07-22

- Q: ¿Qué datos debe mostrar cada tarjeta de propiedad en la grilla? → A: Título, imagen, precio, estado, dirección, dormitorios, baños y metros cuadrados.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Ver el listado inicial de propiedades (Prioridad: P1)

Como usuario del sitio, quiero abrir la página principal y ver propiedades paginadas para explorar el catálogo desde la primera impresión sin tener que navegar a otra sección.

**Por qué esta prioridad**: La página principal es el punto de entrada del producto; si el listado inicial falla, la experiencia principal de exploración queda inutilizada.

**Prueba independiente**: Se valida abriendo la ruta `/` y comprobando que la vista solicita la primera página de propiedades, muestra tarjetas con datos y presenta el estado adecuado según la respuesta del backend.

**Escenarios de aceptación**:

1. **Dado** que abro la ruta `/`, **Cuando** la página termina de cargar, **Entonces** solicita propiedades con `Page=1` y `PageSize=9` y muestra el contenido recibido o el estado vacío correspondiente.
2. **Dado** que el backend devuelve propiedades, **Cuando** se renderiza la primera página, **Entonces** cada tarjeta muestra los datos públicos disponibles de la propiedad y la información de paginación visible coincide con la respuesta recibida.
3. **Dado** que la interfaz se visualiza en escritorio, tableta o móvil, **Cuando** cambia el ancho disponible, **Entonces** la grilla muestra 3, 2 y 1 columnas respectivamente.

---

### Historia de Usuario 2 - Navegar entre páginas de resultados (Prioridad: P2)

Como usuario del catálogo, quiero moverme entre páginas con anterior y siguiente para revisar más propiedades sin perder el contexto del listado.

**Por qué esta prioridad**: La navegación por páginas es el mecanismo principal para recorrer un inventario mayor al espacio visible de la pantalla.

**Prueba independiente**: Se valida cargando una lista con más de una página y comprobando que los controles de paginación cambian los resultados y respetan el estado habilitado o deshabilitado.

**Escenarios de aceptación**:

1. **Dado** que existen varias páginas de resultados, **Cuando** presiono "siguiente", **Entonces** la vista avanza a la página siguiente, actualiza las tarjetas visibles y refleja el nuevo estado de paginación.
2. **Dado** que estoy en la primera página, **Cuando** se muestran los controles de paginación, **Entonces** el control "anterior" permanece deshabilitado.
3. **Dado** que estoy en la última página, **Cuando** se muestran los controles de paginación, **Entonces** el control "siguiente" permanece deshabilitado.

---

### Historia de Usuario 3 - Entender estados vacíos y de error (Prioridad: P3)

Como usuario, quiero distinguir claramente cuándo no hay resultados y cuándo ocurre un error para no confundir una ausencia real de datos con un fallo de consulta.

**Por qué esta prioridad**: Los estados de carga, vacío y error son necesarios para una experiencia confiable y evitan interpretaciones erróneas de la información mostrada.

**Prueba independiente**: Se valida simulando una respuesta vacía, una respuesta con error y una consulta en curso, comprobando que cada caso muestra un estado distinto y comprensible.

**Escenarios de aceptación**:

1. **Dado** que el backend devuelve una colección vacía, **Cuando** la vista termina de cargar, **Entonces** se muestra un estado vacío claro y no una grilla sin contexto.
2. **Dado** que la consulta falla por un error del backend o por un problema de red, **Cuando** la vista no puede recuperar datos, **Entonces** se muestra un estado de error claro y la información anterior no se presenta como vigente.
3. **Dado** que la consulta sigue en curso, **Cuando** la página aún está obteniendo datos, **Entonces** se muestra un estado de carga sin controles que sugieran que el resultado ya está disponible.

---

### Casos Límite

- Si la página solicitada ya no existe porque el total cambió, la vista debe responder con un estado coherente y controles de navegación consistentes.
- Si el backend devuelve `ValidationProblemDetails`, la vista debe tratarlo como error de consulta y no como estado vacío.
- Si el usuario cambia de página mientras una respuesta anterior sigue en tránsito, la última interacción debe prevalecer en la interfaz.
- Si una página contiene más tarjetas de las esperadas para el espacio visible, la composición debe mantener legibilidad y separación visual.

## Fuera de alcance

- CRUD completo de propiedades.
- Búsqueda avanzada, filtros avanzados o ordenamiento complejo.
- Cambios en el backend, la persistencia o el dominio.
- Cualquier funcionalidad no trazada en `tasks.md` de esta spec.

## Requisitos *(obligatorio)*

### Requisitos Funcionales

- **FR-001**: El sistema DEBE convertir la ruta `/` en la página principal de listado paginado de propiedades.
- **FR-002**: La página principal DEBE solicitar los parámetros de consulta `Page` y `PageSize` en cada carga de datos.
- **FR-003**: En la carga inicial, la página principal DEBE usar `Page=1` y `PageSize=9`.
- **FR-004**: La página principal DEBE renderizar cada propiedad recibida como una tarjeta con título, imagen, precio, estado, dirección, dormitorios, baños y metros cuadrados.
- **FR-005**: La vista DEBE mostrar metadatos de paginación visibles para la persona usuaria: página actual, total de páginas y total de elementos.
- **FR-006**: La vista DEBE incluir controles de paginación "anterior" y "siguiente".
- **FR-007**: Los controles de paginación DEBEN deshabilitarse cuando `HasPrevious` o `HasNext` indiquen que no es posible avanzar o retroceder.
- **FR-008**: La vista DEBE presentar estados distintos y comprensibles para carga, vacío, error y datos disponibles.
- **FR-009**: La grilla de propiedades DEBE mostrarse en 3 columnas en escritorio, 2 columnas en tableta y 1 columna en móvil.
- **FR-010**: El frontend DEBE consumir el backend mediante un cliente tipado registrado por DI y alineado con Refit.
- **FR-011**: Los componentes Razor DEBEN evitar el uso directo de `HttpClient` y DEBEN evitar `RestService.For` fuera del registro de DI.
- **FR-012**: La funcionalidad DEBE mantenerse organizada en `Components/Pages` para la ruta principal y en `Features/Properties/List` para los elementos de la experiencia de listado.
- **FR-013**: La interfaz DEBE respetar `wwwroot/app.css` como fuente visual canónica y NO DEBE introducir estilos inline ni colores hardcodeados.
- **FR-014**: Si la interfaz requiere íconos, DEBE usar Lucide Icons y NO DEBE introducir frameworks CSS externos.
- **FR-015**: La vista DEBE reaccionar de forma clara cuando el backend responda con errores de validación o cuando falle la red.

### Entidades Clave *(incluir si la funcionalidad involucra datos)*

- **Elemento de propiedad pública**: Representación visible de una propiedad en la grilla, con la información de catálogo que el contrato del listado expone al frontend.
- **Respuesta paginada de propiedades**: Conjunto de elementos junto con los metadatos necesarios para navegar entre páginas y entender el alcance del resultado.
- **Estado de paginación**: Información que indica la página actual, el total de páginas, el total de elementos y si existen páginas anteriores o siguientes.
- **Estado de pantalla**: Representación visual del momento de la carga, el vacío, el error o los datos disponibles.

## Criterios de Éxito *(obligatorio)*

### Resultados Medibles

- **SC-001**: En el 100% de las cargas iniciales validadas, la página principal muestra el listado de propiedades junto con sus metadatos de paginación.
- **SC-002**: En el 100% de los cambios de página validados, la vista actualiza los resultados visibles y refleja correctamente el nuevo estado de navegación.
- **SC-003**: En el 100% de los escenarios vacíos validados, la interfaz muestra un estado vacío explícito y comprensible.
- **SC-004**: En el 100% de los escenarios de error validados, la interfaz muestra un estado de error claro y no presenta datos antiguos como si fueran actuales.
- **SC-005**: En las validaciones de escritorio, tableta y móvil, la grilla conserva legibilidad y muestra 3, 2 y 1 columnas respectivamente.
- **SC-006**: En el 100% de los recorridos de usuario validados, los controles de paginación no sugieren acciones imposibles cuando no existen páginas anterior o siguiente.

## Suposiciones

- La experiencia inicial utiliza un tamaño de página fijo de 9 elementos para mantener la composición visual alineada con la grilla de propiedades.
- El contrato descrito en `ListPropertiesSlice.cs` y en `v1.json` es la fuente de verdad del backend para esta pantalla.
- La página principal se limita a lectura y navegación del catálogo, sin editar ni crear propiedades.
- No se incluyen búsqueda avanzada, filtros avanzados ni ordenamientos complejos en esta iniciativa.
