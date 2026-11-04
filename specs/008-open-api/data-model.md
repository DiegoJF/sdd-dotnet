# Modelo de Datos: OpenAPI v1 público

## Entidad principal: OpenApiDocumentV1

Descripción: Documento contractual versionado de la API pública de `RealtorApi`, publicado como JSON estático en `wwwroot/openapi/v1.json` y servido por `GET /openapi/v1.json`.

Campos/segmentos funcionales:

- `openapi` (versión de especificación)
- `info` (metadatos de título y versión de API)
- `paths` (operaciones públicas incluidas en alcance)
- `components/schemas` (tipos compartidos reutilizables)

Reglas:

- Debe incluir el 100% de endpoints públicos funcionales en alcance.
- Debe excluir endpoints técnicos de exploración interactiva (no Swagger UI/ReDoc UI).
- Debe ser generado desde implementación real del backend, no editado manualmente.

## Entidad derivada: RuntimeEndpointInventory

Descripción: Inventario de endpoints expuestos por el backend en runtime obtenido desde `EndpointDataSource`.

Atributos funcionales:

- `method` (GET/POST/PUT)
- `route` (plantilla de ruta)
- `isPublicFunctional` (true para `GET /health` y rutas bajo `/api/properties`)

Reglas:

- Se filtra para comparar solo endpoints públicos funcionales del contrato.
- Rutas técnicas como `/openapi/v1.json` se excluyen de la comparación de drift.

## Entidad derivada: DocumentedPathInventory

Descripción: Inventario de operaciones documentadas en `openapi/v1.json` para comparación de drift.

Atributos funcionales:

- `method`
- `route`
- `schemaRefs` relevantes para operación (cuando aplique)

Reglas:

- Debe normalizar constraints de ASP.NET Core (`{id:guid}` a `{id}`) antes de comparar.
- Debe mantener correspondencia uno-a-uno con el inventario runtime filtrado.

## Entidad de tipos compartidos: SharedSchemas

Descripción: Componentes reutilizables en `components/schemas` del contrato.

Tipos obligatorios:

- `PropertyStatus`
- `ProblemDetails`
- `HttpValidationProblemDetails`

Reglas:

- Deben exponerse por `$ref` desde operaciones aplicables.
- Debe evitarse duplicación inline de estructuras equivalentes.
- `ProblemDetails` y `HttpValidationProblemDetails` deben representar consistentemente errores 4xx/5xx relevantes.

## Entidad de validación: OpenApiValidationResult

Descripción: Resultado de validaciones de contrato en pipeline reproducible y pruebas de drift.

Estados funcionales:

- `BuildGenerated` (documento generado)
- `LintPassed` (Redocly sin errores)
- `SmokeClientGenerated` (NSwag consumió contrato)
- `DriftCheckPassed` (inventarios alineados)

Reglas:

- Si falta cualquier herramienta externa requerida (Node/Redocly/NSwag), el estado final debe ser Failed.
- No se permiten estados Skip automáticos para validaciones obligatorias de esta feature.

## Transiciones funcionales

1. `BuildGenerated`: `dotnet build` produce/actualiza `openapi/v1.json`.
2. `LintPassed`: Redocly valida el documento usando `.redocly.yaml`.
3. `SmokeClientGenerated`: NSwag genera cliente de prueba desde el documento.
4. `DriftCheckPassed`: comparación runtime/documento confirma alineación.
5. Cualquier fallo en pasos 1-4 produce `Failed` con causa explícita.

## Relaciones

- `OpenApiDocumentV1.paths` se compara contra `RuntimeEndpointInventory` (filtrado y normalizado).
- `OpenApiDocumentV1.components/schemas` define `SharedSchemas` reutilizados por operaciones.
- `OpenApiValidationResult` depende de ejecución secuencial de build, lint, smoke test y drift check.
