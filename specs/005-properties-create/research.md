# Investigación Técnica: Alta de propiedades con imagen opcional

## Decisión 1: Ubicación del slice y estructura vertical

- Decisión: Implementar el caso de uso en `app/backend/src/RealtorApi/Features/Properties/CreateProperty` con archivos separados para slice, request/response, handler, mapping y validator.
- Rationale: Mantiene cohesión por caso de uso y respeta la arquitectura Vertical Slice aprobada en specs previas.
- Alternatives considered:
  - Registrar endpoint directo en `Program.cs`: rechazado por romper auto-registro por `ISlice`.
  - Centralizar lógica en carpetas técnicas transversales: rechazado por degradar trazabilidad del caso de uso.

## Decisión 2: Contrato de entrada para datos + archivo opcional

- Decisión: Exponer una operación pública de creación que acepte datos de propiedad y archivo de imagen opcional en la misma solicitud HTTP.
- Rationale: Cubre los flujos P1 y P2 con un único caso de uso, simplificando integración para clientes.
- Alternatives considered:
  - Endpoint separado para upload posterior: rechazado porque la spec exige alta con imagen opcional en la misma operación.
  - Requerir imagen siempre: rechazado por incumplir FR-002 y P1.

## Decisión 3: Validación funcional del archivo

- Decisión: Validar formato y tamaño antes de persistir, aceptando únicamente PNG/JPG y hasta 5 MB; tratar archivo vacío (0 bytes) y contenido real inválido como formato no soportado.
- Rationale: Cumple reglas funcionales explícitas (FR-005, FR-006) y evita persistencia parcial por insumos inválidos.
- Alternatives considered:
  - Validar solo extensión: rechazado porque no garantiza integridad del contenido.
  - Omitir límite de tamaño: rechazado por incumplir requerimiento explícito.

## Decisión 4: Semántica de errores HTTP

- Decisión: Responder `400` para validaciones de campos de negocio, `415` para formato inválido/0 bytes/contenido real inválido, `413` para tamaño excedido y `500` para fallo interno de guardado.
- Rationale: Alinea semántica HTTP con el tipo de error y facilita manejo determinístico por clientes.
- Alternatives considered:
  - Responder `400` para todos los errores de imagen: rechazado por menor precisión semántica.

## Decisión 5: Atomicidad de la operación

- Decisión: La alta con imagen es atómica; si falla persistencia del archivo, no se crea la propiedad.
- Rationale: Evita inconsistencias entre base de datos y almacenamiento de archivos.
- Alternatives considered:
  - Crear propiedad sin imagen ante fallo de guardado: rechazado por decisión de clarificación y riesgo de estados inconsistentes.

## Decisión 6: Estrategia de nombre de archivo

- Decisión: Generar nombre interno único (`UUID + extensión válida`) y no depender del nombre original enviado.
- Rationale: Elimina colisiones, reduce riesgo por nombres controlados por cliente y estandariza rutas públicas.
- Alternatives considered:
  - Mantener nombre original: rechazado por colisiones y riesgos de sanitización.
  - Sufijo incremental: rechazado por mayor complejidad y menor robustez en concurrencia.

## Decisión 7: Persistencia de URL de imagen

- Decisión: Persistir en la propiedad una URL pública/relativa consumible por clientes y compatible con el flujo de listado existente.
- Rationale: Respeta FR-008 y continuidad con la spec 004, donde clientes consumen `imageUrl` en respuestas.
- Alternatives considered:
  - Persistir ruta física del servidor: rechazado por acoplamiento de infraestructura.

## Decisión 8: Contrato y validación de extremo a extremo

- Decisión: Documentar el endpoint en OpenAPI y validar escenarios de éxito/error mediante `RealtorApi.http` y pruebas de backend.
- Rationale: Asegura trazabilidad spec -> contrato -> validación ejecutable para fase de implementación.
- Alternatives considered:
  - Omitir contrato formal: rechazado porque el endpoint es público y requiere integración estable.

## Decisión 9: Medición verificable de rendimiento

- Decisión: Medir SC-004 con una muestra de 100 solicitudes válidas mixtas (con y sin imagen) y exigir p95 < 3 segundos.
- Rationale: Convierte el criterio de éxito en evidencia repetible y auditable sin depender de observaciones ad hoc.
- Alternatives considered:
  - Medición manual sin tamaño de muestra: rechazada por baja reproducibilidad.
  - Medición solo sin imagen: rechazada por no representar el comportamiento completo del caso de uso.
