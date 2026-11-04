# Tareas: Listado Paginado de Propiedades

**Entrada**: Documentos de diseño desde `specs/004-properties-list-pagination/`

**Prerrequisitos**: `plan.md` (requerido), `spec.md` (requerido para historias de usuario), `research.md`, `data-model.md`, `contracts/`

**Pruebas**: No se añadieron tareas de prueba automatizada porque la spec no las solicita explícitamente. La validación de esta feature se realizará mediante `quickstart.md` y la prueba manual documentada en `RealtorApi.http`.

**Organización**: Las tareas están agrupadas por historia de usuario para permitir implementación y validación independientes.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos distintos, sin dependencias)
- **[Story]**: Historia de usuario a la que pertenece la tarea (por ejemplo, US1, US2, US3)
- Incluir rutas exactas de archivo en las descripciones

## Fase 1: Preparación (Infraestructura compartida)

**Propósito**: Dejar lista la estructura inicial del caso de uso y las referencias manuales del endpoint

- [X] T001 [P] Crear la estructura `app/backend/src/RealtorApi/Features/Properties/ListProperties/` para alojar el slice del listado paginado
- [X] T002 [P] Añadir ejemplos manuales de `GET /api/properties` y `GET /api/properties?page=1&pageSize=6` en `app/backend/src/RealtorApi/RealtorApi.http`

---

## Fase 2: Fundacional (Prerrequisitos bloqueantes)

**Propósito**: Definir los contratos y la validación comunes que todas las historias usan antes de implementar el endpoint

