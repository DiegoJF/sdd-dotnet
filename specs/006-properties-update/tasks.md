# Tareas: Actualización de propiedades con imagen opcional

**Entrada**: Documentos de diseño desde `specs/006-properties-update/`

**Prerrequisitos**: `plan.md` (requerido), `spec.md` (requerido para historias de usuario), `research.md`, `data-model.md`, `contracts/`

**Pruebas**: No se añaden tareas de pruebas automatizadas porque la spec no las solicita explícitamente. La validación requerida se ejecutará con `quickstart.md` y comprobación manual en `RealtorApi.http`.

**Organización**: Las tareas están agrupadas por historia de usuario para permitir implementación y validación independientes.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos distintos, sin dependencias)
- **[Story]**: Historia de usuario a la que pertenece la tarea (por ejemplo, US1, US2, US3)
- Todas las tareas incluyen ruta exacta de archivo

---

## Fase 1: Preparación (Infraestructura compartida)

**Propósito**: Dejar lista la estructura del caso de uso y los ejemplos manuales del endpoint

- [X] T001 [P] Crear la estructura `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/` para alojar el slice de actualización
- [X] T002 [P] Añadir ejemplos manuales de `PUT /api/properties/{id}` (sin imagen, con imagen válida, 400, 404, 413, 415 y 500) en `app/backend/src/RealtorApi/RealtorApi.http`

---

## Fase 2: Fundacional (Prerrequisitos bloqueantes)

**Propósito**: Definir contratos, comandos, validaciones y mappings antes de implementar historias

**⚠️ CRÍTICO**: Ninguna historia puede comenzar hasta que esta fase esté completa

- [X] T003 [P] Definir los contratos `UpdatePropertyRequest` (5 campos editables + `image` opcional) y `UpdatePropertyResponse` (`id`, `title`, `status`, `imageUrl`) en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyContracts.cs`
- [X] T004 [P] Definir `UpdatePropertyCommand` con `id` (Guid), `title`, `description`, `address`, `price`, `status` e `image` (IFormFile nullable) en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyCommand.cs`
- [X] T005 [P] Implementar `UpdatePropertyValidator : AbstractValidator<UpdatePropertyRequest>` con reglas FluentValidation para los 5 campos de negocio obligatorios (`title` max 160, `description` max 1200, `address` max 240, `price` > 0, `status` en enum `PropertyStatusValue`) que produce `400 Bad Request` en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyValidator.cs`
- [X] T006 [P] Implementar validación de imagen (PNG/JPG, 0 bytes → `415`, contenido real inválido → `415`, tamaño > 5 MB → `413`) con enum `UpdatePropertyImageValidationError` y clase estática `UpdatePropertyImageValidator` siguiendo el patrón de `CreatePropertyImageValidator` en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyValidator.cs`
- [X] T007 [P] Implementar mappings explícitos sin lógica de negocio: método de extensión `ToCommand(this UpdatePropertyRequest request, Guid id)` que acepta `id` como parámetro separado (proviene del path, no del body del formulario) y `property → UpdatePropertyResponse`; en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyMapping.cs`

**Checkpoint**: La base técnica del slice queda estable para implementar endpoint y handler.

---

## Fase 3: Historia de Usuario 1 - Actualización de propiedad sin imagen (Prioridad: P1) 🎯 MVP

**Objetivo**: Actualizar campos de negocio de una propiedad existente sin imagen y conservar `imageUrl` previo intacto

**Prueba independiente**: `PUT /api/properties/{id}` con datos válidos y sin imagen devuelve `200 OK` con campos actualizados e `imageUrl` idéntico al previo

### Implementación para la Historia de Usuario 1

- [X] T008 [US1] Implementar slice `PUT /api/properties/{id}` llamando a `UpdatePropertyImageValidator.Validate(request.Image)` (definido en T006) para devolver `415` o `413` según corresponda, binding `[FromForm]` del request, extracción de `id` del path y auto-registro mediante `ISlice` en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertySlice.cs`
- [X] T009 [US1] Implementar handler: `FindAsync(id)` → `404 Not Found` si la propiedad no existe → asignar `Title`, `Description`, `Address`, `Price`, `Status`, `UpdatedAt` → conservar `ImageUrl` previo si `command.Image` es `null` → `SaveChangesAsync` → retornar `200 OK` con `UpdatePropertyResponse` en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs`
- [X] T010 [US1] Registrar handler con `IHandler` en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs`

**Checkpoint**: La actualización sin imagen debe funcionar end-to-end como incremento MVP independiente.

---

## Fase 4: Historia de Usuario 2 - Actualización de propiedad con imagen válida (Prioridad: P2)

**Objetivo**: Reemplazar o asignar imagen en una propiedad existente con nueva URL pública persistida

**Prueba independiente**: `PUT /api/properties/{id}` con datos válidos e imagen PNG/JPG <= 5 MB devuelve `200 OK` con `imageUrl` nuevo distinto al previo

### Implementación para la Historia de Usuario 2

