# Feature Specification: Persistencia Inicial de Propiedades

**Feature Branch**: `[003-properties-persistence-seeding]`

**Created**: 2026-07-09

**Status**: Implementada

**Input**: User description: "Crear la especificación 003-properties-persistence-seeding para definir un modelo persistente inicial de propiedades y estados, con migración única, carga automática idempotente desde fuentes externas y rutas de imagen públicas listas para consulta."

## Clarifications

### Session 2026-07-09

- Q: ¿Qué política se aplica si una propiedad del seed ya existe en la base? → A: Upsert: actualizar campos funcionales y conservar identidad existente.
- Q: ¿Cómo se comporta el arranque si faltan manifiesto o JSON críticos? → A: Fail-fast: abortar arranque.
- Q: ¿Qué hacer si falta la imagen de una propiedad durante seeding? → A: Omitir solo esa propiedad, registrar error estructurado y continuar.

## Ciclo de estado del spec *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementación -> Implementada**
- Está prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Está prohibido cerrar en **Implementada** sin evidencia de validación en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- [2026-07-09] Borrador - Spec creado por `/speckit.specify`
- [2026-07-09] En implementación - Estado activo alineado con tareas pendientes en `tasks.md` y validación final pendiente en `quickstart.md`.
- [2026-07-09] Implementada - Cierre automático aplicado tras validar `tasks.md` sin pendientes y evidencia de validación no vacía en `quickstart.md`.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Carga Inicial Confiable (Priority: P1)

Como responsable de operaciones, quiero que el servicio inicie con los datos base de propiedades y estados cargados automáticamente para disponer del inventario inicial sin pasos manuales.

**Why this priority**: Sin carga inicial confiable no existe base de datos utilizable para las consultas y procesos posteriores.

**Independent Test**: Puede probarse iniciando el servicio sobre una base vacía y verificando que queden creados registros de estados y propiedades sin intervención manual.

**Acceptance Scenarios**:

1. **Given** una base vacía y fuentes de datos válidas, **When** inicia el servicio, **Then** se crea el esquema requerido y se insertan estados y propiedades iniciales.
2. **Given** una base con datos ya inicializados, **When** inicia el servicio nuevamente, **Then** no se duplican registros existentes.

---

### User Story 2 - Integridad de Recursos Externos (Priority: P2)

Como responsable de plataforma, quiero que los archivos de datos y recursos visuales estén disponibles en el entorno de ejecución para que la carga inicial y las consultas futuras puedan resolver información completa.

**Why this priority**: La disponibilidad consistente de fuentes externas evita fallos en arranque y datos incompletos para consumo funcional.

**Independent Test**: Puede probarse verificando que, en ejecución y publicación, los archivos declarados estén presentes y sean legibles por el proceso de inicialización.

**Acceptance Scenarios**:

1. **Given** el artefacto de despliegue generado, **When** el servicio arranca, **Then** puede leer el manifiesto y los archivos de datos declarados.
2. **Given** recursos de imagen incluidos, **When** finaliza la inicialización, **Then** los recursos quedan accesibles en su ubicación final servible por la API.

---

### User Story 3 - Referencias de Imagen Consistentes (Priority: P3)

Como consumidor de la API, quiero que cada propiedad devuelva una URL de imagen pública y estable para poder renderizar imágenes directamente sin conocer rutas internas del servidor.

**Why this priority**: Una URL pública consistente habilita integración directa de clientes y evita acoplamiento a rutas físicas internas.

**Independent Test**: Puede probarse consultando propiedades inicializadas y validando que la URL almacenada sea servible por la API y no exponga rutas físicas internas.

**Acceptance Scenarios**:

1. **Given** una propiedad inicializada con imagen, **When** se recupera su información, **Then** la URL de imagen apunta a una ruta pública de la API.
2. **Given** una propiedad inicializada, **When** se inspecciona la URL almacenada, **Then** no contiene rutas locales o de soporte interno.

---

### Edge Cases

