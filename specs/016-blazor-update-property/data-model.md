# Modelo Conceptual: Edición de inmueble existente

## Propósito

Definir las entidades funcionales y estados de interacción para actualizar inmuebles existentes desde frontend Blazor, con validaciones, manejo de errores y conservación opcional de imagen.

## Entidades conceptuales

### 1. FormularioEdicionInmueble

Representa el conjunto de datos editables enviados al endpoint de actualización.

Atributos:
- `Id` (guid/string, requerido en ruta)
- `Title` (string, editable)
- `Description` (string, editable)
- `Address` (string, editable)
- `Price` (decimal, editable)
- `Status` (enum/int según contrato, editable)
- `Image` (archivo opcional para reemplazo)

Reglas:
- Debe inicializarse con datos actuales del inmueble.
- Debe alinearse al contrato `UpdatePropertyRequest`.

### 2. EstadoCargaEdicion

Representa la carga inicial de datos de la pantalla de edición.

Atributos:
- `IsLoading` (bool)
- `LoadSucceeded` (bool)
- `LoadErrorMessage` (string?)

Reglas:
- Si falla carga inicial, se debe mostrar error recuperable con acciones de reintento y salida al listado.

### 3. EstadoValidacionEdicion

Representa validaciones locales y errores de backend asociados al formulario.

Atributos:
- `FieldErrors` (diccionario campo -> mensajes)
- `SummaryErrors` (lista de errores globales)
- `CanSubmit` (bool)

Reglas:
- Debe bloquear envío cuando existan errores activos.
- Debe mapear validaciones de backend (`400`) a campos correspondientes cuando aplique.

### 4. EstadoImagenEdicion

Representa el comportamiento opcional de la imagen durante la edición.

Atributos:
- `CurrentImageUrl` (string?)
- `SelectedReplacementImage` (archivo?)
- `PreviewUrl` (string?)
- `ImageValidationError` (string?)

Reglas:
- Si no hay selección nueva, la imagen actual se conserva.
- Si hay selección nueva válida, se usa como reemplazo.

### 5. ResultadoActualizacionInmueble

Representa la salida funcional del intento de guardado.

Atributos:
- `IsSuccess` (bool)
- `UpdatedPropertyId` (guid?)
- `ResponseFamily` (`success` | `validation` | `not-found` | `payload-too-large` | `unsupported-media-type` | `server-error`)
- `Message` (string?)

Reglas:
- En `success`, debe mostrar confirmación y navegar al detalle actualizado.
- En error, debe mantener datos del formulario para corrección/reintento.

## Relaciones

- `FormularioEdicionInmueble` depende de `EstadoCargaEdicion` para su inicialización.
- `EstadoValidacionEdicion` gobierna la habilitación de guardado del `FormularioEdicionInmueble`.
- `EstadoImagenEdicion` se integra al `FormularioEdicionInmueble` como decisión opcional de reemplazo.
- `ResultadoActualizacionInmueble` actualiza los estados visuales y decide navegación post-guardado.

## Casos límite cubiertos

- `id` inexistente en carga inicial: estado de error recuperable con reintento/volver al listado.
- `404` al guardar: informar indisponibilidad del inmueble y preservar cambios no guardados.
- `400` con errores por campo: renderizar errores en campos y conservar valores ingresados.
- `413` o `415`: mostrar error comprensible de archivo y mantener formulario.
- `500`: mostrar error global y conservar datos para reintento.

## Notas de alcance

- No se modifica persistencia ni contratos backend fuera del consumo existente.
- No incluye creación, eliminación, edición masiva ni concurrencia optimista.
