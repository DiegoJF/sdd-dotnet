# Especificación de Funcionalidad: Edición de inmueble existente en frontend Blazor

**Rama de Funcionalidad**: `016-blazor-update-property`

**Creado**: 2026-07-25

**Estado**: Implementada

**Entrada**: Descripción del usuario: "Quiero crear una funcionalidad que permita actualizar la información de un inmueble existente. El nombre de la spec sera 016-blazor-update-property. Ya existe un endpoint para realizar esta operación. Su definición se encuentra en app/backend/src/RealtorApi/wwwroot/openapi/v1.json y su operationId es UpdateProperty. Utiliza este contrato como fuente de verdad para identificar la ruta, los parámetros, el cuerpo de la solicitud, las validaciones y la respuesta esperada. Dentro del componente app/frontend/src/RealtorWeb/Features/Properties/List/PropertyCard.razor, se debe agregar un botón o acción para editar el inmueble representado por la tarjeta. Al seleccionar esta acción, el usuario debe ser dirigido a una página de edición dentro del proyecto app/frontend/src/RealtorWeb/RealtorWeb.csproj. La página de edición debe obtener la información actual, mostrar formulario editable, aplicar validaciones, consumir UpdateProperty, manejar estados visuales, prevenir envíos duplicados, conservar datos ante error, confirmar éxito y navegar al detalle tras actualizar." 

## Ciclo de estado del spec *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementacion -> Implementada**
- Esta prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Esta prohibido cerrar en **Implementada** sin evidencia de validacion en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- 2026-07-25 Borrador - Spec creado por flujo interactivo `/speckit-specify-prompt-interactive`
- 2026-07-25 Implementada - Cierre automático aplicado tras validar `tasks.md` sin pendientes y `quickstart.md` con evidencia de validación no vacía

## Clarificaciones

### Sesion 2026-07-25

- Q: Despues de actualizar correctamente un inmueble, a que pantalla debe volver el usuario como comportamiento principal? -> A: Ir siempre al detalle del inmueble recien actualizado.
- Q: Si no se puede cargar la informacion inicial del inmueble para editar, que comportamiento debe tener la interfaz? -> A: Mostrar mensaje de error en la misma pantalla, con accion Reintentar y accion Volver al listado.
- Q: Cual es el criterio principal de exito funcional de la iniciativa? -> A: El usuario puede actualizar un inmueble cumpliendo validaciones del contrato, ver confirmacion y navegar al detalle actualizado sin perder datos ingresados ante error del servidor.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Iniciar edicion desde el listado (Prioridad: P1)

Como persona operadora del catalogo, quiero iniciar la edicion de un inmueble desde su tarjeta en el listado para modificar su informacion sin buscar rutas manualmente.

**Por que esta prioridad**: Es la puerta de entrada al flujo de actualizacion desde la interfaz principal de trabajo.

**Prueba independiente**: Se valida desde el listado al accionar editar en una tarjeta y comprobar navegacion a la pagina de edicion del inmueble seleccionado.

**Escenarios de aceptacion**:

1. **Dado** que la persona operadora visualiza el listado de inmuebles, **Cuando** activa la accion de editar de una tarjeta, **Entonces** el sistema la dirige a la pagina de edicion del inmueble correspondiente.
2. **Dado** que la pagina de edicion abre para un inmueble existente, **Cuando** carga correctamente la informacion inicial, **Entonces** el formulario muestra los datos actuales listos para modificacion.

---

### Historia de Usuario 2 - Guardar cambios con validacion y feedback (Prioridad: P1)

Como persona operadora del catalogo, quiero modificar y guardar la informacion editable del inmueble con validaciones claras para asegurar que la actualizacion sea correcta.

**Por que esta prioridad**: Representa el objetivo funcional principal de la iniciativa.

**Prueba independiente**: Se valida editando datos permitidos, corrigiendo errores de validacion y enviando cambios hasta obtener confirmacion y navegacion al detalle.

**Escenarios de aceptacion**:

1. **Dado** que la pagina de edicion presenta datos cargados, **Cuando** la persona operadora modifica campos editables con valores validos, **Entonces** el formulario permite envio y ejecuta la actualizacion.
2. **Dado** que existen errores de validacion en uno o mas campos, **Cuando** la persona operadora intenta guardar, **Entonces** el sistema bloquea el envio y muestra mensajes de validacion claros junto a cada campo afectado.
3. **Dado** que la actualizacion se completa correctamente, **Cuando** el sistema recibe confirmacion de exito, **Entonces** muestra confirmacion visible y navega al detalle del inmueble actualizado.
4. **Dado** que la persona operadora intenta activar el guardado varias veces de forma seguida, **Cuando** ya existe una solicitud en curso, **Entonces** el sistema evita solicitudes duplicadas.

---

