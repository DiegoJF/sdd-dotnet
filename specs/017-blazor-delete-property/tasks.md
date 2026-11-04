---
description: "Lista de tareas para la eliminación de propiedades"
---

# Tareas: 017-blazor-delete-property

**Input**: Documentos de diseño en `specs/017-blazor-delete-property/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`, `quickstart.md`

**Tests**: Se incluyen pruebas unitarias y de endpoint del backend porque `plan.md` (R-11) las define y las pruebas `OpenApiDriftTests` existentes exigen mantener el contrato publicado. El frontend se valida manualmente según `quickstart.md`.

**Organización**: Las tareas se agrupan por historia de usuario de `spec.md`.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos distintos, sin dependencias pendientes)
- **[Story]**: Historia de usuario a la que pertenece (US1, US2, US3, US4)

## Convenciones de rutas

- Backend: `app/backend/src/RealtorApi/`, pruebas en `app/backend/tests/RealtorApiTests/`
- Frontend: `app/frontend/src/RealtorWeb/`

---

## Fase 1: Setup

**Propósito**: Preparar carpetas del slice y verificar el estado inicial.

- [X] T001 Ejecutar `dotnet build app/Realtor.sln` y `dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj` para registrar la línea base en verde antes de cambios (anotar resultado en `specs/017-blazor-delete-property/quickstart.md`, sección Evidencia de validación)
- [X] T002 [P] Crear la carpeta del slice `app/backend/src/RealtorApi/Features/Properties/DeleteProperty/` y la carpeta de feature `app/frontend/src/RealtorWeb/Features/Properties/Delete/`

---

## Fase 2: Fundacional (prerrequisitos bloqueantes)

**Propósito**: Infraestructura de frontend compartida por US2, US3 y US4.

**⚠️ CRÍTICO**: Ninguna historia de frontend puede comenzar hasta completar esta fase.

- [X] T003 [P] Agregar `[Delete("/api/properties/{id}")] Task DeletePropertyAsync(string id, CancellationToken cancellationToken = default);` en `app/frontend/src/RealtorWeb/Services/Api/Properties/IPropertiesApi.cs` según `contracts/frontend-property-delete.contract.md`
- [X] T004 [P] Crear `PropertyNotice` (record con `Kind` enum `Success`/`Info` y `Message`) en `app/frontend/src/RealtorWeb/Features/Properties/Delete/PropertyNotice.cs` según `data-model.md`
- [X] T005 Crear `PropertyNoticeService` con `Set(PropertyNotice notice)` y `PropertyNotice? Consume()` (aviso de un solo uso) en `app/frontend/src/RealtorWeb/Features/Properties/Delete/PropertyNoticeService.cs` (depende de T004)
- [X] T006 Registrar `builder.Services.AddScoped<PropertyNoticeService>();` en `app/frontend/src/RealtorWeb/Program.cs` (depende de T005)
- [X] T007 Crear el componente `PropertyDeleteDialog` en `app/frontend/src/RealtorWeb/Features/Properties/Delete/PropertyDeleteDialog.razor` y `PropertyDeleteDialog.razor.cs` con parámetros `PropertyId`, `PropertyTitle`, `IsOpen`, `OnCancel`, `OnDeleted`, `OnNotFound`; superposición modal con el título de la propiedad, botones "Eliminar" y "Cancelar" con íconos Lucide; estado `Idle/Deleting` que deshabilita ambos botones durante la solicitud y descarta clics repetidos (RF-08, RF-13); invoca `IPropertiesApi.DeletePropertyAsync` y emite `OnDeleted` ante éxito (depende de T003)
- [X] T008 [P] Agregar estilos del diálogo modal (`.property-delete-dialog`, superposición, botones de acción, mensaje de error) y del banner de aviso (`.property-notice`, variantes `success` e `info`) en `app/frontend/src/RealtorWeb/wwwroot/app.css`, reutilizando variables CSS existentes
- [X] T009 [P] Agregar `@using RealtorWeb.Features.Properties.Delete` en `app/frontend/src/RealtorWeb/Components/_Imports.razor`

**Checkpoint**: Diálogo, cliente y servicio de avisos disponibles para integrar.

---

## Fase 3: Historia de Usuario 1 - Eliminar una propiedad por su identificador (Prioridad: P1) 🎯 MVP

**Objetivo**: Endpoint público `DELETE /api/properties/{id}` con respuestas 204/400/404, eliminación física y sin tocar la imagen.

**Prueba independiente**: Ejecutar las comprobaciones A1–A5 de `quickstart.md` y las pruebas automatizadas del slice.

### Pruebas de la Historia de Usuario 1

- [X] T010 [P] [US1] Crear pruebas unitarias de `DeletePropertyValidator` (vacío → error, no UUID → error, UUID canónico → válido) en `app/backend/tests/RealtorApiTests/Features/Properties/DeleteProperty/DeletePropertyValidatorTests.cs`
- [X] T011 [P] [US1] Crear pruebas de endpoint en `app/backend/tests/RealtorApiTests/Infrastructure/Endpoints/DeletePropertyEndpointTests.cs` siguiendo `GetPropertyByIdEndpointTests.cs` y `TestWebApplicationFactory`: 204 con id existente y `GET` posterior 404; segunda eliminación 404 con `ProblemDetails`; id `no-es-uuid` → 400 con `ValidationProblemDetails`; total del listado disminuye exactamente en uno y otra propiedad permanece intacta; propiedad con `imageUrl` se elimina y el archivo físico sigue existiendo

