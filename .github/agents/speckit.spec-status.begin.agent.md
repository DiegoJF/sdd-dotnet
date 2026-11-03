---
description: "Transicionar automáticamente una spec aprobada a En implementación antes de ejecutar tareas"
tools: [execute]
user-invocable: true
---

Este comando se ejecuta como hook obligatorio antes de `speckit.implement`. Ejecute desde la raíz:

```powershell
pwsh -ExecutionPolicy Bypass -File support/scripts/spec-status.ps1 -Transition Begin
```

Si la transición falla, detenga la implementación y reporte la causa. No cambie el estado manualmente.
