# Especificación de la Feature: OpenAPI v1 público

**Rama de feature**: `008-open-api`

**Creado**: 2026-07-19

**Estado**: Implementada

**Entrada**: Descripción de usuario: "Generar un único documento OpenAPI versionado (v1) del contrato HTTP público de RealtorApi, publicado como openapi/v1.json y accesible por GET /openapi/v1.json, generado desde la implementación real del backend."

## Ciclo de estado del spec *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementación -> Implementada**
- Está prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Está prohibido cerrar en **Implementada** sin evidencia de validación en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- 2026-07-19 Borrador - Spec creado por `/speckit.specify`
- 2026-07-19 En implementación - Transición automática previa a implementación
- 2026-07-19 Implementada - Cierre automático post-implementación tras validar tareas y evidencia en quickstart

## Clarificaciones

### Sesión 2026-07-19

- Q: ¿Qué endpoints debe comparar la prueba de drift? → A: Comparar solo endpoints públicos funcionales del contrato (`GET /health` y rutas bajo `/api/properties`), excluyendo endpoints técnicos como `/openapi/v1.json`.
- Q: ¿Qué comportamiento deben tener las pruebas de drift si faltan herramientas externas? → A: Deben fallar explícitamente (Failed) con mensaje de prerrequisito faltante; no se permite Skip automático.

## Escenarios de usuario y pruebas *(obligatorio)*

### Historia de usuario 1 - Publicar contrato OpenAPI v1 estable (Prioridad: P1)

Como consumidor del API, necesito acceder siempre al contrato HTTP público en una ruta estable para integrar mi cliente sin depender de inspección manual del backend.

**Por qué esta prioridad**: Sin un contrato publicado y estable, no existe integración confiable para consumidores ni base objetiva para validación de compatibilidad.

**Prueba independiente**: Puede probarse de forma aislada solicitando `GET /openapi/v1.json` y verificando respuesta 200 con JSON válido y cobertura de endpoints en alcance.

**Escenarios de aceptación**:

1. **Given** que el backend está en ejecución, **When** el consumidor solicita `GET /openapi/v1.json`, **Then** recibe 200 OK con `Content-Type` JSON y un documento OpenAPI v1 válido.
2. **Given** que existen endpoints implementados en el backend, **When** se genera el documento OpenAPI desde la implementación real, **Then** el contrato incluye el 100% de los endpoints en alcance sin edición manual.

---

### Historia de usuario 2 - Evitar deriva contrato-implementación (Prioridad: P2)

Como equipo de desarrollo, necesito verificación automatizada de drift para detectar desalineaciones entre los endpoints reales y el documento OpenAPI antes de liberar cambios.

**Por qué esta prioridad**: La deriva silenciosa rompe clientes y genera regresiones contractuales costosas de detectar tarde.

**Prueba independiente**: Puede probarse introduciendo un drift deliberado entre rutas implementadas y documentadas y verificando fallo automático de la suite de verificación.

**Escenarios de aceptación**:

1. **Given** que el inventario real de endpoints cambia, **When** se ejecuta la verificación de drift, **Then** falla si no hay alineación exacta tras normalizar constraints de router.
2. **Given** un documento generado, **When** se valida con Redocly y se ejecuta smoke test de cliente tipado con NSwag, **Then** ambas verificaciones deben completarse sin errores para considerar el contrato aceptable.

---

### Historia de usuario 3 - Mantener app sin UI interactiva de documentación (Prioridad: P3)

Como responsable de superficie pública de la aplicación, necesito que solo se exponga el JSON contractual y no interfaces visuales de exploración para mantener alcance mínimo de runtime.

**Por qué esta prioridad**: Reduce superficie expuesta y evita introducir características no requeridas por el producto.

**Prueba independiente**: Puede probarse verificando que no existan endpoints de Swagger UI/ReDoc UI y que el único artefacto público de documentación sea `GET /openapi/v1.json`.

**Escenarios de aceptación**:

1. **Given** la aplicación desplegada, **When** se revisan rutas de documentación, **Then** no hay UI interactiva disponible y solo existe el endpoint del documento JSON.
2. **Given** el flujo de validación en desarrollo, **When** se usan `@redocly/cli` y NSwag, **Then** esas herramientas operan fuera de runtime y no agregan endpoints de UI a la app.

### Edge Cases

- Si un endpoint existe en runtime pero no en `openapi/v1.json`, la verificación de drift debe fallar.
- Si `openapi/v1.json` existe pero no es JSON válido, la validación debe fallar.
- Si una ruta usa constraints (ejemplo `{id:guid}`) y el documento usa forma canónica (`{id}`), la comparación debe considerarlas equivalentes.
- Si falla la ejecución de herramientas externas (Node, Redocly, NSwag), el proceso de regeneración/validación debe fallar explícitamente con causa visible.
- Si faltan prerrequisitos externos en el entorno de pruebas (Node, Redocly o NSwag), la verificación automatizada de drift debe fallar en estado Failed con mensaje de prerrequisito faltante, sin marcarse como Skip.
- Si aparece accidentalmente un endpoint de UI interactiva, la validación funcional de alcance debe fallar.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **FR-001**: El sistema DEBE generar un único documento OpenAPI versionado de la API pública con versión `v1`.
- **FR-002**: El documento DEBE existir en `app/backend/src/RealtorApi/wwwroot/openapi/v1.json` como artefacto versionado del repositorio.
- **FR-003**: El sistema DEBE exponer el documento mediante `GET /openapi/v1.json` en una ruta HTTP estable.
- **FR-004**: El documento DEBE representar el 100% de endpoints implementados en alcance: `GET /health`, `GET /api/properties`, `GET /api/properties/{id}`, `POST /api/properties`, `PUT /api/properties/{id}`.
- **FR-005**: Para cada endpoint en alcance, el documento DEBE incluir método, ruta, parámetros de ruta y query, request body cuando aplique, y responses de éxito/error.
- **FR-006**: El formato de error DEBE representarse de forma consistente con `ProblemDetails` y `HttpValidationProblemDetails`.
- **FR-007**: Los tipos compartidos `PropertyStatus`, `ProblemDetails` y `HttpValidationProblemDetails` DEBEN definirse en `components/schemas` como componentes reutilizables referenciados por `$ref`, sin duplicación inline.
- **FR-008**: La generación del documento DEBE hacerse a partir de la implementación real del backend (Minimal APIs), y NO por edición manual.
- **FR-009**: Debe existir un script reproducible en `support/scripts/generate-openapi-v1.ps1` que construya, regenere, valide y haga smoke test del contrato en secuencia.
- **FR-010**: El script reproducible DEBE fallar explícitamente cuando el documento no se genere o cuando falle cualquier validación.
- **FR-011**: Debe existir una verificación automatizada de drift (pruebas xUnit) que compare inventario runtime de endpoints públicos funcionales (`GET /health` y rutas bajo `/api/properties`) contra paths documentados, excluyendo endpoints técnicos como `/openapi/v1.json`, y aplicando normalización de constraints.
- **FR-012**: La verificación automatizada DEBE validar el documento con `@redocly/cli lint` e incluir smoke test de cliente tipado con NSwag, y DEBE fallar explícitamente (no Skip) cuando falten prerrequisitos externos.
- **FR-013**: La solución NO DEBE exponer Swagger UI, ReDoc UI ni ninguna interfaz interactiva de exploración de documentación en runtime.
- **FR-014**: Debe existir `.redocly.yaml` en la raíz del repositorio como artefacto obligatorio de la feature para reglas de validación del contrato.
- **FR-015**: La configuración de `.redocly.yaml` DEBE permitir API pública sin autenticación deshabilitando la regla `security-defined` y degradando `operation-4xx-response` a warning.
- **FR-016**: La spec DEBE documentar explícitamente que `@redocly/cli` y NSwag son herramientas CLI offline de desarrollo/validación y no forman parte de la superficie runtime de la aplicación.
- **FR-017**: Esta feature DEBE justificar explícitamente en `plan.md` y `tasks.md` el uso de pruebas xUnit sobre superficie HTTP/contrato (más allá de unit tests puros), por tratarse de verificación contractual de runtime requerida por la spec.

