# Tareas: Alta de propiedades con imagen opcional

**Entrada**: Documentos de diseño desde `specs/005-properties-create/`

**Prerrequisitos**: `plan.md` (requerido), `spec.md` (requerido para historias de usuario), `research.md`, `data-model.md`, `contracts/`

**Pruebas**: No se añadieron tareas de pruebas automatizadas porque la spec no las solicita explícitamente. La validación requerida se ejecutará con `quickstart.md` y comprobación manual en `RealtorApi.http`.

**Organización**: Las tareas están agrupadas por historia de usuario para permitir implementación y validación independientes.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos distintos, sin dependencias)
- **[Story]**: Historia de usuario a la que pertenece la tarea (por ejemplo, US1, US2, US3)
- Todas las tareas incluyen ruta exacta de archivo

## Fase 1: Preparación (Infraestructura compartida)

**Propósito**: Dejar lista la estructura del caso de uso y los ejemplos manuales del endpoint

- [X] T001 [P] Crear la estructura `app/backend/src/RealtorApi/Features/Properties/CreateProperty/` para alojar el slice de alta
- [X] T002 [P] Añadir ejemplos manuales de `POST /api/properties` (sin imagen, con imagen válida, 400, 413, 415 y 500) en `app/backend/src/RealtorApi/RealtorApi.http`

---

## Fase 2: Fundacional (Prerrequisitos bloqueantes)

**Propósito**: Definir contratos, modelos y validaciones comunes antes de implementar historias

- [X] T003 [P] Definir los contratos `CreatePropertyRequest` y `CreatePropertyResponse` en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyContracts.cs`
- [X] T004 [P] Definir el modelo de comando de creación en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyCommand.cs`
- [X] T005 [P] Implementar validación de campos de negocio con respuesta `400 Bad Request` en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyValidator.cs`
- [X] T006 [P] Implementar validación de imagen (PNG/JPG, 0 bytes inválido, contenido real inválido, max 5 MB) en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyValidator.cs`
- [X] T007 [P] Implementar mapeos explícitos request/command/entidad/response sin lógica de negocio en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyMapping.cs`
- [X] T008 [P] Implementar utilitario de almacenamiento con nombre único (`UUID + extensión válida`) en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyImageStorage.cs`

**Checkpoint**: La base técnica del slice queda estable para implementar endpoint y handler.

---

## Fase 3: Historia de Usuario 1 - Alta de propiedad sin imagen (Prioridad: P1) 🎯 MVP

**Objetivo**: Crear propiedades válidas sin archivo de imagen y persistir `imageUrl = null`

**Prueba independiente**: `POST /api/properties` con datos válidos y sin imagen devuelve creación exitosa con `imageUrl` nulo

### Implementación para la Historia de Usuario 1

- [X] T009 [US1] Implementar endpoint `POST /api/properties` y binding del request en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertySlice.cs`
- [X] T010 [US1] Implementar handler de alta sin imagen con persistencia de `Property` e `ImageUrl = null` en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs`
- [X] T011 [US1] Integrar auto-registro del nuevo handler por `IHandler` en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs`

**Checkpoint**: La alta sin imagen debe funcionar end-to-end como incremento MVP.

---

## Fase 4: Historia de Usuario 2 - Alta de propiedad con imagen válida (Prioridad: P2)

**Objetivo**: Crear propiedades con imagen válida almacenada y URL pública/relativa persistida

**Prueba independiente**: Solicitud válida con imagen PNG/JPG <= 5 MB crea propiedad y persiste `imageUrl` no nulo

### Implementación para la Historia de Usuario 2

- [X] T012 [US2] Extender handler para guardar imagen en `app/backend/src/RealtorApi/wwwroot/assets/properties/` y persistir `imageUrl` en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs`
- [X] T013 [P] [US2] Aplicar estrategia de nombre interno único (`UUID + extensión válida`) en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyImageStorage.cs`
- [X] T014 [US2] Verificar compatibilidad de la propiedad creada con el flujo de listado existente en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs`

**Checkpoint**: La creación con imagen válida queda funcional y consumible por clientes.

---

## Fase 5: Historia de Usuario 3 - Rechazo por validación de imagen y errores esperados (Prioridad: P3)

**Objetivo**: Endurecer reglas de error (`400`, `413`, `415`, `500`) y atomicidad sin persistencias parciales

**Prueba independiente**: Solicitudes inválidas y fallos internos devuelven códigos esperados y no crean propiedad/archivo parcial

