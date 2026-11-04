# Modelo de Datos Conceptual: Fundación Backend Base

## Propósito

Definir los objetos conceptuales de infraestructura requeridos por la iniciativa 002. Este modelo no representa persistencia ni entidades de dominio de negocio.

## Entidades conceptuales

### 1. SliceDefinition

Representa una unidad vertical que declara endpoints Minimal API.

Campos:
- `TypeName` (string, requerido): nombre del tipo que implementa `ISlice`.
- `AssemblyName` (string, requerido): ensamblado fuente para descubrimiento.
- `IsMapped` (bool, requerido): indica si sus endpoints ya fueron mapeados.

Reglas de validación:
- Debe implementar contrato `ISlice`.
- No se admiten duplicados por `TypeName + AssemblyName`.

Relaciones:
- 1:N con `EndpointDefinition`.

Transiciones de estado:
- `Descubierta` -> `Registrada` -> `Mapeada`.

### 2. EndpointDefinition

Representa un endpoint declarado por una slice.

Campos:
- `Route` (string, requerido): ruta HTTP (ejemplo: `/health`).
- `HttpMethod` (string, requerido): método HTTP (`GET`, `POST`, etc.).
- `EndpointName` (string, opcional): nombre lógico del endpoint.
- `RequiresValidator` (bool, requerido): indica si existe `IValidator<T>` aplicable.

Reglas de validación:
- `Route` debe iniciar con `/`.
- `HttpMethod` debe pertenecer al conjunto permitido por ASP.NET Core Minimal APIs.

Relaciones:
- N:1 con `SliceDefinition`.
- 0:1 con `ValidationBinding`.

### 3. HandlerRegistration

Representa un handler autodetectado por `IHandler`.

Campos:
- `HandlerTypeName` (string, requerido)
- `AssemblyName` (string, requerido)
- `ServiceLifetime` (string, requerido; valor esperado: `Scoped` o convención definida)

Reglas de validación:
- Debe implementar `IHandler`.
- No se admiten duplicados por `HandlerTypeName + AssemblyName`.

Relaciones:
- Asociada a `SliceDefinition` de forma indirecta por caso de uso futuro (sin handlers concretos en esta spec).

### 4. ValidationBinding

Representa la resolución de un `IValidator<T>` para un request.

Campos:
- `RequestTypeName` (string, requerido)
- `ValidatorTypeName` (string, opcional)
- `Mode` (string, requerido): `Validated` o `PassThrough`.

Reglas de validación:
- Si `ValidatorTypeName` es nulo, `Mode` debe ser `PassThrough`.
- Si existe validator, `Mode` debe ser `Validated`.

Relaciones:
- 1:1 opcional con `EndpointDefinition` para requests con body/DTO validable.

Transiciones de estado:
- `Resolved` -> (`Validated` | `PassThrough`).

### 5. ExpectedErrorResult

Representa un error esperado de aplicación listo para mapearse a `ProblemDetails`.

Campos:
- `Code` (string, requerido)
- `Title` (string, requerido)
- `Detail` (string, opcional)
- `Status` (int, requerido)
- `Extensions` (diccionario, opcional)

Reglas de validación:
- `Status` debe corresponder a un código HTTP válido de error (`4xx`/`5xx`).
- `Code` y `Title` no pueden ser vacíos.

Relaciones:
- 1:1 con `ProblemDetailsPayload`.

### 6. ProblemDetailsPayload

Representa el payload HTTP estandarizado de error.

Campos:
- `Type` (string, opcional)
- `Title` (string, requerido)
- `Status` (int, requerido)
- `Detail` (string, opcional)
- `Instance` (string, opcional)
- `Errors` (diccionario string->array string, opcional; solo para validación)

Reglas de validación:
- Para validación fallida, debe existir `Errors` y `Status = 400`.
- Para error esperado no validación, `Errors` es opcional.

Relaciones:
- Puede originarse desde `ExpectedErrorResult` o desde `ValidationBinding`.

### 7. HealthProbe

Representa la sonda técnica `/health` migrada a slice.

Campos:
- `Route` (string, requerido; valor esperado `/health`)
- `StatusValue` (string, requerido; valor esperado `ok` cuando está saludable)
- `InternalOnly` (bool, requerido; valor esperado `true`)

Reglas de validación:
- Debe mapearse vía `ISlice`.
- No requiere autenticación en aplicación.
- Debe aplicarse restricción de origen interno en capa de infraestructura externa (ingress/reverse proxy).

Relaciones:
- 1:1 con `EndpointDefinition`.

## Notas de alcance

- Este modelo es conceptual y de contrato interno.
- No define tablas, migraciones, DbContext ni persistencia.
- No incluye entidades de dominio de negocio.