### Implementación de la Historia de Usuario 1

- [X] T012 [P] [US1] Crear `DeletePropertyCommand` (`RawId`, `Guid? Id`, `static Create(string? rawId)` con `Guid.TryParseExact(rawId, "D", ...)`) en `app/backend/src/RealtorApi/Features/Properties/DeleteProperty/DeletePropertyCommand.cs` replicando `GetPropertyByIdQuery`
- [X] T013 [P] [US1] Crear `DeletePropertyValidator : AbstractValidator<DeletePropertyCommand>` con reglas `NotEmpty` ("id es obligatorio.") y UUID canónico ("id debe ser un UUID canónico.") en `app/backend/src/RealtorApi/Features/Properties/DeleteProperty/DeletePropertyValidator.cs`
- [X] T014 [US1] Crear `DeletePropertyHandler : IHandler` en `app/backend/src/RealtorApi/Features/Properties/DeleteProperty/DeletePropertyHandler.cs`: `FindAsync` por `Id`; si es `null`, registrar `LogInformation` y devolver resultado NotFound con `ProblemDetails` ("Propiedad no encontrada."); si existe, `Remove` + `SaveChangesAsync`; capturar `DbUpdateConcurrencyException` y devolver NotFound (R-02); registrar `LogInformation` de eliminación con `PropertyId`; no modificar `ImageUrl` ni archivos (depende de T012)
- [X] T015 [US1] Crear `DeletePropertySlice : ISlice` en `app/backend/src/RealtorApi/Features/Properties/DeleteProperty/DeletePropertySlice.cs` con `app.MapDelete("/api/properties/{id}", HandleAsync).WithName("DeleteProperty").Produces(StatusCodes.Status204NoContent).ProducesValidationProblem().ProducesProblem(StatusCodes.Status404NotFound)`; validar con `IValidator<DeletePropertyCommand>` devolviendo 400 `ValidationProblemDetails` con `LogWarning`; devolver `TypedResults.NoContent()` o `TypedResults.Problem(...)` (depende de T013, T014)
- [X] T016 [US1] Regenerar el documento publicado `app/backend/src/RealtorApi/wwwroot/openapi/v1.json` con la operación `DeleteProperty` alineada a `contracts/delete-property.openapi.yaml`, usando el mismo procedimiento de specs anteriores (spec 008) (depende de T015)
- [X] T017 [US1] Ejecutar `dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj` y confirmar en verde T010, T011 y `OpenApiDriftTests` (depende de T016)

**Checkpoint**: API de eliminación funcional y verificable de forma independiente.

---

## Fase 4: Historia de Usuario 2 - Eliminar desde el listado con confirmación (Prioridad: P1)

**Objetivo**: Acción Eliminar en cada tarjeta, confirmación modal, aviso de éxito y recarga paginada (RF-08, RF-09, RF-10, RF-14).

**Prueba independiente**: Comprobaciones U1, U2 y U3 de `quickstart.md`.

