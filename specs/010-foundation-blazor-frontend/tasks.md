# Tareas: Foundation Blazor Frontend

**Entrada**: Documentos de diseño desde `specs/010-foundation-blazor-frontend/`

**Prerrequisitos**: `plan.md` (obligatorio), `spec.md` (obligatorio para historias de usuario), `research.md`, `data-model.md`, `contracts/`

**Pruebas**: La spec no solicita pruebas automatizadas; la validación requerida es compilación de `RealtorWeb` y evidencia manual en `quickstart.md`.

**Organización**: Las tareas se agrupan por historia de usuario para permitir implementación y validación independientes.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos diferentes, sin dependencias)
- **[Story]**: Historia de usuario a la que pertenece la tarea (por ejemplo, US1, US2, US3)
- Todas las tareas incluyen rutas exactas de archivo

## Fase 1: Preparación (Infraestructura compartida)

**Propósito**: Preparar dependencias y estructura mínima para la fundación frontend

- [X] T001 Agregar dependencias `Lucide.Blazor`, `Refit` y `Refit.HttpClientFactory` en `app/frontend/src/RealtorWeb/RealtorWeb.csproj`
- [X] T002 [P] Crear estructura base de clientes tipados en `app/frontend/src/RealtorWeb/Services/Api/`
- [X] T003 [P] Definir configuración base de `RealtorApi` en `app/frontend/src/RealtorWeb/appsettings.json` y `app/frontend/src/RealtorWeb/appsettings.Development.json`

---

## Fase 2: Fundacional (Prerrequisitos bloqueantes)

**Propósito**: Eliminar dependencias externas y establecer la base transversal que bloquea todas las historias

**⚠️ CRÍTICO**: Ninguna historia de usuario puede comenzar hasta completar esta fase

- [X] T004 Quitar referencia a `lib/bootstrap/dist/css/bootstrap.min.css` en `app/frontend/src/RealtorWeb/Components/App.razor`
- [X] T005 Eliminar directorio `app/frontend/src/RealtorWeb/wwwroot/lib/bootstrap/`, validar con búsqueda global que no quedan referencias a Bootstrap y registrar evidencia en `specs/010-foundation-blazor-frontend/quickstart.md`
- [X] T006 Reescribir la base de `app/frontend/src/RealtorWeb/wwwroot/app.css` con tokens `:root` (color, spacing, radius, shadow, layout) y reset visual propio
- [X] T007 [P] Definir clases globales de íconos `.icon`, `.icon-sm`, `.icon-md`, `.icon-lg` y estrategia `currentColor` en `app/frontend/src/RealtorWeb/wwwroot/app.css`
- [X] T008 [P] Definir patrones reutilizables de estado `loading`, `empty`, `error`, `success` en `app/frontend/src/RealtorWeb/wwwroot/app.css`
- [X] T009 Limpiar y neutralizar estilos hardcodeados críticos heredados en `app/frontend/src/RealtorWeb/Components/Layout/MainLayout.razor.css` como paso transitorio de migración
- [X] T010 Limpiar y neutralizar estilos hardcodeados e íconos embebidos heredados en `app/frontend/src/RealtorWeb/Components/Layout/NavMenu.razor.css` como paso transitorio de migración

**Checkpoint**: Bootstrap queda removido y la base visual transversal está lista para construir historias.

---

## Fase 3: Historia de Usuario 1 - Base visual propia y consistente (Prioridad: P1) 🎯 MVP

**Objetivo**: Establecer un sistema visual interno coherente para layout, navegación y componentes base sin CSS de terceros

**Prueba independiente**: Al iniciar la app, la navegación y el contenido principal se renderizan con clases internas; no hay carga de frameworks CSS externos

### Implementación de la Historia de Usuario 1

- [X] T011 [US1] Reemplazar la estructura de layout en `app/frontend/src/RealtorWeb/Components/Layout/MainLayout.razor` para usar `.app-shell`, `.app-sidebar`, `.app-main`, `.app-content`
- [X] T012 [US1] Reescribir `app/frontend/src/RealtorWeb/Components/Layout/NavMenu.razor` con navegación vertical propia (`.sidebar-brand`, `.sidebar-nav`, `.sidebar-link`, `.sidebar-link-active`) sin clases Bootstrap
- [X] T013 [P] [US1] Integrar íconos Lucide en `app/frontend/src/RealtorWeb/Components/Layout/NavMenu.razor` para items de navegación y acciones base
- [X] T014 [P] [US1] Implementar estilos base de botones, cards, formularios, badges y cabeceras de página en `app/frontend/src/RealtorWeb/wwwroot/app.css`
- [X] T015 [US1] Limpiar dependencias de estilos heredados en `app/frontend/src/RealtorWeb/Components/Pages/Home.razor`, `app/frontend/src/RealtorWeb/Components/Pages/Error.razor` y `app/frontend/src/RealtorWeb/Components/Pages/NotFound.razor`

