# Investigación - Fundación de la Solución Realtor

## Decisión 1: Baseline de plataforma

- Decisión: Usar .NET SDK `11.0.100-preview.5.26302.115` como baseline de la iniciativa, derivado de `global.json` sin modificaciones.
- Rationale: RF-010 exige que la versión se derive del archivo existente y prohíbe su cambio en foundation.
- Alternatives considered:
  - Instalar/usar otra versión de SDK local: descartado por romper trazabilidad de baseline.
  - Modificar `global.json`: descartado por incumplimiento explícito de RF-010.

## Decisión 2: Arranque técnico mínimo del backend

- Decisión: Limitar el arranque del backend a registro de servicios base de plataforma, middleware esencial y mapeo de endpoint técnico mínimo (sin lógica de negocio).
- Rationale: RF-006, RF-007, RF-008 y RF-009 acotan el alcance a foundation sin features funcionales.
- Alternatives considered:
  - Incluir casos de uso o handlers de dominio desde foundation: descartado por violar el alcance no funcional.
  - Crear controllers para acelerar bootstrap: descartado por constitución (Minimal APIs, sin controllers).

## Decisión 3: Contrato externo en fase foundation

- Decisión: Definir únicamente un contrato HTTP técnico de disponibilidad (`GET /health`) para validación de arranque.
- Rationale: Permite prueba end-to-end mínima de plataforma sin introducir negocio y habilita validación objetiva de wiring inicial.
- Alternatives considered:
  - No publicar ningún contrato en esta fase: descartado porque reduce verificabilidad externa de arranque.
  - Exponer endpoints de negocio preliminares: descartado por fuera de alcance foundation.

## Decisión 4: Manejo de rutas/proyectos preexistentes

- Decisión: En implementación posterior, crear artefactos de forma idempotente y no destructiva, preservando contenido existente cuando coincida con nombres/rutas objetivo.
- Rationale: Los casos límite de la spec exigen evitar sobreescritura destructiva y mantener consistencia.
- Alternatives considered:
  - Re-generar toda la estructura siempre: descartado por riesgo de pérdida de cambios válidos.
  - Renombrar rutas para evitar conflictos: descartado por romper canon de RF-001..RF-005.

## Verificación de alcance sin negocio (ejecutada)

- Backend validado con endpoint técnico único `GET /health`.
- Se eliminaron artefactos de ejemplo del template (`/weatherforecast`, páginas demo `Counter` y `Weather`).
- No se introdujeron entidades de dominio ni casos de uso funcionales de producto.

## Evidencia de idempotencia aplicada

- La creación de carpetas y solución se ejecutó con comprobaciones previas de existencia.
- Se conservó la ruta canónica `app/Realtor.sln` y se evitó sobreescritura destructiva.
- Se ajustó el nombre del proyecto de pruebas frontend a `RealtorWebTests.csproj` para evitar colisión de nombres en la solución, manteniendo la ruta canónica de carpeta.
