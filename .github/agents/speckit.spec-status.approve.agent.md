---
description: "Registrar la aprobación explícita de una spec revisada y transicionar Borrador a Aprobada"
tools: [execute]
user-invocable: true
---

Use este comando solo después de que una persona haya aprobado explícitamente la revisión de la spec activa. No apruebe por inferencia ni modifique el archivo por cuenta propia.

Ejecute desde la raíz:

```powershell
pwsh -ExecutionPolicy Bypass -File support/scripts/spec-status.ps1 -Transition Approve
```

Informe el estado resultante. Si el script bloquea la transición por marcadores de clarificación o checklists pendientes, conserve el estado actual y reporte cada causa.
