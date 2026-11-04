# Investigación Técnica: Fundación Backend Base

## Decisión 1: Descubrimiento de endpoints por contrato `ISlice` + reflection por ensamblado

- Decisión: Adoptar un contrato `ISlice` y registrar/mapear endpoints mediante scanning de ensamblado (`RegisterSlices` + `MapSliceEndpoints`) sin registros manuales por clase.
- Rationale: Cumple RF-004, RF-005 y RF-006, evita acoplamiento de `Program.cs` con cada endpoint nuevo y habilita escalabilidad por vertical slices.
- Alternatives considered:
  - Registro manual por endpoint en `Program.cs`: rechazado por alto riesgo de regresiones y violación de RF-006.
  - Registro por convención de nombre de clase: rechazado por fragilidad y menor trazabilidad que un contrato explícito.

## Decisión 2: Auto-registro de handlers por marker interface `IHandler`

- Decisión: Resolver registro de handlers con assembly scanning basado en `IHandler`, sin wiring individual.
- Rationale: Cumple RF-007 y prepara el backend para casos de uso futuros sin introducir handlers concretos en esta iniciativa.
- Alternatives considered:
  - Registro explícito por handler en `Program.cs`: rechazado por mantenimiento costoso y fricción en crecimiento.
  - Uso de librería externa de mediación completa: rechazado en foundation por agregar complejidad no requerida por la spec.

## Decisión 3: Validación automática de Minimal APIs con FluentValidation y filtro por convención

- Decisión: Implementar `ValidationFilterFactory` que detecte `IValidator<T>` de forma automática; si no hay validator, pass-through.
- Rationale: Cumple RF-008, RF-009, RF-010 y RF-011, estandariza respuestas de validación y reduce duplicación por endpoint.
- Alternatives considered:
  - Validación manual en cada endpoint: rechazada por repetición y riesgo de inconsistencias.
  - Fallar cuando no existe validator: rechazado por contradecir explícitamente el comportamiento pass-through requerido.

## Decisión 4: Error handling esperado con `Result` + mapeo central a `ProblemDetails`

- Decisión: Definir capacidad transversal para mapear errores esperados modelados como `Result` hacia `ProblemDetails` con status code coherente.
- Rationale: Cumple RF-012 y favorece uniformidad de contrato HTTP para futuras features.
- Alternatives considered:
  - Devolver errores ad-hoc por endpoint: rechazado por inconsistencia y baja observabilidad.
  - Manejo solo por excepciones: rechazado para errores esperados por menor expresividad semántica.

## Decisión 5: Logging estructurado con `ILogger`

- Decisión: Establecer logging estructurado en infraestructura transversal y ruta de errores esperados.
- Rationale: Cumple RF-013 y permite diagnosticar validaciones, mapeos y salud operativa sin introducir lógica de negocio.
- Alternatives considered:
  - Logging mínimo no estructurado: rechazado por menor capacidad de diagnóstico.
  - Framework de logging adicional: rechazado por ser innecesario en foundation.

## Decisión 6: Política de exposición de `/health`

- Decisión: `/health` permanece sin autenticación de aplicación y su acceso se restringe a red/origen interno mediante ingress o reverse proxy.
- Rationale: Alinea la aclaración aceptada en la sesión de clarify, mantiene utilidad operativa y minimiza superficie pública.
- Alternatives considered:
  - Exposición pública sin restricciones: rechazada por riesgo de enumeración y telemetría externa.
  - Requerir autenticación en aplicación para `/health`: rechazada por complejidad operativa innecesaria para sonda técnica.

## Decisión 7: Alcance de pruebas

- Decisión: Usar pruebas unitarias/integración ligera en `app/backend/tests/RealtorApiTests` para validar infraestructura transversal y endpoint `/health` migrado.
- Rationale: Cumple RF-016 y el principio constitucional que limita pruebas a unit tests salvo justificación explícita.
- Alternatives considered:
  - End-to-end completo con infraestructura externa: rechazado por exceder alcance foundation.
  - Sin pruebas de infraestructura: rechazado por incumplimiento de criterios de éxito CE-003.

## Resultado de investigación

No quedan incógnitas técnicas pendientes para esta iniciativa. La fase de diseño puede ejecutarse sin marcadores de aclaración.