# Especificación de Funcionalidad: Alta de inmuebles en frontend Blazor

**Rama de Funcionalidad**: `015-blazor-create-property`

**Creado**: 2026-07-25

**Estado**: Implementada

**Entrada**: Descripción del usuario: "Quiero crear una funcionalidad para dar de alta nuevos inmuebles. Ya existe el endpoint para realizar esta tarea, el endpoint se encuentra en `v1.json` y el operationId es `CreateProperty`. Queremos crear una pagina en el proyecto `RealtorWeb.csproj` con un formulario donde se pueda ingresar la informacion del inmueble, aplicando validaciones en todos los campos, la carga de la imagen que representa el inmueble debe ser opcional."

## Ciclo de estado del spec *(obligatorio)*

- Estado inicial: **Borrador**
- Transiciones permitidas: **Borrador -> Aprobada -> En implementacion -> Implementada**
- Esta prohibido cerrar en **Implementada** sin todas las tareas completadas en `tasks.md`
- Esta prohibido cerrar en **Implementada** sin evidencia de validacion en `quickstart.md`

## Trazabilidad de estado *(obligatorio)*

- 2026-07-25 Borrador - Spec creado por `/speckit.specify`
- 2026-07-25 En implementación -> Implementada - Cierre automático validado (tasks sin pendientes y quickstart con evidencia)

## Clarificaciones

### Sesion 2026-07-25

- Q: Que politica funcional aplicar para validaciones del formulario en frontend? -> A: Validar en frontend las mismas reglas funcionales del backend, mostrando errores por campo en tiempo real y un resumen al enviar.
- Q: Que comportamiento debe verse tras alta exitosa? -> A: Redirigir automaticamente al detalle del inmueble recien creado y mostrar confirmacion de exito.
- Q: Como manejar errores al enviar? -> A: Para 400 mostrar errores por campo y conservar datos; para 413, 415 y 500 mostrar mensaje global y conservar datos para reintento.
- Q: Como debe comportarse el campo de imagen opcional antes de enviar? -> A: Mostrar vista previa, validar tipo y tamano al seleccionar, y permitir quitar o reemplazar antes del envio.
- Q: Que funcionalidades quedan fuera de alcance? -> A: Edicion, eliminacion, alta masiva y multiples imagenes por inmueble.
- Q: Que evidencia funcional se exige para cierre? -> A: Validacion manual completa de exito, 400, 413, 415 y 500, incluyendo conservacion de datos y redireccion a detalle.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Registrar un inmueble con validaciones en frontend (Prioridad: P1)

Como persona operadora del catalogo, quiero completar un formulario de alta con validaciones claras en tiempo real para registrar un inmueble correctamente en un solo intento.

**Por qué esta prioridad**: Es el flujo principal de negocio de la iniciativa y habilita el alta funcional de inmuebles desde el frontend.

**Prueba independiente**: Se valida completando todos los campos requeridos con datos validos y verificando que el formulario permite enviar, sin errores activos, hacia el endpoint de alta.

**Escenarios de aceptación**:

1. **Dado** que la persona operadora abre la pagina de alta, **Cuando** completa los campos con datos validos, **Entonces** el formulario refleja estado valido en todos los campos y permite enviar.
2. **Dado** que existe un dato invalido en cualquier campo, **Cuando** ese campo pierde foco o cambia su valor, **Entonces** se muestra un error por campo en tiempo real y se impide el envio valido hasta corregirlo.
3. **Dado** que existen multiples errores al intentar enviar, **Cuando** la persona operadora confirma el envio, **Entonces** se muestra un resumen de errores y cada campo afectado mantiene su mensaje contextual.

---

### Historia de Usuario 2 - Completar alta exitosa con redireccion a detalle (Prioridad: P2)

Como persona operadora del catalogo, quiero ver confirmacion de exito y navegar automaticamente al detalle del inmueble creado para verificar inmediatamente el resultado del alta.

**Por qué esta prioridad**: Cierra el flujo funcional con evidencia visible de que la operacion de creacion se completo correctamente.

**Prueba independiente**: Se valida enviando una alta valida y verificando confirmacion visible de exito y redireccion automatica al detalle del recurso creado.

**Escenarios de aceptación**:

1. **Dado** un formulario valido, **Cuando** la persona operadora envia la solicitud de alta, **Entonces** el sistema confirma exito y redirige automaticamente al detalle del inmueble creado.
2. **Dado** que la redireccion a detalle ocurre tras el alta, **Cuando** la nueva pantalla carga, **Entonces** se observa el inmueble creado correspondiente al identificador retornado por la operacion de creacion.

---

### Historia de Usuario 3 - Gestionar imagen opcional y recuperarse de errores (Prioridad: P3)

Como persona operadora del catalogo, quiero adjuntar imagen solo cuando lo necesite y poder recuperarme de errores sin perder la informacion ingresada.

**Por qué esta prioridad**: Asegura robustez y usabilidad del flujo de alta bajo condiciones reales de validacion y errores de servicio.

**Prueba independiente**: Se valida con envio sin imagen, con imagen valida y con respuestas de error 400, 413, 415 y 500, comprobando los mensajes esperados y la conservacion de datos.

**Escenarios de aceptación**:

1. **Dado** que la imagen es opcional, **Cuando** la persona operadora no adjunta archivo y envia datos validos, **Entonces** la operacion de alta puede completarse exitosamente.
2. **Dado** que la persona operadora adjunta una imagen, **Cuando** la selecciona antes del envio, **Entonces** la interfaz muestra vista previa, valida tipo y tamano, y permite quitar o reemplazar el archivo.
3. **Dado** que la solicitud falla con 400, **Cuando** se procesa la respuesta, **Entonces** se muestran errores por campo y se conserva toda la informacion ya ingresada.
4. **Dado** que la solicitud falla con 413, 415 o 500, **Cuando** se procesa la respuesta, **Entonces** se muestra un mensaje global de error y se conservan los datos para reintentar.

