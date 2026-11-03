# Aprobar estado de spec

Use este comando solo después de recibir aprobación explícita de la revisión por parte de una persona.

Ejecute `pwsh -ExecutionPolicy Bypass -File support/scripts/spec-status.ps1 -Transition Approve` desde la raíz del repositorio. El script valida que no queden marcadores de clarificación ni casillas incompletas en checklists existentes, y luego cambia `Borrador` a `Aprobada`.

Si la validación falla, no cambie el estado y reporte las causas concretas.
