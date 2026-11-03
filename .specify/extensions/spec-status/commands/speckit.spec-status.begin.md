# Iniciar implementación

Este comando es obligatorio antes de ejecutar tareas de implementación.

Ejecute `pwsh -ExecutionPolicy Bypass -File support/scripts/spec-status.ps1 -Transition Begin` desde la raíz del repositorio. El script permite únicamente `Aprobada` -> `En implementación`, de forma idempotente.

Si la spec no está aprobada, detenga el flujo y reporte el bloqueo.
