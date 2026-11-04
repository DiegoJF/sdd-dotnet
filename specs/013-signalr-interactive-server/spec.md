# Especificación de Feature: Corrección de Interactividad Blazor en Servidor

**Rama de Feature**: `013-signalr-interactive-server`

**Creado**: 2026-07-23

**Estado**: Implementada

**Entrada**: Descripción del usuario: "Habilitar interactividad de servidor en las páginas Blazor del frontend RealtorWeb para que los botones de navegación funcionen, manteniendo el alcance solo en páginas host interactivas de listado y detalle."

## Ciclo de estado del spec *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementación -> Implementada**
- Está prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Está prohibido cerrar en **Implementada** sin evidencia de validación en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- 2026-07-23 Borrador - Spec creado por `/speckit.specify`
- 2026-07-23 Aprobada -> En implementación - Inicio de `/speckit.implement`
- 2026-07-23 En implementación -> Implementada - Cierre por finalización de `/speckit.implement` con validaciones cumplidas (tasks.md sin pendientes y evidencia de validación en quickstart.md)

## Clarificaciones

### Session 2026-07-23

- Q: ¿Qué páginas deben permanecer estáticas en esta iniciativa? → A: Mantener explícitamente Error y NotFound en modo estático como requisito verificable de esta spec.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Navegar entre páginas del listado sin recarga completa (Prioridad: P1)

Como persona usuaria del listado de propiedades, quiero que los botones de paginación funcionen de manera interactiva para moverme entre resultados sin perder continuidad.

**Por qué esta prioridad**: La exploración de propiedades depende de la paginación; si no responde, el recorrido principal del producto queda bloqueado.

**Prueba independiente**: Puede validarse en la página principal navegando con "Siguiente" y "Anterior" y verificando actualización de resultados sin recarga completa del navegador.

**Escenarios de aceptación**:

1. **Dado** que la persona está en la primera página con más resultados disponibles, **Cuando** pulsa "Siguiente", **Entonces** se muestra la siguiente página de propiedades sin recarga completa del navegador.
2. **Dado** que la persona está en una página posterior a la primera, **Cuando** pulsa "Anterior", **Entonces** se muestra la página previa de propiedades sin recarga completa del navegador.
3. **Dado** que la persona está en los extremos de paginación, **Cuando** observa los controles de navegación, **Entonces** los botones respetan su estado deshabilitado según disponibilidad de navegación.

---

### Historia de Usuario 2 - Volver del detalle al listado en todos los estados (Prioridad: P2)

Como persona usuaria en la vista de detalle de una propiedad, quiero regresar al listado desde cualquier estado de la pantalla para continuar explorando opciones.

**Por qué esta prioridad**: La navegación de retorno evita callejones sin salida y mantiene el flujo de uso entre listado y detalle.

**Prueba independiente**: Puede validarse abriendo el detalle en estado con datos, no encontrado y error general, y comprobando que la acción de volver siempre retorna al listado.

**Escenarios de aceptación**:

1. **Dado** que la persona está en detalle con datos cargados, **Cuando** pulsa "Volver al listado", **Entonces** navega a la ruta principal del listado.
2. **Dado** que la persona está en detalle no encontrado, **Cuando** pulsa "Volver al listado", **Entonces** navega a la ruta principal del listado.
3. **Dado** que la persona está en detalle con error general, **Cuando** pulsa "Volver al listado", **Entonces** navega a la ruta principal del listado.

---

### Historia de Usuario 3 - Recuperarse de errores con reintento funcional (Prioridad: P3)

Como persona usuaria, quiero que la acción "Reintentar" vuelva a lanzar la carga de información en pantallas con error para no tener que refrescar manualmente.

**Por qué esta prioridad**: Permite recuperación rápida ante fallos transitorios y reduce fricción cuando ocurre un error.

**Prueba independiente**: Puede validarse forzando estado de error en listado y detalle y verificando que "Reintentar" ejecuta nuevamente la carga de datos.

**Escenarios de aceptación**:

1. **Dado** un estado de error en el listado, **Cuando** la persona pulsa "Reintentar", **Entonces** se ejecuta una nueva carga de resultados.
2. **Dado** un estado de error en el detalle, **Cuando** la persona pulsa "Reintentar", **Entonces** se ejecuta una nueva carga del detalle solicitado.

