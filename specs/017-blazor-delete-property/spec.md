# Especificación de Funcionalidad: Eliminación de inmueble existente

**Rama de Funcionalidad**: `017-blazor-delete-property`

**Creado**: 2026-11-04

**Estado**: Implementada

**Entrada**: Descripción del usuario: "Como operador del catálogo Realtor, quiero eliminar una propiedad existente mediante su identificador, para retirar del sistema aquellos inmuebles que ya no deben publicarse ni estar disponibles para consulta. La solución deberá cubrir backend (endpoint público `DELETE /api/properties/{id}` siguiendo vertical slices) y frontend (acción de eliminación con confirmación explícita, invocación mediante el cliente Refit tipado, feedback de éxito o error y actualización de la vista)."

## Ciclo de estado del spec *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementación -> Implementada**
- Está prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Está prohibido cerrar en **Implementada** sin evidencia de validación en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- 2026-11-04 Borrador - Spec creado por flujo interactivo `/speckit-specify-prompt-interactive`
clarify y plan completados)
- 2026-11-04 En implementación - Inicio de `speckit.implement`
- 2026-11-04 Implementada - Cierre de `speckit.implement`: 32/32 tareas completadas, build sin errores, 46/46 pruebas superadas y evidencia U1–U8 registrada en `quickstart.md`

## Clarificaciones

### Sesión 2026-11-04

- Q: ¿La eliminación debe ser definitiva o recuperable? -> A: Eliminación definitiva (física). La propiedad deja de existir y no puede recuperarse.
- Q: ¿Qué ocurre con el recurso indicado por `imageUrl`? -> A: Solo se elimina el registro de la propiedad; el recurso de imagen no se modifica y su gestión queda fuera de alcance.
- Q: ¿Desde dónde está disponible la acción de eliminar? -> A: Desde el listado y desde la página de detalle. Al eliminar desde el detalle, se navega al listado mostrando el mensaje de éxito.
- Q: ¿Qué mecanismo de confirmación se usa? -> A: Diálogo modal reutilizable que muestra el título de la propiedad, con acciones "Eliminar" y "Cancelar", idéntico en listado y detalle.
- Q: ¿Cómo debe quedar el listado paginado tras eliminar? -> A: Se recarga la página actual desde el servidor; si queda vacía y no es la primera, se navega a la página anterior.
- Q: ¿Qué ocurre con el diálogo si la eliminación falla? -> A: El diálogo permanece abierto, muestra el error dentro y reactiva "Eliminar" (reintentar) y "Cancelar" (cerrar).
- Q: ¿Qué ocurre si al confirmar desde el detalle se recibe 404? -> A: Se cierra el diálogo, se navega al listado y se muestra el mensaje informativo "La propiedad ya no existe".
- Nota: RF-01 a RF-03

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Eliminar una propiedad mediante su identificador (Prioridad: P1)

Como operador del catálogo, quiero eliminar una propiedad por su identificador para retirarla definitivamente del catálogo.

**Por qué esta prioridad**: Es la capacidad base sobre la que se apoya toda la interfaz.

**Prueba independiente**: Se valida enviando solicitudes de eliminación con identificadores existentes, inexistentes e inválidos, y consultando después el listado y el detalle.

**Escenarios de aceptación**:

1. **Dado** que existe una propiedad con un identificador válido, **Cuando** se solicita su eliminación, **Entonces** el sistema responde `204 No Content` y la propiedad deja de estar disponible en el catálogo.
2. **Dado** que no existe una propiedad con el identificador solicitado, **Cuando** se intenta eliminarla, **Entonces** el sistema responde `404 Not Found` y no se modifica ninguna propiedad.
3. **Dado** que el identificador recibido tiene un formato inválido, **Cuando** se intenta eliminar, **Entonces** el sistema responde `400 Bad Request`.
4. **Dado** que el catálogo contiene varias propiedades, **Cuando** se elimina una de ellas, **Entonces** las demás permanecen disponibles y sin modificaciones.

---

### Historia de Usuario 2 - Eliminar desde el listado con confirmación (Prioridad: P1)

