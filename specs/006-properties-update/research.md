# Investigación Técnica: Actualización de propiedades con imagen opcional

## Decisión 1: Reutilización de CreatePropertyImageStorage

- **Decisión**: Reutilizar directamente la clase estática `CreatePropertyImageStorage` del slice `CreateProperty` para el guardado de imagen en `UpdateProperty`.
- **Rationale**: La lógica de almacenamiento (generación de UUID, validación de extensión, escritura asíncrona, TryDelete ante fallo) ya está probada en spec 005 y cubre todos los requisitos de spec 006. Duplicar implicaría mantener dos implementaciones equivalentes sin valor diferencial.
- **Alternatives considered**:
  - Crear `UpdatePropertyImageStorage` duplicado: rechazado por violación de DRY sin justificación constitucional.
  - Mover a infraestructura compartida: rechazado porque implicaría refactor no trazado a tarea y está fuera del alcance de esta spec.

## Decisión 2: Búsqueda de propiedad antes de actualizar

- **Decisión**: Usar `dbContext.Properties.FindAsync(id, cancellationToken)` para localizar la propiedad por `Id` antes de ejecutar ningún cambio. Si el resultado es `null`, devolver `404 Not Found` sin escritura.
- **Rationale**: `FindAsync` primero busca en el Change Tracker de EF Core y luego en la base de datos; es la forma más eficiente para búsqueda por clave primaria. Garantiza la regla funcional FR-002 y FR-003.
- **Alternatives considered**:
  - `FirstOrDefaultAsync(p => p.Id == id)`: rechazado por omitir el cache del Change Tracker; funcionalmente equivalente pero menos eficiente.
  - Asumir que el id existe y manejar excepción de FK: rechazado por semántica inadecuada y mayor complejidad de error handling.

## Decisión 3: Campos editables en UpdatePropertyRequest

- **Decisión**: `UpdatePropertyRequest` incluye los campos: `title`, `description`, `address`, `price`, `status` e `image` (opcional). Los campos `bedroomCount`, `bathroomCount` y `areaSquareMeters` no son editables mediante este endpoint.
- **Rationale**: Confirmado en sesión de clarificación (Q1). Los atributos estructurales de la propiedad (superficie, habitaciones, baños) se consideran inmutables una vez creada; solo los atributos de presentación y gestión (título, descripción, dirección, precio, estado) son editables operativamente.
- **Alternatives considered**:
  - Incluir todos los campos del alta: rechazado por decisión explícita del usuario en clarificación Q1.

## Decisión 4: Semántica PUT estricta — todos los campos obligatorios

- **Decisión**: Todos los campos editables de `UpdatePropertyRequest` son obligatorios. Si cualquiera se omite, el FluentValidator devuelve `400 Bad Request`. No hay comportamiento tipo PATCH.
- **Rationale**: Confirmado en sesión de clarificación (Q2). Mantiene consistencia con la semántica REST de PUT, simplifica el validator (sin lógica diferencial de `null` vs ausente) y obliga al cliente a enviar el estado completo del recurso.
- **Alternatives considered**:
  - Campos opcionales con conservación de valor previo: rechazado por clarificación Q2.
  - Campos omitidos → null: rechazado por riesgo de pérdida silenciosa de datos.

## Decisión 5: Respuesta exitosa — 200 OK con shape de PropertyResponse

- **Decisión**: El endpoint retorna `200 OK` con el mismo shape de respuesta que `CreatePropertyResponse`: `{ id, title, status, imageUrl }`.
- **Rationale**: Confirmado en clarificación Q3. Reutiliza el tipo de respuesta ya definido, evita divergencia de contratos y permite al cliente renderizar el recurso actualizado sin consulta adicional.
- **Alternatives considered**:
  - `204 No Content`: rechazado por clarificación Q3; el cliente necesita el `imageUrl` resultante para actualizar la vista.
  - Shape de respuesta nuevo con más campos: rechazado por complejidad innecesaria y divergencia de contratos.

## Decisión 6: Atomicidad de la operación con imagen

- **Decisión**: Si el guardado de imagen falla durante la actualización, el bloque `catch` debe ejecutar `TryDelete` del archivo parcialmente escrito (si existe) y retornar fallo de storage sin persistir ningún cambio en la base de datos (`SaveChangesAsync` no se llama ante excepción).
- **Rationale**: Idéntica a Decisión 5 de spec 005. Garantiza FR-015: sin actualización parcial inconsistente.
- **Alternatives considered**:
  - Actualizar datos sin imagen y retornar parcialmente: rechazado por FR-015 y riesgo de incoherencia.

## Decisión 7: Conservación de imageUrl cuando no se envía imagen

- **Decisión**: En el handler, si `command.Image` es `null`, el `imageUrl` a persistir se toma del valor existente en la entidad recuperada de base de datos (`property.ImageUrl`). No se pasa `null` al método de update.
- **Rationale**: Garantiza FR-006 y FR-018: el `imageUrl` previo (sea `null` o URL) se conserva íntegramente.
- **Alternatives considered**:
  - Pasar siempre un valor desde el request: rechazado porque el request no transporta el imageUrl previo.

## Decisión 8: Concurrencia — last-write-wins

- **Decisión**: No se implementa control de concurrencia optimista (sin ETag, sin `rowVersion`). Si dos requests actualizan la misma propiedad simultáneamente, la última escritura que llegue a `SaveChangesAsync` prevalece.
- **Rationale**: Confirmado en clarificación Q4. El caso de uso de catálogo inmobiliario con pocos operadores concurrentes no justifica la complejidad de control optimista en esta spec; queda como mejora futura.
- **Alternatives considered**:
  - ETag o `rowVersion`: rechazado por clarificación Q4 y por implicar un cambio de esquema no aprobado.

## Decisión 9: Endpoint público sin autenticación

- **Decisión**: El endpoint `PUT /api/properties/{id}` no requiere autenticación, consistente con `POST /api/properties` de spec 005.
- **Rationale**: Confirmado en clarificación Q5. Mantiene consistencia con la postura de seguridad actual de las specs aprobadas.
- **Alternatives considered**:
  - Requerir autenticación: rechazado por clarificación Q5; puede añadirse en spec dedicada de autenticación futura.

## Decisión 10: Nombre del campo `status` — string/enum, no `statusId`

- **Decisión**: En el contrato y el código, el campo de estado se llama `status` (string serializado del enum `PropertyStatusValue`), no `statusId`. La spec referenciaba `statusId` en el texto de la clarificación Q1 por error de formulación; en el dominio y codebase el nombre correcto es `status`.
- **Rationale**: `CreatePropertyRequest` ya usa `Status` (string). El dominio usa `PropertyStatusValue` como enum. Mantener `status` asegura consistencia con spec 005 y evita confusión con un campo FK que no existe en el modelo.
- **Alternatives considered**:
  - Introducir campo `statusId` separado: rechazado porque no existe FK de `PropertyStatus` en el modelo de dominio actual.
