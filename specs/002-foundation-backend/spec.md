# Especificación de la Funcionalidad: Fundación Backend Base

**Rama de la funcionalidad**: `002-foundation-backend`

**Creado**: 2026-07-08

**Estado**: Implementada

**Entrada**: Descripción de usuario: "Crear la especificación 002-foundation-backend para establecer la estructura base interna de RealtorApi y capacidades transversales sin lógica de negocio"

## Ciclo de estado del spec *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementación -> Implementada**
- Está prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Está prohibido cerrar en **Implementada** sin evidencia de validación en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- 2026-07-08 Borrador - Spec creado por `/speckit.specify`
- 2026-07-08 Implementada - Cierre automático post-implementación tras validar tasks.md y quickstart.md

## Clarifications

### Session 2026-07-08

- Q: ¿Cuál debe ser la política de exposición de `/health`? → A: Sin autenticación, pero accesible solo desde red/origen interno (ingress/reverse proxy).

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Base transversal extensible sin tocar Program (Prioridad: P1)

Como equipo backend, necesitamos una infraestructura transversal para Minimal APIs basada en descubrimiento automático de slices y handlers, de forma que nuevas features puedan agregarse sin modificar `Program.cs` ni registrar componentes manualmente.

**Por qué esta prioridad**: Esta capacidad define la arquitectura base del backend y condiciona todas las iniciativas futuras.

**Prueba independiente**: Puede validarse creando una slice de prueba en tests y verificando que se descubre y se mapea automáticamente, además de confirmar que `Program.cs` mantiene alcance mínimo.

**Escenarios de aceptación**:

1. **Dado** la aplicación backend con mecanismo centralizado de descubrimiento, **Cuando** existe una implementación de `ISlice` en el ensamblado objetivo, **Entonces** la slice se registra y sus endpoints se mapean sin agregar código nuevo en `Program.cs`.
2. **Dado** el backend inicial, **Cuando** se agrega una nueva slice de infraestructura, **Entonces** no se requieren registros manuales ni convenciones por nombre de clase para que sea encontrada.

---

### Historia de Usuario 2 - Validación automática y manejo consistente de errores (Prioridad: P2)

Como equipo backend, necesitamos una infraestructura de validación automática y traducción consistente de errores esperados a ProblemDetails para asegurar respuestas homogéneas y observables en todas las Minimal APIs futuras.

**Por qué esta prioridad**: Reduce duplicación, evita comportamientos inconsistentes entre endpoints y establece una base de calidad obligatoria para el API.

**Prueba independiente**: Puede validarse con pruebas unitarias que cubran: detección automática de `IValidator<T>`, pass-through cuando no hay validador, respuesta 400 con `ValidationProblemDetails`, y mapeo de errores `Result` a `ProblemDetails`.

**Escenarios de aceptación**:

1. **Dado** un request con `IValidator<T>` registrado, **Cuando** se ejecuta el filtro de validación por convención, **Entonces** se aplica la validación automáticamente sin registrar filtros individuales.
2. **Dado** un request sin `IValidator<T>` registrado, **Cuando** se ejecuta el filtro de validación, **Entonces** la solicitud continúa sin error y sin bloqueo artificial.
3. **Dado** un fallo de validación, **Cuando** se procesa el resultado, **Entonces** la API responde con `ValidationProblemDetails` y HTTP 400.
4. **Dado** un error esperado modelado como `Result`, **Cuando** se convierte a respuesta HTTP, **Entonces** se devuelve `ProblemDetails` con código de estado y payload correctos.

---

### Historia de Usuario 3 - Migrar health a slice como sonda E2E de infraestructura (Prioridad: P3)

Como equipo técnico, necesitamos que el endpoint `/health` existente se implemente con el patrón `ISlice` para demostrar de extremo a extremo que el descubrimiento y mapeo centralizados funcionan sin introducir lógica de negocio.

**Por qué esta prioridad**: Proporciona una verificación visible y de bajo riesgo del andamiaje técnico antes de incorporar features funcionales.

**Prueba independiente**: Puede validarse ejecutando pruebas del endpoint `/health` y confirmando que continúa respondiendo correctamente tras su migración a slice.

**Escenarios de aceptación**:

1. **Dado** el endpoint `/health` migrado al patrón `ISlice`, **Cuando** se inicia la aplicación, **Entonces** el endpoint queda disponible mediante el mecanismo centralizado de mapeo.
2. **Dado** consumidores del endpoint de salud, **Cuando** consultan `/health`, **Entonces** reciben una respuesta de estado correcta equivalente a la sonda de infraestructura esperada.
3. **Dado** la política de seguridad de infraestructura, **Cuando** se expone `/health`, **Entonces** su acceso queda restringido a red/origen interno sin autenticación de aplicación.

### Casos límite

