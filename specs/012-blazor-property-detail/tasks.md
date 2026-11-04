# Tareas: Detalle de propiedad en frontend Blazor

**Entrada**: Documentos de diseño desde `specs/012-blazor-property-detail/`

**Prerrequisitos**: `plan.md` (obligatorio), `spec.md` (obligatorio para historias de usuario), `research.md`, `data-model.md`, `contracts/`

**Pruebas**: La spec no solicita pruebas automatizadas; la validación requerida es compilación de `RealtorWeb`, verificación manual de navegación/estados y evidencia en `quickstart.md`.

**Organización**: Las tareas se agrupan por historia de usuario para permitir implementación y validación independientes de cada incremento.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos diferentes, sin dependencias)
- **[Story]**: Historia de usuario a la que pertenece la tarea (por ejemplo, US1, US2, US3)
- Todas las tareas incluyen rutas exactas de archivo

## Fase 1: Preparación (Infraestructura compartida)

**Propósito**: Preparar la estructura mínima de la feature de detalle sin tocar backend

- [X] T001 [P] Crear la estructura de feature en `app/frontend/src/RealtorWeb/Features/Properties/Detail/` con archivos `PropertyDetail.razor`, `PropertyDetail.razor.cs`, `PropertyDetailState.razor` y `PropertyDetailModels.cs`
- [X] T002 [P] Crear la página enrutable `app/frontend/src/RealtorWeb/Components/Pages/PropertyDetailPage.razor` para la ruta `/properties/{id}` delegando en la feature
- [X] T003 [P] Extender el cliente Refit en `app/frontend/src/RealtorWeb/Services/Api/Properties/IPropertiesApi.cs` con `GetPropertyByIdAsync` para `GET /api/properties/{id}`

---

## Fase 2: Fundacional (Prerrequisitos bloqueantes)

**Propósito**: Establecer contratos y estados base que bloquean todas las historias

**⚠️ CRÍTICO**: Ninguna historia de usuario puede comenzar hasta completar esta fase

- [X] T004 Definir el modelo tipado de detalle (contract-first) en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetailModels.cs` alineado a `PropertyDetailResponse`
- [X] T005 [P] Agregar soporte de navegación por id en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyListModels.cs` incorporando `Id` en `PropertyListItem`
- [X] T006 [P] Implementar los estados visuales reutilizables de detalle en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetailState.razor` para `loading`, `not-found`, `error`, `success`
- [X] T007 Consolidar estilos fundacionales del detalle y placeholder en `app/frontend/src/RealtorWeb/wwwroot/app.css` usando clases canónicas y sin estilos inline

**Checkpoint**: Base contractual y visual lista para implementar historias de usuario.

---

## Fase 3: Historia de Usuario 1 - Consultar detalle completo de una propiedad (Prioridad: P1) 🎯 MVP

**Objetivo**: Abrir el detalle desde el listado y mostrar todos los datos públicos requeridos

**Prueba independiente**: Desde `/`, pulsar "Ver" en cualquier tarjeta y validar que `/properties/{id}` muestra título, imagen/placeholder, descripción, dirección, precio, estado, dormitorios, baños y m²

### Implementación de la Historia de Usuario 1

- [X] T008 [US1] Agregar el botón "Ver" con navegación Blazor a `/properties/{id}` en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyCard.razor` usando clases `btn btn-primary`
- [X] T009 [US1] Actualizar el render del listado en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyList.razor` para conservar la tarjeta existente con la nueva acción "Ver"
- [X] T010 [US1] Implementar la carga inicial de detalle por id en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor.cs` consumiendo `IPropertiesApi.GetPropertyByIdAsync`
- [X] T011 [P] [US1] Implementar la vista de contenido exitoso en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor` mostrando todos los campos públicos del contrato
- [X] T012 [US1] Implementar placeholder de imagen nula con icono Lucide en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor`
- [X] T013 [US1] Ajustar composición visual desktop-first del detalle en `app/frontend/src/RealtorWeb/wwwroot/app.css` para legibilidad y responsive

**Checkpoint**: La navegación listado → detalle funciona y el contenido completo se muestra correctamente.

---

## Fase 4: Historia de Usuario 2 - Entender claramente cuando no hay detalle disponible (Prioridad: P2)

**Objetivo**: Distinguir explícitamente no encontrado (404) del resto de errores

**Prueba independiente**: Probar un id inexistente y un fallo de red/backend para confirmar estados visuales y mensajes diferenciados

### Implementación de la Historia de Usuario 2

- [X] T014 [US2] Mapear `ApiException` con estado 404 a `not-found` en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor.cs`
- [X] T015 [US2] Mapear errores no-404 y de red a estado `error` en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor.cs`
- [X] T016 [US2] Renderizar mensajes diferenciados para `not-found` y `error` en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetailState.razor`
- [X] T017 [P] [US2] Mostrar estado de carga explícito durante consulta de detalle en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor`
- [X] T018 [US2] Afinar estilos de estado para no encontrado/error en `app/frontend/src/RealtorWeb/wwwroot/app.css` reutilizando clases canónicas

