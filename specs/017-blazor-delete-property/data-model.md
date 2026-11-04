# Modelo de datos: 017-blazor-delete-property

## Decisión de eliminación

La eliminación es **física** (ver `research.md`, R-01). No se agregan columnas, índices ni migraciones. La entidad `Property` no cambia.

## Entidades

### Property (existente, sin cambios)

| Campo | Tipo | Observación en esta iniciativa |
|---|---|---|
| `Id` | `Guid` | Identificador usado para eliminar. |
| `ImageUrl` | `string?` | No se modifica ni se elimina el recurso referenciado (RN-02). |
| Resto de campos | — | Se eliminan junto con la fila. |

**Relaciones**: `Property` no tiene entidades dependientes; la eliminación no produce cascadas (RF-06).

## Objetos de la operación (backend)

### DeletePropertyCommand

| Campo | Tipo | Regla |
|---|---|---|
| `RawId` | `string` | Obligatorio; UUID canónico (formato `D`). Si falla → 400. |
| `Id` | `Guid?` | Resultado del parseo; `null` cuando `RawId` es inválido. |

### Resultado del handler

| Resultado | Código HTTP | Cuerpo |
|---|---|---|
| Eliminada | `204 No Content` | Vacío |
| No encontrada (incluye concurrencia) | `404 Not Found` | `ProblemDetails` |
| Identificador inválido | `400 Bad Request` | `ValidationProblemDetails` |

## Modelos de frontend

### Estado del diálogo de eliminación

```text
Cerrado --(Eliminar)--> Confirmando
Confirmando --(Cancelar)--> Cerrado            [sin solicitud]
Confirmando --(Eliminar)--> Eliminando         [botones deshabilitados]
Eliminando --(204)--> Cerrado + OnDeleted
Eliminando --(404)--> Cerrado + OnNotFound
Eliminando --(otro error/red)--> ErrorEnDialogo [botones habilitados]
ErrorEnDialogo --(Eliminar)--> Eliminando
ErrorEnDialogo --(Cancelar)--> Cerrado
```

### PropertyNotice

| Campo | Tipo | Valores |
|---|---|---|
| `Kind` | enum | `Success`, `Info` |
| `Message` | `string` | "La propiedad fue eliminada correctamente." / "La propiedad ya no existe." |

Aviso de un solo uso almacenado en `PropertyNoticeService` (ámbito por circuito).
