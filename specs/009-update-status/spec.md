# Especificación de funcionalidad: Actualización de estado de propiedades

**Rama de la funcionalidad**: `feat/009-update-status`

**Creado**: 2026-07-21

**Estado**: Implementada

**Entrada**: Descripción del usuario: "Quiero crear una nueva especificación llamada 009-update-status para el backend de Realtor API, basada en el estilo de la especificación 006-properties-update, pero con un alcance mucho más pequeño. Objetivo: crear un endpoint público para actualizar únicamente el estado de una propiedad existente identificada por su id."

## Ciclo de estado de la spec *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementación -> Implementada**
- Está prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Está prohibido cerrar en **Implementada** sin evidencia de validación en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- 2026-07-21 Borrador - Spec creada por `/speckit.specify`
- 2026-07-21 Aprobada -> En implementación - Inicio de la implementación tras validar prerrequisitos y disponibilidad de tasks.md.
- 2026-07-21 En implementación -> Implementada - Cierre aprobado tras verificar que `tasks.md` no contiene tareas pendientes y que `quickstart.md` incluye evidencia de validación no vacía.

## Escenarios de usuario y pruebas *(obligatorio)*

### Historia de usuario 1 - Actualizar solo el estado de una propiedad (Prioridad: P1)

Como operador del catálogo, quiero cambiar el estado de una propiedad existente identificada por su id, para reflejar su situación actual sin alterar ningún otro dato persistido.

**Por qué esta prioridad**: Es el caso de uso principal y único de la iniciativa. Sin este flujo no existe valor funcional para la spec.

**Prueba independiente**: Puede verificarse enviando una solicitud válida para una propiedad existente y comprobando que solo cambia el estado mientras el resto de campos, incluido `imageUrl`, permanece intacto.

**Escenarios de aceptación**:

1. **Dado** que existe una propiedad con un id válido, **cuando** se envía una solicitud con un estado válido, **entonces** el sistema responde `200 OK` y devuelve la propiedad actualizada con el nuevo estado.
2. **Dado** que existe una propiedad con un id válido, **cuando** se actualiza su estado, **entonces** todos los demás datos persistidos permanecen sin modificación, incluido `imageUrl`.
3. **Dado** que la propiedad ya tenía un estado previo distinto, **cuando** la actualización finaliza correctamente, **entonces** el estado nuevo queda persistido para futuras consultas.

---

### Historia de usuario 2 - Rechazar solicitudes inválidas o inexistentes (Prioridad: P2)

Como consumidor de la API, quiero recibir errores claros cuando el estado falte, sea inválido o la propiedad no exista, para corregir la solicitud sin efectos secundarios.

**Por qué esta prioridad**: Garantiza integridad de datos y una experiencia predecible, pero depende del flujo principal de actualización.

**Prueba independiente**: Puede verificarse con solicitudes que omiten el estado, envían un valor no permitido o apuntan a un id inexistente, comprobando el código HTTP y la ausencia de cambios persistidos.

**Escenarios de aceptación**:

1. **Dado** que la solicitud no incluye estado, **cuando** se intenta actualizar la propiedad, **entonces** el sistema responde `400 Bad Request`.
2. **Dado** que la solicitud incluye un estado no válido, **cuando** se intenta actualizar la propiedad, **entonces** el sistema responde `400 Bad Request`.
3. **Dado** que no existe una propiedad con el id solicitado, **cuando** se intenta actualizar el estado, **entonces** el sistema responde `404 Not Found`.
4. **Dado** que la solicitud incluye campos distintos de `status`, **cuando** se intenta procesar la actualización, **entonces** el sistema responde `400 Bad Request` y no modifica otros datos.

---

### Casos límite

- Si el cuerpo llega vacío, la operación debe rechazarse con `400 Bad Request`.
- Si el valor de estado no corresponde a un valor admitido por el catálogo actual, la operación debe rechazarse con `400 Bad Request`.
- Si el identificador del path no corresponde a ninguna propiedad persistida, la operación no debe modificar ningún dato.
- Si la propiedad ya tiene el estado solicitado, la operación sigue siendo exitosa y conserva intactos los demás campos.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **FR-001**: El sistema DEBE exponer una operación pública para actualizar el estado de una propiedad existente identificada por su `id`.
- **FR-002**: El sistema DEBE permitir únicamente la modificación del campo `status` en esta operación.
- **FR-003**: El sistema DEBE rechazar con `400 Bad Request` cualquier solicitud que omita `status`, envíe un valor no válido, intente incluir otros campos editables o envíe propiedades JSON inesperadas.
- **FR-004**: El sistema DEBE verificar que la propiedad exista antes de aplicar cambios y DEBE responder `404 Not Found` cuando no exista.
- **FR-005**: El sistema DEBE devolver `200 OK` con la propiedad actualizada cuando la actualización sea exitosa.
- **FR-006**: El sistema DEBE conservar sin cambios todos los demás datos persistidos de la propiedad, incluido `imageUrl`.
- **FR-007**: El sistema DEBE representar los errores funcionales mediante un formato consistente de ProblemDetails.
- **FR-008**: El sistema DEBE publicar el contrato del endpoint de forma que pueda ser verificado manualmente desde el archivo `.http` del proyecto.

### Entidades clave *(incluir si la funcionalidad involucra datos)*

- **Propiedad**: Registro persistido del catálogo inmobiliario que se identifica por `id` y contiene un estado junto con otros datos visibles para el usuario.
- **Solicitud de actualización de estado**: Entrada mínima que transporta el `status` nuevo para una propiedad existente.
- **Respuesta de propiedad actualizada**: Representación devuelta al confirmar la operación exitosa, con el estado nuevo y el resto de datos conservados.
- **ProblemDetails de validación**: Respuesta estructurada para errores de validación o ausencia de la propiedad.

## Criterios de éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de las solicitudes válidas a propiedades existentes devuelve `200 OK` y actualiza solo el estado.
- **SC-002**: El 100% de las solicitudes sin `status`, con un `status` no válido o con propiedades JSON inesperadas devuelve `400 Bad Request`.
- **SC-003**: El 100% de las solicitudes dirigidas a un `id` inexistente devuelve `404 Not Found`.
- **SC-004**: En una verificación manual con el archivo `.http`, un revisor puede demostrar el flujo exitoso y los dos errores principales en menos de 5 minutos.

## Suposiciones

- Los valores válidos de `status` ya existen en el dominio actual del catálogo y se reutilizan sin cambios.
- El contrato de respuesta de propiedad ya definido en specs previas se mantiene como base del objeto devuelto.
- El endpoint es público y no requiere autenticación.
- La imagen y cualquier comportamiento relacionado con archivos quedan fuera de alcance para esta iniciativa.