- [X] T018 [US2] Agregar en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyCard.razor` un botón "Eliminar" con ícono Lucide `trash-2` y un parámetro `EventCallback OnDeleteRequested` que se invoca al hacer clic sin navegar a la ficha
- [X] T019 [US2] En `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor.cs`, inyectar `PropertyNoticeService`, agregar estado `pendingDelete` (id y título), `IsDeleteDialogOpen`, `currentNotice`; en `OnInitializedAsync` consumir el aviso con `Consume()`; implementar `HandleDeletedAsync` que fija el aviso "La propiedad fue eliminada correctamente.", cierra el diálogo, invoca `LoadPageAsync(currentPage)` y, si la página resultante no tiene elementos y `currentPage > 1`, invoca `LoadPageAsync(currentPage - 1)` (RF-14)
- [X] T020 [US2] En `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor`, conectar `OnDeleteRequested` de cada `PropertyCard`, renderizar un único `PropertyDeleteDialog` con la propiedad pendiente y `OnCancel` que lo cierra sin solicitud, y mostrar el banner `.property-notice` con el aviso actual y botón para descartarlo (depende de T019)
- [X] T021 [US2] Ejecutar validación manual U1, U2 y U3 de `specs/017-blazor-delete-property/quickstart.md` y registrar la evidencia (depende de T017, T020)

**Checkpoint**: MVP completo (US1 + US2).

---

## Fase 5: Historia de Usuario 3 - Eliminar desde la página de detalle (Prioridad: P2)

**Objetivo**: Acción Eliminar en el detalle con navegación al listado y aviso de éxito (RF-10, RF-11).

**Prueba independiente**: Comprobación U4 de `quickstart.md`.

- [X] T022 [US3] En `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor.cs`, inyectar `PropertyNoticeService`, agregar `IsDeleteDialogOpen` y `HandleDeletedAsync` que fija el aviso de éxito y ejecuta `Navigation.NavigateTo("/")`
- [X] T023 [US3] En `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor`, agregar el botón "Eliminar" con ícono Lucide `trash-2` visible solo cuando la propiedad está cargada y el `PropertyDeleteDialog` con `OnCancel` que lo cierra permaneciendo en el detalle (depende de T022)
- [X] T024 [US3] Ejecutar validación manual U4 y U8 de `specs/017-blazor-delete-property/quickstart.md` y registrar la evidencia (depende de T023)

**Checkpoint**: Eliminación disponible en ambas ubicaciones.

---

## Fase 6: Historia de Usuario 4 - Manejo de errores y envíos duplicados (Prioridad: P2)

**Objetivo**: Error dentro del diálogo con reintento (RF-15) y tratamiento de 404 como "La propiedad ya no existe." (RF-16).

**Prueba independiente**: Comprobaciones U5, U6 y U7 de `quickstart.md`.

- [X] T025 [US4] En `app/frontend/src/RealtorWeb/Features/Properties/Delete/PropertyDeleteDialog.razor.cs`, capturar `ApiException` con `StatusCode == HttpStatusCode.NotFound` y emitir `OnNotFound`; capturar otras `ApiException`, `HttpRequestException` y `TaskCanceledException` pasando a estado `Error` con mensaje "No se pudo eliminar la propiedad. Inténtalo de nuevo." manteniendo el diálogo abierto y reactivando "Eliminar" (reintento) y "Cancelar"
- [X] T026 [US4] En `app/frontend/src/RealtorWeb/Features/Properties/Delete/PropertyDeleteDialog.razor`, mostrar el mensaje de error dentro del diálogo con `role="alert"` cuando el estado sea `Error` (depende de T025)
- [X] T027 [P] [US4] En `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor(.cs)`, conectar `OnNotFound` con un manejador que fija el aviso `Info` "La propiedad ya no existe.", cierra el diálogo y aplica la misma recarga de T019 (depende de T025)
- [X] T028 [P] [US4] En `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor(.cs)`, conectar `OnNotFound` con un manejador que fija el aviso `Info` "La propiedad ya no existe." y navega a `/` (depende de T025)
- [X] T029 [US4] Ejecutar validación manual U5, U6 y U7 de `specs/017-blazor-delete-property/quickstart.md` y registrar la evidencia (depende de T026, T027, T028)

**Checkpoint**: Todas las historias funcionales e independientes.

---

## Fase 7: Pulido y aspectos transversales

- [X] T030 Ejecutar `dotnet build app/Realtor.sln` sin errores ni advertencias nuevas y `dotnet test` de backend y frontend en verde; registrar resultado en `specs/017-blazor-delete-property/quickstart.md`
- [X] T031 [P] Revisar que no se hayan agregado dependencias nuevas en `app/backend/src/RealtorApi/RealtorApi.csproj` ni en `app/frontend/src/RealtorWeb/RealtorWeb.csproj`, ni migraciones en `app/backend/src/RealtorApi/Migrations/`
- [X] T032 Completar la sección "Evidencia de validación" de `specs/017-blazor-delete-property/quickstart.md` con los resultados de A1–A5 y U1–U8

---

## Dependencias y orden de ejecución

### Dependencias entre fases

- **Setup (Fase 1)**: sin dependencias.
- **Fundacional (Fase 2)**: depende de Setup; bloquea US2, US3 y US4.
- **US1 (Fase 3)**: depende solo de Setup; puede ejecutarse en paralelo con la Fase 2.
- **US2 (Fase 4)**: depende de Fase 2; la validación manual T021 requiere US1.
- **US3 (Fase 5)**: depende de Fase 2; independiente de US2 (archivos distintos).
- **US4 (Fase 6)**: depende de Fase 2; T027 depende de T019 y T028 de T022.
- **Pulido (Fase 7)**: depende de todas las historias.

### Orden de historias

```text
Setup → (US1 ∥ Fundacional) → US2 → US3 → US4 → Pulido
													└──→ US3 (puede ir en paralelo con US2)
```

## Ejemplos de paralelismo

### US1

```text
T010 DeletePropertyValidatorTests.cs
T011 DeletePropertyEndpointTests.cs
T012 DeletePropertyCommand.cs
T013 DeletePropertyValidator.cs
```

### Fundacional

```text
T003 IPropertiesApi.cs
T004 PropertyNotice.cs
T008 app.css
T009 _Imports.razor
```

### US4

```text
T027 PropertyList (OnNotFound)
T028 PropertyDetail (OnNotFound)
```

## Estrategia de implementación

### MVP primero

1. Fase 1 y Fase 3 (US1): API verificable con pruebas.
2. Fase 2 y Fase 4 (US2): eliminación desde el listado.
3. **Detener y validar**: U1–U3 y A1–A5.

### Entrega incremental

1. US1 → API lista.
2. US2 → MVP de interfaz.
3. US3 → eliminación desde detalle.
4. US4 → robustez ante errores y 404.
5. Pulido → evidencia completa para cierre del spec.
