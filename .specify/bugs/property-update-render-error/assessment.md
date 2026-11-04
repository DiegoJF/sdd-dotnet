# Bug Assessment: Render inválido en PropertyUpdateForm por `return;` dentro del markup

- **Slug**: property-update-render-error
- **Created**: 2026-07-25
- **Source**: pasted text (prueba manual de la feature `016-blazor-update-property`, incluye stack trace)
- **Verdict**: valid
- **Severity**: high

## Reporte

Durante la prueba manual de la edición de propiedades, al navegar al formulario de
actualización la página no se renderiza y se produce una excepción no controlada:

```text
System.InvalidOperationException: Render output is invalid for component of type
'RealtorWeb.Features.Properties.Update.PropertyUpdateForm'. A frame of type 'Element'
was left unclosed. Do not use try/catch inside rendering logic, because partial output
cannot be undone.
   at Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder.AssertTreeIsValid(...)
   ...
   at Microsoft.AspNetCore.Components.ComponentBase.CallOnParametersSetAsync()
```

La traza confirma que la excepción ocurre durante el primer render disparado por
`OnParametersSetAsync` → `StateHasChanged`.

## Síntoma

- **Observado**: La página `/properties/{id}/edit` lanza `InvalidOperationException`
  ("A frame of type 'Element' was left unclosed") durante el render y no puede mostrarse.
- **Esperado**: `PropertyUpdateForm` renderiza el estado de carga, luego carga los datos
  actuales del inmueble y muestra el formulario editable (FR-003, FR-011).

## Reproducción

1. Abrir el listado de propiedades.
2. Seleccionar **Editar** en una tarjeta existente.
3. Navegar a la página de actualización (`/properties/{id}/edit`).
4. La página falla en el render inicial antes de mostrar cualquier estado.

Nota: el fallo ocurre en el **primer** render porque el estado inicial del componente es
`PropertyUpdateStateType.LoadingInitialData`, que activa la rama de código defectuosa.

## Suspected Code Paths

