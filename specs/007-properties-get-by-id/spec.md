# Feature Specification: Consulta de propiedad por id

**Feature Branch**: `[007-properties-get-by-id]`

**Created**: 2026-07-11

**Status**: Implementgit ada

**Input**: User description: "Crear la especificación 007-properties-get-by-id para definir e implementar un endpoint público de consulta por id que devuelva una única propiedad en GET /api/properties/{id}, alineado con el contrato de 004-properties-list-pagination y con imageUrl absoluto y público cuando exista imagen."

## Ciclo de estado del spec *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementación -> Implementada**
- Está prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Está prohibido cerrar en **Implementada** sin evidencia de validación en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- [2026-07-11] Borrador - Spec creado por `/speckit.specify`
- [2026-07-11] Aprobada -> En implementación - Inicio de implementación por transición automática previa
- [2026-07-11] En implementación -> Implementada - Cierre automático tras completar tareas y evidencias de validación (2026-07-11T10:12:42.6744178-04:00)

## Clarifications

### Session 2026-07-11

- Q: ¿Qué código debe devolver el endpoint cuando el id de la ruta es inválido por formato? → A: `400 Bad Request` con ProblemDetails.
- Q: ¿Cuál es el formato canónico del identificador `id` en la ruta? → A: UUID (GUID RFC 4122).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Consultar una propiedad existente por id (Priority: P1)

Como consumidor del catálogo, quiero consultar una propiedad específica por su identificador para obtener todos sus datos públicos en una sola respuesta.

**Why this priority**: Es el flujo principal de consulta unitaria y habilita páginas de detalle, validaciones de existencia y navegación directa desde otros listados.

**Independent Test**: Se prueba invocando la consulta con un id existente y verificando una respuesta exitosa con todos los campos públicos requeridos, sin metadatos de paginación.

**Acceptance Scenarios**:

