# Investigación Técnica: Detalle de propiedad en frontend Blazor

## Decisión 1: Consumir `GET /api/properties/{id}` extendiendo `IPropertiesApi` con contrato tipado del OpenAPI

- Decision: Agregar en `IPropertiesApi` un método `GetPropertyByIdAsync(string id, CancellationToken)` y modelar `PropertyDetailResponse` en la feature de detalle reutilizando exactamente los campos públicos del contrato OpenAPI vigente.
- Rationale: Mantiene consistencia contract-first, evita duplicaciones inconsistentes y cumple la regla de comunicación frontend-backend vía Refit con DI.
- Alternatives considered:
  - Crear DTO manual desvinculado del OpenAPI: descartado por riesgo de drift de contrato.
  - Consumir con `HttpClient` directo en componente: descartado por restricción arquitectónica.

## Decisión 2: Distinguir explícitamente `404` de errores generales usando `ApiException`

- Decision: Mapear `ApiException` por código de estado; `404` activa estado visual de no encontrado y cualquier otro código (incluyendo red) activa estado de error general.
- Rationale: Cumple requisito funcional de diferenciación semántica entre recurso inexistente y fallo operativo.
- Alternatives considered:
  - Mensaje único para todos los errores: descartado por no cumplir FR-005.
  - Interpretar texto de error en vez de status code: descartado por fragilidad.

## Decisión 3: Mantener cuatro estados de detalle con clases de estado canónicas

- Decision: Implementar `loading`, `success`, `not-found` y `error` con soporte visual basado en clases de estado del sistema (`state-loading`, `state-error`, `state-empty`, `state-success` y/o variantes coherentes).
- Rationale: Reutiliza patrón existente en `app.css` y mantiene experiencia consistente con feature 011.
- Alternatives considered:
  - Render condicional mínimo sin estados diferenciados: descartado por baja claridad UX.
  - Estados manejados con notificaciones flotantes: descartado por baja persistencia visual.

## Decisión 4: Resolver `imageUrl = null` con placeholder visual consistente

- Decision: Cuando no exista imagen, renderizar un placeholder dentro del bloque de media de la propiedad con clase CSS dedicada en `app.css` y un ícono Lucide en `currentColor`.
- Rationale: Evita huecos visuales, mantiene proporción de tarjeta y cumple lineamientos de iconografía.
- Alternatives considered:
  - Dejar área vacía: descartado por inconsistencia visual.
  - Imagen externa hardcodeada: descartado por acoplamiento y dependencia innecesaria.

## Decisión 5: Navegación declarativa desde el listado con ruta canónica `/properties/{id}`

- Decision: El botón "Ver" en `PropertyCard` navegará a `/properties/{id}` con mecanismo de navegación Blazor declarativo y sin lógica de negocio en el componente.
- Rationale: Cumple la clarificación registrada y mantiene componentes de tarjeta enfocados en presentación.
- Alternatives considered:
  - Manejar navegación imperativa compleja en code-behind de tarjeta: descartado por acoplamiento.
  - Usar una ruta alternativa (`/property/{id}`): descartado por decisión de clarificación.

## Decisión 6: Ajuste mínimo de feature 011 para habilitar la navegación

- Decision: Extender `PropertyListItem` para incluir `Id` (ya presente en contrato backend) y usarlo únicamente para construir la navegación del botón "Ver", sin rediseñar ni alterar el resto de datos de la tarjeta.
- Rationale: Cumple restricción de alcance mínimo sobre listado y habilita el requisito de navegación por tarjeta.
- Alternatives considered:
  - Crear endpoint o lookup adicional para resolver id: descartado por complejidad y fuera de alcance.
  - Navegar sin id: descartado por imposibilidad funcional.

## Resultado de investigación

No quedan incógnitas técnicas pendientes (`NEEDS CLARIFICATION = 0`).
La fase de diseño queda lista para derivar `tasks.md`.