- [X] T011 [US2] Extender handler: si `command.Image != null` invocar `CreatePropertyImageStorage.SaveAsync(command.Image, environment, cancellationToken)` y asignar `property.ImageUrl = storedImage.RelativePath` antes de `SaveChangesAsync` en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs`

**Checkpoint**: La actualización con imagen queda funcional y el `imageUrl` nuevo es consumible por clientes. La verificación de compatibilidad con el listado se realiza en la Fase Final (T012).

---

## Fase 5: Historia de Usuario 3 - Rechazo por validación y manejo de errores (Prioridad: P3)

**Objetivo**: Garantizar códigos de error correctos, atomicidad ante fallo de imagen y ausencia de efectos secundarios ante cualquier rechazo

**Prueba independiente**: Solicitudes con id inexistente, imagen inválida o campos de negocio inválidos devuelven el código esperado y la propiedad conserva sus valores previos sin modificación

### Implementación para la Historia de Usuario 3

- [X] T013 [US3] Añadir atomicidad con bloque `catch (Exception exception) when (command.Image is not null)` en handler: si `storedImage != null` ejecutar `CreatePropertyImageStorage.TryDelete(storedImage.PhysicalPath)`; no invocar `SaveChangesAsync` ante cualquier excepción (incluido fallo de `SaveChangesAsync` tras imagen ya escrita en disco); retornar `500 Internal Server Error` en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs`
- [X] T014 [P] [US3] Añadir logging estructurado en handler para: éxito de actualización, `404 Not Found`, fallo de almacenamiento de imagen (con path) y excepciones inesperadas en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs`
- [X] T015 [US3] Verificar que ante rechazo por `400`, `404`, `413` o `415` la propiedad conserva exactamente sus valores previos y no queda ningún archivo almacenado parcialmente en `app/backend/src/RealtorApi/wwwroot/assets/properties/`

**Checkpoint**: El flujo completo de errores queda consistente con la spec y el contrato OpenAPI.

---

## Fase Final: Pulido y Aspectos Transversales

**Propósito**: Cerrar evidencia funcional, de contrato y de rendimiento

- [X] T012 [P] Verificar manualmente mediante `GET /api/properties` que la propiedad actualizada (con y sin imagen) aparece con los valores correctos incluyendo `imageUrl`, como evidencia de compatibilidad con el listado existente en `specs/006-properties-update/quickstart.md`
- [X] T016 [P] Verificar consistencia final del endpoint con `specs/006-properties-update/contracts/backend-properties-update.openapi.yaml`
- [X] T017 [P] Ejecutar los escenarios 1–8 del `specs/006-properties-update/quickstart.md`, registrar el código HTTP observado, body de respuesta y tiempos por escenario como evidencia funcional escrita en el mismo archivo
- [X] T018 Ejecutar la validación manual en `app/backend/src/RealtorApi/RealtorApi.http` y registrar resultado en `specs/006-properties-update/quickstart.md`
- [X] T019 Medir SC-005 con 100 solicitudes válidas mixtas y registrar p95 en `specs/006-properties-update/quickstart.md`

---

## Dependencias y Orden de Ejecución

### Dependencias por Fase

- **Preparación (Fase 1)**: Sin dependencias; puede iniciar de inmediato
- **Fundacional (Fase 2)**: Depende de Fase 1 y bloquea todas las historias
- **Historias (Fases 3, 4 y 5)**: Dependen de Fase 2
- **US2 (Fase 4)**: Depende de la base funcional de US1 (Fase 3) en el mismo handler
- **US3 (Fase 5)**: Depende de US1 y US2 para validar atomicidad y errores del flujo completo
- **Pulido (Fase Final)**: Depende de completar todas las historias objetivo

### Dependencias entre Historias

- **US1 (P1)**: Puede iniciar tras Fase 2; no depende de otras historias
- **US2 (P2)**: Extiende el handler de US1; requiere T009 y T010 completados
- **US3 (P3)**: Extiende el handler de US2; requiere T011 completado (T012 se realiza en Fase Final como verificación funcional independiente)

### Dentro de Cada Historia

- Contratos y command deben existir antes de implementar slice y handler
- El slice (ISlice) debe registrarse antes de validar el endpoint de forma manual
- El handler (IHandler) se registra junto con la implementación de US1
- La historia se considera completa solo con su prueba independiente satisfecha

### Oportunidades de Paralelismo

- **Fase 1**: T001 y T002 son independientes entre sí
- **Fase 2**: T003, T004, T005, T006, T007 son independientes entre sí (archivos distintos)
- **Fase 5**: T013 y T014 pueden ejecutarse en paralelo (mismo archivo, distinto bloque)
- **Fase Final**: T012, T016 y T017 pueden ejecutarse en paralelo

---

## Ejemplo de Paralelismo: Fase Fundacional

```bash
# Los siguientes archivos no tienen dependencias entre sí y pueden implementarse simultáneamente:
UpdatePropertyContracts.cs   # T003
UpdatePropertyCommand.cs     # T004
UpdatePropertyValidator.cs   # T005, T006
UpdatePropertyMapping.cs     # T007
```

## Ejemplo de Paralelismo: Fase Final

```bash
# T012, T016 y T017 pueden ejecutarse simultáneamente:
"Verificar compatibilidad con listado GET /api/properties"
"Verificar consistencia con backend-properties-update.openapi.yaml"
"Ejecutar escenarios 1-8 del quickstart.md y registrar evidencia"
```

---

## Estrategia de Implementación

### MVP (implementar primero)

Completar **Fases 1, 2 y 3 (US1)** para tener el flujo mínimo funcional:
- `PUT /api/properties/{id}` sin imagen con `200 OK`
- `404 Not Found` ante id inexistente
- `400 Bad Request` ante campos de negocio inválidos
- `imageUrl` previo conservado

### Incremento 1

Completar **Fase 4 (US2)**:
- `PUT /api/properties/{id}` con imagen PNG/JPG <= 5 MB
- Nuevo `imageUrl` persistido y consumible

### Incremento 2 (cierre)

Completar **Fase 5 (US3)** y **Fase Final**:
- Atomicidad garantizada ante `500`
- Logging estructurado
- Evidencia de validación y medición SC-005 en `quickstart.md`
