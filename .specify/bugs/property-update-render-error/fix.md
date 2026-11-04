# Bug Fix: Render inválido en PropertyUpdateForm por `return;` dentro del markup

- **Slug**: property-update-render-error
- **Fixed**: 2026-07-25
- **Assessment**: ./assessment.md
- **Status**: applied

## Summary

Se eliminaron los dos `return;` que estaban dentro del markup de `PropertyUpdateForm.razor` y
se reestructuraron los estados como ramas mutuamente excluyentes (`@if / else if / else`). Así,
los elementos envolventes `<article>` y `<div class="card-body">` siempre se cierran y el árbol
de render queda válido, resolviendo `InvalidOperationException: A frame of type 'Element' was
left unclosed`.

## Changes

| File | Change | Notes |
|------|--------|-------|
| `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor` | modified | Se quitaron los `return;` del markup; los estados `LoadingInitialData` y `LoadFailed` ahora son ramas `@if` / `else if`, y el resto (banner de submit, resumen de errores y `<EditForm>`) se envuelve en un `else` para los estados editables. |

## Diff Highlights

Antes (control de flujo inválido en el render):

```razor
@if (_state is PropertyUpdateStateType.LoadingInitialData)
{
    <PropertyUpdateState State="PropertyUpdateStateType.LoadingInitialData" Message="@_statusMessage" />
    return;
}

@if (_state is PropertyUpdateStateType.LoadFailed)
{
    <PropertyUpdateState State="PropertyUpdateStateType.LoadFailed" Message="@_statusMessage">
        ...
    </PropertyUpdateState>
    return;
}

@if (_state is PropertyUpdateStateType.Submitting or ...)
{ ... }
```

Después (ramas mutuamente excluyentes, sin salidas anticipadas):

```razor
@if (_state is PropertyUpdateStateType.LoadingInitialData)
{
    <PropertyUpdateState State="PropertyUpdateStateType.LoadingInitialData" Message="@_statusMessage" />
}
else if (_state is PropertyUpdateStateType.LoadFailed)
{
    <PropertyUpdateState State="PropertyUpdateStateType.LoadFailed" Message="@_statusMessage">
        ...
    </PropertyUpdateState>
}
else
{
    @if (_state is PropertyUpdateStateType.Submitting or ...)
    { ... }

    @if (_submitSummaryErrors.Count > 0) { ... }

    <EditForm ...> ... </EditForm>
}
```

## Tests Added or Updated

- Ninguno en este cambio. Ver **Deviations from Assessment** para el motivo de gobernanza y
  **Follow-ups** para la prueba de regresión recomendada.

## Local Verification

- Comando: `dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj -c Debug` → **Build succeeded** (net11.0).
- Diagnóstico del editor sobre `PropertyUpdateForm.razor` → sin errores.
- Verificación manual pendiente de repetir por la persona operadora (ver escenario más abajo).

## Deviations from Assessment

- El assessment recomendó agregar una prueba de render (bUnit) que ejercite todos los estados.
  No se agregó en este fix porque:
  1. El proyecto de pruebas del frontend (`app/frontend/test/RealtorWeb`) usa solo xUnit y **no**
     referencia bUnit; incorporarlo es una decisión de tooling nueva.
  2. La constitución exige que todo artefacto (incluidas dependencias y pruebas nuevas) esté
     trazado a una tarea en `tasks.md`, y no existe tarea que habilite agregar bUnit.
  3. `tasks.md` de `016-blazor-update-property` declara explícitamente que la spec no exige TDD ni
     pruebas automatizadas nuevas.
  Por lo anterior, la prueba de regresión se traslada a **Follow-ups** para que pase por el flujo
  spec-driven (crear/actualizar tarea antes de introducir la dependencia y el test).

## Follow-ups

- Crear una tarea en `tasks.md` (o una spec de calidad) para agregar bUnit al proyecto de pruebas
  del frontend y una prueba de render de `PropertyUpdateForm` que verifique, sin excepción, cada
  estado: `LoadingInitialData`, `LoadFailed`, `Editing`, `Submitting`, `SubmitSucceeded`,
  `SubmitFailed` (incluyendo casos con `IPropertiesApi` devolviendo detalle válido y `404`).
- Añadir a `plan.md`/convenciones frontend la regla: "No usar `return;`, `try/catch` ni control de
  flujo que interrumpa el árbol de render dentro del markup de componentes Razor".
- Revisar por qué la validación de `quickstart.md` cerró la spec como *Implementada* sin detectar
  que la ruta `/properties/{id}/edit` no renderizaba.

## Escenario de prueba manual a repetir

1. Iniciar frontend + backend.
2. Abrir listado, seleccionar **Editar** en una propiedad existente.
3. Confirmar que se muestra el estado de carga y luego el formulario con los datos actuales.
4. Forzar un `id` inexistente y confirmar el estado `LoadFailed` con acciones Reintentar/Volver.
5. Editar campos válidos, guardar y confirmar navegación al detalle actualizado.
