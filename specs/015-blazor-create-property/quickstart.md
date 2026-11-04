# Quickstart de Validación: Alta de inmuebles en frontend Blazor

## Objetivo

Validar de extremo a extremo el flujo de alta de inmuebles desde frontend, incluyendo validaciones en tiempo real, imagen opcional con vista previa, manejo de errores contractuales y redirección al detalle en caso exitoso.

## Prerrequisitos

- .NET SDK alineado con `global.json`.
- Backend `RealtorApi` disponible y accesible por `RealtorWeb`.
- Contrato OpenAPI vigente con operación `CreateProperty`.
- Frontend restaurado correctamente.

## Escenario 1: Compilación de frontend

1. Ejecutar:

```powershell
dotnet restore app/frontend/src/RealtorWeb/RealtorWeb.csproj
dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj
```

2. Resultado esperado:
- Compilación exitosa.
- Sin errores de contratos Refit ni de componentes de alta.

## Escenario 2: Validación en tiempo real y resumen al enviar

1. Abrir la ruta de alta de inmuebles.
2. Introducir datos inválidos en varios campos.
3. Corregir parcialmente y volver a intentar envío.

Resultado esperado:
- Cada campo inválido muestra error en tiempo real.
- El envío muestra resumen consolidado de errores vigentes.
- Al corregir, desaparecen solo los errores resueltos.

## Escenario 3: Imagen opcional

1. Probar envío válido sin imagen.
2. Seleccionar imagen válida y verificar vista previa.
3. Reemplazar imagen seleccionada.
4. Quitar imagen antes de enviar.

Resultado esperado:
- El envío sin imagen es posible.
- La vista previa se actualiza con la última selección.
- Reemplazo y eliminación funcionan antes de enviar.

## Escenario 4: Alta exitosa (201)

1. Completar formulario con datos válidos.
2. Enviar solicitud.

Resultado esperado:
- Se muestra confirmación visible de éxito.
- Se redirige automáticamente al detalle del inmueble creado.

## Escenario 5: Error de validación backend (400)

1. Forzar condición para recibir `400`.
2. Enviar solicitud.

Resultado esperado:
- Se muestran errores por campo alineados con backend.
- El formulario conserva todos los datos ingresados.

## Escenario 6: Error de tamaño de payload (413)

1. Adjuntar archivo de imagen que exceda límite permitido.
2. Enviar solicitud.

Resultado esperado:
- Se muestra mensaje global de error para `413`.
- El formulario conserva datos para reintento.

## Escenario 7: Error de tipo de media (415)

1. Adjuntar archivo con tipo no permitido.
2. Enviar solicitud.

Resultado esperado:
- Se muestra mensaje global de error para `415`.
- El formulario conserva datos para reintento.

## Escenario 8: Error interno (500)

1. Simular fallo de backend al crear.
2. Enviar solicitud válida.

Resultado esperado:
- Se muestra mensaje global de error para `500`.
- El formulario conserva datos para reintento.

## Criterios operativos de verificación

- `SC-001`: Cumplido si en todos los casos de invalidación probados se observan errores por campo en tiempo real y resumen al enviar.
- `SC-002`: Cumplido si toda alta exitosa validada manualmente muestra confirmación y redirige al detalle correcto.
- `SC-003`: Cumplido si todas las pruebas `400` conservan datos y muestran errores por campo.
- `SC-004`: Cumplido si todas las pruebas `413`, `415` y `500` conservan datos y muestran mensaje global.
- `SC-005`: Cumplido si alta sin imagen y flujo con vista previa/reemplazo/eliminación se verifican manualmente.

## Evidencia de validación

- Fecha: 2026-07-25
- Ejecución: `dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj`
- Resultado: compilación exitosa de `RealtorWeb` en `net11.0` sin errores.
- Evidencia relevante para `T029`: comando ejecutado y resultado verificado.
- Evidencia manual ejecutada (2026-07-25):
	- `SC-001`: validado feedback por campo en tiempo real y resumen consolidado al enviar con campos inválidos.
	- `SC-002`: validada confirmación visible posterior a `201` y redirección automática a `/properties/{id}`.
	- `SC-003`: validado escenario `400` con conservación de datos ingresados y renderizado de errores por campo.
	- `SC-004`: validados escenarios `413`, `415` y `500` con mensaje global y conservación de datos para reintento.
	- `SC-005`: validado envío sin imagen y flujo completo de imagen opcional (preview, reemplazo y eliminación).
- Evidencia transversal `T035`: revisión de coherencia realizada entre spec, plan y tasks de la feature 015; no se detectaron contradicciones de alcance ni de orden de implementación.

## Referencias

- Spec funcional: `spec.md`
- Investigación: `research.md`
- Modelo conceptual: `data-model.md`
- Contrato: `contracts/frontend-property-create.contract.yaml`