---

### Casos Límite

- ¿Qué ocurre si la página de listado no tiene página previa o siguiente? Los botones deben permanecer deshabilitados y no deben disparar navegación.
- ¿Qué ocurre si se pulsa repetidamente "Reintentar" durante un fallo persistente? La experiencia debe seguir mostrando un estado de error estable, sin bloquear la interfaz ni romper la navegación.
- ¿Qué ocurre con páginas informativas sin acciones interactivas? Deben conservar el comportamiento estático actual y no verse afectadas por este cambio.

## Requisitos *(obligatorio)*

### Requisitos Funcionales

- **FR-001**: El sistema DEBE habilitar interactividad de servidor de forma granular únicamente en las dos páginas host que contienen las experiencias de listado y detalle de propiedades.
- **FR-002**: El sistema NO DEBE habilitar interactividad de forma global para toda la aplicación.
- **FR-003**: El sistema DEBE mantener explícitamente en modo estático las páginas Error y NotFound.
- **FR-004**: El sistema DEBE permitir que la acción "Siguiente" avance la paginación del listado y actualice el contenido sin recarga completa del navegador.
- **FR-005**: El sistema DEBE permitir que la acción "Anterior" retroceda la paginación del listado y actualice el contenido sin recarga completa del navegador.
- **FR-006**: El sistema DEBE respetar el estado deshabilitado de los controles "Anterior" y "Siguiente" en función de la disponibilidad real de navegación.
- **FR-007**: El sistema DEBE permitir que la acción "Volver al listado" en la vista de detalle navegue siempre a la ruta principal del listado, incluyendo estados de no encontrado y error general.
- **FR-008**: El sistema DEBE permitir que la acción "Reintentar" en estados de error del listado vuelva a ejecutar la carga de resultados.
- **FR-009**: El sistema DEBE permitir que la acción "Reintentar" en estados de error del detalle vuelva a ejecutar la carga del detalle.
- **FR-010**: Esta iniciativa NO DEBE introducir cambios en backend, contratos de integración, ni lógica funcional existente fuera de la habilitación de interactividad en páginas host.

### Entidades Clave *(incluir si la feature involucra datos)*

- **Página host interactiva**: Superficie de navegación que contiene componentes con acciones de usuario y que debe operar con circuito interactivo de servidor.
- **Estado de navegación del listado**: Resultado de disponibilidad de avance y retroceso entre páginas de propiedades.
- **Estado de vista de detalle**: Resultado visible para la persona usuaria en detalle (cargado, no encontrado o error general) con acciones de retorno y recuperación.

## Criterios de Éxito *(obligatorio)*

### Resultados Medibles

- **SC-001**: En una corrida de 20 interacciones de paginación del listado (10 de avance y 10 de retroceso), el 100% actualiza resultados sin recarga completa del navegador.
- **SC-002**: En una corrida de 15 intentos sobre "Volver al listado" desde detalle (5 con datos, 5 no encontrado y 5 error), el 100% retorna a la ruta principal del listado en una sola acción.
- **SC-003**: En una corrida de 10 reintentos en errores del listado y 10 en errores del detalle, el 100% dispara una nueva solicitud de carga de datos.
- **SC-004**: En validación funcional de páginas informativas no interactivas, el 100% mantiene su comportamiento estático sin cambios de experiencia.
- **SC-005**: En la revisión final de alcance, el 100% de los cambios funcionales queda limitado a las páginas host de listado y detalle, sin modificaciones funcionales en backend, sin cambios en contratos Refit y sin activación interactiva global.

## Supuestos

- La capacidad de interactividad de servidor ya está disponible en la configuración base de la aplicación y solo falta su activación en páginas host específicas.
- Las acciones de navegación y reintento existentes en listado y detalle son correctas a nivel funcional y requieren únicamente activación de interactividad para ejecutarse en tiempo de uso.
- La ruta principal del listado permanece como punto de retorno válido para la experiencia de detalle.
- El alcance de la iniciativa se limita al frontend y no requiere cambios contractuales ni operativos en servicios externos.
