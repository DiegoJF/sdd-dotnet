# Tareas: Alta de inmuebles en frontend Blazor

**Entrada**: Documentos de diseño desde `/specs/015-blazor-create-property/`

**Prerrequisitos**: plan.md (requerido), spec.md (requerido para historias), research.md, data-model.md, contracts/

**Pruebas**: No se generan tareas de pruebas automatizadas porque la spec define validación manual como criterio de cierre.

**Organización**: Tareas agrupadas por historia de usuario para implementación y validación independiente.

## Fase 1: Configuración (Infraestructura compartida)

**Propósito**: Preparar estructura y contratos de integración para la funcionalidad de alta.

- [X] T001 Crear página enrutable de alta en app/frontend/src/RealtorWeb/Components/Pages/PropertyCreatePage.razor
- [X] T002 Crear slice base de alta en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor
- [X] T003 [P] Crear code-behind inicial del formulario en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor.cs
- [X] T004 [P] Crear modelo de estado de pantalla de alta en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateState.razor

---

## Fase 2: Fundacional (Bloqueantes)

**Propósito**: Base funcional común que debe quedar lista antes de implementar historias.

**⚠️ CRITICAL**: Ninguna historia puede comenzar antes de completar esta fase.

- [X] T005 Definir modelos contract-first de creación en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateModels.cs
- [X] T006 Extender cliente tipado con operación CreateProperty en app/frontend/src/RealtorWeb/Services/Api/Properties/IPropertiesApi.cs
- [X] T007 Implementar mapeo base de errores por familia de respuesta (400, 413, 415, 500) en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor.cs
- [X] T008 [P] Agregar estilos base de formulario, errores y resumen en app/frontend/src/RealtorWeb/wwwroot/app.css
- [X] T009 [P] Agregar estilos base de estado de envío (editing/submitting/success/error) en app/frontend/src/RealtorWeb/wwwroot/app.css

**Hito**: Fundación lista; ya se puede implementar cada historia.

---

## Fase 3: Historia de Usuario 1 - Registrar inmueble con validaciones en frontend (Prioridad: P1) 🎯 PMV

**Objetivo**: Capturar datos del inmueble con validaciones equivalentes a backend y feedback inmediato.

**Prueba independiente**: Completar formulario con datos válidos y confirmar que permite envío; introducir datos inválidos y confirmar errores por campo en tiempo real + resumen al enviar.

### Implementación de Historia de Usuario 1

- [X] T010 [US1] Implementar campos del formulario alineados a CreatePropertyRequest en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor
- [X] T011 [US1] Implementar validación en tiempo real por campo en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor.cs
- [X] T012 [US1] Implementar resumen de errores al enviar en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor
- [X] T013 [P] [US1] Implementar mensajes de error por campo en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor
- [X] T014 [P] [US1] Agregar estilos de errores de campo y resumen en app/frontend/src/RealtorWeb/wwwroot/app.css
- [X] T015 [US1] Integrar página enrutable con el formulario de alta en app/frontend/src/RealtorWeb/Components/Pages/PropertyCreatePage.razor

**Hito**: US1 funcional y validable de manera independiente.

---

## Fase 4: Historia de Usuario 2 - Alta exitosa con redirección a detalle (Prioridad: P2)

**Objetivo**: Ejecutar alta exitosa, mostrar confirmación y navegar al detalle del inmueble creado.

**Prueba independiente**: Enviar alta válida y verificar confirmación visible + redirección automática a la ruta de detalle del id retornado.

### Implementación de Historia de Usuario 2

- [X] T016 [US2] Implementar envío del formulario a CreateProperty en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor.cs
- [X] T017 [US2] Implementar confirmación visual de alta exitosa en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor
- [X] T018 [US2] Implementar redirección automática a /properties/{id} en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor.cs
- [X] T019 [P] [US2] Agregar estado visual de envío en curso y bloqueo de doble submit en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor
- [X] T020 [P] [US2] Agregar estilos de éxito y submitting en app/frontend/src/RealtorWeb/wwwroot/app.css

**Hito**: US2 funcional y validable de manera independiente.

---

## Fase 5: Historia de Usuario 3 - Imagen opcional y recuperación ante errores (Prioridad: P3)

**Objetivo**: Mantener imagen opcional con vista previa/reemplazo/eliminación y conservar datos ante errores 400/413/415/500.

**Prueba independiente**: Validar envío sin imagen, vista previa con imagen, reemplazo/eliminación, y respuestas 400/413/415/500 con conservación de datos.

### Implementación de Historia de Usuario 3

- [X] T021 [US3] Implementar selector de imagen opcional y binding en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor
- [X] T022 [US3] Implementar vista previa de imagen seleccionada en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor
- [X] T023 [US3] Implementar acciones quitar/reemplazar imagen antes de enviar en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor.cs
- [X] T024 [US3] Implementar validación temprana de tipo/tamaño de archivo en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor.cs
- [X] T025 [US3] Implementar manejo de respuesta 400 con errores por campo y conservación de datos en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor.cs
- [X] T026 [US3] Implementar manejo de respuestas 413/415/500 con mensaje global y conservación de datos en app/frontend/src/RealtorWeb/Features/Properties/Create/PropertyCreateForm.razor.cs
- [X] T027 [P] [US3] Agregar estilos de preview y mensajes globales de error en app/frontend/src/RealtorWeb/wwwroot/app.css

