# Investigación Técnica: OpenAPI v1 público

## Decisión 1: Generación del documento desde implementación real

- **Decisión**: Generar `v1.json` mediante capacidades OpenAPI del backend al compilar/ejecutar el proyecto (`dotnet build` como paso de pipeline), evitando edición manual del archivo contractual.
- **Rationale**: Mantiene la implementación real como fuente de verdad y elimina drift por mantenimiento manual del documento.
- **Alternatives considered**:
  - Mantener YAML/JSON manual en `contracts` como fuente principal: rechazada por alto riesgo de desalineación.
  - Generar con herramienta externa desconectada del runtime: rechazada por menor fidelidad frente a endpoints reales.

## Decisión 2: Publicación estática y estable sin UI interactiva

- **Decisión**: Publicar el documento en `wwwroot/openapi/v1.json` y exponerlo por `GET /openapi/v1.json`, sin habilitar Swagger UI ni ReDoc UI.
- **Rationale**: Cumple el requisito de contrato consumible y estable, minimizando superficie pública en runtime.
- **Alternatives considered**:
  - Exponer solo endpoint dinámico de OpenAPI sin archivo versionado: rechazada por menor trazabilidad y versionado del artefacto.
  - Habilitar Swagger UI para inspección: rechazada por exclusión explícita de UI interactiva.

## Decisión 3: Validación OpenAPI con Redocly configurado

- **Decisión**: Validar `v1.json` con `npx @redocly/cli lint` usando `.redocly.yaml` obligatorio en raíz, deshabilitando `security-defined` y degradando `operation-4xx-response` a warning.
- **Rationale**: Permite validar APIs públicas sin autenticación sin falsos negativos y mantiene exigencia de calidad contractual.
- **Alternatives considered**:
  - Ejecutar Redocly sin configuración: rechazada por errores esperados incompatibles con el alcance de API pública anónima.
  - Omitir lint automático: rechazada por menor garantía de calidad del contrato.

## Decisión 4: Smoke test de consumibilidad con NSwag

- **Decisión**: Generar un cliente C# tipado de prueba con `dotnet tool run nswag openapi2csclient` hacia `artifacts/openapi-client-smoke/` como validación de consumibilidad.
- **Rationale**: Verifica que un consumidor real puede usar el contrato sin correcciones manuales.
- **Alternatives considered**:
  - Validar solo sintaxis JSON/OpenAPI: rechazada por no cubrir consumibilidad práctica.
  - Usar NSwag en runtime de la aplicación: rechazada por requisito de no ampliar superficie runtime.

## Decisión 5: Estrategia de drift contractual

- **Decisión**: Implementar pruebas xUnit que comparen inventario runtime (`EndpointDataSource`) con inventario documentado en `v1.json`, con normalización de constraints de ruta y filtro a endpoints públicos funcionales (`GET /health` y `/api/properties*`), excluyendo endpoints técnicos como `/openapi/v1.json`.
- **Rationale**: Detecta desalineación funcional real sin ruido por endpoints técnicos no contractuales.
- **Alternatives considered**:
  - Comparar todos los endpoints runtime: rechazada por falsos positivos en rutas técnicas.
  - Validar solo endpoints listados manualmente: rechazada por menor robustez ante evolución del backend.

## Decisión 6: Fallo explícito por prerrequisitos faltantes

- **Decisión**: Tanto el script `generate-openapi-v1.ps1` como las pruebas de drift deben fallar explícitamente cuando falte Node, Redocly o NSwag; no se permite marcar Skip automático.
- **Rationale**: Evita verdes falsos en CI/local y obliga a mantener entorno de validación completo.
- **Alternatives considered**:
  - Skip automático cuando faltan herramientas: rechazada por ocultar defectos de proceso.
  - Ejecutar comparaciones parciales sin CLIs: rechazada por cobertura incompleta del criterio de éxito.
