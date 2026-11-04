# Especificación de Funcionalidad: Listado Paginado de Propiedades

**Rama de la funcionalidad**: `[004-properties-list-pagination]`

**Creado**: 2026-07-09

**Estado**: Implementada

**Entrada**: Descripción del usuario: "Crear la especificación 004-properties-list-pagination. Definir e implementar un endpoint que devuelva la lista paginada de propiedades con imagen pública absoluta por cada elemento."

## Ciclo de estado de la especificación *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementación -> Implementada**
- Está prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Está prohibido cerrar en **Implementada** sin evidencia de validación en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- [2026-07-09] Borrador - Spec creado por `/speckit.specify`
- [2026-07-09T21:11:59.8996469-04:00] Aprobada -> En implementación - Motivo: inicio formal de la implementación tras validar prerequisitos y permitir la transición.
- [2026-07-09T21:18:45.1600761-04:00] En implementación -> Implementada - Motivo: tareas completas en `tasks.md` y evidencia de validación presente en `quickstart.md`.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Ver el listado inicial de propiedades (Prioridad: P1)

Como persona que consulta propiedades, quiero ver un listado inicial paginado y ordenado de propiedades para explorar opciones sin tener que ajustar filtros manualmente desde el inicio.

**Por qué esta prioridad**: El listado inicial es el punto de entrada principal del consumo de la API y debe funcionar de forma consistente antes que cualquier navegación avanzada.

**Prueba independiente**: Se puede probar realizando una consulta sin parámetros y verificando que la respuesta devuelve la primera página con el tamaño por defecto y el orden esperado.

**Escenarios de aceptación**:

1. **Dado** que existen propiedades publicadas, **Cuando** consulto el listado sin parámetros, **Entonces** la respuesta devuelve la página 1 con hasta 6 resultados ordenados por título en orden ascendente.
2. **Dado** que existen propiedades publicadas, **Cuando** consulto el listado sin parámetros, **Entonces** cada elemento incluye identificador, título, descripción, dirección, precio, estado, número de dormitorios, número de baños, metros cuadrados y URL de imagen.

---

### Historia de Usuario 2 - Navegar entre páginas de resultados (Prioridad: P2)

Como persona que revisa inventario, quiero solicitar páginas específicas de resultados para seguir explorando propiedades sin perder consistencia entre consultas.

**Por qué esta prioridad**: La paginación permite recorrer catálogos más grandes sin sobrecargar la respuesta ni obligar a cargar todo el inventario de una vez.

**Prueba independiente**: Se puede probar solicitando una página concreta con un tamaño definido y verificando que la respuesta corresponde a ese tramo del inventario y mantiene metadatos coherentes.

**Escenarios de aceptación**:

1. **Dado** que existen suficientes propiedades, **Cuando** consulto la página 2 con tamaño 6, **Entonces** la respuesta devuelve los elementos correctos de esa página y metadatos coherentes con el total disponible.
2. **Dado** que solicito una página sin resultados, **Cuando** la página está fuera del rango disponible, **Entonces** la respuesta devuelve una lista vacía y metadatos consistentes con la consulta realizada.

---

### Historia de Usuario 3 - Consumir imágenes públicas y validar parámetros (Prioridad: P3)

Como persona que integra la API en un cliente web, quiero recibir URLs públicas absolutas de las imágenes y mensajes de validación claros para poder renderizar el listado y corregir entradas inválidas con rapidez.

**Por qué esta prioridad**: Las imágenes públicas son necesarias para una experiencia visual completa y la validación evita consultas incorrectas o ambiguas.

**Prueba independiente**: Se puede probar consultando el listado y comprobando que la URL de imagen usa el esquema y host de la petición, y también enviando parámetros inválidos para comprobar el error de validación.

**Escenarios de aceptación**:

1. **Dado** que una propiedad tiene imagen asociada, **Cuando** consulto el listado, **Entonces** la URL de imagen devuelta es absoluta, pública y no expone rutas físicas internas.
2. **Dado** que envío un valor inválido para `page` o `pageSize`, **Cuando** consulto el listado, **Entonces** la API responde con estado 400 y un detalle de validación comprensible.