- ¿Qué ocurre si existen múltiples implementaciones de `ISlice` de prueba en el ensamblado objetivo? El registro debe evitar duplicados y mantener consistencia del contrato.
- ¿Qué ocurre si un request no tiene validador asociado? La ejecución debe continuar como pass-through sin lanzar error por infraestructura ausente.
- ¿Qué ocurre si el mapeo de error esperado recibe un resultado inválido o incompleto? Debe producir una respuesta de error consistente y trazable sin exponer detalles internos sensibles.
- ¿Qué ocurre si `/health` migra a slice pero se olvida registrar el mecanismo centralizado? La prueba de humo debe detectar la ausencia del endpoint para bloquear regresiones de infraestructura.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: El sistema DEBE mantener la solución y proyectos existentes de la spec 001 sin recrearlos.
- **RF-002**: El sistema DEBE definir la estructura interna del backend con organización compatible con Vertical Slice Architecture, preparada para features futuras y sin features implementadas.
- **RF-003**: El sistema DEBE incluir infraestructura transversal en backend para endpoints, validación y handlers, sin introducir casos de uso de negocio.
- **RF-004**: El sistema DEBE definir el contrato `ISlice` para representar slices de Minimal APIs.
- **RF-005**: El sistema DEBE registrar slices mediante descubrimiento por ensamblado (`RegisterSlices`) usando reflection y sin registro manual por clase.
- **RF-006**: El sistema DEBE mapear endpoints de slices mediante un mecanismo centralizado (`MapSliceEndpoints`) sin modificar `Program.cs` al agregar nuevos endpoints.
- **RF-007**: El sistema DEBE auto-registrar handlers por ensamblado mediante un marker interface `IHandler`, sin registrar handlers de forma individual.
- **RF-008**: El sistema DEBE registrar validadores FluentValidation por ensamblado sin registro individual de validadores.
- **RF-009**: El sistema DEBE aplicar validación automática para Minimal APIs mediante `ValidationFilterFactory` basada en convención y detección de `IValidator<T>`.
- **RF-010**: El sistema DEBE permitir pass-through cuando no existe validador aplicable, sin generar error de infraestructura.
- **RF-011**: El sistema DEBE devolver `ValidationProblemDetails` con HTTP 400 ante errores de validación.
- **RF-012**: El sistema DEBE proveer conversión centralizada de errores esperados modelados con `Result` hacia `ProblemDetails` coherente.
- **RF-013**: El sistema DEBE habilitar logging estructurado con `ILogger` para capacidades transversales y manejo de errores.
- **RF-014**: El sistema DEBE migrar el endpoint `/health` existente al patrón `ISlice` como única sonda de infraestructura de extremo a extremo.
- **RF-014-A**: El sistema DEBE mantener `/health` sin autenticación de aplicación y restringir su acceso a red/origen interno mediante configuración de ingress o reverse proxy.
- **RF-015**: El sistema DEBE limitar `Program.cs` a configuración de servicios, middleware, registro de infraestructura y mapeo centralizado de endpoints.
- **RF-016**: El sistema DEBE incluir pruebas unitarias en `app/backend/tests/RealtorApiTests` para: descubrimiento de slices en tests sin duplicados, validación automática con y sin validador, respuesta 400 con `ValidationProblemDetails`, mapeo de `Result` a `ProblemDetails`, y respuesta correcta de `/health` migrado.
- **RF-017**: El sistema DEBE mantener fuera de alcance toda entidad de dominio, DbContext, configuraciones EF Core, migraciones, seeders, conexiones de base de datos y features de negocio.
- **RF-018**: El sistema DEBE evitar controllers y mantener el enfoque en Minimal APIs.
- **RF-019**: El sistema DEBE ubicar slices y validadores de prueba exclusivamente en el proyecto de tests, nunca en el proyecto de producción.
- **RF-020**: El sistema DEBE usar la versión de plataforma derivada de `global.json` existente sin modificar dicho archivo.

### Entidades clave *(incluir si la funcionalidad involucra datos)*

- **Slice Contract (`ISlice`)**: Contrato transversal que encapsula el mapeo de endpoints de una unidad vertical, descubrible por ensamblado.
- **Slice Registration Set**: Conjunto de tipos `ISlice` descubiertos y registrados para habilitar mapeo centralizado sin duplicados.
- **Handler Marker (`IHandler`)**: Contrato marcador para auto-registro de handlers de casos de uso futuros mediante escaneo de ensamblados.
- **Validation Convention Context**: Información del request usada por `ValidationFilterFactory` para resolver si existe un `IValidator<T>` aplicable o ejecutar pass-through.
- **Expected Error Result**: Representación estándar de error esperado que se transforma a `ProblemDetails` de forma consistente.
- **Health Slice**: Implementación de infraestructura de `/health` bajo patrón `ISlice` usada como verificación end-to-end de la base transversal.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **CE-001**: El backend compila correctamente en el 100% de las ejecuciones de verificación realizadas para esta iniciativa con la infraestructura transversal registrada.
- **CE-002**: El endpoint `/health` queda disponible mediante descubrimiento y mapeo centralizados sin registro manual adicional en `Program.cs`.
- **CE-003**: El 100% de los casos de prueba definidos para infraestructura transversal (slices, validación, errores, health) pasan en el proyecto de tests backend.
- **CE-004**: Cualquier request con validador aplicable recibe validación automática y cualquier request sin validador continúa sin error de infraestructura.
- **CE-005**: No se incorpora ninguna entidad, conexión de base de datos, caso de uso de negocio ni endpoint funcional de producto dentro del alcance implementado.

## Suposiciones

- La solución base y los proyectos creados en la spec 001 existen y son reutilizables sin recreación.
- El endpoint `/health` actual se considera infraestructura y su comportamiento funcional se preserva durante la migración al patrón `ISlice`.
- La restricción de acceso interno de `/health` se aplicará en capa de infraestructura (ingress/reverse proxy), no mediante autenticación en la aplicación.
- Las pruebas unitarias de infraestructura se ejecutan en el proyecto `app/backend/tests/RealtorApiTests` y pueden definir tipos de prueba (`ISlice`, `IValidator<T>`) sin afectar producción.
- El descubrimiento por ensamblado se limita a los ensamblados explícitamente definidos por la infraestructura para evitar registros inesperados.
- Los mecanismos de validación y mapeo de errores se diseñan como capacidades base para uso posterior en features de negocio.