Como operador del catálogo, quiero eliminar una propiedad desde su tarjeta en el listado, confirmando explícitamente la acción, para evitar eliminaciones accidentales.

**Por qué esta prioridad**: Es el punto de trabajo principal del operador y el escenario exigido por la iniciativa.

**Prueba independiente**: Se valida desde el listado accionando Eliminar, cancelando y confirmando en el diálogo, y comprobando el estado de la vista.

**Escenarios de aceptación**:

1. **Dado** que el usuario está en el listado, **Cuando** selecciona Eliminar en una tarjeta, **Entonces** se muestra un diálogo modal con el título de la propiedad y las acciones "Eliminar" y "Cancelar".
2. **Dado** que se muestra el diálogo de confirmación, **Cuando** el usuario confirma, **Entonces** la propiedad se elimina, desaparece de la vista y se muestra un mensaje de éxito.
3. **Dado** que se muestra el diálogo de confirmación, **Cuando** el usuario cancela, **Entonces** no se envía ninguna solicitud de eliminación y la propiedad permanece disponible.

---

### Historia de Usuario 3 - Eliminar desde la página de detalle (Prioridad: P2)

Como operador del catálogo, quiero eliminar una propiedad mientras reviso su detalle para no tener que volver al listado a buscarla.

**Por qué esta prioridad**: Complementa el flujo principal desde un contexto de revisión completa.

**Prueba independiente**: Se valida desde el detalle confirmando la eliminación y comprobando la navegación al listado con mensaje de éxito.

**Escenarios de aceptación**:

1. **Dado** que el usuario está en el detalle de una propiedad, **Cuando** selecciona Eliminar y confirma en el diálogo, **Entonces** la propiedad se elimina, el usuario es dirigido al listado y se muestra un mensaje de éxito.
2. **Dado** que el usuario está en el detalle, **Cuando** cancela el diálogo, **Entonces** permanece en el detalle sin cambios.

---

### Historia de Usuario 4 - Manejo de errores y prevención de envíos duplicados (Prioridad: P2)

Como operador del catálogo, quiero recibir un mensaje comprensible cuando la eliminación falla y que la interfaz permanezca consistente.

**Por qué esta prioridad**: Garantiza estabilidad operativa ante fallos de red o del servidor.

**Prueba independiente**: Se valida forzando respuestas de error, pérdida de conexión y clics repetidos durante una eliminación en curso.

**Escenarios de aceptación**:

**Entonces** la propiedad permanece visible, el diálogo permanece abierto mostrando un mensaje indicando que la operación no pudo completarse, y las acciones "Eliminar" (reintentar) y "Cancelar" quedan habilitadas.
2. **Dado** que existe una eliminación en curso, **Cuando** el usuario intenta accionar nuevamente Eliminar o confirmar, **Entonces** el sistema no envía solicitudes adicionales.

---

### Casos límite

la segunda operación responde `404 Not Found`; la interfaz cierra el diálogo y muestra el mensaje informativo "La propiedad ya no existe". Desde el listado, se recarga la página actual (RF-14); desde el detalle, se navega al listado.
- Ante dos eliminaciones concurrentes sobre el mismo identificador, exactamente una responde `204 No Content` y la otra `404 Not Found`, sin afectar otras propiedades.
- Una propiedad con valor en `imageUrl` se elimina igual que cualquier otra; el recurso de imagen no se modifica.
- Si se recibe una respuesta de error o se pierde la conexión, la propiedad permanece visible y el diálogo permanece abierto mostrando un mensaje de error comprensible, permitiendo reintentar o cancelar. La excepción es `404 Not Found`, que se trata como propiedad ya eliminada.
- Al eliminar la única propiedad de la última página (distinta de la primera), el listado muestra la página anterior en lugar de una página vacía.
- Mientras una eliminación está en curso,
- Al consultar el detalle de una propiedad eliminada, el sistema responde `404 Not Found` y la interfaz muestra que la propiedad no existe.

## Requisitos *(obligatorio)*

### Requisitos Funcionales

