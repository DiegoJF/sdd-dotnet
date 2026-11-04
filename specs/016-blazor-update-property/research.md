# Investigación Técnica: Edición de inmueble en frontend Blazor

## Decisión 1: Crear una ruta dedicada de edición por inmueble

- Decision: Definir una página enrutable de edición por identificador (`/properties/{id}/edit`) y encapsular la UI/estado en un slice `Features/Properties/Update`.
- Rationale: Mantiene consistencia con el patrón de `Create` y `Detail`, facilita reutilización y evita acoplar lógica de edición al listado.
- Alternatives considered:
  - Modal de edición dentro del listado: descartado por complejidad de estado y peor trazabilidad funcional.
  - Reutilizar página de detalle para editar en línea: descartado por mezclar responsabilidades de lectura/escritura.

## Decisión 2: Reutilizar `IPropertiesApi` con operación tipada `UpdateProperty`

- Decision: Integrar el guardado desde el cliente Refit existente (`IPropertiesApi`) agregando operación `PUT /api/properties/{id}` con body multipart acorde a contrato.
- Rationale: Evita clientes HTTP paralelos, preserva centralización de contratos y sigue el stack obligatorio.
- Alternatives considered:
  - `HttpClient` directo en componentes Razor: descartado por inconsistencia arquitectónica.
  - Cliente API nuevo sólo para update: descartado por duplicación técnica.

## Decisión 3: Cargar datos iniciales con el flujo ya existente de detalle

- Decision: Obtener la información inicial del inmueble con la operación ya disponible de consulta por id, mostrando estado de carga y fallback de error recuperable.
- Rationale: Reduce riesgo funcional y aprovecha contrato ya implementado para poblar formulario.
- Alternatives considered:
  - Carga diferida parcial del formulario: descartado por generar estados intermedios ambiguos.
  - Inicializar formulario vacío y esperar interacción: descartado por incumplir requisito de mostrar datos actuales.

## Decisión 4: Validación en frontend alineada a contrato + mapeo de errores backend

- Decision: Aplicar validación local por campo (incluyendo reglas de negocio vigentes del proyecto) y mapear `400` de backend a mensajes por campo, manteniendo resumen global cuando corresponda.
- Rationale: Cumple requisito de claridad de validación y evita pérdida de contexto al usuario.
- Alternatives considered:
  - Validar sólo en backend: descartado por feedback tardío.
  - Sólo validación local sin mapear `400`: descartado por riesgo de inconsistencia con contrato real.

## Decisión 5: Manejo explícito de respuestas de `UpdateProperty`

- Decision: Manejar explícitamente `200`, `400`, `404`, `413`, `415` y `500` con mensajes comprensibles, preservando datos para reintento en fallos.
- Rationale: Está exigido por la spec y por el contrato OpenAPI del endpoint.
- Alternatives considered:
  - Mensaje global único para todos los errores: descartado por baja capacidad de recuperación.
  - Limpieza de formulario tras error: descartado por degradación de UX.

## Decisión 6: Preservar imagen existente cuando no se seleccione nueva

- Decision: Tratar la imagen como reemplazo opcional: si no hay nueva selección no se modifica imagen actual; si hay nueva selección válida, se envía al endpoint.
- Rationale: Requisito funcional explícito y alineado con esquema `UpdatePropertyRequest` (`image` nullable/opcional).
- Alternatives considered:
  - Forzar imagen en cada actualización: descartado por romper flujo esperado.
  - Limpiar imagen cuando no se envía archivo: descartado por contradicción con la spec.

## Resultado de investigación

No quedan incógnitas materiales (`NEEDS CLARIFICATION = 0`).
La fase de diseño puede concretarse en modelo conceptual, contrato de integración frontend y guía de validación end-to-end.
