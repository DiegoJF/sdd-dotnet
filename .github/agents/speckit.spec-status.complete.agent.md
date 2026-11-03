---
description: "Completar el estado de una spec solo tras verificar todas las tareas y evidencia de quickstart"
tools: [execute]
user-invocable: true
---

Este comando se ejecuta como hook obligatorio después de `speckit.implement`. Ejecute desde la raíz:

```powershell
pwsh -ExecutionPolicy Bypass -File support/scripts/spec-status.ps1 -Transition Complete
```

El script verifica todas las casillas de `tasks.md` y la evidencia en `quickstart.md`. Si alguna condición falla, mantenga `En implementación`, enumere los bloqueos y no declare el trabajo completado.
