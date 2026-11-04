# Feature Specification: Actualización de propiedades con imagen opcional

**Feature Branch**: `feat/006-properties-update`

**Created**: 2026-07-11

**Status**: Implementada

**Input**: User description: "Crear la especificación 006-properties-update para definir un nuevo caso de uso de actualización de propiedades mediante un slice Minimal API en backend. La especificación debe cubrir un endpoint público de actualización por id (PUT /api/properties/{id}), que reciba los datos editables de la propiedad y permita de forma opcional el upload de imagen, manteniendo la misma lógica implementada en 005-properties-create para manejo de imágenes."

## Ciclo de estado del spec *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementación -> Implementada**
- Está prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Está prohibido cerrar en **Implementada** sin evidencia de validación en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- 2026-07-11 Borrador - Spec creado por `/speckit.specify`
- 2026-07-11 Borrador → Aprobada - Transición automática previa a implementación
- 2026-07-11 Aprobada → En implementación - Transición automática previa a implementación
- 2026-07-11 En implementación → Implementada - Cierre validado sin tareas pendientes y con evidencia en quickstart

## Clarifications

### Session 2026-07-11

- Q: ¿Cuáles son los campos de negocio editables en la actualización? → A: Los mismos que el alta (005): `title`, `description`, `price`, `address` y `status`; la imagen se maneja aparte. Nota: el campo se llama `status` en el dominio (string/enum), no `statusId`.
- Q: Semántica PUT — ¿todos los campos editables son obligatorios o los omitidos conservan su valor previo? → A: Todos obligatorios; campo omitido en el body devuelve `400 Bad Request` (reemplazo total estricto).
- Q: ¿Qué debe devolver el sistema ante una actualización exitosa? → A: `200 OK` con el mismo shape de respuesta que el alta (005) — datos completos de la propiedad actualizada incluyendo `id` e `imageUrl`.
- Q: ¿Cómo se manejan actualizaciones concurrentes sobre la misma propiedad? → A: Last-write-wins sin control de concurrencia optimista; comportamiento extensible en spec futura.
- Q: ¿El endpoint requiere autenticación o es público como el de creación (005)? → A: Público, sin autenticación, igual que el endpoint de alta.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Actualización de propiedad sin imagen (Priority: P1)

Como operador del catálogo, quiero actualizar los datos de una propiedad existente sin necesidad de enviar una nueva imagen, para corregir o enriquecer la información sin perder la imagen previamente asociada.

**Why this priority**: Es el flujo mínimo de edición de propiedades; permite correcciones de datos críticos (precio, título, descripción) sin afectar el recurso visual ya cargado. Sin este flujo, ninguna actualización básica es posible.

**Independent Test**: Puede probarse enviando un request de actualización con datos válidos y sin archivo de imagen, verificando que los campos de negocio se actualizan y que `imageUrl` conserva el valor previo registrado en la base de datos.

**Acceptance Scenarios**:

1. **Given** que existe una propiedad con `imageUrl` no nulo, **When** el operador envía datos válidos de actualización sin imagen, **Then** el sistema actualiza los campos de negocio y devuelve la propiedad actualizada con el `imageUrl` previo sin modificación.
2. **Given** que existe una propiedad con `imageUrl` en `null`, **When** el operador envía datos válidos de actualización sin imagen, **Then** el sistema actualiza los campos de negocio y `imageUrl` permanece en `null`.
3. **Given** que la propiedad fue actualizada sin imagen, **When** un cliente consulta el registro posteriormente, **Then** obtiene los datos actualizados con el mismo `imageUrl` que existía antes de la actualización.

---

### User Story 2 - Actualización de propiedad con imagen válida (Priority: P2)

Como operador del catálogo, quiero reemplazar o agregar la imagen de una propiedad existente junto con una actualización de datos, para mantener el material visual del catálogo vigente.

**Why this priority**: Aporta valor directo al catálogo visual del sistema y complementa P1, pero requiere que la búsqueda y actualización base ya funcionen.

**Independent Test**: Puede probarse enviando datos válidos y una imagen PNG o JPG de hasta 5 MB para una propiedad existente, verificando que la propiedad queda actualizada y que `imageUrl` apunta a la nueva imagen persistida.

**Acceptance Scenarios**:

1. **Given** que existe una propiedad con cualquier valor de `imageUrl`, **When** el operador envía datos válidos y una imagen PNG/JPG válida de hasta 5 MB, **Then** el sistema almacena la nueva imagen, actualiza los datos de negocio y persiste un nuevo `imageUrl` público/relativo asociado.
2. **Given** que la propiedad fue actualizada con nueva imagen, **When** un cliente consulta el registro posteriormente, **Then** recibe el nuevo `imageUrl` persistido para su consumo.

---

### User Story 3 - Rechazo por validación y manejo de errores (Priority: P3)

Como operador del catálogo, quiero recibir errores claros y precisos cuando el request de actualización sea inválido o la propiedad no exista, para corregir el envío sin ambigüedad y sin efectos secundarios no deseados.

**Why this priority**: Es un flujo de robustez que garantiza integridad de datos y guía al usuario ante errores, pero no bloquea la entrega del valor base de P1 y P2.

**Independent Test**: Puede probarse enviando requests con id inexistente, imagen inválida (formato no permitido, tamaño excedido, contenido real inválido) o datos de negocio inválidos, verificando los códigos HTTP correspondientes y ausencia de cambios en la base de datos.

**Acceptance Scenarios**:

1. **Given** que no existe una propiedad con el id solicitado, **When** el operador solicita la actualización, **Then** el sistema responde con `404 Not Found` y no realiza ningún cambio.
2. **Given** que existe una propiedad y el operador envía imagen con formato distinto de PNG/JPG, **When** solicita la actualización, **Then** el sistema rechaza la solicitud con `415 Unsupported Media Type` y no modifica la propiedad.
3. **Given** que existe una propiedad y el operador envía imagen mayor a 5 MB, **When** solicita la actualización, **Then** el sistema rechaza con `413 Payload Too Large` y no modifica la propiedad.
4. **Given** que existe una propiedad y el operador envía datos de negocio inválidos o incompletos, **When** solicita la actualización, **Then** el sistema responde con `400 Bad Request` y no modifica la propiedad.
5. **Given** que la solicitud es rechazada por cualquier validación, **When** se consulta el registro a posteriori, **Then** la propiedad conserva exactamente los valores que tenía antes del intento de actualización.

---

### Edge Cases

- Si el `id` del path no corresponde a ninguna propiedad persistida, la operación debe rechazarse con `404 Not Found` sin ejecutar ninguna escritura.
- Si la imagen enviada está vacía (0 bytes), la solicitud debe rechazarse con `415 Unsupported Media Type` y la propiedad no debe modificarse.
- Si la imagen tiene extensión `.jpg` o `.png` pero su contenido real no corresponde a esos formatos, la solicitud debe rechazarse con `415 Unsupported Media Type`.
- El sistema debe ignorar el nombre original del archivo enviado y generar un nombre único interno (UUID + extensión válida) para evitar colisiones en el almacenamiento.
- Si el guardado de imagen falla durante la actualización, la operación completa debe ser atómica: ni los datos de negocio ni la referencia a imagen deben quedar actualizados parcialmente.
- Si el request no incluye imagen, el `imageUrl` existente en la base de datos DEBE conservarse sin modificación, independientemente de si su valor era `null` o una URL válida.
- No se debe alterar el `id` ni los metadatos de auditoría de la propiedad (fecha de creación) durante la actualización.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema DEBE exponer una operación pública de actualización de propiedades que identifique la propiedad por su `id` único.
- **FR-002**: El sistema DEBE buscar la propiedad por `id` antes de ejecutar cualquier cambio.
- **FR-003**: El sistema DEBE responder `404 Not Found` y no realizar ninguna modificación cuando el `id` no corresponda a ninguna propiedad existente.
- **FR-004**: El sistema DEBE actualizar los campos `title`, `description`, `price`, `address` y `status` de la propiedad cuando el `id` exista y todos estos campos se proporcionen con valores válidos.
- **FR-005**: El sistema DEBE permitir que la imagen sea opcional en la operación de actualización.
- **FR-006**: El sistema DEBE conservar el `imageUrl` existente sin modificación cuando el request no incluya imagen, incluyendo el caso en que el valor previo sea `null`.
- **FR-007**: El sistema DEBE aceptar únicamente imágenes en formato PNG o JPG cuando se envía archivo de imagen.
- **FR-008**: El sistema DEBE rechazar imágenes cuyo tamaño supere 5 MB.
- **FR-009**: El sistema DEBE almacenar la nueva imagen válida en la ubicación pública definida para activos de propiedades y reemplazar el `imageUrl` de la propiedad con la nueva ruta pública/relativa.
- **FR-010**: El sistema DEBE asegurar que el nuevo `imageUrl` público/relativo persistido sea consumible directamente por clientes sin transformación adicional en el flujo de lectura existente.
- **FR-011**: El sistema DEBE devolver `200 OK` con el mismo shape de respuesta que el alta (005) — datos completos de la propiedad actualizada incluyendo `id`, campos de negocio editables e `imageUrl` resultante.
- **FR-012**: El sistema DEBE devolver `415 Unsupported Media Type` cuando la imagen enviada no sea PNG o JPG, cuando esté vacía (0 bytes), o cuando su contenido real no corresponda al formato declarado.
- **FR-013**: El sistema DEBE devolver `413 Payload Too Large` cuando la imagen supere 5 MB.
- **FR-014**: El sistema DEBE devolver `400 Bad Request` cuando cualquiera de los campos editables obligatorios (`title`, `description`, `price`, `address`, `status`) sea inválido, incompleto u omitido en el body.
- **FR-015**: El sistema DEBE tratar la actualización con imagen como una operación atómica; si falla el almacenamiento de imagen, NO DEBE persistir ningún cambio sobre la propiedad.
- **FR-016**: El sistema DEBE devolver `500 Internal Server Error` cuando falle el almacenamiento interno de la imagen.
- **FR-017**: El sistema DEBE generar un nombre único interno para cada imagen persistida (UUID + extensión válida) y NO DEBE depender del nombre original del archivo enviado.
- **FR-018**: La propiedad actualizada DEBE ser recuperable correctamente en los flujos de listado y consulta aprobados en specs previas.