- ¿Qué ocurre si el manifiesto referencia un archivo de datos inexistente o inaccesible en runtime?
- ¿Cómo responde el sistema cuando una imagen declarada no está disponible durante la inicialización?
- ¿Cómo se comporta la carga automática si solo una parte del conjunto de datos ya existe en la base?
- ¿Qué ocurre cuando el estado referenciado por una propiedad no pertenece al catálogo permitido?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema MUST mantener un catálogo de estados de propiedad con exactamente tres valores de negocio iniciales: Available, Rented y Maintenance.
- **FR-002**: El sistema MUST persistir propiedades con identificador, datos descriptivos, ubicación, precio, estado, atributos físicos, URL de imagen y marcas temporales de creación/actualización.
- **FR-003**: El sistema MUST garantizar que el estado de una propiedad se almacene y recupere con representación textual estable para preservar legibilidad y compatibilidad evolutiva.
- **FR-004**: El sistema MUST aplicar reglas de integridad de datos con parámetros normativos: PropertyStatus.Value max 32; PropertyStatus.Description max 200; Property.Title max 160; Property.Description max 1200; Property.Address max 240; Property.ImageUrl max 260; Property.Price decimal(12,2); Property.AreaSquareMeters decimal(10,2); BedroomCount y BathroomCount >= 0; AreaSquareMeters > 0; campos requeridos según data-model.md.
- **FR-005**: El sistema MUST crear o actualizar el esquema de persistencia al inicio del servicio, siempre antes de habilitar el procesamiento normal de solicitudes.
- **FR-006**: El sistema MUST ejecutar una inicialización automática de datos en cada arranque, aun cuando no existan cambios de esquema pendientes.
- **FR-007**: El sistema MUST garantizar idempotencia de la inicialización para evitar duplicados y mantener consistencia entre arranques repetidos.
- **FR-007.1**: Cuando una propiedad del seed ya exista, el sistema MUST aplicar upsert actualizando campos funcionales permitidos y preservando su identidad persistente.
- **FR-008**: El sistema MUST consumir un manifiesto central para resolver ubicaciones de fuentes de datos y recursos, evitando rutas dispersas o implícitas.
- **FR-008.1**: Si faltan el manifiesto o JSON críticos requeridos para inicialización, el sistema MUST abortar el arranque con error explícito y trazable.
- **FR-009**: El sistema MUST disponer en runtime los archivos de datos y recursos visuales requeridos para la inicialización y consumo posterior.
- **FR-009.1**: Si falta la imagen de una propiedad específica, el sistema MUST omitir únicamente esa propiedad, registrar error estructurado y continuar con el resto del seeding.
- **FR-012**: Cuando una propiedad del seed referencie un estado fuera del catálogo permitido, el sistema MUST omitir ese registro, registrar error estructurado y continuar con el resto del seeding.
- **FR-010**: El sistema MUST almacenar en cada propiedad una URL de imagen pública servible por la API, y MUST NOT almacenar rutas físicas internas del proyecto.
- **FR-011**: El sistema MUST permitir verificar mediante pruebas que la estructura de persistencia, la inicialización automática y la consistencia de URLs de imagen cumplen los criterios funcionales definidos.

### Key Entities *(include if feature involves data)*

- **PropertyStatus**: Catálogo de estado operativo de una propiedad. Contiene el valor textual del estado permitido y su semántica de negocio.
- **Property**: Registro principal del inventario inmobiliario. Incluye identificación, datos descriptivos, ubicación, precio, estado, características físicas, URL de imagen pública y trazabilidad temporal.
- **SeedManifest**: Documento de referencia central para ubicar fuentes de datos y recursos visuales requeridos por la inicialización automática.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: En una base vacía, el 100% de los arranques iniciales crean esquema y cargan datos base sin intervención manual.
- **SC-002**: En al menos 3 arranques consecutivos sobre la misma base, la variación de conteo de registros iniciales es 0% (sin duplicados).
- **SC-003**: El 100% de las propiedades inicializadas con imagen almacenan una URL pública válida y consumible por la API.
- **SC-004**: El 100% de los recursos críticos declarados en el manifiesto (manifiesto y JSON de estados/propiedades) se resuelven correctamente en entorno de ejecución durante inicialización; la ausencia de una imagen individual se gestiona según FR-009.1 sin bloquear el arranque.
- **SC-005**: La suite de pruebas de la feature alcanza 100% de aprobación en validaciones de integridad de persistencia, idempotencia y consistencia de recursos.

## Assumptions

- Se asume que los archivos de datos y recursos visuales entregados por negocio contienen estructura válida y consistente para la carga inicial.
- Se asume que el entorno de despliegue permite acceso de lectura a los recursos declarados en el manifiesto durante el arranque.
- Se asume que la exposición de rutas públicas de imagen seguirá el esquema estándar de publicación de archivos estáticos de la API.
- Se asume que esta iniciativa cubre modelo persistente e inicialización base, sin incluir todavía casos de uso completos de creación/edición por interfaz externa.
