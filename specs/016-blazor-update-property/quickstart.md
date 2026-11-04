# Quickstart de Validación: Edición de inmueble en frontend Blazor

## Objetivo

Validar de extremo a extremo el flujo de edición de inmuebles existentes desde la UI: acceso desde listado, carga inicial, validaciones, actualización por contrato `UpdateProperty`, manejo de errores y navegación al detalle tras éxito.

## Prerrequisitos

- .NET SDK alineado con `global.json`.
- Backend `RealtorApi` en ejecución y accesible por `RealtorWeb`.
- OpenAPI vigente con operación `UpdateProperty` en `app/backend/src/RealtorApi/wwwroot/openapi/v1.json`.
- Frontend restaurado correctamente.

## Escenario 1: Compilación de frontend

1. Ejecutar:

```powershell
dotnet restore app/frontend/src/RealtorWeb/RealtorWeb.csproj
dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj
```

2. Resultado esperado:
- Compilación exitosa.
- Sin errores de integración Refit para operaciones de propiedades.

## Escenario 2: Entrada al flujo de edición desde listado

1. Abrir la página de listado de propiedades.
2. Seleccionar la acción Editar de una tarjeta.

Resultado esperado:
- Navegación a la ruta de edición del inmueble seleccionado.
- Inicio de carga de datos actuales.

## Escenario 3: Carga inicial correcta de datos

1. Abrir edición para un inmueble existente.

Resultado esperado:
- El formulario muestra valores actuales del inmueble.
- Estado visual de carga desaparece al completar.

## Escenario 4: Validaciones y bloqueo de envío

1. Modificar campos con valores inválidos.
2. Intentar guardar.

Resultado esperado:
- Mensajes claros junto a campos inválidos.
- El envío permanece bloqueado mientras haya errores.

## Escenario 5: Actualización exitosa (200)

1. Corregir campos a valores válidos.
2. Guardar cambios.

Resultado esperado:
- Se muestra confirmación visible de éxito.
- Se redirige automáticamente al detalle del inmueble actualizado.

## Escenario 6: Error de carga inicial

1. Abrir edición para un id inexistente o forzar fallo de carga.

Resultado esperado:
- Mensaje de error en la misma pantalla.
- Acciones visibles: Reintentar y Volver al listado.

## Escenario 7: Errores de guardado (`400`, `404`, `413`, `415`, `500`)

1. Provocar cada código de error de forma controlada.

Resultado esperado:
- `400`: errores de validación por campo.
- `404`: mensaje comprensible de recurso no disponible.
- `413` y `415`: mensaje comprensible sobre archivo/medio.
- `500`: mensaje global de error.
- En todos los casos de error, los datos ingresados se conservan para reintento.

## Escenario 8: Prevención de solicitudes duplicadas

1. Durante guardado en curso, hacer múltiples clics rápidos en Guardar.

Resultado esperado:
- Se emite una sola solicitud de actualización.
- La UI mantiene estado de guardado hasta respuesta.

## Escenario 9: Conservación/reemplazo de imagen

1. Guardar cambios sin seleccionar nueva imagen.
2. Guardar cambios seleccionando nueva imagen válida.

Resultado esperado:
- Sin nueva imagen: se conserva imagen existente.
- Con nueva imagen: se aplica reemplazo según contrato.

## Criterios operativos de verificación

- `SC-001`: Cumplido si editar desde tarjeta abre edición del inmueble correcto en todos los casos validados.
- `SC-002`: Cumplido si toda actualización válida finaliza con confirmación y redirección al detalle.
- `SC-003`: Cumplido si toda invalidación bloquea envío y muestra mensajes por campo.
- `SC-004`: Cumplido si todos los errores `400`, `404`, `413`, `415`, `500` preservan datos y muestran feedback comprensible.
- `SC-005`: Cumplido si se verifica conservación de imagen cuando no se reemplaza y reemplazo correcto cuando sí se selecciona nueva.

## Evidencia de validación

- Fecha: 2026-07-25
- Compilación ejecutada:
	- `dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj`
	- Resultado: compilación exitosa de `RealtorWeb` (`net11.0`).
- Pruebas ejecutadas:
	- `dotnet test app/frontend/test/RealtorWeb/RealtorWebTests.csproj`
	- Resultado: 1 prueba total, 1 exitosa, 0 fallidas, 0 omitidas.
	- Observación: warning de vulnerabilidad moderada `NU1902` reportado por dependencia transitiva en proyecto de pruebas.
- Validación funcional de trazabilidad (documental):
	- Cobertura FR/RV/SC revisada contra `tasks.md` y artefactos de implementación de la feature Update.
	- Estados visuales y manejo de errores quedaron implementados en `PropertyUpdateState.razor` y `PropertyUpdateForm.razor(.cs)`.

Pendiente para validación manual en entorno interactivo:
- Ejecución guiada de escenarios 2 a 9 contra backend activo para confirmar comportamiento end-to-end en UI.

## Referencias

- Spec funcional: `spec.md`
- Plan de implementación: `plan.md`
- Investigación: `research.md`
- Modelo conceptual: `data-model.md`
- Contrato de integración: `contracts/frontend-property-update.contract.yaml`