### Historia de Usuario 3 - Manejo de errores y conservacion de datos (Prioridad: P2)

Como persona operadora del catalogo, quiero recuperarme de errores de carga o guardado sin perder la informacion ingresada para poder completar la actualizacion.

**Por que esta prioridad**: Asegura continuidad operativa y evita retrabajo ante fallas.

**Prueba independiente**: Se valida forzando errores en carga inicial y guardado, comprobando mensajes comprensibles, acciones de recuperacion y conservacion de datos.

**Escenarios de aceptacion**:

1. **Dado** que la pagina de edicion no puede obtener la informacion inicial, **Cuando** se presenta el fallo, **Entonces** el sistema muestra un mensaje de error en la misma pantalla con acciones Reintentar y Volver al listado.
2. **Dado** que ocurre un error al guardar cambios, **Cuando** se procesa la respuesta de error, **Entonces** el sistema muestra un mensaje comprensible y conserva la informacion ingresada para reintento.
3. **Dado** que el servidor retorna errores de validacion por campo, **Cuando** el sistema los recibe, **Entonces** los presenta en los campos correspondientes sin limpiar los valores editados.

---

### Historia de Usuario 4 - Conservacion o reemplazo opcional de imagen (Prioridad: P2)

Como persona operadora del catalogo, quiero conservar la imagen actual cuando no elijo una nueva y reemplazarla solo cuando lo decida para controlar el contenido visual del inmueble.

**Por que esta prioridad**: Evita cambios no deseados en la imagen y respeta la opcionalidad del contrato.

**Prueba independiente**: Se valida una actualizacion sin nueva imagen y otra con nueva imagen, confirmando que ambos comportamientos siguen el contrato de UpdateProperty.

**Escenarios de aceptacion**:

1. **Dado** que la persona operadora guarda cambios sin seleccionar nueva imagen, **Cuando** se procesa la actualizacion, **Entonces** la imagen existente del inmueble se conserva.
2. **Dado** que la persona operadora selecciona una nueva imagen, **Cuando** guarda cambios validos, **Entonces** la actualizacion aplica el reemplazo de imagen segun el contrato del endpoint.
3. **Dado** que la seleccion de nueva imagen es opcional, **Cuando** la persona operadora decide no cambiarla, **Entonces** el flujo de guardado sigue disponible si el resto de campos es valido.

---

### Casos limite

- Si la pagina de edicion se abre para un `id` inexistente, la carga inicial debe tratarse como error recuperable en pantalla, con opcion de reintentar o volver al listado.
- Si el servidor responde `404 Not Found` al guardar, el sistema debe informar que el inmueble ya no esta disponible para actualizar y mantener los datos editados hasta que la persona usuaria decida la siguiente accion.
- Si el servidor responde `400 Bad Request` con errores de validacion, el sistema debe reflejar esos errores por campo y bloquear nuevos envios validos hasta correccion.
- Si el servidor responde `413 Payload Too Large` o `415 Unsupported Media Type`, el sistema debe mostrar error comprensible y preservar el resto del formulario para reintento.
- Si el servidor responde `500 Internal Server Error`, el sistema debe mostrar error global comprensible y conservar la informacion ingresada.

## Requisitos *(obligatorio)*

### Requisitos Funcionales

- **FR-001**: El sistema DEBE ofrecer una accion de editar en cada tarjeta de inmueble del listado para iniciar el flujo de actualizacion.
- **FR-002**: Al iniciar la edicion, el sistema DEBE dirigir a una pagina de edicion del inmueble seleccionado dentro de la aplicacion frontend.
- **FR-003**: La pagina de edicion DEBE cargar y mostrar la informacion actual del inmueble antes de permitir guardado.
- **FR-004**: El sistema DEBE permitir modificar los campos editables del inmueble definidos por el contrato `UpdatePropertyRequest`: `title`, `description`, `address`, `price`, `status` y `image` opcional.
- **FR-005**: El sistema DEBE aplicar validaciones funcionales en todos los campos editables y mostrar mensajes de validacion claros junto al campo correspondiente.
- **FR-006**: El sistema NO DEBE permitir el envio del formulario mientras existan errores de validacion activos.
- **FR-007**: El sistema DEBE consumir el endpoint `UpdateProperty` del contrato OpenAPI, usando la ruta `/api/properties/{id}` y el parametro de ruta `id` del inmueble a actualizar.
- **FR-008**: El sistema DEBE enviar la actualizacion con el formato de body permitido por contrato para `UpdatePropertyRequest` (`multipart/form-data` o `application/x-www-form-urlencoded`).
- **FR-009**: El sistema DEBE manejar al menos las respuestas del contrato de `UpdateProperty`: `200`, `400`, `404`, `413`, `415` y `500`, mostrando feedback comprensible para la persona usuaria.
- **FR-010**: Cuando la actualizacion sea exitosa (`200`), el sistema DEBE mostrar confirmacion visible y navegar al detalle del inmueble actualizado.
- **FR-011**: Durante carga inicial y guardado, el sistema DEBE mostrar estados visuales de progreso.
- **FR-012**: El sistema DEBE evitar solicitudes duplicadas causadas por multiples clics mientras exista una solicitud en curso.
- **FR-013**: Si el guardado falla, el sistema DEBE conservar la informacion ingresada para permitir correccion o reintento.
- **FR-014**: Si no se selecciona nueva imagen, el sistema DEBE conservar la imagen existente del inmueble.
- **FR-015**: El reemplazo de imagen DEBE ser opcional y solo aplicarse cuando la persona usuaria seleccione una nueva imagen valida segun contrato.
- **FR-016**: La iniciativa DEBE reutilizar patrones existentes de UI, estilos, servicios HTTP, modelos y manejo de errores del frontend actual.

