# Quickstart de validación: Feature 013

## Objetivo

Validar de extremo a extremo que la interactividad de servidor queda habilitada solo en páginas host requeridas y que la experiencia funcional de navegación/reintento opera sin recarga completa del navegador.

## Prerrequisitos

- .NET 11 SDK instalado.
- Backend Realtor corriendo y accesible por el frontend.
- Frontend RealtorWeb compilable en entorno local.

## Preparación

1. Restaurar dependencias:

```powershell
dotnet restore app/frontend/src/RealtorWeb/RealtorWeb.csproj
```

2. Compilar frontend:

```powershell
dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj -c Debug
```

3. Ejecutar la app frontend según el flujo local del equipo.

## Escenarios de validación

## Escenario 1: Paginación interactiva en listado

1. Abrir la ruta `/`.
2. Pulsar `Siguiente`.
3. Verificar que se muestran nuevos resultados sin recarga completa de la página.
4. Pulsar `Anterior`.
5. Verificar retorno a la página previa sin recarga completa.
6. Verificar estado `disabled` de `Anterior`/`Siguiente` conforme a `CanGoPrevious` y `CanGoNext`.

Resultado esperado:
- Navegación de paginación funcional con actualización de UI en circuito interactivo.

## Escenario 2: Retorno desde detalle al listado

1. Desde listado, abrir un detalle de propiedad.
2. Pulsar `Volver al listado` en estado con datos.
3. Repetir validación en estado `NotFound`.
4. Repetir validación en estado `Error`.

Resultado esperado:
- En todos los casos, la acción vuelve a `/` en un solo paso.

## Escenario 3: Reintento en estados de error

1. Forzar un estado de error en listado.
2. Pulsar `Reintentar` y confirmar nueva carga.
3. Forzar un estado de error en detalle.
4. Pulsar `Reintentar` y confirmar nueva carga.

Resultado esperado:
- `Reintentar` dispara nueva solicitud de carga en ambas superficies.

## Escenario 4: Confirmación de páginas estáticas

1. Abrir ruta de error de aplicación (`/Error`).
2. Abrir ruta de no encontrado (`/not-found`).
3. Confirmar que estas páginas no dependen de interactividad de servidor para renderizar su contenido.

Resultado esperado:
- `Error.razor` y `NotFound.razor` permanecen en Static SSR.

## Criterio de salida

La feature se considera validada cuando todos los escenarios pasan y no se detectan cambios en backend, contratos Refit ni lógica existente fuera de las páginas host de interactividad.

## Evidencia de validación

**Fecha**: 2026-07-23

### Compilación (T017)

```powershell
dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj -v minimal
```

Resultado:

```text
RealtorWeb net11.0 succeeded (3.8s) → app\frontend\src\RealtorWeb\bin\Debug\net11.0\RealtorWeb.dll
Build succeeded in 5.1s
```

### Activación de interactividad (US1/US2)

- `Components/Pages/Home.razor`: se declaró `@rendermode InteractiveServer` tras `@page "/"`. Habilita el circuito interactivo para paginación (Anterior/Siguiente) del listado.
- `Components/Pages/PropertyDetailPage.razor`: se declaró `@rendermode InteractiveServer` tras `@page "/properties/{Id}"`. Habilita el circuito interactivo para Volver al listado y Reintentar en detalle.
- La directiva `InteractiveServer` resuelve mediante `@using static Microsoft.AspNetCore.Components.Web.RenderMode` presente en `Components/_Imports.razor`.

### Estado disabled de paginación (Escenario 1)

- `Features/Properties/List/PropertyList.razor.cs` conserva `CanGoPrevious => response?.HasPrevious == true` y `CanGoNext => response?.HasNext == true`, y `NextAsync`/`PreviousAsync` respetan esas guardas. Sin cambios funcionales; la vista deshabilita Anterior/Siguiente según dichas propiedades.

### Retorno al listado (Escenario 2)

- `Features/Properties/Detail/PropertyDetail.razor.cs` mantiene `BackToList()` navegando a `/`.
- `Features/Properties/Detail/PropertyDetailState.razor` expone `Actions` (que incluye Volver al listado) en los estados `NotFound` y `Error`; el estado con datos también dispone de la acción.

### Reintento en estados de error (Escenario 3)

- Listado: `ReloadAsync() => LoadPageAsync(currentPage)` en `PropertyList.razor.cs` relanza la carga desde el host interactivo `Home.razor`.
- Detalle: `ReloadAsync() => LoadAsync()` en `PropertyDetail.razor.cs` relanza la carga desde el host interactivo `PropertyDetailPage.razor`.

### No regresión de alcance (Escenario 4 / Polish)

- `Components/App.razor` y `Components/Routes.razor`: sin `@rendermode` global; no hay activación interactiva global.
- `Components/Pages/Error.razor` y `Components/Pages/NotFound.razor`: permanecen en Static SSR (sin `@rendermode`).
- Backend (`app/backend/src/`): sin cambios en esta feature.
- Contrato Refit (`Services/Api/Properties/IPropertiesApi.cs`): sin cambios.
- `Program.cs`: configuración `AddInteractiveServerComponents()` + `AddInteractiveServerRenderMode()` ya presente; no se modificó.

### Revalidación final de cierre (T020)

- Reconfirmado tras completar US1, US2 y US3 que `App.razor` y `Routes.razor` no habilitan interactividad global. El cambio funcional queda limitado a las dos páginas host (`Home.razor`, `PropertyDetailPage.razor`).
