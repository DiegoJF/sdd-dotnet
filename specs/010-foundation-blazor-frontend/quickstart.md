# Quickstart de Validación: Foundation Blazor Frontend

## Objetivo

Validar que `RealtorWeb` queda con fundación frontend propia: sin Bootstrap, con layout desktop first, sistema de íconos Lucide y andamiaje base Refit sin lógica de negocio.

## Prerrequisitos

- .NET SDK alineado con `global.json`.
- Restore exitoso de paquetes NuGet.
- Repositorio en la raíz `c:\sdd`.

## Escenario 1: Restaurar y compilar frontend

1. Ejecutar:

```powershell
dotnet restore app/frontend/src/RealtorWeb/RealtorWeb.csproj
dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj
```

2. Resultado esperado:
- Restauración y compilación en verde (`exit code 0`).
- Sin errores de referencias faltantes tras eliminar Bootstrap.

## Escenario 2: Verificar eliminación total de Bootstrap

1. Confirmar que `Components/App.razor` no referencia `lib/bootstrap/dist/css/bootstrap.min.css`.
2. Confirmar que no existe `wwwroot/lib/bootstrap/`.
3. Levantar app y revisar recursos cargados en navegador.

Resultado esperado:
- Cero CSS de framework externo cargado.
- Solo estilos propios (`app.css` y recursos del proyecto).

## Escenario 3: Verificar layout desktop first

1. Iniciar frontend:

```powershell
dotnet run --project app/frontend/src/RealtorWeb/RealtorWeb.csproj
```

2. Abrir la app en desktop.
3. Validar sidebar izquierda estable + contenido principal amplio.
4. Redimensionar a tablet y móvil.

Resultado esperado:
- Desktop: shell principal con sidebar y área principal clara.
- Tablet: navegación compacta usable.
- Móvil: navegación colapsada/drawer usable.

## Escenario 4: Verificar sistema de íconos Lucide

1. Revisar dependencias del proyecto para confirmar paquete Lucide integrado.
2. Revisar componentes de navegación/acciones base para confirmar uso de íconos Lucide.
3. Verificar clases `.icon`, `.icon-sm`, `.icon-md`, `.icon-lg` en `wwwroot/app.css`.

Resultado esperado:
- Lucide es la fuente principal de íconos.
- Íconos heredan color por `currentColor` y tamaños por clases CSS.
- No hay Bootstrap Icons ni librerías alternativas activas.

## Escenario 5: Verificar andamiaje base Refit

1. Revisar `Program.cs` para confirmar registro con `AddRefitClient<T>()` y `IHttpClientFactory`.
2. Revisar estructura de interfaces API por módulo/feature.
3. Confirmar que componentes Razor no usan `HttpClient` directo ni `RestService.For<T>()`.

Resultado esperado:
- Registro base de comunicación tipada disponible para features futuras.
- Sin endpoints de negocio implementados en esta spec.

## Referencias de diseño

- Spec funcional: `spec.md`
- Investigación: `research.md`
- Modelo conceptual: `data-model.md`
- Contrato de fundación: `contracts/frontend-foundation.contract.yaml`

## Evidencia de validacion

Fecha de ejecucion: 2026-07-22

### 1. Restauracion y compilacion

Comando ejecutado:

```powershell
dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj
```

Resultado obtenido:
- Build en verde (`exit code 0`).
- Ensamblado generado: `app/frontend/src/RealtorWeb/bin/Debug/net11.0/RealtorWeb.dll`.

### 2. Eliminacion de Bootstrap y CSS de terceros

Validaciones ejecutadas:
- Se removio la referencia a `lib/bootstrap/dist/css/bootstrap.min.css` en `Components/App.razor`.
- Se elimino el directorio `wwwroot/lib/bootstrap/`.
- Busqueda global en frontend por `bootstrap|Bootstrap` sin coincidencias.

Resultado obtenido:
- `0 CSS de terceros` activo en el frontend.

### 3. Verificacion de Lucide

Validaciones ejecutadas:
- Paquete `Lucide.Blazor` agregado en `RealtorWeb.csproj`.
- Navegacion con componentes `<Icon />` en `Components/Layout/NavMenu.razor`.
- Clases `.icon`, `.icon-sm`, `.icon-md`, `.icon-lg` y estrategia `currentColor` definidas en `wwwroot/app.css`.

Resultado obtenido:
- `Lucide activo` como sistema principal de iconos.

### 4. Verificacion base Refit

Validaciones ejecutadas:
- Configuracion `RealtorApi:BaseUrl` agregada en `appsettings.json` y `appsettings.Development.json`.
- Registro tipado con `AddRefitClient<T>()` en `Program.cs` via `AddRealtorApiClients(...)`.
- Interfaces base creadas por modulo:
	- `Services/Api/System/IBackendHealthApi.cs`
	- `Services/Api/Properties/IPropertiesApi.cs`

Resultado obtenido:
- Bootstrap de comunicacion tipada disponible para specs futuras.

### 5. Auditoria HttpClient directo / RestService.For

Comprobacion ejecutada:
- Busqueda en `app/frontend/src/RealtorWeb/Components/` por `HttpClient|RestService.For`.

Resultado obtenido:
- Sin hallazgos.
- No se requirieron correcciones adicionales sobre componentes (T026 cerrado sin cambios funcionales).

### 6. Fuente visual unica efectiva

Comprobacion ejecutada:
- `wwwroot/app.css` concentra tokens, layout, componentes, estados y responsive.
- `Components/Layout/MainLayout.razor.css` y `Components/Layout/NavMenu.razor.css` quedaron sin reglas activas (solo comentario de neutralizacion).

Resultado obtenido:
- `wwwroot/app.css` es la unica fuente visual efectiva.

### 7. Control de alcance no-CRUD

Comprobacion ejecutada:
- Revisados componentes de paginas/layout y clientes API de fundacion.
- No se introdujeron pantallas ni flujos CRUD de propiedades en la UI.

Resultado obtenido:
- Alcance preservado segun `spec.md` y `tasks.md`.

### 8. Matriz responsive

| Resolucion | Caso minimo | Esperado | Obtenido |
|------------|-------------|----------|----------|
| Desktop (1024px+) | Sidebar estable + contenido amplio | Shell en dos columnas (`.app-sidebar` + `.app-main`) | Cumple |
| Tablet (768-1023px) | Navegacion compacta | Sidebar en modo superior con toggle, contenido fluido | Cumple |
| Movil (<768px) | Navegacion drawer colapsada | Sidebar off-canvas + overlay + cierre al seleccionar opcion | Cumple |

### 9. Consistencia de contrato

Archivo validado: `contracts/frontend-foundation.contract.yaml`

Resultado obtenido:
- Clases requeridas del shell presentes.
- Politicas de iconos y API bootstrap alineadas con implementacion.
- Reglas de limpieza (bootstrap + CSS por componente) cumplidas.
