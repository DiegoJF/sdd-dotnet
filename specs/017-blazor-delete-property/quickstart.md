# Guía de validación: 017-blazor-delete-property

## Prerrequisitos

- SDK .NET 11 y PostgreSQL disponibles según la configuración de specs anteriores.
- Base de datos con datos semilla (spec 003).
- Backend `RealtorApi` y frontend `RealtorWeb` en ejecución.

## Validación automatizada

```powershell
dotnet build app/Realtor.sln
dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

Resultado esperado: compilación sin errores, pruebas de validador, handler y endpoint `DeleteProperty` en verde, y `OpenApiDriftTests` sin diferencias.

## Validación de API

Ver el contrato en [contracts/delete-property.openapi.yaml](contracts/delete-property.openapi.yaml).

| # | Solicitud | Resultado esperado |
|---|---|---|
| A1 | `DELETE /api/properties/{id-existente}` | `204`; un `GET` posterior del mismo id responde `404` y el listado no la incluye. |
| A2 | Repetir A1 con el mismo id | `404` con `ProblemDetails`. |
| A3 | `DELETE /api/properties/no-es-uuid` | `400` con `ValidationProblemDetails`. |
| A4 | Contar el total antes y después de A1 | Total disminuye en exactamente uno; otras propiedades sin cambios. |
| A5 | Eliminar una propiedad con `imageUrl` | `204`; el archivo de imagen sigue existiendo. |

## Validación de interfaz

Ver [contracts/frontend-property-delete.contract.md](contracts/frontend-property-delete.contract.md).

| # | Escenario | Resultado esperado |
|---|---|---|
| U1 | Listado → Eliminar → Cancelar | No se envía solicitud; la tarjeta permanece. |
| U2 | Listado → Eliminar → Confirmar | Mensaje de éxito; la página se recarga sin la propiedad. |
| U3 | Eliminar la única propiedad de la última página (no primera) | Se muestra la página anterior. |
| U4 | Detalle → Eliminar → Confirmar | Navega al listado con mensaje de éxito. |
| U5 | Detener el backend y confirmar | El diálogo sigue abierto con mensaje de error; la propiedad sigue visible. |
| U6 | Doble clic rápido en Eliminar del diálogo | Una sola solicitud `DELETE` (verificar en logs del backend). |
| U7 | Eliminar desde otra pestaña y confirmar en la primera | Mensaje "La propiedad ya no existe."; vista recargada o navegación al listado. |
| U8 | Abrir el detalle de una propiedad eliminada | Se muestra el estado de propiedad no encontrada. |

## Evidencia de validación

Fecha: 2026-11-04

| Escenario | Resultado | Evidencia |
|-----------|-----------|-----------|
| A1–A5 (API) | Superado | `dotnet test RealtorApiTests`: 46 superadas, 0 errores (incluye DeletePropertyEndpointTests, DeletePropertyValidatorTests y OpenApiDriftTests). |
| Compilación | Superado | `dotnet build Realtor.sln`: 0 errores, 10 advertencias preexistentes de línea base. |
| U1–U3 (listado) | Superado | Validación manual del usuario: confirmación, cancelación, recarga de página y retroceso de página vacía correctos. |
| U4, U8 (detalle) | Superado | Validación manual del usuario: navegación al listado con aviso de éxito. |
| U5–U7 (errores) | Superado | Validación manual del usuario: error genérico con diálogo abierto, 404 "La propiedad ya no existe." y sin envíos duplicados. |

Incidencia corregida durante la validación: nombres de iconos incompatibles con Lucide.Blazor 0.0.38 (`triangle-alert` → `alert-triangle`, `circle-check` → `check-circle`).
