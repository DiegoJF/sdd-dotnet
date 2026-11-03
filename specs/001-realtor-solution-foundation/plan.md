# Plan de Implementación: Fundación de la Solución Realtor

**Rama**: `001-realtor-solution-foundation` (sin rama Git dedicada detectada) | **Fecha**: 2026-11-03 | **Especificación**: [spec.md](./spec.md)

**Entrada**: Fundación de la solución Realtor, sin lógica de negocio, funcionalidades de producto ni entidades de dominio.

## Resumen

Crear una única solución `app/Realtor.sln` que contenga los proyectos de backend, frontend y sus pruebas en las rutas aprobadas. El backend usará ASP.NET Core Minimal APIs y una organización preparada para slices verticales; el frontend será una Blazor Web App con Razor Components vacía. La solución debe compilar con el SDK fijado en `global.json`, sin modificar ese archivo ni incorporar ejemplos funcionales de las plantillas.

## Contexto Técnico

- **Lenguaje y versión**: C# con TFM `net11.0`; SDK exacto `11.0.100-preview.5.26302.115`, tomado de `global.json`.
- **Dependencias principales**: ASP.NET Core Minimal APIs, Blazor Web App con Razor Components y xUnit para ambos proyectos de pruebas.
- **Persistencia**: PostgreSQL es el motor obligatorio del proyecto; esta fundación no define entidades, esquema ni migraciones.
- **Pruebas**: Solo pruebas unitarias, conforme a la constitución. Los proyectos iniciales deben restaurar, compilar y ser ejecutables por `dotnet test`.
- **Plataforma**: ASP.NET Core y navegador; desarrollo local en Windows y despliegue no limitado a Windows.
- **Tipo de proyecto**: Solución web full-stack con dos aplicaciones y dos proyectos de pruebas.
- **Rendimiento**: Sin objetivos de rendimiento de producto en esta fase.
- **Restricciones**: Mantener exactamente los cuatro proyectos de aplicación/pruebas solicitados; no crear lógica de negocio, entidades, migraciones, endpoints de producto ni páginas de demostración. No modificar `global.json`.
- **Alcance**: Una solución, un backend Minimal API, una Blazor Web App y sus dos proyectos de pruebas.

## Investigación y Decisiones

1. **SDK y TFM**: Se deriva la versión exclusivamente de `global.json`. Se elige `net11.0`; no se usa una versión estable distinta ni se cambia el pin del repositorio. La disponibilidad local del SDK exacto es un prerrequisito de compilación.
2. **Formato de solución**: Se usa el formato `.sln` solicitado explícitamente, indicando `--format sln` al crear la solución para no depender del formato predeterminado del SDK.
3. **Backend**: Se parte de la plantilla `webapi` con `--use-minimal-apis` explícito. Se conserva OpenAPI para describir la superficie técnica y se elimina cualquier endpoint o modelo de demostración que la plantilla genere. No se crean controllers. La guía oficial confirma que Minimal APIs se organizan con `WebApplication`, servicios, middleware y mapeo de rutas; la lógica de casos de uso seguirá slices verticales cuando una spec futura los introduzca.
4. **Frontend**: Se parte de la plantilla `blazor` con `--empty --interactivity None`. La documentación oficial confirma que `--empty` omite páginas y estilos de ejemplo y `None` produce renderizado estático del servidor. Se evita añadir un proyecto cliente o un modo interactivo antes de que exista un caso de uso.
5. **Estilos e iconos**: El CSS propio vive en `app/frontend/src/RealtorWeb/wwwroot/app.css`; no se conserva Bootstrap ni se agrega un framework CSS. Lucide será el sistema de iconos cuando una futura interfaz necesite iconografía; la fundación vacía no añade iconos de muestra.
6. **Pruebas**: Se elige xUnit en ambos proyectos por su plantilla oficial y soporte directo de `dotnet test`. No se conservan pruebas de ejemplo sin valor. Solo se permitirán pruebas unitarias; no se incorporan pruebas de integración.
7. **Errores y logging**: Registrar `AddProblemDetails` y configurar `UseExceptionHandler` como manejo base de errores. Usar `ILogger` de ASP.NET Core con salida estructurada en consola; no agregar un proveedor externo ni lógica de errores de dominio.
8. **Integraciones aún sin consumidor**: No hay entidades, contratos HTTP de producto ni validadores en esta iniciativa. Por eso no se registran un `DbContext`, clientes Refit ni validadores vacíos. EF Core/Npgsql, Refit con `IHttpClientFactory` y FluentValidation siguen siendo elecciones obligatorias al introducir sus respectivos consumidores en una spec aprobada; no se inventan interfaces, datos ni llamadas externas para esta base.

