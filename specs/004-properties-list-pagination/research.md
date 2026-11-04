# Investigación Técnica: Listado Paginado de Propiedades

## Decisión 1: Ubicación del slice

- Decisión: Crear el caso de uso en `app/backend/src/RealtorApi/Features/Properties/ListProperties` con un slice `ISlice` dedicado y componentes adyacentes para handler, mapeo y validación si hacen falta.
- Rationale: Respeta la arquitectura Vertical Slice del backend, mantiene el endpoint delgado y agrupa toda la lógica del caso de uso en un solo lugar.
- Alternatives considered:
  - Registrar la ruta directamente en `Program.cs`: rechazado porque rompe el patrón de auto-descubrimiento.
  - Crear una carpeta técnica global de consultas: rechazado por la gobernanza de slices.

## Decisión 2: Estrategia de acceso a datos

- Decisión: Consultar `AppDbContext.Properties` con `AsNoTracking()`, ordenar por `Title` ascendente y aplicar `CountAsync` + `Skip` + `Take` en servidor.
- Rationale: Evita cargar entidades completas innecesariamente y permite calcular metadatos de paginación sin trabajo adicional en memoria.
- Alternatives considered:
  - Materializar todas las propiedades y paginar en memoria: rechazado por costo innecesario y peor escalabilidad.
  - Usar `Include` o navegación extra no requerida: rechazado porque el endpoint solo necesita lectura simple.

## Decisión 3: Validación de parámetros

- Decisión: Validar `page` y `pageSize` con FluentValidation y devolver `400` con `ValidationProblemDetails` cuando alguno sea menor o igual que cero.
- Rationale: Cumple el contrato de error esperado, mantiene la validación consistente con el stack del proyecto y evita mezclar la validación con el mapeo de datos.
- Alternatives considered:
  - Validar solo con `if` manual en el handler: rechazado porque dispersa la regla y dificulta reutilización.
  - Lanzar excepciones de dominio para entrada inválida: rechazado por ser más costoso que una validación explícita.

## Decisión 4: Construcción de `imageUrl`

- Decisión: Tratar el valor persistido de `Property.ImageUrl` como referencia del archivo y convertirlo en URL absoluta en el mapeo de respuesta usando `Request.Scheme` y `Request.Host`.
- Rationale: Mantiene compatibilidad con el modelo persistente actual, evita exponer rutas internas y cumple el formato público requerido.
- Alternatives considered:
  - Persistir URLs absolutas en la base de datos: rechazado por acoplar la persistencia al host de despliegue.
  - Devolver rutas relativas: rechazado por la spec.

## Decisión 5: Contrato público de respuesta

- Decisión: Exponer una envoltura de respuesta con `items` y metadatos `page`, `pageSize`, `totalItems`, `totalPages`, `hasNext` y `hasPrevious`.
- Rationale: El contrato queda explícito, fácil de consumir y suficientemente estable para pruebas e integración.
- Alternatives considered:
  - Devolver solo un arreglo: rechazado porque la spec exige metadatos de paginación.
  - Usar nombres internos del dominio como contrato público: rechazado por acoplamiento innecesario.

## Decisión 6: Manejo de consultas vacías

- Decisión: Si la página solicitada no tiene elementos, devolver `200` con `items` vacío y metadatos consistentes.
- Rationale: Mantiene el contrato de paginación y evita tratar una página vacía como error funcional.
- Alternatives considered:
  - Responder `404`: rechazado porque la consulta sigue siendo válida.
  - Responder `204`: rechazado porque eliminaría los metadatos requeridos.

## Decisión 7: Contrato y pruebas

- Decisión: Documentar el endpoint en OpenAPI y validar manualmente con `RealtorApi.http`, además de pruebas de integración del backend.
- Rationale: Asegura trazabilidad entre spec, contrato y verificación ejecutable.
- Alternatives considered:
  - Omitir contrato formal: rechazado porque el endpoint es público.
  - Confiar solo en pruebas unitarias: rechazado porque el binding HTTP y la construcción de URL requieren verificación de extremo a extremo.