**Checkpoint**: La fundación visual P1 queda funcional y demostrable sin dependencia de CSS externo.

---

## Fase 4: Historia de Usuario 2 - Layout desktop first reutilizable (Prioridad: P2)

**Objetivo**: Implementar comportamiento responsive del shell (desktop/tablet/móvil) con navegación estable y patrón colapsado/drawer en móvil

**Prueba independiente**: En desktop hay sidebar estable; en tablet hay versión compacta; en móvil la navegación se abre/cierra en modo colapsado o drawer

### Implementación de la Historia de Usuario 2

- [X] T016 [US2] Implementar reglas responsive del shell (`desktop`, `tablet`, `mobile`) en `app/frontend/src/RealtorWeb/wwwroot/app.css`
- [X] T017 [US2] Incorporar markup de navegación colapsada/drawer con atributos de accesibilidad en `app/frontend/src/RealtorWeb/Components/Layout/NavMenu.razor`
- [X] T018 [US2] Ajustar la interacción de apertura/cierre para móvil en `app/frontend/src/RealtorWeb/Components/Layout/NavMenu.razor.js`
- [X] T019 [US2] Ajustar composición de contenido y cabeceras responsive en `app/frontend/src/RealtorWeb/Components/Layout/MainLayout.razor` y `app/frontend/src/RealtorWeb/wwwroot/app.css`
- [X] T020 [US2] Retirar definitivamente los estilos por componente (`MainLayout.razor.css` y `NavMenu.razor.css`) o dejarlos sin reglas activas, trasladando cualquier regla restante a `app/frontend/src/RealtorWeb/wwwroot/app.css`

**Checkpoint**: El layout base queda reutilizable y estable en las tres resoluciones objetivo.

---

## Fase 5: Historia de Usuario 3 - Base de comunicación tipada frontend-backend (Prioridad: P3)

**Objetivo**: Dejar registrado el andamiaje base de Refit por módulo/feature sin endpoints de negocio y sin uso directo de `HttpClient` en componentes

**Prueba independiente**: `Program.cs` registra clientes Refit con `IHttpClientFactory`, existen interfaces base por módulo y el frontend compila sin consumo de red directo en componentes

### Implementación de la Historia de Usuario 3

- [X] T021 [US3] Crear opciones de configuración para URL backend en `app/frontend/src/RealtorWeb/Services/Api/RealtorApiOptions.cs`
- [X] T022 [P] [US3] Crear interfaces Refit base por módulo en `app/frontend/src/RealtorWeb/Services/Api/System/IBackendHealthApi.cs` y `app/frontend/src/RealtorWeb/Services/Api/Properties/IPropertiesApi.cs`
- [X] T023 [US3] Crear extensión de registro DI para clientes tipados en `app/frontend/src/RealtorWeb/Services/Api/ServiceCollectionExtensions.cs`
- [X] T024 [US3] Registrar configuración y `AddRefitClient<T>()` en `app/frontend/src/RealtorWeb/Program.cs` sin `RestService.For<T>()`
- [X] T025 [US3] Auditar `app/frontend/src/RealtorWeb/Components/` para detectar uso directo de `HttpClient` o `RestService.For<T>()` y documentar hallazgos en `specs/010-foundation-blazor-frontend/quickstart.md`
- [X] T026 [US3] Corregir los hallazgos de la auditoría en `app/frontend/src/RealtorWeb/Components/` para que el consumo quede únicamente vía servicios tipados registrados en DI

**Checkpoint**: La base de comunicación queda lista para features futuras sin introducir endpoints de negocio.

---

## Fase 6: Pulido y aspectos transversales

**Propósito**: Cerrar evidencia de validación, asegurar compilación y sincronizar artefactos