- [PropertyUpdateForm.razor](app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor#L5-L9) — bloque `@if (_state is PropertyUpdateStateType.LoadingInitialData)` que contiene `return;` dentro del markup.
- [PropertyUpdateForm.razor](app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor#L11-L20) — bloque `@if (_state is PropertyUpdateStateType.LoadFailed)` que también contiene `return;`.
- Contexto envolvente: los elementos `<article class="card property-update-card">` y `<div class="card-body">` se abren antes de esos `@if`, y el resto del markup (incluido `<EditForm>`) se cierra después.

## Hipótesis de causa raíz

**Confianza: alta.**

El markup de un archivo `.razor` se compila en el método `BuildRenderTree(RenderTreeBuilder builder)`.
En [PropertyUpdateForm.razor](app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor#L3-L9) la estructura generada es, en esencia:

```csharp
builder.OpenElement(0, "article");     // <article ...>
builder.OpenElement(1, "div");         //   <div class="card-body">
if (_state is PropertyUpdateStateType.LoadingInitialData)
{
    // render <PropertyUpdateState .../>
    return;                            // <-- sale de BuildRenderTree
}
// ... resto del markup ...
builder.CloseElement();                //   </div>   (NUNCA se ejecuta)
builder.CloseElement();                // </article> (NUNCA se ejecuta)
```

El `return;` colocado dentro del `@if` **aborta `BuildRenderTree` mientras `<article>` y
`<div class="card-body">` siguen abiertos** (sus `CloseElement` quedan pendientes). Blazor
valida el árbol al final del render (`AssertTreeIsValid`) y detecta que hay un frame de tipo
`Element` sin cerrar, lanzando `InvalidOperationException`.

El texto del mensaje sobre "try/catch inside rendering logic" es una **guía genérica** del
framework: en este componente NO hay try/catch en el markup; el disparador real es el uso de
`return;` como control de flujo dentro del render, que produce salida parcial imposible de
deshacer. `PropertyUpdateForm.razor` es el único componente del feature que usa `return;` en
markup (los demás componentes de `Features/Properties/**` no lo hacen), lo que confirma que el
patrón es la desviación introducida en esta feature.

## Remediación propuesta

**Preferida**: Eliminar los dos `return;` del markup y expresar los estados como ramas
mutuamente excluyentes sin salidas anticipadas, de modo que los elementos envolventes siempre
se cierren. Dos opciones equivalentes:

- Reestructurar con `@if / else if / else` de forma que:
  - `LoadingInitialData` y `LoadFailed` rendericen solo el `<PropertyUpdateState>` (y sus acciones), y
  - el `<EditForm>` se renderice únicamente en los estados editables (`Editing`, `Submitting`, `SubmitSucceeded`, `SubmitFailed`),
  todo dentro de `<article><div class="card-body"> ... </div></article>`, que siempre se abren y cierran.
- Alternativa mínima: mantener la estructura actual pero cambiar cada `@if (...) { ...; return; }`
  por un `@if (...) { ... } else { ...resto... }`, garantizando que no exista un `return` que
  interrumpa el árbol.

Esto conserva el comportamiento funcional de la spec (US1/US3: mostrar carga, error recuperable
con Reintentar/Volver, y formulario editable) sin salida parcial del render.

**Archivos likely to change**:
- `app/frontend/src/RealtorWeb/Features/Properties/Update/PropertyUpdateForm.razor` (único cambio necesario para la corrección del render)

No se requiere cambiar el code-behind (`PropertyUpdateForm.razor.cs`) ni los modelos; la lógica
de estados ya es correcta, solo la proyección a markup es inválida.

**Tests to add or update**:
- Agregar prueba de render (bUnit) para `PropertyUpdateForm` que verifique que el componente
  renderiza **sin excepción** en cada estado: `LoadingInitialData`, `LoadFailed`, `Editing`,
  `Submitting`, `SubmitSucceeded`, `SubmitFailed`.
- Caso con `IPropertiesApi` mockeado devolviendo un detalle válido (verifica la transición
  carga → formulario) y otro devolviendo `404` (verifica el estado `LoadFailed` con acciones
  Reintentar/Volver), asegurando que ninguno lanza `InvalidOperationException`.

## Riesgos y consideraciones

- **Regresión de UX**: al reestructurar los estados, verificar que el resumen de errores de
  submit (`_submitSummaryErrors`) y el `<EditForm>` sigan mostrándose en los estados correctos
  (`SubmitFailed` debe conservar el formulario y los datos ingresados — FR-013, RV-009).
- **Prerender / SSR estático**: la traza muestra `EndpointHtmlRenderer` (render estático de
  endpoint). La corrección debe validarse tanto en el primer render estático como en el
  interactivo, ya que el fallo aparece en el render inicial.
- **Sin impacto en backend/contrato**: el endpoint `UpdateProperty` no está involucrado; el fallo
  es puramente de renderizado en el frontend.

## Gap Analysis (spec / plan / tasks)

- **spec.md**: Sin gap de requisitos; FR-003/FR-011 y las historias US1/US3 describen correctamente
  el comportamiento esperado. El defecto es de implementación, no de especificación.
- **tasks.md**: T013 ("Renderizar estados visuales de carga y contenido listo para edición") y T014
  ("fallback de carga fallida") están marcadas `[X]`, pero la implementación resultante rompe el
  render. Gap de **verificación**: la nota "Esta spec no exige TDD ni pruebas automatizadas nuevas"
  dejó el feature sin una prueba de render que hubiera detectado el `return;` en markup.
- **plan.md**: Recomendado incorporar como convención (o checklist) la regla "no usar `return;`,
  `try/catch` ni control de flujo que interrumpa el árbol dentro del markup de componentes Razor",
  para prevenir reincidencia.

## Escenario de prueba manual a repetir

1. Iniciar frontend + backend.
2. Abrir listado, seleccionar **Editar** en una propiedad existente.
3. Confirmar que se muestra el estado de carga y luego el formulario con los datos actuales.
4. Forzar un `id` inexistente y confirmar el estado `LoadFailed` con acciones Reintentar/Volver.
5. Editar campos válidos, guardar y confirmar navegación al detalle actualizado.

## Preguntas abiertas

- [NEEDS CLARIFICATION: ¿Cómo se registró evidencia de validación en `quickstart.md` que permitió
  cerrar la spec como "Implementada" si el formulario no renderiza? Conviene revisar si la
  validación manual documentada realmente ejercitó la ruta `/properties/{id}/edit`.]
