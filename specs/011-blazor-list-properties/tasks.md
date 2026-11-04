# Tareas: Listado de propiedades en la página principal

**Entrada**: Documentos de diseño desde `specs/011-blazor-list-properties/`

**Prerrequisitos**: `plan.md` (obligatorio), `spec.md` (obligatorio para historias de usuario), `research.md`, `data-model.md`, `contracts/`

**Pruebas**: La spec no solicita pruebas automatizadas; la validación requerida es compilación de `RealtorWeb`, verificación manual de la ruta `/` y evidencia en `quickstart.md`.

**Organización**: Las tareas se agrupan por historia de usuario para permitir implementación y validación independientes de cada incremento.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos diferentes, sin dependencias)
- **[Story]**: Historia de usuario a la que pertenece la tarea (por ejemplo, US1, US2, US3)
- Todas las tareas incluyen rutas exactas de archivo

## Fase 1: Preparación (Infraestructura compartida)

**Propósito**: Preparar la estructura mínima para la feature de listado paginado

- [X] T001 [P] Crear la estructura de feature en `app/frontend/src/RealtorWeb/Features/Properties/List/`
- [X] T002 [P] Definir el contrato de listado en `app/frontend/src/RealtorWeb/Services/Api/Properties/IPropertiesApi.cs` para `GET /api/properties` con `Page` y `PageSize`
- [X] T003 [P] Crear los modelos de presentación para el listado en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyListModels.cs`
- [X] T004 [P] Alinear la configuración del cliente en `app/frontend/src/RealtorWeb/Services/Api/ServiceCollectionExtensions.cs` y `app/frontend/src/RealtorWeb/Services/Api/RealtorApiOptions.cs` con el consumo de propiedades

---

## Fase 2: Fundacional (Prerrequisitos bloqueantes)

**Propósito**: Establecer la base que todas las historias usan para renderizado, estado y composición visual

**⚠️ CRÍTICO**: Ninguna historia de usuario puede comenzar hasta completar esta fase

- [X] T005 Crear el contenedor de estado de listado en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyListState.razor`
- [X] T006 [P] Crear la tarjeta de propiedad en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyCard.razor`
- [X] T007 [P] Definir la hoja de apoyo de la feature en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor.cs`
- [X] T008 Consolidar los patrones visuales del listado en `app/frontend/src/RealtorWeb/wwwroot/app.css` para tarjetas, estados y grilla responsive

**Checkpoint**: La base visual y de estados del listado queda lista para construir la experiencia principal.

---

## Fase 3: Historia de Usuario 1 - Ver el listado inicial de propiedades (Prioridad: P1) 🎯 MVP

**Objetivo**: Abrir `/` y mostrar la primera página de propiedades con sus metadatos de paginación

**Prueba independiente**: La ruta `/` carga `Page=1` y `PageSize=9`, muestra tarjetas con los campos públicos acordados y presenta página actual, total de páginas y total de elementos

### Implementación de la Historia de Usuario 1