---

### Casos límite

- Si la persona operadora reemplaza una imagen previamente seleccionada antes del envio, la ultima seleccion debe prevalecer en la vista previa y en el envio.
- Si la persona operadora quita la imagen antes de enviar, el formulario debe volver al estado de imagen opcional sin error por obligatoriedad.
- Si la solicitud devuelve errores y la persona corrige solo una parte de los campos invalidos, el sistema debe mantener visibles unicamente los errores aun vigentes.
- Si ocurre error tecnico (413, 415 o 500) inmediatamente despues de un intento valido, los datos ingresados deben permanecer intactos para permitir reintento.

## Requisitos *(obligatorio)*

### Requisitos Funcionales

- **FR-001**: El sistema DEBE ofrecer una pagina de alta de inmuebles en el frontend Blazor para capturar titulo, descripcion, direccion, precio, estado, cantidad de dormitorios, cantidad de banos, area en metros cuadrados e imagen opcional.
- **FR-001A**: La pagina de alta DEBE estar disponible en la ruta funcional canonica `/properties/create`.
- **FR-002**: El sistema DEBE aplicar en frontend las reglas funcionales de validacion definidas explícitamente en la sección "Reglas de validación funcional del formulario" y mantenerlas alineadas con el backend.
- **FR-003**: El sistema DEBE mostrar errores por campo en tiempo real cuando un dato no cumpla la validacion correspondiente.
- **FR-004**: El sistema DEBE mostrar un resumen de errores al enviar cuando existan validaciones pendientes.
- **FR-005**: El sistema DEBE permitir enviar el formulario sin imagen y tratar la imagen como opcional.
- **FR-006**: Cuando la persona usuaria seleccione una imagen, el sistema DEBE mostrar vista previa y validar tipo y tamano antes del envio.
- **FR-007**: El sistema DEBE permitir quitar o reemplazar la imagen seleccionada antes de enviar.
- **FR-008**: Ante una respuesta 400, el sistema DEBE mostrar errores por campo y conservar la informacion ingresada.
- **FR-009**: Ante respuestas 413, 415 o 500, el sistema DEBE mostrar mensaje global de error y conservar la informacion ingresada para reintento.
- **FR-010**: Cuando la operacion de alta sea exitosa, el sistema DEBE mostrar confirmacion y redirigir automaticamente al detalle del inmueble creado.
- **FR-011**: La iniciativa DEBE limitarse a alta individual de inmuebles y NO DEBE incluir edicion, eliminacion, alta masiva ni multiples imagenes por inmueble.

### Reglas de validación funcional del formulario

- **RV-001**: Los campos `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount` y `areaSquareMeters` son requeridos para una alta válida.
- **RV-002**: El campo `image` es opcional y su ausencia no invalida la solicitud si el resto de reglas se cumple.
- **RV-003**: `price`, `bedroomCount`, `bathroomCount` y `areaSquareMeters` deben presentarse como valores numéricos válidos según el contrato funcional vigente.
- **RV-004**: Si un valor no cumple una regla, el sistema debe reflejar el error en el campo correspondiente durante la edición.
- **RV-005**: Si existen uno o más errores al enviar, el sistema debe mostrar un resumen de errores sin perder los valores ingresados.
- **RV-006**: Si se selecciona imagen, su validación previa al envío debe comprobar formatos permitidos `image/png` y `image/jpeg`, y límite máximo de 5 MB por archivo.

### Entidades Clave

- **Formulario de alta de inmueble**: Superficie funcional donde se capturan y validan los datos requeridos para registrar un inmueble.
- **Estado de validacion de campo**: Resultado visible por campo que indica si un valor es valido o cual regla no se cumple.
- **Resumen de errores de envio**: Consolidado visible de errores de validacion cuando la solicitud no puede procesarse correctamente.
- **Imagen opcional del inmueble**: Archivo adjunto no obligatorio con vista previa, validacion previa y posibilidad de reemplazo o eliminacion antes del envio.
- **Resultado de alta**: Salida funcional del proceso de creacion, ya sea exito con redireccion al detalle o error manejado con mensajes y conservacion de datos.

## Fuera de alcance

- Edicion de inmuebles existentes.
- Eliminacion de inmuebles.
- Alta masiva de inmuebles.
- Carga de multiples imagenes por inmueble en la misma operacion.

## Criterios de Éxito *(obligatorio)*

### Resultados Medibles

- **SC-001**: En el 100% de los casos de datos invalidos validados, la interfaz muestra errores por campo en tiempo real y resumen al enviar.
- **SC-002**: En el 100% de las altas exitosas validadas manualmente, la interfaz muestra confirmacion y redirige al detalle del inmueble creado.
- **SC-003**: En el 100% de los casos con respuesta 400 validados manualmente, la interfaz conserva datos y muestra errores por campo.
- **SC-004**: En el 100% de los casos con respuesta 413, 415 y 500 validados manualmente, la interfaz conserva datos y muestra mensaje global de error.
- **SC-005**: En el 100% de las validaciones manuales de imagen opcional, se confirma que el sistema permite alta sin imagen y, con imagen seleccionada, permite vista previa, reemplazo y eliminacion antes de enviar.

## Suposiciones

- El contrato funcional existente de alta de inmuebles se mantiene estable durante la implementacion de esta iniciativa.
- El identificador del inmueble creado esta disponible para resolver la redireccion automatica hacia su vista de detalle.
- La validacion manual de los codigos 400, 413, 415 y 500 puede realizarse en entorno de desarrollo controlado.