- [X] T027 [P] Ejecutar compilación final con `dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj` y resolver incidencias en `app/frontend/src/RealtorWeb/`
- [X] T028 [P] Actualizar evidencia real de validación en `specs/010-foundation-blazor-frontend/quickstart.md` (fecha, comandos y resultados)
- [X] T029 [P] Verificar consistencia final del contrato en `specs/010-foundation-blazor-frontend/contracts/frontend-foundation.contract.yaml`
- [X] T030 Registrar comprobación de “0 CSS de terceros” y “Lucide activo” en `specs/010-foundation-blazor-frontend/quickstart.md`
- [X] T031 [P] Validar que `app/frontend/src/RealtorWeb/wwwroot/app.css` quede como única fuente visual efectiva y que `MainLayout.razor.css`/`NavMenu.razor.css` no aporten reglas activas al resultado final
- [X] T032 [P] Ejecutar control de alcance para confirmar que no se introdujeron pantallas o flujos CRUD de propiedades y registrar evidencia en `specs/010-foundation-blazor-frontend/quickstart.md`
- [X] T033 [P] Registrar matriz de validación responsive (desktop/tablet/móvil) con casos mínimos y resultado esperado/obtenido en `specs/010-foundation-blazor-frontend/quickstart.md`

---

## Dependencias y orden de ejecución

### Dependencias por fase

- **Preparación (Fase 1)**: Sin dependencias; puede iniciar de inmediato
- **Fundacional (Fase 2)**: Depende de Fase 1; bloquea todas las historias
- **Historias de usuario (Fases 3, 4 y 5)**: Dependen de Fase 2
- **Pulido (Fase 6)**: Depende de completar las historias objetivo

### Dependencias entre historias de usuario

- **US1 (P1)**: Inicia tras Fundacional; establece MVP visual
- **US2 (P2)**: Depende de US1 para extender el shell a responsive
- **US3 (P3)**: Puede iniciar tras Fundacional y cerrar al final tras validar que layout y componentes no consumen red directamente

### Dentro de cada historia

- Actualizar markup antes de cerrar CSS definitivo de esa historia
- Integrar íconos y estados visuales antes del checkpoint de historia
- Completar registro Refit y configuración antes de verificar compilación final

### Oportunidades de paralelización

- Fase 1: T002 y T003
- Fase 2: T007 y T008; T009 y T010
- US1: T013 y T014
- US3: T021 y T022; T025 y T026
- Fase 6: T027, T028, T029, T030, T031, T032 y T033

---

## Ejemplo de paralelización: Historia de Usuario 1

```bash
Tarea: "Integrar íconos Lucide en app/frontend/src/RealtorWeb/Components/Layout/NavMenu.razor"
Tarea: "Implementar estilos base de botones, cards, formularios, badges y cabeceras en app/frontend/src/RealtorWeb/wwwroot/app.css"
```

## Ejemplo de paralelización: Historia de Usuario 2

```bash
Tarea: "Implementar reglas responsive del shell en app/frontend/src/RealtorWeb/wwwroot/app.css"
Tarea: "Ajustar interacción de apertura/cierre móvil en app/frontend/src/RealtorWeb/Components/Layout/NavMenu.razor.js"
```

## Ejemplo de paralelización: Historia de Usuario 3

```bash
Tarea: "Crear opciones de configuración backend en app/frontend/src/RealtorWeb/Services/Api/RealtorApiOptions.cs"
Tarea: "Crear interfaces Refit base en app/frontend/src/RealtorWeb/Services/Api/System/IBackendHealthApi.cs y app/frontend/src/RealtorWeb/Services/Api/Properties/IPropertiesApi.cs"
```

---

## Estrategia de implementación

### MVP primero (solo Historia de Usuario 1)

1. Completar Fase 1: Preparación
2. Completar Fase 2: Fundacional
3. Completar Fase 3: US1
4. **DETENER Y VALIDAR**: Verificar shell visual propio sin CSS de terceros

### Entrega incremental

1. Base técnica (Fases 1 + 2)
2. US1 (fundación visual MVP)
3. US2 (responsive y navegación móvil)
4. US3 (comunicación tipada base con Refit)
5. Pulido y evidencia (`quickstart.md` + contrato)

### Estrategia paralela de equipo

1. Equipo completa Fases 1 y 2
2. Luego:
   - Desarrollador A: US1
   - Desarrollador B: US2
   - Desarrollador C: US3
3. Integración final en Fase 6

---

## Notas

- `[P]` implica archivos distintos y sin bloqueo directo
- Cada historia conserva criterio de validación independiente
- Evitar introducir lógica de negocio de propiedades en esta spec
- El cierre de la spec requiere evidencia actualizada en `quickstart.md`
