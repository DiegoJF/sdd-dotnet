# Investigación Técnica: Alta de inmuebles en frontend Blazor

## Decisión 1: Implementar la página de alta como ruta dedicada en frontend y encapsular la experiencia en un slice propio

- Decision: Definir una ruta de alta en `Components/Pages` y encapsular la UI/estado en `Features/Properties/Create`.
- Rationale: Mantiene consistencia con el patrón de features del frontend actual, separa navegación de lógica de formulario y evita acoplamiento con el listado o detalle.
- Alternatives considered:
  - Reutilizar la página principal con un modal de alta: descartado por baja trazabilidad funcional y complejidad de estados.
  - Ubicar toda la lógica en la página enrutable sin slice: descartado por escalabilidad y mantenibilidad.

## Decisión 2: Consumir `CreateProperty` exclusivamente por Refit en `IPropertiesApi` registrado por DI

- Decision: Usar el cliente tipado existente y agregar/usar operación `CreateProperty` con `multipart/form-data`, sin `HttpClient` directo en componentes.
- Rationale: Cumple constitución (stack y arquitectura), reduce riesgo de divergencia de contrato y mantiene centralizada la integración backend.
- Alternatives considered:
  - Llamar API con `HttpClient` directo en Razor: descartado por violar lineamientos del proyecto.
  - Crear cliente paralelo ad hoc para alta: descartado por duplicación técnica.

## Decisión 3: Validaciones de frontend equivalentes a backend con feedback inmediato

- Decision: Aplicar validación por campo en tiempo real y resumen consolidado al enviar, alineado con reglas funcionales de backend.
- Rationale: Reduce errores evitables, mejora completitud del formulario y cumple decisión confirmada en discovery.
- Alternatives considered:
  - Validar solo al enviar: descartado por peor experiencia y menor tasa de éxito al primer intento.
  - Validación parcial en frontend y total en backend: descartado por inconsistencia percibida.

## Decisión 4: Imagen opcional con prevalidación local, vista previa y acciones de reemplazo/eliminación

- Decision: Tratar la imagen como opcional, validando tipo/tamaño al seleccionar, mostrando vista previa y permitiendo quitar/reemplazar antes de enviar.
- Rationale: Evita rechazos tardíos y respeta el requisito de opcionalidad sin bloquear flujo principal.
- Alternatives considered:
  - No mostrar vista previa: descartado por baja confirmación visual.
  - Validar imagen solo tras respuesta backend: descartado por feedback tardío.

## Decisión 5: Manejo explícito de errores por familia de respuesta

- Decision: Para `400`, mapear errores a campos y conservar valores. Para `413`, `415` y `500`, mostrar mensaje global y conservar datos para reintento.
- Rationale: Hace recuperable el flujo y evita pérdida de trabajo de la persona usuaria.
- Alternatives considered:
  - Mensaje global para todos los errores: descartado por baja precisión.
  - Limpiar formulario en error: descartado por mala experiencia.

## Decisión 6: Cierre exitoso con redirección automática al detalle del inmueble creado

- Decision: Tras `201 Created`, mostrar confirmación y navegar a la vista de detalle usando el identificador retornado.
- Rationale: Proporciona evidencia inmediata de alta exitosa y continuidad de flujo con la spec 012.
- Alternatives considered:
  - Permanecer en formulario con mensaje: descartado por menor verificabilidad del resultado.
  - Redirigir al listado: descartado por menor precisión de validación del recurso creado.

## Resultado de investigación

No quedan incógnitas materiales (`NEEDS CLARIFICATION = 0`).
La fase de diseño puede concretarse en modelo conceptual, contrato funcional y guía de validación ejecutable.