- [X] T009 [US1] Reemplazar el contenido de `app/frontend/src/RealtorWeb/Components/Pages/Home.razor` para delegar en la feature de listado
- [X] T010 [US1] Implementar la carga inicial de propiedades en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor.cs` usando `IPropertiesApi`
- [X] T011 [P] [US1] Renderizar la grilla inicial de propiedades en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor` con `properties-grid` y `property-card`
- [X] T012 [P] [US1] Mostrar metadatos de paginación visibles en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor` para página actual, total de páginas y total de elementos
- [X] T013 [US1] Asegurar que `app/frontend/src/RealtorWeb/wwwroot/app.css` muestre 3 columnas en escritorio, 2 en tableta y 1 en móvil para `properties-grid`

**Checkpoint**: La página principal ya muestra el listado inicial de propiedades y sus metadatos.

---

## Fase 4: Historia de Usuario 2 - Navegar entre páginas de resultados (Prioridad: P2)

**Objetivo**: Permitir avanzar y retroceder entre páginas sin perder el contexto del listado

**Prueba independiente**: Los botones anterior/siguiente cambian la página, actualizan las tarjetas y respetan `HasPrevious` / `HasNext`

### Implementación de la Historia de Usuario 2

- [X] T014 [US2] Implementar la navegación anterior/siguiente en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor.cs` con cancelación de solicitudes obsoletas
- [X] T015 [P] [US2] Renderizar los controles de paginación en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor` con estados deshabilitados según `HasPrevious` y `HasNext`
- [X] T016 [US2] Ajustar el estado visual de navegación en `app/frontend/src/RealtorWeb/wwwroot/app.css` para que los controles reflejen disponibilidad e inactividad

**Checkpoint**: La navegación por páginas funciona de forma independiente y coherente.

---

## Fase 5: Historia de Usuario 3 - Entender estados vacíos y de error (Prioridad: P3)

**Objetivo**: Distinguir de forma clara entre carga, vacío, error y datos disponibles

**Prueba independiente**: La pantalla muestra un estado de carga mientras consulta, un estado vacío cuando no hay resultados y un estado de error cuando falla la consulta o la red

### Implementación de la Historia de Usuario 3

- [X] T017 [US3] Implementar el estado de carga en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyListState.razor`
- [X] T018 [US3] Implementar el estado vacío en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyListState.razor`
- [X] T019 [US3] Implementar el estado de error en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyListState.razor` con mensajes claros y sin mostrar datos obsoletos
- [X] T020 [US3] Ajustar la experiencia de recuperación y reintento en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor.cs` para fallos del backend o de red

**Checkpoint**: La experiencia del listado distingue claramente entre carga, vacío, error y contenido.

---

## Fase 6: Pulido y aspectos transversales

**Propósito**: Cerrar evidencia de validación, asegurar compilación y sincronizar artefactos

- [X] T021 [P] Ejecutar compilación final con `dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj` y resolver incidencias en `app/frontend/src/RealtorWeb/`
- [X] T022 [P] Actualizar evidencia real de validación en `specs/011-blazor-list-properties/quickstart.md` con fecha, comandos y resultados
- [X] T023 [P] Registrar consistencia final del contrato visual en `specs/011-blazor-list-properties/contracts/frontend-properties-list.contract.yaml`
- [X] T024 [P] Verificar que `app/frontend/src/RealtorWeb/Features/Properties/List/` y `app/frontend/src/RealtorWeb/Components/Pages/Home.razor` reflejen el alcance aprobado sin introducir CRUD, filtros o búsqueda avanzada
- [X] T025 [P] Confirmar que `app/frontend/src/RealtorWeb/wwwroot/app.css` mantiene la grilla responsive 3/2/1, los estados visuales y las clases canónicas sin estilos inline
- [X] T026 [P] Actualizar el bloque SPECKIT de `c:\sdd\.github\copilot-instructions.md` para apuntar a `specs/011-blazor-list-properties/plan.md`

---

## Dependencias y orden de ejecución

### Dependencias por fase

- **Preparación (Fase 1)**: Sin dependencias; puede iniciar de inmediato
- **Fundacional (Fase 2)**: Depende de Fase 1; bloquea todas las historias
- **Historias de usuario (Fases 3, 4 y 5)**: Dependen de Fase 2
- **Pulido (Fase 6)**: Depende de completar las historias objetivo

### Dependencias entre historias de usuario

- **US1 (P1)**: Inicia tras la fase fundacional y entrega el MVP del listado
- **US2 (P2)**: Depende de US1 para extender la interacción con la misma vista
- **US3 (P3)**: Puede iniciarse tras la fase fundacional y cerrarse al final validando estados visuales

### Dentro de cada historia

- Preparar estructura y modelos antes de enlazar el render final
- Implementar la carga de datos antes de la navegación
- Integrar estados visuales antes del checkpoint de cada historia

### Oportunidades de paralelización

- Fase 1: T001, T002, T003 y T004
- Fase 2: T006, T007 y T008 pueden avanzar en paralelo tras T005
- US1: T011 y T012 pueden avanzar en paralelo tras T010
- US2: T015 puede avanzar en paralelo a ajustes visuales de T016 una vez cerrada la lógica de T014
- Fase 6: T021, T022, T023, T024 y T025 pueden ejecutarse en paralelo

## Estrategia de implementación

### MVP primero (solo Historia de Usuario 1)

1. Completar Fase 1: Preparación
2. Completar Fase 2: Fundacional
3. Completar Fase 3: US1
4. **DETENER Y VALIDAR**: Verificar la página principal con listado inicial y metadatos

### Entrega incremental

1. Base técnica (Fases 1 + 2)
2. US1 (listado inicial MVP)
3. US2 (navegación por páginas)
4. US3 (estados de carga, vacío y error)
5. Cierre con validación y evidencia en `quickstart.md`