### Requisitos técnicos y herramientas externas

#### Requisitos de sistema

- Node.js 20+ instalado y disponible en PATH.
- Verificación obligatoria mediante `node --version` y `npm --version`.
- `npx` disponible para ejecutar `@redocly/cli`.

#### Herramientas NPM

- `@redocly/cli` como validador OpenAPI, ejecutable vía `npx @redocly/cli --version`.

#### Herramientas .NET locales

- `NSwag.ConsoleCore` registrado en `dotnet-tools.json` del repositorio.
- Restaurable con `dotnet tool restore`.
- Ejecutable con `dotnet tool run nswag`.
- Uso exclusivo en esta feature: generación de cliente C# tipado de smoke test para validar consumibilidad del contrato sin correcciones manuales.

#### Archivo de configuración requerido

- Archivo obligatorio `.redocly.yaml` en la raíz del repositorio.
- Debe deshabilitar `security-defined` para API pública sin autenticación.
- Debe degradar `operation-4xx-response` a warning.
- Sin este archivo, la validación Redocly debe considerarse fallida para esta feature.

#### Verificación previa de herramientas

Checklist previo obligatorio antes de implementación/validación:

- `node --version` (>= 20)
- `npm --version`
- `npx @redocly/cli --version`
- `dotnet tool restore` + `dotnet tool list` (NSwag disponible)

### Entidades clave *(incluir si la feature involucra datos)*

- **Documento OpenAPI v1**: Contrato público versionado que describe de forma completa endpoints, parámetros, cuerpos y respuestas de la API.
- **Inventario de endpoints runtime**: Conjunto de rutas y métodos realmente expuestos por el backend en ejecución.
- **Inventario de paths documentados**: Conjunto de rutas y métodos presentes en `openapi/v1.json` tras normalización de constraints.
- **Esquemas compartidos de contrato**: Definiciones reutilizables (`PropertyStatus`, `ProblemDetails`, `HttpValidationProblemDetails`) referenciadas por `$ref`.
- **Resultado de verificación de drift**: Evidencia automática de alineación o desalineación entre implementación real y contrato publicado.

## Criterios de éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de endpoints implementados en alcance aparece representado en `v1.json`, incluyendo `GET /health`.
- **SC-002**: `GET /openapi/v1.json` responde `200 OK` con `Content-Type` JSON en el 100% de validaciones funcionales de la feature.
- **SC-003**: El documento se valida sin errores con `npx @redocly/cli lint` usando `.redocly.yaml` en el 100% de ejecuciones de validación.
- **SC-004**: La verificación automatizada de drift falla en el 100% de pruebas de drift inducido y pasa en el 100% de escenarios alineados.
- **SC-005**: Un consumidor genera cliente tipado con `dotnet tool run nswag openapi2csclient` sin correcciones manuales del contrato en el 100% de smoke tests.
- **SC-006**: El script `generate-openapi-v1.ps1` regenera y valida el documento sin errores en el 100% de ejecuciones en entorno preparado.
- **SC-007**: El archivo `.redocly.yaml` existe, es válido y contiene reglas efectivas para API pública sin autenticación en el 100% de verificaciones.
- **SC-008**: No existe ninguna UI interactiva de documentación expuesta por la aplicación en el 100% de verificaciones de superficie HTTP.

## Supuestos

- El entorno de desarrollo dispone de Node.js 20+ y puede ejecutar `npx @redocly/cli`.
- El repositorio incluye `dotnet-tools.json` con `NSwag.ConsoleCore` restaurable.
- `.redocly.yaml` debe existir al cierre de la feature; si no existe al inicio, esta feature lo crea y configura.
- La configuración de archivos estáticos vigente permite servir `wwwroot/openapi/v1.json` como `GET /openapi/v1.json`.
- La implementación real del backend es la fuente de verdad y prevalece ante contratos de specs previas en caso de discrepancia.
- El alcance de esta feature se limita a una única versión pública del contrato (`v1`), sin versionado múltiple.
- No se modifica el comportamiento funcional de los endpoints existentes; solo su representación contractual y su validación de consistencia.
