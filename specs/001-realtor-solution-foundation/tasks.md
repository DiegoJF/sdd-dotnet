# Tareas: Fundación de la Solución Realtor

**Entrada**: Documentos de diseño en `specs/001-realtor-solution-foundation/`

**Prerrequisitos**: `plan.md` y `spec.md`.

**Pruebas**: La spec requiere proyectos de pruebas y compilación correcta, pero no solicita escribir casos de prueba. Se crean proyectos xUnit vacíos y se ejecutan `dotnet build` y `dotnet test` como validación final.

**Organización**: Tareas agrupadas por historia de usuario y orden de prioridad. La investigación y las decisiones técnicas se consolidan en `plan.md`; no se crean documentos auxiliares.

## Fase 1: Preparación

**Propósito**: Confirmar el SDK de origen antes de crear proyectos.

- [x] T001 Verificar que `global.json` fija el SDK `11.0.100-preview.5.26302.115` y que `dotnet --version` lo resuelve, sin modificar `global.json`.

## Fase 2: Fundamentos Compartidos

**Propósito**: Crear la solución única que contendrá los cuatro proyectos.

- [x] T002 Crear la solución vacía en formato `.sln` en `app/Realtor.sln`.

**Punto de control**: La solución existe y está vacía antes de generar los proyectos.

## Fase 3: Historia de Usuario 1 - Crear la base única de solución (Prioridad: P1)

**Objetivo**: Crear una solución compilable con backend, frontend y sus proyectos de pruebas en las rutas aprobadas.

**Prueba independiente**: `dotnet sln app/Realtor.sln list` muestra exactamente los cuatro proyectos solicitados y cada archivo `.csproj` existe en su ruta canónica.

### Implementación

- [x] T003 [P] [US1] Crear el proyecto ASP.NET Core Web API con Minimal APIs explícitas y TFM `net11.0` en `app/backend/src/RealtorApi/RealtorApi.csproj`.
- [x] T004 [P] [US1] Crear el proyecto xUnit vacío de backend en `app/backend/tests/RealtorApiTests/RealtorApiTests.csproj`, sin conservar tests de plantilla.
- [x] T005 [P] [US1] Crear la Blazor Web App vacía, con `--empty` y `--interactivity None`, sin contenido de bienvenida en `app/frontend/src/RealtorWeb/RealtorWeb.csproj`.
- [x] T006 [P] [US1] Crear el proyecto xUnit vacío de frontend en `app/frontend/test/RealtorWeb/RealtorWeb.Tests.csproj`, sin conservar tests de plantilla.
- [x] T007 [P] [US1] Agregar los cuatro proyectos a `app/Realtor.sln` sin crear una segunda solución.
- [x] T008 [P] [US1] Agregar la referencia de `app/backend/tests/RealtorApiTests/RealtorApiTests.csproj` a `app/backend/src/RealtorApi/RealtorApi.csproj` y la referencia de `app/frontend/test/RealtorWeb/RealtorWeb.Tests.csproj` a `app/frontend/src/RealtorWeb/RealtorWeb.csproj`.
- [x] T009 [P] [US1] Asegurar que los estilos propios de la aplicación se centralizan en `app/frontend/src/RealtorWeb/wwwroot/app.css` y que `app/frontend/src/RealtorWeb/Components/App.razor` no referencia Bootstrap ni otro framework CSS.

**Punto de control**: Las rutas y referencias de los cuatro proyectos coinciden con `spec.md` y `plan.md`; las plantillas no agregan features de demostración.

## Fase 4: Historia de Usuario 2 - Asegurar arranque técnico mínimo sin negocio (Prioridad: P2)

**Objetivo**: Mantener el backend limitado a servicios, middleware y mapeo técnico, sin lógica de negocio, controllers ni entidades.

**Prueba independiente**: Inspeccionar `app/backend/src/RealtorApi/Program.cs` y confirmar que solo registra ProblemDetails, manejo global de errores y logging estructurado, configura el pipeline y mapea OpenAPI en desarrollo; no contiene endpoints de producto.

### Implementación

- [x] T010 [US2] Configurar `app/backend/src/RealtorApi/Program.cs` con servicios base, `AddProblemDetails`, `UseExceptionHandler`, logging estructurado JSON y mapeo de OpenAPI solo en desarrollo; retirar del mismo proyecto endpoints, modelos o archivos de demostración generados por la plantilla.

**Punto de control**: `Program.cs` no contiene lógica de producto; el proyecto no contiene controllers ni entidades de dominio.

## Fase 5: Historia de Usuario 3 - Dejar base lista para evolución por specs (Prioridad: P3)

**Objetivo**: Confirmar que las rutas y límites del diseño permiten continuar sobre una única solución sin reestructuración ni proyectos genéricos adicionales.

