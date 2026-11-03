# Cerrar implementación

Este comando es obligatorio al terminar las tareas de implementación.

Ejecute `pwsh -ExecutionPolicy Bypass -File support/scripts/spec-status.ps1 -Transition Complete` desde la raíz del repositorio. El script solo permite `En implementación` -> `Implementada` si todas las tareas están completas y `quickstart.md` contiene comandos ejecutados con resultados satisfactorios.

Si alguna condición falla, mantenga `En implementación` y reporte cada bloqueo concreto.
