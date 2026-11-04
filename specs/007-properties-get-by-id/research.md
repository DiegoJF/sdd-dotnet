# Investigación Técnica: Consulta de propiedad por id

## Decisión 1: Reutilizar el contrato de campos públicos de spec 004

- **Decisión**: La respuesta `200` de `GET /api/properties/{id}` expondrá exactamente los mismos campos públicos definidos en el item de listado de spec 004: `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters`, `imageUrl`.
- **Rationale**: Evita divergencia entre vista de listado y vista unitaria, simplifica consumo frontend y reduce mapeos duplicados.
- **Alternatives considered**:
  - Respuesta reducida (solo `id`, `title`, `status`): rechazada por pérdida de información y necesidad de consultas adicionales.
  - Respuesta expandida con metadatos extra: rechazada por salir del alcance y romper consistencia con 004.

## Decisión 2: Construcción de imageUrl absoluto por request

- **Decisión**: Si la propiedad tiene imagen (`ImageUrl` relativo persistido), devolver `imageUrl` absoluto y público usando esquema/host actuales: `http(s)://{host}/assets/properties/{fileName}`.
- **Rationale**: Cumple el requerimiento de URL consumible externamente y soporta variación entre entornos (localhost, staging, producción).
- **Alternatives considered**:
  - Devolver ruta relativa persistida: rechazada por incumplir requisito de URL absoluta pública.
  - Persistir URL absoluta en base de datos: rechazada por acoplar datos a un host concreto.

## Decisión 3: Semántica de error para id inválido e inexistente

- **Decisión**: Id con formato inválido (no UUID) devuelve `400 Bad Request` con ProblemDetails; id válido pero no encontrado devuelve `404 Not Found` con ProblemDetails.
- **Rationale**: Distingue correctamente error de entrada (cliente) frente a ausencia de recurso, mejora trazabilidad y pruebas.
- **Alternatives considered**:
  - Usar `404` también para id inválido: rechazada por ocultar error de validación de entrada.
  - Usar `422` para id inválido: rechazada por menor consistencia con el resto de endpoints actuales.

## Decisión 4: Consulta por clave primaria con cancelación

- **Decisión**: Resolver la entidad con `FindAsync(id, cancellationToken)` antes del mapeo de respuesta.
- **Rationale**: Es el patrón eficiente para PK en EF Core y mantiene soporte explícito de cancelación.
- **Alternatives considered**:
  - `FirstOrDefaultAsync` por expresión: funcional, pero menos directo para PK.
  - Cargar por proyección sin validar existencia primero: rechazada por complejidad innecesaria.

## Decisión 5: Nullabilidad explícita de imageUrl

- **Decisión**: Si la propiedad no tiene imagen asociada, `imageUrl` se devuelve en `null`.
- **Rationale**: Evita valores sintéticos o rutas no válidas; el cliente puede representar estado sin imagen de forma explícita.
- **Alternatives considered**:
  - Devolver string vacío: rechazada por ambigüedad semántica.
  - Devolver imagen placeholder fija: rechazada por ser decisión de presentación, no de API.

## Decisión 6: Contrato OpenAPI y pruebas mínimas

- **Decisión**: Publicar contrato en `contracts/backend-properties-get-by-id.openapi.yaml` con respuestas `200`, `400` y `404`; cubrir en quickstart escenarios de éxito, id inválido, id inexistente, propiedad sin imagen y host/esquema variable.
- **Rationale**: Hace verificable el comportamiento de la spec y prepara la fase de tareas/implementación con criterios observables.
- **Alternatives considered**:
  - Posponer contrato a la fase de implementación: rechazada por romper enfoque spec-driven.
