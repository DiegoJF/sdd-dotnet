# Tareas: Actualización de estado de propiedades

**Entrada**: Documentos de diseño desde `/specs/009-update-status/`

**Requisitos previos**: plan.md (obligatorio), spec.md (obligatorio para historias de usuario), research.md, data-model.md, contracts/

**Pruebas**: La especificación exige verificación manual desde `.http`; no se han solicitado pruebas automatizadas como parte del alcance funcional mínimo.

**Organización**: Las tareas se agrupan por historia de usuario para permitir implementación y validación independientes.

## Formato: `[ID] [P?] [Historia] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos diferentes, sin dependencias)
- **[Historia]**: Historia de usuario a la que pertenece la tarea (por ejemplo, US1, US2)
- Incluir rutas exactas de archivo en la descripción

## Fase 1: Preparación (Infraestructura compartida)

**Propósito**: Preparar la estructura de archivos para el nuevo slice y la validación manual asociada

- [X] T001 [P] Crear la carpeta del slice de estado y sus archivos base en `app/backend/src/RealtorApi/Features/Properties/UpdateStatus/`
- [X] T002 [P] Añadir los ejemplos manuales iniciales de `PATCH` en `app/backend/src/RealtorApi/RealtorApi.http`

---

## Fase 2: Fundacional (Requisitos bloqueantes)

**Propósito**: Infraestructura mínima que debe existir antes de implementar cualquier historia de usuario

**⚠️ CRÍTICO**: Ninguna historia de usuario puede comenzar hasta que esta fase esté completa

- [X] T003 [P] Definir `UpdateStatusRequest`, `UpdateStatusResponse` y `UpdateStatusCommand` en `app/backend/src/RealtorApi/Features/Properties/UpdateStatus/UpdateStatusContracts.cs` y `app/backend/src/RealtorApi/Features/Properties/UpdateStatus/UpdateStatusCommand.cs`
- [X] T004 [P] Implementar el mapeo del slice en `app/backend/src/RealtorApi/Features/Properties/UpdateStatus/UpdateStatusMapping.cs`
- [X] T005 Implementar la validación del body en `app/backend/src/RealtorApi/Features/Properties/UpdateStatus/UpdateStatusValidator.cs` para exigir `status` válido y rechazar cuerpos vacíos, incompletos o con propiedades JSON inesperadas

**Punto de control**: La base del slice queda lista para exponer el endpoint y conectar la persistencia

---

## Fase 3: Historia de usuario 1 - Actualizar solo el estado de una propiedad (Prioridad: P1) 🎯 MVP

**Objetivo**: Permitir que un operador actualice únicamente el estado de una propiedad existente por id sin alterar otros datos persistidos

**Prueba independiente**: Un `PATCH` válido sobre una propiedad existente devuelve `200 OK`, cambia solo `status` y conserva `imageUrl` y el resto de campos

### Implementación de la historia de usuario 1

- [X] T006 [US1] Implementar la lógica de persistencia en `app/backend/src/RealtorApi/Features/Properties/UpdateStatus/UpdateStatusHandler.cs` para buscar la propiedad con `FindAsync`, actualizar solo `Status` y `UpdatedAt`, y guardar los cambios
- [X] T007 [US1] Exponer el endpoint público `PATCH /api/properties/{id}/status` en `app/backend/src/RealtorApi/Features/Properties/UpdateStatus/UpdateStatusSlice.cs` devolviendo `200 OK` con la propiedad actualizada

**Punto de control**: La historia principal queda operativa y puede demostrarse de forma independiente

---

## Fase 4: Historia de usuario 2 - Rechazar solicitudes inválidas o inexistentes (Prioridad: P2)

**Objetivo**: Rechazar con ProblemDetails las solicitudes sin `status`, con `status` inválido o dirigidas a una propiedad inexistente

**Prueba independiente**: Un `PATCH` sin `status`, con `status` no válido o hacia un `id` inexistente responde `400` o `404` según corresponda, sin modificar datos persistidos

### Implementación de la historia de usuario 2

