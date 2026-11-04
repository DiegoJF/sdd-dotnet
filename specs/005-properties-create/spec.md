# Feature Specification: Alta de propiedades con imagen opcional

**Feature Branch**: `[005-properties-create]`

**Created**: 2026-07-11

**Status**: Implementada

**Input**: User description: "Crear la especificación 005-properties-create para definir un nuevo caso de uso de alta de propiedades mediante un slice Minimal API en backend. La especificación debe cubrir un endpoint público de creación que reciba los datos de la propiedad y permita de forma opcional el upload de imagen. Si no se envía imagen, la propiedad debe persistirse correctamente con imageUrl en null. Si se envía imagen válida, debe almacenarse en app/backend/src/RealtorApi/wwwroot/assets/properties y persistirse la URL pública/relativa correspondiente en la propiedad para su consumo posterior por clientes. Incluir criterios de validación funcional del upload (formatos PNG/JPG y tamaño máximo 5MB), reglas de respuesta y errores esperados, escenarios independientes por prioridad (P1/P2/P3), requisitos funcionales testables y criterios de éxito medibles. Mantener alineación con la arquitectura existente de Vertical Slice (slice + handler + mapping + validator), auto-registro por IHandler e ISlice, y mapeos explícitos por slice sin lógica de negocio en mapping. Respetar gobernanza Spec-Driven del repositorio y dependencias con specs previas de foundation, persistencia y listado."

## Ciclo de estado del spec *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementación -> Implementada**
- Está prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Está prohibido cerrar en **Implementada** sin evidencia de validación en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- 2026-07-11 Borrador - Spec creado por `/speckit.specify`
- 2026-07-11 Aprobada -> En implementación - Transición automática previa a implementación
- 2026-07-11 En implementación -> Implementada - Cierre validado sin tareas pendientes y con evidencia en quickstart

## Clarifications

### Session 2026-07-11

- Q: Si falla el guardado de imagen, ¿la alta debe ser atómica o parcial? → A: La alta es atómica; si falla la imagen no se crea la propiedad.
- Q: ¿Qué códigos HTTP se devuelven para errores de formato, tamaño y fallo interno de guardado? → A: 415 para formato inválido, 413 para tamaño mayor a 5 MB y 500 para fallo interno de guardado.
- Q: ¿Cómo se gestionan colisiones de nombre al guardar imágenes? → A: El sistema genera un nombre único interno con UUID y conserva la extensión válida.
- Q: ¿Cómo se tratan imagen vacía (0 bytes) y contenido real inválido? → A: Ambos casos se tratan como formato no soportado y deben responder 415.
- Q: ¿Qué código se usa para validaciones de campos de negocio inválidos (title, price, etc.)? → A: Se responde 400 para validaciones de campos de negocio.
- Q: ¿Cómo se mide SC-004 de forma verificable? → A: Se mide con 100 solicitudes válidas mixtas (con y sin imagen) y se exige p95 menor a 3 segundos.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Alta de propiedad sin imagen (Priority: P1)

Como operador del catálogo, quiero registrar una propiedad con sus datos obligatorios aunque no incluya imagen, para publicar inventario nuevo sin depender de material visual inmediato.

**Why this priority**: Es el flujo mínimo de negocio para habilitar altas de propiedades y evitar bloqueo operativo por falta de imagen.

**Independent Test**: Puede probarse enviando una solicitud de alta sin archivo de imagen y verificando que la propiedad queda creada, disponible para consulta posterior y con `imageUrl` en `null`.

**Acceptance Scenarios**:

1. **Given** que el operador envía datos válidos de una propiedad sin imagen, **When** confirma el alta, **Then** el sistema crea la propiedad y la devuelve como creada con `imageUrl` en `null`.
2. **Given** que la propiedad fue creada sin imagen, **When** un cliente consulta ese registro posteriormente, **Then** obtiene la propiedad persistida con `imageUrl` en `null`.

---

### User Story 2 - Alta de propiedad con imagen válida (Priority: P2)

Como operador del catálogo, quiero registrar una propiedad junto con una imagen válida, para que los clientes consuman el recurso visual desde la misma alta.

**Why this priority**: Aporta valor comercial directo al mejorar la presentación de la propiedad, pero depende de que el flujo P1 ya exista.

**Independent Test**: Puede probarse enviando una alta con datos válidos y una imagen PNG o JPG de hasta 5 MB, verificando que la propiedad se crea y que `imageUrl` apunta a la ruta pública/relativa esperada.

**Acceptance Scenarios**:

1. **Given** que el operador envía datos válidos y una imagen PNG/JPG dentro del límite permitido, **When** confirma el alta, **Then** el sistema guarda la imagen, crea la propiedad y persiste un `imageUrl` público/relativo asociado.
2. **Given** que la propiedad fue creada con imagen válida, **When** un cliente consulta ese registro posteriormente, **Then** recibe el mismo `imageUrl` persistido para su consumo.

---

### User Story 3 - Rechazo por validación de imagen (Priority: P3)

Como operador del catálogo, quiero recibir errores claros cuando la imagen no cumple formato o tamaño, para corregir el envío sin ambiguedad.

**Why this priority**: Es un flujo de robustez y calidad de datos que protege el sistema y guía al usuario, pero no bloquea la entrega del valor base de P1.

**Independent Test**: Puede probarse enviando altas con imagen inválida (formato no permitido o peso mayor a 5 MB) y verificando rechazo sin crear propiedad ni archivo.

