# Contrato de interfaz: eliminación de propiedad en frontend

## Cliente Refit (`IPropertiesApi`)

```text
[Delete("/api/properties/{id}")]
Task DeletePropertyAsync(string id, CancellationToken cancellationToken = default)
```

- `204` → la tarea finaliza sin excepción.
- `404` → `ApiException` con `StatusCode == NotFound`.
- `400`, `5xx` → `ApiException`.
- Fallo de red → `HttpRequestException` o `TaskCanceledException`.

## Componente `PropertyDeleteDialog`

| Parámetro | Tipo | Descripción |
|---|---|---|
| `PropertyId` | `string` | Identificador a eliminar. |
| `PropertyTitle` | `string` | Título mostrado en el diálogo. |
| `IsOpen` | `bool` | Controla la visibilidad. |
| `OnCancel` | `EventCallback` | Cancelación sin solicitud. |
| `OnDeleted` | `EventCallback` | Eliminación con `204`. |
| `OnNotFound` | `EventCallback` | Respuesta `404`. |

Comportamiento obligatorio:

- Muestra el título y las acciones "Eliminar" y "Cancelar" (RF-08).
- Durante la solicitud, ambas acciones permanecen deshabilitadas (RF-13).
- Ante errores distintos de `404`, permanece abierto con mensaje de error y acciones habilitadas (RF-15).

## Puntos de integración

| Ubicación | Acción tras `OnDeleted` | Acción tras `OnNotFound` |
|---|---|---|
| `PropertyCard` / `PropertyList` | Aviso de éxito + recarga de página actual; página anterior si queda vacía (RF-14) | Aviso "La propiedad ya no existe." + misma recarga |
| `PropertyDetail` | Aviso de éxito en `PropertyNoticeService` + navegar a `/` (RF-11) | Aviso "La propiedad ya no existe." + navegar a `/` (RF-16) |