**Checkpoint**: La vista diferencia claramente `404` de error general y mantiene estado de carga explícito.

---

## Fase 5: Historia de Usuario 3 - Volver al listado sin perder orientación (Prioridad: P3)

**Objetivo**: Garantizar retorno claro al listado desde la vista de detalle

**Prueba independiente**: Abrir un detalle y regresar a `/` en una única acción visible

### Implementación de la Historia de Usuario 3

- [X] T019 [US3] Agregar acción "Volver al listado" en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor` con navegación Blazor a `/`
- [X] T020 [P] [US3] Exponer la acción de retorno también en estados `not-found` y `error` en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetailState.razor`
- [X] T021 [US3] Ajustar estilos de la acción de retorno en `app/frontend/src/RealtorWeb/wwwroot/app.css` manteniendo consistencia visual del sistema

**Checkpoint**: El flujo detalle → listado queda disponible en éxito y en escenarios de falla.

---

## Fase 6: Pulido y aspectos transversales

**Propósito**: Cerrar validación, evidencia y consistencia final de artefactos

- [X] T022 [P] Ejecutar compilación final con `dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj` y resolver incidencias en `app/frontend/src/RealtorWeb/`
- [X] T023 [P] Registrar evidencia de validación manual en `specs/012-blazor-property-detail/quickstart.md` incluyendo protocolo medible (muestra de 20 aperturas y viewports 1366x768, 1024x768, 390x844) con fecha, comandos y resultados
- [X] T024 [P] Verificar consistencia del contrato visual en `specs/012-blazor-property-detail/contracts/frontend-property-detail.contract.yaml` tras la implementación
- [X] T025 [P] Confirmar que el alcance implementado en `app/frontend/src/RealtorWeb/Features/Properties/List/` y `app/frontend/src/RealtorWeb/Features/Properties/Detail/` no introduce edición/eliminación/cambio de estado
- [X] T026 [P] (Operativa/no funcional) Actualizar el bloque SPECKIT en `.github/copilot-instructions.md` para mantener el contexto de la iniciativa activa

---

## Dependencias y orden de ejecución

### Dependencias por fase

- **Preparación (Fase 1)**: Sin dependencias; puede iniciar de inmediato
- **Fundacional (Fase 2)**: Depende de Fase 1; bloquea todas las historias
- **Historias de usuario (Fases 3, 4 y 5)**: Dependen de Fase 2
- **Pulido (Fase 6)**: Depende de completar las historias objetivo

### Dependencias entre historias de usuario

- **US1 (P1)**: Inicia tras la fase fundacional y entrega el MVP funcional
- **US2 (P2)**: Depende de US1 para reutilizar la pantalla de detalle con manejo diferenciado de errores
- **US3 (P3)**: Depende de US1 y se extiende a estados de US2 para retorno consistente

### Dentro de cada historia

- Modelos y contratos primero
- Carga de datos antes del render final de éxito
- Estados visuales antes del checkpoint de validación
- Navegación de retorno antes del cierre de la historia

### Oportunidades de paralelización

- Fase 1: T001, T002 y T003
- Fase 2: T005, T006 y T007 (tras T004)
- US1: T011 (tras T010)
- US2: T017 en paralelo con T016 después de T014/T015
- US3: T020 en paralelo con T021 después de T019
- Fase 6: T022, T023, T024 y T025

## Ejemplo de paralelización: US1

```bash
# US1 no tiene paralelismo real: T011 y T012 editan el mismo archivo y deben ejecutarse en secuencia (T011 -> T012):
T011 [US1] Implementar vista de contenido exitoso en app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor
T012 [US1] Implementar placeholder de imagen nula en app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor
```

## Ejemplo de paralelización: US2

```bash
# Paralelo de estado de carga y estado de error/no encontrado:
T016 [US2] Renderizar mensajes diferenciados en app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetailState.razor
T017 [US2] Mostrar estado de carga explícito en app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor
```

## Ejemplo de paralelización: US3

```bash
# Paralelo de retorno en estados y ajuste visual:
T020 [US3] Exponer retorno al listado en estados de app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetailState.razor
T021 [US3] Ajustar estilos de retorno en app/frontend/src/RealtorWeb/wwwroot/app.css
```

## Estrategia de implementación

### MVP primero (solo Historia de Usuario 1)

1. Completar Fase 1: Preparación
2. Completar Fase 2: Fundacional
3. Completar Fase 3: US1
4. **DETENER Y VALIDAR**: Confirmar navegación listado → detalle + render completo del contenido

### Entrega incremental

1. Base técnica (Fases 1 + 2)
2. US1 (detalle completo desde "Ver")
3. US2 (diferenciación no encontrado/error)
4. US3 (retorno al listado en todos los estados)
5. Cierre con evidencia de quickstart y compilación