### Casos Límite

- ¿Qué ocurre cuando no se envían parámetros de paginación? La API usa los valores por defecto definidos para la primera consulta.
- ¿Qué ocurre cuando la consulta no tiene elementos disponibles? La API devuelve una lista vacía con metadatos consistentes.
- ¿Qué ocurre cuando la página solicitada supera el total de páginas disponibles? La API devuelve una respuesta vacía sin romper el contrato de paginación.
- ¿Qué ocurre si el host de la solicitud cambia entre entornos? La URL de imagen se construye con el esquema y host de la petición actual.

## Requisitos *(obligatorio)*

### Requisitos Funcionales

- **FR-001**: El sistema DEBE exponer el listado paginado de propiedades mediante una consulta pública en la ruta `/api/properties`.
- **FR-002**: El sistema DEBE aceptar los parámetros de paginación `page` y `pageSize` como entrada de la consulta.
- **FR-003**: Cuando no se reciban parámetros de paginación, el sistema DEBE usar `page = 1` y `pageSize = 6`.
- **FR-004**: El sistema DEBE ordenar los resultados por `title` en orden ascendente antes de aplicar la paginación.
- **FR-005**: El sistema DEBE devolver por cada propiedad los campos `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
- **FR-006**: El sistema DEBE incluir metadatos de paginación con `page`, `pageSize`, `totalItems`, `totalPages`, `hasNext` y `hasPrevious`.
- **FR-007**: El sistema DEBE construir `imageUrl` como una URL absoluta y pública basada en el esquema y host de la solicitud actual, siguiendo el patrón `http(s)://{host}/assets/properties/{fileName}`.
- **FR-008**: El sistema NO DEBE devolver rutas físicas internas ni URLs relativas en la respuesta final.
- **FR-009**: El sistema DEBE validar que `page` y `pageSize` sean valores positivos y devolver un error de validación con estado 400 si la consulta es inválida.
- **FR-010**: El sistema DEBE mantener la operación cancelable para detener el trabajo en curso cuando la petición ya no sea necesaria.
- **FR-011**: El sistema DEBE construir la respuesta con un contrato público explícito, sin exponer detalles internos del modelo persistente.
- **FR-012**: Cuando una consulta no encuentre elementos para la página solicitada, el sistema DEBE devolver una colección vacía con metadatos de paginación consistentes.

### Entidades Clave *(incluir si la funcionalidad involucra datos)*

- **Propiedad**: Registro del inventario inmobiliario que se presenta al consumidor de la API. Incluye datos descriptivos, estado, atributos físicos y referencia de imagen.
- **Elemento de listado de propiedades**: Vista pública de una propiedad dentro de la respuesta paginada. Contiene solo los campos necesarios para consumo externo.
- **Metadatos de paginación**: Información auxiliar que permite conocer el tamaño de la página, el total de elementos y si existen páginas anteriores o siguientes.

## Criterios de Éxito *(obligatorio)*

### Resultados Medibles

- **SC-001**: En el 100% de las consultas sin parámetros, la API devuelve la primera página con hasta 6 resultados y el orden por título ascendente.
- **SC-002**: En consultas con página y tamaño válidos, el 100% de las respuestas muestran metadatos de paginación coherentes con el total disponible.
- **SC-003**: En el 100% de las consultas con parámetros inválidos, la API responde con error de validación y estado 400.
- **SC-004**: En el 100% de las propiedades devueltas, la URL de imagen es pública, absoluta y utilizable directamente por un cliente externo.
- **SC-005**: En el 100% de las respuestas válidas, no aparecen rutas físicas internas ni referencias relativas en el campo de imagen.

## Suposiciones

- Se asume que las propiedades existentes ya cuentan con un nombre de archivo de imagen apto para publicación externa.
- Se asume que la API dispone del esquema y host de la solicitud actual para componer URLs públicas absolutas.
- Se asume que el alcance de esta iniciativa se limita a lectura y paginación del inventario de propiedades.
- Se asume que no se requiere un límite máximo adicional de `pageSize` más allá de su validación como valor positivo.
