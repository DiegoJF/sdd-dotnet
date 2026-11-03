---
description: "Ejecutar tareas de implementación siguiendo la spec, el plan y el ciclo obligatorio de estados"
---

## Entrada del Usuario

```text
$ARGUMENTS
```

Considere la entrada del usuario antes de continuar, si no está vacía.

## Comprobaciones Previas

### Hooks previos

Revise `.specify/extensions.yml` y las entradas `hooks.before_implement`. Ignore hooks con `enabled: false`; no evalúe expresiones `condition`. Si un hook obligatorio está habilitado, emita `EXECUTE_COMMAND: {command}` y espere a que finalice antes de seguir. Los hooks opcionales se presentan con su comando y prompt.

### Prerrequisitos de la iniciativa

1. Desde la raíz, ejecute `.specify/scripts/powershell/check-prerequisites.ps1 -Json -RequireTasks -IncludeTasks` y obtenga `FEATURE_DIR` y `AVAILABLE_DOCS`.
2. Si existe `FEATURE_DIR/checklists/`, cuente sus casillas fuera de bloques de código. Si alguna está incompleta, muestre el resumen y solicite autorización antes de seguir. Deténgase ante una respuesta negativa.
3. Lea `spec.md`, `plan.md`, `tasks.md` y `.specify/memory/constitution.md`. Lea también los documentos opcionales que existan.
4. Compruebe `git rev-parse --git-dir` y verifique los ignores de las tecnologías realmente presentes. No cree ignores para tecnologías ausentes y no sobrescriba archivos existentes.
5. Confirme que `spec.md` contiene exactamente un campo `**Estado**` y que su valor es `Aprobada` o `En implementación`. Un estado `Aprobada` se cambia a `En implementación` al comienzo mediante `pwsh -ExecutionPolicy Bypass -File support/scripts/spec-status.ps1 -Transition Begin`. Si el hook previo ya realizó el cambio, la operación es idempotente. Ante otro estado, deténgase e indique el bloqueo; no edite el estado manualmente.

## Ejecución

1. Analice `tasks.md`: fases, identificadores, dependencias, rutas y marcadores `[P]`.
2. Complete tareas en orden de dependencia. Las tareas `[P]` solo pueden ejecutarse en paralelo si no comparten archivos ni dependen de tareas incompletas.
3. Antes de cada cambio, confirme la tarea exacta, que siga sin completar y que el cambio esté dentro de `spec.md` y `plan.md`.
4. Ejecute las pruebas solicitadas antes del código cuando aplique. Respete el runner .NET, framework y sintaxis apropiados.
5. Valide cada tarea con el chequeo más acotado disponible. Marque su casilla como `[X]` únicamente después de verificarla.
6. Si una tarea falla, no marque `[X]`. Repare el mismo alcance y vuelva a validar; detenga dependencias bloqueadas e informe la causa.
7. No implemente funciones ausentes de la spec, no cree capas o artefactos no autorizados y no cambie arquitectura sin actualizar los documentos rectores.

## Cierre y Estado

Antes de declarar finalizada la iniciativa:

1. Ejecute las validaciones finales definidas en `tasks.md` y registre en `FEATURE_DIR/quickstart.md` la fecha, los comandos o escenarios realmente ejecutados y sus resultados.
2. La evidencia debe incluir este formato:

   ```markdown
   ## Evidencia de validación

   - Comando: `comando ejecutado`
   - Resultado: Correcto
   ```

   Repita las dos líneas para cada validación ejecutada. No registre comandos planeados ni resultados no observados.

3. Ejecute `pwsh -ExecutionPolicy Bypass -File support/scripts/spec-status.ps1 -Transition Complete`. El validador cambia `En implementación` a `Implementada` solo si todas las tareas están marcadas y `quickstart.md` contiene evidencia satisfactoria.
4. Si el validador falla, mantenga el estado `En implementación`, comunique cada bloqueo concreto y no declare la spec implementada.
5. Procese los hooks `hooks.after_implement` con las mismas reglas: hooks obligatorios usan `EXECUTE_COMMAND` y deben terminar antes del informe final; hooks opcionales se presentan al usuario.

## Informe Final

Informe tareas completadas, validaciones y resultados, estado final de la spec, blockers pendientes y cualquier advertencia relevante. No afirme que se completó el trabajo si quedan tareas abiertas o falla una validación.