1. **Given** que existe una propiedad con el id solicitado, **When** realizo la consulta por id, **Then** la respuesta devuelve estado 200 con `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
2. **Given** que existe una propiedad con imagen registrada, **When** realizo la consulta por id, **Then** `imageUrl` se devuelve como URL absoluta y pública utilizable por un cliente externo.
3. **Given** que existe una propiedad sin imagen registrada, **When** realizo la consulta por id, **Then** `imageUrl` se devuelve en `null`.

---

### User Story 2 - Manejar consulta de id inexistente (Priority: P2)

Como consumidor del catálogo, quiero recibir una respuesta clara cuando el id consultado no exista para poder manejar el caso en la interfaz sin ambigüedad.

**Why this priority**: Este flujo evita errores silenciosos y permite una experiencia robusta de navegación y recuperación ante enlaces o referencias obsoletas.

**Independent Test**: Se prueba invocando la consulta con un id válido en formato pero inexistente en datos, verificando estado 404 con ProblemDetails.

**Acceptance Scenarios**:

1. **Given** que no existe una propiedad para el id solicitado, **When** realizo la consulta por id, **Then** la respuesta devuelve estado 404 y ProblemDetails.
2. **Given** que se recibe 404 por id inexistente, **When** inspecciono el cuerpo de error, **Then** contiene información de problema coherente con el contrato público del sistema.

---

### User Story 3 - Manejar id inválido y variación de entorno (Priority: P3)

Como consumidor de la API, quiero recibir un error consistente cuando el id sea inválido y obtener URLs correctas en distintos entornos para integrar el endpoint sin ajustes específicos por ambiente.

**Why this priority**: Refuerza la compatibilidad entre entornos y la calidad del contrato público frente a entradas inválidas.

**Independent Test**: Se prueba enviando un id con formato inválido y verificando respuesta de error; además se prueba la misma consulta en distintos hosts/esquemas comprobando que la URL de imagen usa el host y esquema de cada request.

**Acceptance Scenarios**:

1. **Given** que el id de la ruta no cumple el formato esperado por el contrato público, **When** realizo la consulta, **Then** la respuesta devuelve `400 Bad Request` con ProblemDetails.
2. **Given** que la misma propiedad se consulta desde dos entornos con distinto esquema u host, **When** la propiedad tiene imagen, **Then** `imageUrl` refleja el esquema y host de cada request.

---

### Edge Cases

- Consulta con id inexistente en almacenamiento: debe devolver 404 con ProblemDetails.
- Consulta con id inválido según el contrato público: debe devolver `400 Bad Request` con ProblemDetails y sin respuesta de propiedad.
- Propiedad existente sin imagen asociada: `imageUrl` debe devolverse como `null`.
- Propiedad existente con imagen asociada: `imageUrl` debe ser absoluta y pública, sin rutas internas ni relativas.
- Cambio de esquema/host entre entornos (por ejemplo, desarrollo local y entorno productivo): la URL devuelta debe ajustarse al request actual.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema DEBE exponer una operación pública de consulta unitaria de propiedades en la ruta `/api/properties/{id}`, donde `id` sigue el formato UUID (GUID RFC 4122) como identificador canónico de propiedad.
- **FR-002**: El sistema DEBE devolver estado 200 cuando el id solicitado exista.
- **FR-003**: En la respuesta 200, el sistema DEBE devolver exactamente los campos públicos `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
- **FR-004**: La respuesta de consulta unitaria DEBE mantener consistencia de contrato con los campos públicos ya definidos en el listado de propiedades de la spec 004.
- **FR-005**: El sistema DEBE devolver `imageUrl` como URL absoluta y pública cuando la propiedad tenga imagen asociada.
- **FR-006**: El sistema DEBE construir `imageUrl` usando el esquema y host de la solicitud actual, con el patrón `http(s)://{host}/assets/properties/{fileName}`.
- **FR-007**: El sistema NO DEBE devolver rutas físicas internas ni URLs relativas en `imageUrl`.
- **FR-008**: Cuando la propiedad exista pero no tenga imagen asociada, el sistema DEBE devolver `imageUrl` en `null`.
- **FR-009**: Cuando el id solicitado no exista, el sistema DEBE devolver estado 404 con ProblemDetails.
- **FR-010**: Cuando el id de la ruta sea inválido según el contrato público, el sistema DEBE devolver `400 Bad Request` con ProblemDetails y sin cuerpo de propiedad.
- **FR-011**: La operación DEBE devolver una única propiedad y NO DEBE incluir metadatos de paginación.
- **FR-012**: El contrato OpenAPI de la operación DEBE definirse en `specs/007-properties-get-by-id/contracts/backend-properties-get-by-id.openapi.yaml`.
- **FR-013**: La especificación DEBE incluir un modelo de datos y una guía de validación manual para esta operación en los artefactos de la misma feature.
- **FR-014**: La operación DEBE mantener compatibilidad con la gobernanza y dependencias ya aprobadas en las specs fundacionales y de propiedades previas.

### Key Entities *(include if feature involves data)*

- **Propiedad pública unitaria**: Vista externa de una propiedad individual consultada por id, con atributos descriptivos, ubicación, precio, estado, atributos físicos y referencia de imagen pública.
- **Identificador de propiedad**: Dato de entrada de la ruta en formato UUID (GUID RFC 4122) que selecciona de forma única la propiedad a consultar y determina si la respuesta es exitosa o de error.
- **Error de consulta**: Estructura de ProblemDetails para representar id inválido o inexistente de forma consistente para consumidores externos.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: El 100% de consultas con id existente devuelven estado 200 y todos los campos públicos definidos para la propiedad.
- **SC-002**: El 100% de consultas con id inexistente devuelven estado 404 con estructura de ProblemDetails.
- **SC-003**: El 100% de consultas con id inválido devuelven `400 Bad Request` con estructura de ProblemDetails.
- **SC-004**: En el 100% de respuestas con imagen, `imageUrl` es consumible externamente como URL absoluta y pública.
- **SC-005**: En el 100% de respuestas de consulta unitaria no se incluyen metadatos de paginación.

## Assumptions

- El identificador de propiedad canónico del módulo es UUID (GUID RFC 4122) y se mantiene en esta operación.
- La consulta unitaria reutiliza la misma definición de campos públicos del dominio de propiedades ya validada en la spec 004.
- La publicación de archivos de imagen en `/assets/properties/` se mantiene disponible para consumo externo en los entornos donde se despliegue la solución.
- Esta entrega se limita a lectura por id y no modifica reglas de creación, actualización, paginación ni estado de propiedades definidas en specs previas.