- [X] T003 [P] Definir el contrato público `PropertyListItem` y `PagedPropertyListResponse` en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesContracts.cs`
- [X] T004 [P] Definir el modelo de consulta y los metadatos de paginación en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesPagination.cs`
- [X] T005 [P] Implementar la validación de `page` y `pageSize` en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesValidator.cs`
- [X] T006 [P] Propagar `CancellationToken` desde `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesSlice.cs` hasta `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesHandler.cs` y usarlo en las consultas EF Core

**Checkpoint**: La base del caso de uso queda lista y el endpoint puede implementarse sobre contratos y validación estables.

---

## Fase 3: Historia de Usuario 1 - Ver el listado inicial de propiedades (Prioridad: P1) 🎯 MVP

**Objetivo**: Exponer el endpoint `GET /api/properties` con la primera página por defecto y los campos públicos esenciales de cada propiedad

**Prueba independiente**: Una consulta sin parámetros devuelve la página 1 con hasta 6 resultados ordenados por `title` ascendente y con los campos públicos del contrato

### Implementación para la Historia de Usuario 1

- [X] T007 [US1] Implementar el slice `GET /api/properties` en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesSlice.cs`
- [X] T008 [P] [US1] Implementar la consulta base con `AsNoTracking()` y orden por `Title` ascendente en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesHandler.cs`

**Checkpoint**: En este punto el listado inicial ya debe responder con datos públicos válidos usando la página por defecto.

---

## Fase 4: Historia de Usuario 2 - Navegar entre páginas de resultados (Prioridad: P2)

**Objetivo**: Completar la paginación del listado con metadatos coherentes para páginas explícitas

**Prueba independiente**: Una consulta con `page` y `pageSize` válidos devuelve el subconjunto correcto y metadatos consistentes con el total disponible

### Implementación para la Historia de Usuario 2

- [X] T009 [P] [US2] Completar la paginación con `CountAsync`, `Skip`, `Take`, `totalItems`, `totalPages`, `hasNext` y `hasPrevious` en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesHandler.cs`
- [X] T010 [P] [US2] Mapear la respuesta paginada al contrato público en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesMapping.cs`

**Checkpoint**: El endpoint debe devolver páginas explícitas con metadatos de paginación correctos, incluidas respuestas vacías consistentes.

---

## Fase 5: Historia de Usuario 3 - Consumir imágenes públicas y validar parámetros (Prioridad: P3)

**Objetivo**: Asegurar la validación de entrada y la composición de URLs públicas absolutas para cada imagen

**Prueba independiente**: Parámetros inválidos devuelven `400`, y cada elemento incluye `imageUrl` absoluta construida con el host y esquema de la solicitud actual

### Implementación para la Historia de Usuario 3

- [X] T011 [P] [US3] Construir `imageUrl` absoluta pública usando `Request.Scheme`, `Request.Host` y `ImageUrl` persistido en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesMapping.cs`
- [X] T012 [US3] Integrar la validación de `page` y `pageSize` con respuesta `400` en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesSlice.cs`

**Checkpoint**: El endpoint ya valida la entrada y devuelve imágenes públicas absolutas sin exponer rutas físicas internas.

---

## Fase Final: Pulido y Aspectos Transversales

**Propósito**: Cerrar la feature con evidencia de validación y documentación actualizada

- [X] T013 [P] Actualizar `specs/004-properties-list-pagination/quickstart.md` con la evidencia de validación manual y el resultado esperado del endpoint
- [X] T014 Ejecutar la comprobación manual documentada en `app/backend/src/RealtorApi/RealtorApi.http` y verificar que la respuesta cumple la spec

---

## Dependencias y Orden de Ejecución

### Dependencias por Fase

- **Preparación (Fase 1)**: Sin dependencias; puede comenzar de inmediato
- **Fundacional (Fase 2)**: Depende de Preparación y bloquea todas las historias de usuario
- **Historias de Usuario (Fases 3+)**: Todas dependen de la Fase Fundacional
- **Pulido (Fase Final)**: Depende de que las historias deseadas estén completadas

### Dependencias entre Historias de Usuario

- **Historia de Usuario 1 (P1)**: Puede comenzar después de la Fase Fundacional; no depende de otras historias
- **Historia de Usuario 2 (P2)**: Puede comenzar después de la Fase Fundacional; usa la base del listado pero sigue siendo validable de forma independiente
- **Historia de Usuario 3 (P3)**: Puede comenzar después de la Fase Fundacional; se apoya en el slice ya creado para cerrar validación e imagen pública

### Dentro de Cada Historia

- La validación debe existir antes de integrar el endpoint
- La consulta con `AsNoTracking()` y el orden por `Title` deben existir antes de completar la paginación
- La paginación debe quedar cerrada antes de construir la URL pública de imagen
- El contrato público debe quedar estable antes de considerar la historia completa

### Oportunidades de Paralelismo

- Las tareas marcadas con `[P]` en Preparación pueden ejecutarse en paralelo
- Las tareas marcadas con `[P]` en la Fase Fundacional pueden ejecutarse en paralelo
- Tras completar la Fase Fundacional, las historias pueden avanzar en paralelo si el equipo lo permite
- Dentro de una misma historia, las tareas marcadas con `[P]` pueden ejecutarse en paralelo cuando no comparten archivos

---

## Ejemplo de Paralelismo: Historia de Usuario 1

```bash
# Ejecutar en paralelo las tareas de soporte de la historia 1:
Task: "Implementar la consulta base con AsNoTracking() y orden por Title ascendente en app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesHandler.cs"
Task: "Implementar el slice GET /api/properties en app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesSlice.cs"
```

---

## Estrategia de Implementación

### MVP Primero (Solo Historia de Usuario 1)

1. Completar Fase 1: Preparación
2. Completar Fase 2: Fundacional
3. Completar Fase 3: Historia de Usuario 1
4. Detenerse y validar que `GET /api/properties` responde con la página por defecto

### Entrega Incremental

1. Completar Preparación + Fundacional
2. Implementar Historia de Usuario 1 y validar el listado base
3. Implementar Historia de Usuario 2 y validar la paginación explícita
4. Implementar Historia de Usuario 3 y validar la entrada inválida y la URL pública de imagen
5. Cerrar con la evidencia manual en `quickstart.md`

### Estrategia de Equipo Paralelo

Con más de un desarrollador:

1. El equipo completa Preparación + Fundacional de forma conjunta
2. Tras eso, un desarrollador puede tomar US1 mientras otro avanza US2 y un tercero US3
3. Cada historia debe poder demostrarse sin bloquear a las demás

---

## Notas

- Las tareas con `[P]` usan archivos distintos y no dependen de trabajo incompleto
- La etiqueta `[Story]` mantiene trazabilidad directa con la historia de usuario correspondiente
- La validación manual es obligatoria para esta feature aunque no se hayan solicitado pruebas automatizadas
- Evitar tareas vagas, dependencias cruzadas innecesarias y cambios fuera del alcance del listado paginado
