# Investigación Técnica: Actualización de estado de propiedades

## Decisión 1: Reutilizar el enum de estado existente

- **Decisión**: Usar `PropertyStatusValue` del dominio actual como fuente de verdad para los valores válidos del campo `status`.
- **Rationale**: La solución ya tiene un catálogo de estados persistido y un enum del dominio; introducir otro tipo o tabla duplicaría reglas sin beneficio.
- **Alternatives considered**:
  - Crear un nuevo catálogo o entidad para esta spec: rechazado por duplicación innecesaria.
  - Convertir `status` en un string libre: rechazado porque perdería validación de dominio.

## Decisión 2: Buscar la propiedad con `FindAsync`

- **Decisión**: Resolver la propiedad por `id` con `FindAsync` antes de cualquier mutación.
- **Rationale**: Es la ruta más directa para una clave primaria y permite devolver `404 Not Found` sin tocar persistencia cuando el recurso no existe.
- **Alternatives considered**:
  - `FirstOrDefaultAsync`: funcionalmente válida, pero más indirecta para una clave primaria.
  - Asumir existencia y capturar excepción: rechazado por semántica incorrecta.

## Decisión 3: Contrato mínimo de request y response

- **Decisión**: El request expone solo `status`; la response devuelve la propiedad actualizada con el mismo shape funcional usado por el catálogo, conservando `imageUrl`.
- **Rationale**: El alcance pedido es estrictamente de cambio de estado. Incluir más campos o imagen rompería la intención de la feature.
- **Alternatives considered**:
  - Reusar el contrato completo de actualización con imagen: rechazado por exceder alcance.
  - Retornar solo `204 No Content`: rechazado porque el usuario pide la entidad actualizada.

## Decisión 4: Semántica de errores funcionales

- **Decisión**: `400 Bad Request` para `status` omitido o inválido; `404 Not Found` cuando el `id` no existe.
- **Rationale**: Mantiene consistencia con ProblemDetails y con la semántica funcional requerida.
- **Alternatives considered**:
  - `422 Unprocessable Entity`: rechazado por no ser el código esperado por la spec.
  - `200` con error embebido: rechazado por romper semántica HTTP.

## Decisión 5: Validación manual desde `.http`

- **Decisión**: Documentar ejemplos de PATCH en `RealtorApi.http` para éxito y errores principales.
- **Rationale**: La spec exige evidencia manual simple y verificable dentro del proyecto.
- **Alternatives considered**:
  - Documentar solo pruebas automatizadas: rechazado porque la spec pide ejemplo manual explícito.