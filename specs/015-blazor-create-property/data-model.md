# Modelo Conceptual: Alta de inmuebles en frontend Blazor

## Propósito

Definir los conceptos funcionales del flujo de alta de inmueble con imagen opcional, validación equivalente a backend y navegación posterior al detalle.

## Entidades conceptuales

### 1. FormularioAltaInmueble

Representa la estructura funcional de captura para crear un inmueble.

Atributos:
- `Title` (string, requerido)
- `Description` (string, requerido)
- `Address` (string, requerido)
- `Price` (decimal, requerido)
- `Status` (enum/int, requerido)
- `BedroomCount` (int, requerido)
- `BathroomCount` (int, requerido)
- `AreaSquareMeters` (decimal, requerido)
- `Image` (file, opcional)

Reglas:
- Debe alinearse con el contrato `CreatePropertyRequest`.
- No debe introducir campos adicionales fuera del contrato.

### 2. EstadoValidacionFormulario

Representa la validez funcional de los campos y su feedback visual.

Atributos:
- `FieldErrors` (diccionario campo -> mensajes)
- `SummaryErrors` (lista de errores para envío)
- `IsValid` (bool)

Reglas:
- Debe actualizarse en tiempo real por campo.
- Debe consolidar resumen al intentar enviar con errores.

### 3. EstadoImagenOpcional

Representa el ciclo de vida de la imagen en el formulario antes del envío.

Atributos:
- `SelectedFileName` (string?)
- `PreviewUrl` (string?)
- `IsImagePresent` (bool)
- `ImageValidationError` (string?)

Reglas:
- Si no hay imagen, el formulario sigue siendo válido según el resto de campos.
- Debe permitir quitar o reemplazar imagen antes de enviar.

### 4. ResultadoAltaInmueble

Representa la salida funcional de la operación de creación.

Atributos:
- `IsSuccess` (bool)
- `CreatedPropertyId` (guid?)
- `GlobalMessage` (string?)
- `ResponseFamily` (`success` | `validation` | `payload-too-large` | `unsupported-media-type` | `server-error`)

Reglas:
- En éxito, debe habilitar redirección al detalle del inmueble creado.
- En error, debe conservar datos de formulario para reintento.

### 5. EstadoPantallaAlta

Representa estados de interacción global de la pantalla.

Atributos:
- `Idle` (inicial)
- `Editing` (edición activa)
- `Submitting` (envío en curso)
- `SubmitSucceeded` (alta confirmada)
- `SubmitFailed` (alta fallida)

Reglas:
- No debe bloquear edición permanente tras error.
- Debe permitir reintento sin perder datos.

## Relaciones

- `FormularioAltaInmueble` se valida mediante `EstadoValidacionFormulario`.
- `EstadoImagenOpcional` forma parte de `FormularioAltaInmueble` y condiciona validación de archivo.
- `ResultadoAltaInmueble` actualiza `EstadoPantallaAlta` tras cada envío.
- En `SubmitSucceeded`, `CreatedPropertyId` alimenta navegación a detalle.

## Casos límite cubiertos

- Reemplazo de imagen antes de enviar: prevalece la última selección.
- Eliminación de imagen antes de enviar: vuelve a estado opcional sin error de obligatoriedad.
- Corrección parcial de errores: persisten solo errores vigentes.
- Error técnico posterior a envío válido: formulario conserva datos completos para reintento.

## Notas de alcance

- Este modelo no incluye edición, eliminación, alta masiva ni múltiples imágenes por inmueble.
- No incluye cambios de persistencia ni modificaciones al contrato backend.