### Key Entities *(include if feature involves data)*

- **Solicitud de actualización de propiedad**: Representa la entrada de datos para editar una propiedad existente, incluyendo el `id` de ruta, los campos de negocio editables y un archivo de imagen opcional.
- **Propiedad**: Representa el registro persistido del catálogo inmobiliario con sus datos principales y atributo `imageUrl` nullable; su `id` identifica de forma única el registro a actualizar.
- **Recurso de imagen de propiedad**: Representa el archivo visual nuevo asociado opcionalmente a la propiedad actualizada y su nueva ruta pública/relativa de consumo.
- **Error de validación de actualización**: Representa el detalle estructurado de incumplimientos en datos, archivo o identidad del recurso, permitiendo corrección por parte del operador.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: El 100% de actualizaciones con datos válidos y sin imagen finaliza con los campos de negocio actualizados y el `imageUrl` previo conservado sin modificación.
- **SC-002**: El 100% de actualizaciones con datos válidos e imagen PNG/JPG de hasta 5 MB finaliza con la nueva imagen persistida y un `imageUrl` actualizado y consumible por clientes.
- **SC-003**: El 100% de solicitudes dirigidas a un `id` inexistente responde con `404 Not Found` sin producir ningún cambio en la base de datos.
- **SC-004**: El 100% de solicitudes con imagen de formato no permitido, vacía o con tamaño mayor a 5 MB es rechazado con el código de error correspondiente sin modificar la propiedad ni almacenar archivos.
- **SC-005**: En una muestra de 100 solicitudes válidas mixtas (con y sin imagen, sobre propiedades existentes), el percentil 95 del tiempo de respuesta debe ser menor a 3 segundos en entorno de validación funcional.

## Assumptions

- Las reglas de negocio base de propiedades definidas en specs previas (001-005) se mantienen vigentes y no cambian en esta iniciativa.
- El alcance se limita a la actualización individual de una propiedad por `id`; actualización masiva y operaciones parciales (PATCH) quedan fuera de esta entrega.
- Todos los campos editables (`title`, `description`, `price`, `address`, `status`) son obligatorios en el body de actualización; si alguno se omite el sistema responde `400 Bad Request` (reemplazo total estricto).
- La actualización concurrente de la misma propiedad sigue semántica last-write-wins sin control de concurrencia optimista; este comportamiento puede extenderse en una spec futura si el caso de uso lo justifica.
- La ruta pública/relativa de la nueva imagen persistida debe ser consumible por clientes sin transformación adicional en el flujo de lectura existente.
- La operación debe convivir con la gobernanza y dependencias de foundation, persistencia, listado y alta aprobadas en las features 001 a 005.
- Las respuestas de error siguen el formato estándar ya usado por el sistema para validaciones funcionales (mismo formato ProblemDetails que en 005).
- La imagen anterior no se elimina físicamente del almacenamiento cuando se reemplaza por una nueva (limpieza de activos huérfanos queda fuera de alcance).