- **RF-01**: El sistema DEBE permitir eliminar una propiedad existente mediante su identificador, respondiendo `204 No Content`.
- **RF-02**: Si no existe una propiedad con el identificador indicado, el sistema DEBE responder `404 Not Found`.
- **RF-03**: Una solicitud que responda `404 Not Found` NO DEBE producir cambios ni efectos secundarios.
- **RF-04**: Si el identificador no tiene un formato válido, el sistema DEBE responder `400 Bad Request`.
- **RF-05**: Después de una eliminación exitosa, la propiedad NO DEBE aparecer en consultas posteriores, incluidos el listado y el detalle por identificador.
- **RF-06**: La eliminación de una propiedad NO DEBE modificar ni eliminar otras propiedades del catálogo.
- **RF-07**: La operación de eliminación DEBE ser pública y no requerir autenticación, coherente con los demás endpoints.
- **RF-08**: Antes de ejecutar la eliminación, el frontend DEBE solicitar confirmación explícita mediante un diálogo modal que muestre el título de la propiedad.
- **RF-09**: Tras una eliminación exitosa, el frontend DEBE retirar la propiedad de la vista actual, impedir la interacción con ella y mostrar un mensaje de confirmación. Si la operación falla, DEBE mostrar un mensaje de error comprensible y mantener un estado consistente.
- **RF-10**: La acción de eliminar DEBE estar disponible en cada tarjeta del listado y en la página de detalle.
- **RF-11**: Al eliminar desde el detalle, el frontend DEBE dirigir al usuario al listado y mostrar el mensaje de éxito.
- **RF-12**: El frontend DEBE invocar la operación mediante el cliente tipado de comunicación con el backend existente.
- **RF-13**: El frontend DEBE impedir el envío de múltiples solicitudes de eliminación mientras una esté en curso.
si la página resultante queda vacía y no es la primera, DEBE mostrar la página anterior.
reactivar las acciones "Eliminar" y "Cancelar".
- **RF-16**: Si la eliminación responde `404 Not Found`, el frontend DEBE cerrar el diálogo y mostrar el mensaje informativo "La propiedad ya no existe"; desde el detalle DEBE navegar al listado y desde el listado DEBE recargar la página actual.

### Reglas de negocio

- **RN-01**: La eliminación es definitiva (física); no existe recuperación de propiedades eliminadas.
- **RN-02**: La eliminación afecta solo al registro de la propiedad; el recurso referenciado por `imageUrl` no se modifica.
- **RN-03**: Cancelar el diálogo de confirmación no produce ninguna solicitud ni cambio.

### Entidades Clave

- **Propiedad**: Inmueble del catálogo identificado por un UUID, cuya eliminación es definitiva.
- **Diálogo de confirmación**: Superficie reutilizable que identifica la propiedad a eliminar y ofrece confirmar o cancelar.
- **Resultado de eliminación**: Éxito con retiro de la vista y mensaje, o error manejado con mensaje y estado consistente.

## Fuera de alcance

- Recuperación de propiedades eliminadas.
- Historial o auditoría de eliminaciones.
- Eliminación o gestión del recurso de imagen asociado.
- Eliminación masiva de múltiples propiedades.
- Autenticación o autorización.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: En el 100% de los casos validados, eliminar una propiedad existente responde `204` y la propiedad no aparece en el listado ni en el detalle.
- **SC-002**: En el 100% de los casos validados, identificadores inexistentes responden `404` e identificadores inválidos responden `400`, sin cambios en el catálogo.
- **SC-003**: En el 100% de los casos validados, el número de propiedades restantes tras una eliminación es exactamente el anterior menos uno y sus datos no cambian.
- **SC-004**: En el 100% de los casos de cancelación, no se envía ninguna solicitud de eliminación.
- **SC-005**: En el 100% de los casos de error simulados, la propiedad permanece visible y se muestra un mensaje de error comprensible.
- **SC-006**: En el 100% de los casos validados, clics repetidos durante una eliminación en curso producen una única solicitud.

## Suposiciones

- Existen el listado de propiedades y la página de detalle implementados en specs anteriores.
- El frontend dispone de un mecanismo para mostrar mensajes de éxito y error, reutilizable en esta iniciativa.
- El identificador de propiedad es un UUID, coherente con los endpoints existentes.