**Prueba independiente**: Comparar el grafo de proyectos de `app/Realtor.sln` y sus rutas con la estructura definida en `specs/001-realtor-solution-foundation/plan.md`.

### Verificación

- [x] T011 [US3] Verificar en `app/Realtor.sln` que existen solo los cuatro proyectos aprobados, que cada proyecto de pruebas referencia únicamente su aplicación y que no se añadieron proyectos de dominio o infraestructura fuera de alcance.

**Punto de control**: La solución puede servir como base de iniciativas posteriores sin introducir una segunda solución ni capas no aprobadas.

## Fase 6: Pulido y Validación Transversal

**Propósito**: Verificar compilación, ejecución de los proyectos de pruebas y alcance sin negocio.

- [x] T012 Ejecutar `dotnet sln app/Realtor.sln list`, `dotnet build app/Realtor.sln` y `dotnet test app/Realtor.sln`; confirmar además que `global.json` permanece sin cambios y no existen páginas de ejemplo, migraciones ni entidades de dominio.

## Dependencias y Orden de Ejecución

### Dependencias entre fases

- Preparación: T001 no depende de otras tareas.
- Fundamentos: T002 depende de T001 y bloquea la creación de proyectos.
- Historia de Usuario 1: T003-T006 dependen de T002; T007-T009 dependen de T003-T006.
- Historia de Usuario 2: T010 depende de T003 y se ejecuta después de completar la Historia de Usuario 1.
- Historia de Usuario 3: T011 depende de T007-T010.
- Pulido: T012 depende de todas las tareas anteriores.

### Orden entre historias

1. Historia de Usuario 1 (P1) establece la solución y los proyectos.
2. Historia de Usuario 2 (P2) configura el arranque del backend ya creado.
3. Historia de Usuario 3 (P3) verifica la estructura final antes de la validación transversal.

### Oportunidades de paralelismo

- Historia de Usuario 1: después de T002, T003, T004, T005 y T006 pueden ejecutarse en paralelo porque crean archivos en proyectos distintos.
- Historia de Usuario 1: después de generar los proyectos, T007, T008 y T009 pueden ejecutarse en paralelo porque modifican la solución, los proyectos de pruebas y los estilos, respectivamente.
- Historia de Usuario 2: T010 es la única tarea de esta historia; no hay paralelismo interno.
- Historia de Usuario 3: T011 es una verificación única y depende de la estructura completa.

### Ejemplo de ejecución paralela: Historia de Usuario 1

```text
En paralelo después de T002:
- T003: Crear backend Minimal API en app/backend/src/RealtorApi/RealtorApi.csproj
- T004: Crear pruebas de backend en app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
- T005: Crear Blazor Web App en app/frontend/src/RealtorWeb/RealtorWeb.csproj
- T006: Crear pruebas de frontend en app/frontend/test/RealtorWeb/RealtorWeb.Tests.csproj

En paralelo después de T003-T006:
- T007: Registrar proyectos en app/Realtor.sln
- T008: Configurar referencias en ambos proyectos de pruebas
- T009: Revisar app.css y referencias CSS en Components/App.razor
```

### Ejemplo de ejecución paralela: Historia de Usuario 2

No aplica: T010 es el único cambio de la historia y modifica `app/backend/src/RealtorApi/Program.cs` junto con la limpieza de archivos de ejemplo relacionados.

### Ejemplo de ejecución paralela: Historia de Usuario 3

No aplica: T011 valida el grafo ya ensamblado en `app/Realtor.sln` y requiere que terminen las tareas anteriores.

## Estrategia de Implementación

### MVP

El MVP es la Historia de Usuario 1: crea la solución y cuatro proyectos con referencias correctas. Completar T001-T009 y validar la lista de proyectos y compilación antes de avanzar. La Historia de Usuario 2 termina el arranque mínimo requerido; la Historia de Usuario 3 y T012 confirman la estructura completa y su compilación/pruebas.

### Entrega Incremental

1. Completar preparación y solución vacía.
2. Generar los proyectos de la Historia de Usuario 1 y ensamblar sus referencias.
3. Completar la configuración mínima de backend de la Historia de Usuario 2.
4. Verificar los límites de la Historia de Usuario 3 y ejecutar la validación final.

## Notas

- Todas las tareas incluyen identificador, checkbox y ruta concreta; las tareas de historias incluyen su etiqueta `[US1]`, `[US2]` o `[US3]`.
- `[P]` se usa solo para tareas con archivos distintos y sin dependencia mutua.
- No se escriben pruebas de lógica porque la spec no las solicita; los proyectos xUnit son parte explícita del alcance y `dotnet test` es una verificación de compilación/descubrimiento.
- No se modifica `global.json`, no se crean artefactos auxiliares de planificación ni archivos no trazados en esta lista.
