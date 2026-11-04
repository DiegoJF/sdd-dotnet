---
name: 016-blazor-update-property
agent: agent
---
Sigue exactamente el proceso, las reglas y el flujo interactivo definidos en:

.github\prompts\speckit-specify-prompt-interactive.prompt.md

Utiliza ese prompt como procedimiento principal para generar la nueva especificación.

La solicitud que debes procesar mediante ese flujo es la siguiente:

Quiero crear una funcionalidad que permita actualizar la información de un inmueble existente. El nombre de la spec sera 016-blazor-update-property.

Ya existe un endpoint para realizar esta operación. Su definición se encuentra en app\backend\src\RealtorApi\wwwroot\openapi\v1.json y su operationId es UpdateProperty. Utiliza este contrato como fuente de verdad para identificar la ruta, los parámetros, el cuerpo de la solicitud, las validaciones y la respuesta esperada.

Dentro del componente app\frontend\src\RealtorWeb\Features\Properties\List\PropertyCard.razor, se debe agregar un botón o acción para editar el inmueble representado por la tarjeta. Al seleccionar esta acción, el usuario debe ser dirigido a una página de edición dentro del proyecto app\frontend\src\RealtorWeb\RealtorWeb.csproj.

La página de edición debe:

Obtener la información actual del inmueble seleccionado.
Mostrar los datos existentes dentro de un formulario.
Permitir modificar la información editable del inmueble.
Aplicar validaciones en todos los campos según las reglas del contrato y las convenciones existentes del proyecto.
Mostrar mensajes de validación claros junto a los campos correspondientes.
Consumir el endpoint cuyo operationId es UpdateProperty para guardar los cambios.
Evitar el envío del formulario mientras existan errores de validación.
Mostrar estados visuales durante la carga y el guardado de la información.
Mostrar una confirmación cuando la actualización se complete correctamente.
Mostrar un mensaje de error comprensible cuando la actualización no pueda realizarse.
Evitar que múltiples clics generen solicitudes duplicadas.
Mantener la información ingresada si el servidor devuelve un error.
Regresar al listado o detalle del inmueble después de una actualización exitosa.

La imagen que representa el inmueble debe conservarse si el usuario no selecciona una nueva. La modificación o reemplazo de la imagen debe ser opcional y debe respetar lo definido por el contrato del endpoint UpdateProperty.

La implementación debe reutilizar los componentes, estilos, servicios HTTP, modelos, sistema de diseño y patrones de manejo de errores que ya existen en app\frontend\src\RealtorWeb\RealtorWeb.csproj.

La especificación debe enfocarse en el comportamiento esperado y en los criterios de aceptación, sin definir todavía detalles internos de implementación.