**Hito**: US3 funcional y validable de manera independiente.

---

## Fase 6: Cierre y Verificación Transversal

**Propósito**: Cierre de calidad y validación final transversal.

- [X] T028 Actualizar guía de validación con resultados ejecutados en specs/015-blazor-create-property/quickstart.md
- [X] T029 Ejecutar compilación de frontend y registrar evidencia en specs/015-blazor-create-property/quickstart.md
- [X] T030 Ejecutar validación manual y registrar evidencia de SC-001 (errores por campo en tiempo real + resumen) en specs/015-blazor-create-property/quickstart.md
- [X] T031 Ejecutar validación manual y registrar evidencia de SC-002 (confirmación + redirección al detalle tras 201) en specs/015-blazor-create-property/quickstart.md
- [X] T032 Ejecutar validación manual y registrar evidencia de SC-003 (respuesta 400 con conservación de datos) en specs/015-blazor-create-property/quickstart.md
- [X] T033 Ejecutar validación manual y registrar evidencia de SC-004 (respuestas 413/415/500 con mensaje global y conservación) en specs/015-blazor-create-property/quickstart.md
- [X] T034 Ejecutar validación manual y registrar evidencia de SC-005 (alta sin imagen + preview/reemplazo/eliminación) en specs/015-blazor-create-property/quickstart.md
- [X] T035 [Transversal-NoFuncional] Revisar coherencia final entre spec, plan y tasks en specs/015-blazor-create-property/spec.md

---

## Dependencias y Orden de Ejecución

### Dependencias entre Fases

- Configuración (Fase 1): sin dependencias.
- Fundacional (Fase 2): depende de Configuración y bloquea historias.
- Historias de Usuario (Fase 3+): dependen de completar Fundacional.
- Cierre (Fase 6): depende de completar las historias objetivo.

### Dependencias entre Historias de Usuario

- US1 (P1): inicia tras Fundacional; no depende de otras historias.
- US2 (P2): inicia tras Fundacional; depende funcionalmente de envío exitoso implementado sobre base de US1.
- US3 (P3): inicia tras Fundacional; reutiliza flujo de envío de US2 para manejo de errores y conservación de datos.

### Oportunidades de Paralelización

- Fase 1: T003 y T004 en paralelo.
- Fase 2: T008 y T009 en paralelo.
- US1: T013 y T014 en paralelo tras T011.
- US2: T019 y T020 en paralelo tras T016.
- US3: T027 en paralelo con T025/T026 tras T024.

---

## Ejemplo de Paralelización: Historia de Usuario 1

```bash
# Ejecutar en paralelo tras completar validación por campo (T011):
Tarea: T013 [US1] Mensajes de error por campo en PropertyCreateForm.razor
Tarea: T014 [US1] Estilos de errores y resumen en app.css
```

## Ejemplo de Paralelización: Historia de Usuario 2

```bash
# Ejecutar en paralelo tras completar envío base (T016):
Tarea: T019 [US2] Estado submitting y bloqueo doble submit
Tarea: T020 [US2] Estilos de éxito y submitting
```

## Ejemplo de Paralelización: Historia de Usuario 3

```bash
# Ejecutar en paralelo tras validación temprana de imagen (T024):
Tarea: T025 [US3] Manejo 400 con errores por campo
Tarea: T026 [US3] Manejo 413/415/500 con mensaje global
Tarea: T027 [US3] Estilos de preview y error global
```

---

## Estrategia de Implementación

### PMV Primero (Solo Historia de Usuario 1)

1. Completar Fase 1 (Configuración).
2. Completar Fase 2 (Fundacional).
3. Completar Fase 3 (US1).
4. Validar independencia de US1 con su criterio de prueba.

### Entrega Incremental

1. Base lista: Fase 1 + Fase 2.
2. Entrega PMV: US1.
3. Entrega valor de cierre de flujo: US2.
4. Entrega robustez y opcionalidad de imagen: US3.
5. Cierre y evidencia: Fase 6.

### Estrategia de Equipo en Paralelo

1. El equipo completa Configuración y Fundacional en conjunto.
2. Luego:
   - Dev A: US1
   - Dev B: US2
   - Dev C: US3
3. Integración final y evidencia en quickstart.

---

## Notes

- Todas las tareas siguen formato checklist ejecutable.
- Cada tarea de historia incluye etiqueta [USx] para trazabilidad.
- La spec no solicita tests automatizados; se prioriza evidencia manual según quickstart.
- Se evita explícitamente ampliar alcance a edición/eliminación/alta masiva/múltiples imágenes.