### Implementación para la Historia de Usuario 3

- [X] T015 [US3] Integrar respuesta `400 Bad Request` para validación de campos de negocio en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertySlice.cs`
- [X] T016 [US3] Integrar respuestas `415 Unsupported Media Type` y `413 Payload Too Large` para errores de upload en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertySlice.cs`
- [X] T017 [US3] Implementar respuesta `500 Internal Server Error` ante fallo de almacenamiento y garantizar atomicidad en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs`

**Checkpoint**: El flujo completo de errores queda consistente con spec y contrato.

---

## Fase Final: Pulido y Aspectos Transversales

**Propósito**: Cerrar evidencia funcional, de contrato y de rendimiento

- [X] T018 [P] Verificar consistencia final del endpoint con `specs/005-properties-create/contracts/backend-properties-create.openapi.yaml`
- [X] T019 [P] Actualizar evidencia de validación funcional y códigos de error en `specs/005-properties-create/quickstart.md`
- [X] T020 Ejecutar la validación manual en `app/backend/src/RealtorApi/RealtorApi.http` y registrar resultado en `specs/005-properties-create/quickstart.md`
- [X] T021 Medir SC-004 con 100 solicitudes válidas mixtas y registrar p95 en `specs/005-properties-create/quickstart.md`

---

## Dependencias y Orden de Ejecución

### Dependencias por Fase

- **Preparación (Fase 1)**: Sin dependencias; puede iniciar de inmediato
- **Fundacional (Fase 2)**: Depende de Preparación y bloquea todas las historias
- **Historias (Fases 3, 4 y 5)**: Dependen de Fase 2
- **Pulido (Fase Final)**: Depende de completar historias objetivo

### Dependencias entre Historias

- **US1 (P1)**: Puede iniciar tras Fase 2; no depende de otras historias
- **US2 (P2)**: Depende de base funcional de US1 en el mismo endpoint
- **US3 (P3)**: Depende de US1 y US2 para validar errores y atomicidad del flujo completo

### Dentro de Cada Historia

- Contratos y validaciones deben existir antes de cerrar el endpoint
- El endpoint debe existir antes de completar el handler
- La estrategia de almacenamiento debe quedar integrada antes de cerrar compatibilidad de listado
- La historia se considera completa solo con su prueba independiente satisfecha

### Oportunidades de Paralelismo

- Fase 1: T001, T002
- Fase 2: T003, T004, T005, T006, T007, T008 (respetando no pisar el mismo bloque de archivo)
- Fase 4: T013 puede ir en paralelo con T012 si se integra por commits coordinados
- Fase final: T018 y T019 pueden ir en paralelo

---

## Ejemplo de Paralelismo: Historia de Usuario 2

```bash
Task: "Extender handler para guardar imagen en app/backend/src/RealtorApi/wwwroot/assets/properties/ y persistir imageUrl en app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs"
Task: "Aplicar estrategia de nombre interno único (UUID + extensión válida) en app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyImageStorage.cs"
```

## Ejemplo de Paralelismo: Fase Final

```bash
Task: "Verificar consistencia final del endpoint con specs/005-properties-create/contracts/backend-properties-create.openapi.yaml"
Task: "Actualizar evidencia de validación funcional y códigos de error en specs/005-properties-create/quickstart.md"
```

---

## Estrategia de Implementación

### MVP Primero (Solo US1)

1. Completar Fase 1
2. Completar Fase 2
3. Completar Fase 3 (US1)
4. Validar creación sin imagen (`imageUrl = null`)

### Entrega Incremental

1. Preparación + Fundacional
2. US1 (alta sin imagen)
3. US2 (alta con imagen válida)
4. US3 (errores y atomicidad)
5. Pulido con evidencia funcional/contrato/rendimiento

### Estrategia de Equipo Paralelo

1. Equipo completa Preparación + Fundacional
2. Después, un desarrollador toma US1, otro prepara integración de imagen (US2), otro endurece errores (US3)
3. Integración final y evidencia conjunta en quickstart

---

## Notas

- Las tareas con `[P]` pueden ejecutarse en paralelo si no comparten el mismo bloque de archivo
- Las etiquetas `[US1]`, `[US2]`, `[US3]` aseguran trazabilidad directa con historias
- La evidencia de `p95 < 3s` es obligatoria para cerrar SC-004
- Evitar cambios fuera del alcance de creación de propiedades con imagen opcional