### Reglas de validacion funcional basadas en contrato

- **RV-001**: El identificador `id` usado para la actualizacion corresponde a un UUID en el parametro de ruta.
- **RV-002**: `title` es obligatorio para guardar y debe contener al menos 3 caracteres no vacios.
- **RV-003**: `description` es obligatoria para guardar y debe contener al menos 10 caracteres no vacios.
- **RV-004**: `address` es obligatoria para guardar y debe contener al menos 5 caracteres no vacios.
- **RV-005**: `price` debe capturarse como valor numerico valido mayor a 0 conforme al contrato de `UpdatePropertyRequest` y a las convenciones vigentes del proyecto.
- **RV-006**: `status` debe representarse conforme al tipo definido por `PropertyStatus` en el contrato y solo permite valores validos del catalogo soportado por la aplicacion.
- **RV-007**: `image` es opcional y puede enviarse como archivo binario o como `null` segun el esquema del contrato.
- **RV-008**: Si se selecciona una nueva imagen, debe validarse en frontend tipo permitido (`image/png` o `image/jpeg`) y limite maximo de 5 MB antes de intentar el guardado.
- **RV-009**: Ante invalidaciones no resueltas en frontend o retornadas por backend, el envio debe bloquearse o rechazarse con mensajes de campo sin perder datos editados.
- **RV-010**: Para errores de medios (`413` o `415`), la interfaz debe mostrar mensaje comprensible y mantener el estado editable del formulario.

### Entidades Clave

- **Formulario de edicion de inmueble**: Superficie funcional donde se muestran los datos actuales y se gestionan cambios para actualizar el inmueble.
- **Solicitud de actualizacion**: Conjunto de datos editables enviados al endpoint `UpdateProperty` para un inmueble identificado por `id`.
- **Estado de validacion por campo**: Resultado visible por campo que permite corregir errores antes del envio.
- **Resultado de actualizacion**: Confirmacion de exito con navegacion al detalle o error manejado con mensajes y conservacion de datos.
- **Imagen del inmueble**: Recurso visual existente que puede conservarse o reemplazarse de forma opcional durante la actualizacion.

## Fuera de alcance

- Crear inmuebles nuevos.
- Eliminar inmuebles.
- Edicion masiva de multiples inmuebles.
- Incorporar funcionalidades de versionado o concurrencia optimista.
- Rediseñar el sistema visual global fuera de lo necesario para la pagina de edicion.

## Criterios de Exito *(obligatorio)*

### Resultados medibles

- **SC-001**: En el 100% de los casos validados, la accion de editar desde una tarjeta dirige a la pagina de edicion del inmueble correcto.
- **SC-002**: En el 100% de los casos de edicion con datos validos, el sistema completa la actualizacion con confirmacion visible y navegacion al detalle del inmueble actualizado.
- **SC-003**: En el 100% de los casos con errores de validacion, la interfaz bloquea el envio invalido y muestra mensajes claros en los campos correspondientes.
- **SC-004**: En el 100% de los casos de error de servidor validados (`400`, `404`, `413`, `415`, `500`), la interfaz muestra feedback comprensible y conserva la informacion ingresada para reintento.
- **SC-005**: En el 100% de los casos validados sin nueva imagen, la imagen existente permanece sin cambios; y en casos con nueva imagen valida, el reemplazo se aplica correctamente.

## Suposiciones

- El contrato OpenAPI actual de `UpdateProperty` se mantiene vigente durante la implementacion de esta iniciativa.
- Existe una vista de detalle navegable del inmueble para consumir la redireccion posterior al guardado exitoso.
- La carga inicial de datos del inmueble en la pagina de edicion utiliza un flujo ya disponible en el frontend para consulta por identificador.
- Los patrones de manejo de errores y validacion existentes en el frontend permiten mapear respuestas `ProblemDetails` a mensajes visibles por campo o globales.
