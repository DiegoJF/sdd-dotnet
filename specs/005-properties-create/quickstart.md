# Quickstart de Validación: Alta de propiedades con imagen opcional

## Objetivo

Validar de extremo a extremo el endpoint público de creación de propiedades con imagen opcional, incluyendo éxito sin imagen, éxito con imagen válida y rechazos por formato/tamaño.

## Prerrequisitos

- .NET SDK `11.0.100-preview.5.26302.115`
- Backend configurado en desarrollo (`app/backend/src/RealtorApi/appsettings.Development.json`)
- Carpeta de activos accesible: `app/backend/src/RealtorApi/wwwroot/assets/properties`

## Preparación

1. Restaurar y compilar:

```powershell
dotnet restore app/Realtor.sln
dotnet build app/backend/src/RealtorApi/RealtorApi.csproj
dotnet build app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

2. Iniciar API:

```powershell
dotnet run --project app/backend/src/RealtorApi/RealtorApi.csproj
```

## Escenario 1: Alta exitosa sin imagen (P1)

1. Ejecutar solicitud de creación sin archivo de imagen.
2. Resultado esperado:
- Estado `201`.
- Propiedad creada correctamente.
- `imageUrl = null`.

## Escenario 2: Alta exitosa con imagen PNG/JPG <= 5 MB (P2)

1. Ejecutar solicitud de creación con imagen válida.
2. Resultado esperado:
- Estado `201`.
- Imagen almacenada en `wwwroot/assets/properties`.
- Nombre persistido generado por servidor (`UUID + extensión válida`).
- `imageUrl` no nulo y utilizable por clientes.

## Escenario 3: Rechazo por formato inválido

1. Enviar alta con archivo no PNG/JPG, archivo vacío (0 bytes) o archivo con contenido real inválido para el formato declarado.
2. Resultado esperado:
- Estado `415 Unsupported Media Type`.
- Respuesta ProblemDetails coherente.
- No se crea propiedad ni archivo.

## Escenario 4: Rechazo por tamaño mayor a 5 MB

1. Enviar alta con imagen que supera 5 MB.
2. Resultado esperado:
- Estado `413 Payload Too Large`.
- Respuesta ProblemDetails coherente.
- No se crea propiedad ni archivo.

## Escenario 5: Atomicidad ante fallo interno de guardado

1. Simular fallo interno de almacenamiento de imagen durante la operación.
2. Resultado esperado:
- Estado `500 Internal Server Error`.
- No se crea propiedad ni archivo persistido parcialmente.

## Escenario 6: Compatibilidad con listado

1. Crear una propiedad válida (con y/o sin imagen).
2. Consultar `GET /api/properties`.
3. Resultado esperado:
- La propiedad recién creada aparece en resultados.
- `imageUrl` respeta lo persistido (`null` o valor público/relativo).

## Escenario 7: Rechazo por campos de negocio inválidos

1. Enviar alta con campos requeridos faltantes o inválidos (por ejemplo, `title` vacío o `price` inválido).
2. Resultado esperado:
- Estado `400 Bad Request`.
- Respuesta ProblemDetails/ValidationProblemDetails coherente.
- No se crea propiedad ni archivo.

## Escenario 8: Medición de rendimiento (SC-004)

1. Ejecutar una muestra de 100 solicitudes válidas mixtas (con y sin imagen) en entorno de validación funcional.
2. Calcular latencias y percentil 95 del tiempo de respuesta.
3. Resultado esperado:
- `p95 < 3 segundos`.
- Registrar evidencia de muestra, distribución y p95 en este archivo al cierre de implementación.

## Validación automatizada sugerida

```powershell
dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
```

Resultado esperado:
- Pruebas de backend verdes para creación con y sin imagen, validaciones de upload y semántica de errores.

## Referencias

- Especificación: [spec.md](spec.md)
- Plan: [plan.md](plan.md)
- Investigación: [research.md](research.md)
- Modelo de datos: [data-model.md](data-model.md)
- Contrato: [contracts/backend-properties-create.openapi.yaml](contracts/backend-properties-create.openapi.yaml)

## Evidencia de validación

Fecha de ejecución: 2026-07-11

Comandos ejecutados:

```powershell
dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
./support/scripts/tmp-createproperty-manual.ps1
```

Resultado de pruebas automatizadas backend:

- Total: 20
- Fallidas: 0
- Exitosas: 20

Resultado de validación manual (script):

- `POST /api/properties` sin imagen: `201`
- `POST /api/properties` con imagen válida: `201`
- `POST /api/properties` con campos inválidos: `400`
- `POST /api/properties` con formato/contenido inválido: `415`
- `POST /api/properties` con imagen > 5 MB: `413`
- `imageUrl` devuelto en creación con imagen: `/assets/properties/764c9ec1dd354efb9c3ab4cd3be5574b.png`

Evidencia SC-004 (100 solicitudes válidas mixtas con y sin imagen):

- Muestra: `100`
- p95 observado: `17.89 ms`
- Umbral requerido: `< 3000 ms`
- Resultado: **PASS**