- [X] T008 [US2] Completar el manejo de errores funcionales en `app/backend/src/RealtorApi/Features/Properties/UpdateStatus/UpdateStatusSlice.cs` y `app/backend/src/RealtorApi/Features/Properties/UpdateStatus/UpdateStatusHandler.cs` para devolver `400 Bad Request` y `404 Not Found` con ProblemDetails, incluyendo el rechazo de propiedades JSON inesperadas
- [X] T009 [P] [US2] Documentar los casos de error y el flujo manual en `app/backend/src/RealtorApi/RealtorApi.http` con ejemplos de `400 Bad Request` y `404 Not Found`

**Punto de control**: Los rechazos funcionales quedan cubiertos y documentados para verificación manual

---

## Fase 5: Pulido y aspectos transversales

**Propósito**: Validaciones finales, trazabilidad contractual y cierre de la funcionalidad

- [X] T010 [P] Regenerar y validar el contrato OpenAPI del backend con `support/scripts/generate-openapi-v1.ps1` y versionar `app/backend/src/RealtorApi/wwwroot/openapi/v1.json`
- [X] T011 [P] Ejecutar `dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj` y confirmar que `OpenApiDriftTests` permanece en verde con el nuevo endpoint
- [X] T012 Actualizar `specs/009-update-status/quickstart.md` con la evidencia real de validación al cerrar la implementación: fecha, comandos ejecutados y resultados obtenidos

---

## Dependencias y orden de ejecución

### Dependencias de fase

- **Preparación (Fase 1)**: Sin dependencias; puede comenzar de inmediato
- **Fundacional (Fase 2)**: Depende de Preparación completada; bloquea todas las historias de usuario
- **Historias de usuario (Fase 3+)**: Dependen de Fundacional completado
- **Pulido (Fase final)**: Depende de que las historias de usuario deseadas estén completas

### Dependencias entre historias de usuario

- **Historia de usuario 1 (P1)**: Puede comenzar tras Fundacional; no depende de otras historias
- **Historia de usuario 2 (P2)**: Puede comenzar tras Fundacional; se apoya en la misma superficie del endpoint pero debe ser comprobable de forma independiente

### Dentro de cada historia de usuario

- El contrato y la validación deben quedar listos antes de finalizar la historia
- La persistencia debe completarse antes de cerrar el endpoint
- La historia debe poder demostrarse con casos aislados antes de pasar a la siguiente prioridad

### Oportunidades de paralelización

- T001 y T002 pueden ejecutarse en paralelo
- T003 y T004 pueden ejecutarse en paralelo
- T009, T010 y T011 pueden ejecutarse en paralelo una vez que el código del slice esté estable

---

## Ejemplo de paralelización: Historia de usuario 1

```bash
Tarea: "Implementar la lógica de persistencia en app/backend/src/RealtorApi/Features/Properties/UpdateStatus/UpdateStatusHandler.cs"
Tarea: "Exponer el endpoint público PATCH /api/properties/{id}/status en app/backend/src/RealtorApi/Features/Properties/UpdateStatus/UpdateStatusSlice.cs"
```

---

## Estrategia de implementación

### MVP primero (solo historia de usuario 1)

1. Completar Fase 1: Preparación
2. Completar Fase 2: Fundacional
3. Completar Fase 3: Historia de usuario 1
4. **DETENER Y VALIDAR**: Demostrar el flujo exitoso con `.http`

### Entrega incremental

1. Completar Preparación + Fundacional → base del slice lista
2. Añadir Historia de usuario 1 → actualización de estado funcional
3. Añadir Historia de usuario 2 → errores consistentes y documentados
4. Cerrar con OpenAPI v1 y validación de drift

### Estrategia paralela de equipo

Con más de una persona:

1. Un desarrollador puede preparar `UpdateStatusHandler.cs` mientras otro completa `UpdateStatusSlice.cs`
2. Otro miembro puede documentar `RealtorApi.http` en paralelo con la regeneración de OpenAPI v1

---

## Notas

- `[P]` significa archivos diferentes y sin dependencia directa
- Cada historia debe poder demostrarse de forma independiente
- La validación manual desde `.http` es parte del alcance mínimo de esta funcionalidad
- En toda spec que toque backend, la tarea de OpenAPI v1 es obligatoria; para cerrar la spec, `dotnet test` debe quedar en verde e incluir `OpenApiDriftTests` como red anti-drift