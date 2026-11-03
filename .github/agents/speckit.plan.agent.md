---
description: "Planificar una spec aprobada y consolidar el diseño en plan.md"
handoffs:
  - label: Crear tareas
    agent: speckit.tasks
    prompt: Desglosar el plan en tareas ejecutables
    send: true
  - label: Crear checklist
    agent: speckit.checklist
    prompt: Crear una lista de verificación para el dominio indicado
---

## Entrada del Usuario

```text
$ARGUMENTS
```

Considere la entrada del usuario antes de continuar, si no está vacía.

## Comprobaciones Previas

1. Revise `.specify/extensions.yml` y procese los hooks `before_plan`: ignore los deshabilitados y no evalúe `condition`. Ejecute los obligatorios mediante `EXECUTE_COMMAND` y espere el resultado; presente los opcionales al usuario.
2. Desde la raíz, ejecute `.specify/scripts/powershell/setup-plan.ps1 -Json` y obtenga `FEATURE_SPEC`, `IMPL_PLAN`, `SPECS_DIR` y `BRANCH`.
3. Lea `FEATURE_SPEC`, `.specify/memory/constitution.md` y la plantilla copiada a `IMPL_PLAN`.
4. Compruebe el campo `**Estado**` de la spec. Si es `Borrador`, presente la revisión y solicite aprobación explícita. Ante aprobación, ejecute `pwsh -ExecutionPolicy Bypass -File support/scripts/spec-status.ps1 -Transition Approve` y confirme `Aprobada` antes de planificar. Si no se aprueba, deténgase. Si ya está `Aprobada`, continúe. Para `En implementación` o `Implementada`, deténgase y explique que la planificación no puede reabrir el ciclo.

## Planificación

1. Extraiga requisitos, historias, criterios de aceptación, restricciones y decisiones del repositorio.
2. Haga preguntas solo si falta una decisión estructural que afecte iniciativas futuras y no esté resuelta en la constitución o la spec.
3. Evalúe los gates constitucionales antes y después del diseño. Deténgase ante una violación no aprobada.
4. Consolide en `plan.md` el contexto técnico, investigación, decisiones, estructura, modelo de datos, contratos o interfaces aplicables y estrategia de validación. No cree `research.md`, `data-model.md`, `contracts/` ni otros documentos de diseño separados.
5. No cree `quickstart.md` durante la planificación. Ese archivo es la única excepción documental permitida y se genera durante `speckit.implement`, con comandos/escenarios realmente ejecutados y resultados satisfactorios.
6. Respete el stack y los límites de `spec.md`; no agregue funcionalidad, dependencias ni proyectos no trazados.

## Actualización del Agente

Mantenga el bloque `SPECKIT START` / `SPECKIT END` de `.github/copilot-instructions.md` apuntando a `IMPL_PLAN`, sin modificar el contenido fuera del bloque.

## Hooks Posteriores

Revise `hooks.after_plan` en `.specify/extensions.yml`. Ignore los deshabilitados y no evalúe `condition`. Ejecute los obligatorios mediante `EXECUTE_COMMAND`; presente los opcionales según su prompt.

## Informe Final

Informe la rama lógica, la ruta de `plan.md`, las decisiones principales, los gates y cualquier pregunta o bloqueo pendiente. Indique que la especificación conserva el estado `Aprobada`; la implementación aún no ha comenzado.