**Acceptance Scenarios**:

1. **Given** que el operador envía una imagen con formato distinto de PNG/JPG, **When** solicita el alta, **Then** el sistema rechaza la solicitud e informa el error de formato permitido.
2. **Given** que el operador envía una imagen mayor a 5 MB, **When** solicita el alta, **Then** el sistema rechaza la solicitud e informa el límite de tamaño.
3. **Given** que la solicitud es rechazada por validación de imagen, **When** se revisa el estado posterior, **Then** no existe propiedad creada ni imagen almacenada para esa operación.

---

### Edge Cases

- Si la imagen enviada está vacía (0 bytes), la solicitud debe rechazarse con `415 Unsupported Media Type`.
- Si la imagen tiene extensión `.jpg` o `.png` pero su contenido real no corresponde a esos formatos, la solicitud debe rechazarse con `415 Unsupported Media Type`.
- El sistema debe ignorar el nombre original para persistencia y generar un nombre único interno (UUID + extensión válida) para evitar colisiones.
- Si el guardado de imagen falla después de validar datos, la operación completa debe revertirse y no debe persistirse la propiedad.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema DEBE exponer una operación pública de alta de propiedades que acepte los datos requeridos de la propiedad.
- **FR-002**: El sistema DEBE permitir que la imagen sea opcional en la operación de alta.
- **FR-003**: El sistema DEBE crear y persistir correctamente una propiedad cuando se envían datos válidos sin imagen.
- **FR-004**: El sistema DEBE persistir `imageUrl` en `null` cuando la alta se completa sin imagen.
- **FR-005**: El sistema DEBE aceptar únicamente imágenes en formato PNG o JPG cuando se envía archivo de imagen.
- **FR-006**: El sistema DEBE rechazar imágenes cuyo tamaño supere 5 MB.
- **FR-007**: El sistema DEBE almacenar la imagen válida en la ubicación pública definida para activos de propiedades y asociarla a la propiedad creada.
- **FR-008**: El sistema DEBE persistir en la propiedad un `imageUrl` público/relativo utilizable por clientes cuando exista imagen válida.
- **FR-009**: El sistema DEBE devolver una respuesta de creación exitosa con los datos principales de la propiedad y su estado de imagen (`null` o URL).
- **FR-010**: El sistema DEBE devolver `415 Unsupported Media Type` cuando la imagen enviada no sea PNG o JPG, cuando esté vacía (0 bytes), o cuando su contenido real no corresponda al formato declarado.
- **FR-011**: El sistema DEBE devolver `413 Payload Too Large` cuando la imagen supere 5 MB.
- **FR-012**: El sistema NO DEBE crear la propiedad ni almacenar imagen cuando la solicitud falle por validaciones funcionales de entrada.
- **FR-013**: El sistema DEBE mantener compatibilidad funcional con los flujos ya aprobados de persistencia y listado para que una propiedad recién creada sea recuperable en consultas posteriores.
- **FR-014**: El sistema DEBE tratar el alta con imagen como una operación atómica; si falla el almacenamiento de imagen, NO DEBE persistir la propiedad.
- **FR-015**: El sistema DEBE devolver `500 Internal Server Error` cuando falle el almacenamiento interno de la imagen.
- **FR-016**: El sistema DEBE generar un nombre único interno para cada imagen persistida (UUID + extensión válida) y NO DEBE depender del nombre original del archivo enviado.
- **FR-017**: El sistema DEBE devolver `400 Bad Request` cuando los campos de negocio requeridos del alta sean inválidos o incompletos.

### Key Entities *(include if feature involves data)*

- **Solicitud de alta de propiedad**: Representa la entrada de datos para crear una propiedad, incluyendo campos de negocio de la propiedad y un archivo de imagen opcional.
- **Propiedad**: Representa el registro persistido del catálogo inmobiliario con sus datos principales y atributo `imageUrl` nullable.
- **Recurso de imagen de propiedad**: Representa el archivo visual asociado opcionalmente a una propiedad creada y su ruta pública/relativa de consumo.
- **Error de validación de alta**: Representa el detalle estructurado de incumplimientos en datos o archivo para permitir corrección por parte del operador.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: El 100% de altas con datos válidos y sin imagen finaliza con creación exitosa y `imageUrl` en `null`.
- **SC-002**: El 100% de altas con datos válidos e imagen PNG/JPG de hasta 5 MB finaliza con creación exitosa y `imageUrl` no nulo consumible por clientes.
- **SC-003**: El 100% de solicitudes con imagen de formato no permitido o tamaño mayor a 5 MB es rechazado con mensaje de validación comprensible para corrección en un único intento posterior.
- **SC-004**: En una muestra de 100 solicitudes válidas mixtas (con y sin imagen), el percentil 95 del tiempo de respuesta debe ser menor a 3 segundos en entorno de validación funcional.

## Assumptions

- Las reglas de negocio base de propiedades definidas en specs previas se mantienen vigentes y no cambian en esta iniciativa.
- El alcance se limita al alta individual de propiedades; la edición posterior de imagen o cargas múltiples queda fuera de esta entrega.
- La ruta pública/relativa de imagen persistida debe ser consumible por clientes sin transformación adicional en el flujo de lectura existente.
- La operación debe convivir con la gobernanza y dependencias de foundation, persistencia y listado aprobadas en las features 001 a 004.
- Las respuestas de error siguen el formato estándar ya usado por el sistema para validaciones funcionales.