Alternativas consideradas: plantilla API con controllers (rechazada por constitución y spec); Blazor interactivo Server/WebAssembly (diferido porque no hay interacción solicitada); conservar las páginas de ejemplo de las plantillas (rechazado porque introduciría funcionalidades fuera de alcance); crear artefactos de diseño separados (rechazado para cumplir la regla de los tres archivos canónicos).

Fuentes consultadas: [referencia oficial de Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-11.0), [estructura de proyectos Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/project-structure?view=aspnetcore-11.0) y [opciones de las plantillas del SDK](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-new-sdk-templates).

## Verificación de Constitución

### Antes del diseño

- **Solución única**: Cumple; la solución principal es `app/Realtor.sln`.
- **Spec-Driven Development**: Cumple; esta iniciativa conserva únicamente `spec.md`, `plan.md` y `tasks.md`. La investigación, el modelo, los contratos y la guía de validación quedan consolidados en este plan en vez de archivos auxiliares.
- **Arquitectura**: Cumple; Minimal APIs sin controllers, con `Program.cs` limitado a servicios, middleware, infraestructura aplicable y mapeo técnico de endpoints. Blazor Web App usa Razor Components.
- **Stack obligatorio**: Cumple para las capacidades aplicables a una fundación vacía: .NET 11, Minimal APIs, Blazor, CSS propio, ProblemDetails y logging estructurado. PostgreSQL/Npgsql, EF Core, Refit, FluentValidation y Lucide no se fuerzan con artefactos ficticios cuando aún no hay datos, consumidor HTTP, validadores ni iconos; la primera iniciativa que los requiera deberá usarlos.
- **Calidad**: Cumple; no se crean entidades ni lógica de dominio y las pruebas quedan limitadas a pruebas unitarias.
- **Idioma**: Cumple; la documentación de la iniciativa se mantiene en español, conservando nombres técnicos propios.

### Después del diseño

El árbol mantiene los cuatro proyectos solicitados y no añade proyectos compartidos ni soluciones paralelas. El alcance no introduce excepciones arquitectónicas; las integraciones diferidas se limitan a capacidades sin consumidor en esta fundación. La única salida documental de planificación es `plan.md`, en cumplimiento de la estructura canónica.

## Estructura del Proyecto

```text
specs/001-realtor-solution-foundation/
├── spec.md
├── plan.md
└── tasks.md

app/
├── Realtor.sln
├── backend/
│   ├── src/RealtorApi/RealtorApi.csproj
│   └── tests/RealtorApiTests/RealtorApiTests.csproj
└── frontend/
    ├── src/RealtorWeb/RealtorWeb.csproj
    └── test/RealtorWeb/RealtorWeb.Tests.csproj
```

**Decisión de estructura**: Los cuatro proyectos quedan dentro de la solución única. Cada proyecto de pruebas referencia únicamente su aplicación correspondiente. Backend y frontend no tienen referencia de proyecto entre sí; su comunicación futura será mediante HTTP tipado con Refit cuando una spec defina el contrato. No se crea un proyecto de dominio ni de infraestructura independiente en esta fundación.

## Modelo de Datos

No hay entidades de dominio ni datos de producto. No se crean tablas, migraciones, seeders ni un modelo relacional inicial. La elección de PostgreSQL/Npgsql queda establecida por la constitución, pero su configuración concreta se incorpora con la primera spec que defina datos persistidos.

## Contratos e Interfaces

No se expone ningún contrato HTTP de producto ni cliente externo. El único endpoint técnico previsto es el documento OpenAPI generado en entorno de desarrollo, sin operaciones de producto. La solución registra los cuatro proyectos y las referencias de pruebas indicadas; no se introduce una interfaz Refit vacía.

## Validación de la Solución

Prerrequisito: tener instalado el SDK `11.0.100-preview.5.26302.115` declarado por `global.json`, acceso a las fuentes NuGet y un repositorio sin archivos de destino que deban sobrescribirse.

```powershell
dotnet sln app/Realtor.sln list
dotnet build app/Realtor.sln
dotnet test app/Realtor.sln
```

Resultado esperado: la solución enumera los cuatro proyectos; restauración, compilación y ejecución de pruebas finalizan correctamente. La revisión de estructura confirma que no hay controllers, endpoints de producto, entidades de dominio, páginas `Counter`/`Weather`, Bootstrap ni migraciones. `Program.cs` contiene solo configuración base y el mapeo técnico definido en tareas.

## Complejidad

No hay proyectos adicionales ni patrones de infraestructura genéricos. La ausencia de integraciones sin consumidor está justificada por el alcance fundacional; no se crean stubs para satisfacer dependencias nominalmente. Toda configuración concreta futura deberá quedar trazada a una tarea aprobada. No se identifican violaciones constitucionales pendientes.
