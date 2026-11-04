# Quickstart de Validación: Detalle de propiedad en frontend Blazor

## Objetivo

Validar que `RealtorWeb` incorpora una vista de detalle de propiedad en `/properties/{id}` consumida por Refit, con estados de carga/contenido/error, distinción explícita de no encontrado, placeholder de imagen y navegación desde el listado con botón "Ver".

## Prerrequisitos

- `.NET SDK` alineado con `global.json`.
- Backend `RealtorApi` disponible en entorno local o URL configurada para `RealtorWeb`.
- Frontend restaurado correctamente.

## Escenario 1: Compilar frontend

1. Ejecutar:

```powershell
dotnet restore app/frontend/src/RealtorWeb/RealtorWeb.csproj
dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj
```

2. Resultado esperado:
- Compilación exitosa.
- Sin errores de contratos Refit ni componentes nuevos de detalle.

## Escenario 2: Navegar desde listado al detalle

1. Ejecutar frontend:

```powershell
dotnet run --project app/frontend/src/RealtorWeb/RealtorWeb.csproj
```

2. Abrir `/` y localizar cualquier tarjeta del listado.
3. Hacer clic en el botón "Ver".

Resultado esperado:
- La navegación abre `/properties/{id}` usando el id de la tarjeta seleccionada.
- No hay rediseño no solicitado del listado.

## Escenario 3: Carga exitosa del detalle

1. Abrir `/properties/{id}` con un id existente.
2. Esperar finalización de la carga.

Resultado esperado:
- Se muestran título, imagen/placeholder, descripción, dirección, precio, estado, dormitorios, baños y metros cuadrados.
- Existe acción visible para volver al listado.

## Escenario 4: No encontrado (404)

1. Abrir `/properties/{id}` con un id inexistente válido.

Resultado esperado:
- Se muestra estado específico de no encontrado.
- Este estado es distinto del error general.
- La acción de volver al listado permanece disponible.

## Escenario 5: Error general de backend o red

1. Forzar indisponibilidad de backend o falla de red.
2. Abrir `/properties/{id}`.

Resultado esperado:
- Se muestra estado de error general (no 404).
- Se evita confundir el caso con no encontrado.

## Escenario 6: `imageUrl` nulo

1. Probar con una propiedad cuyo `imageUrl` sea `null`.

Resultado esperado:
- La vista renderiza placeholder consistente con el diseño.
- No quedan huecos visuales ni estilos inline.

## Escenario 7: Volver al listado

1. Desde una propiedad en detalle, usar la acción de retorno.

Resultado esperado:
- El usuario regresa a `/` en una única acción.

## Escenario 8: Protocolo de medición de completitud y responsive

1. Ejecutar una muestra de 20 aperturas de detalle con ids válidos y registrar por cada apertura si se muestran los 9 campos requeridos:
	- título
	- imagen o placeholder
	- descripción
	- dirección
	- precio
	- estado
	- dormitorios
	- baños
	- metros cuadrados
2. Contabilizar aperturas completas (sin omisiones) y aperturas con omisiones.
3. Repetir validación visual de la vista de detalle en viewports de referencia:
	- 1366x768
	- 1024x768
	- 390x844
4. Verificar en cada viewport que contenido principal, estado visual (loading/not-found/error/success) y acción de volver se vean legibles y accionables, sin solapamientos.

Resultado esperado:
- Se cumplen al menos 19 de 20 aperturas completas (SC-002).
- En los 3 viewports, los elementos críticos son legibles y accionables sin solapamientos (SC-006).

## Referencias

- Spec funcional: `spec.md`
- Investigación: `research.md`
- Modelo conceptual: `data-model.md`
- Contrato: `contracts/frontend-property-detail.contract.yaml`

## Evidencia de validación

### Compilación (Escenario 1) — Ejecutado

- Fecha: 2026-07-22
- Comandos ejecutados:

```powershell
dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj -v minimal
dotnet build app/frontend/test/RealtorWeb/RealtorWebTests.csproj -v minimal
```

- Resultado: **Compilación exitosa** para `RealtorWeb` (`net11.0`) y `RealtorWebTests` (`net11.0`).
- Sin errores de contratos Refit ni de los componentes nuevos de detalle.

### Trazabilidad de artefactos por escenario

- Escenario 2 (navegar "Ver" → detalle): botón declarativo `href="/properties/{id}"` en `app/frontend/src/RealtorWeb/Features/Properties/List/PropertyCard.razor` y ruta `@page "/properties/{Id}"` en `app/frontend/src/RealtorWeb/Components/Pages/PropertyDetailPage.razor`.
- Escenario 3 (carga exitosa con 9 campos + volver): render de contenido en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor` consumiendo `IPropertiesApi.GetPropertyByIdAsync`.
- Escenario 4 (404 no encontrado): mapeo de `ApiException` 404 a estado `NotFound` en `app/frontend/src/RealtorWeb/Features/Properties/Detail/PropertyDetail.razor.cs`.
- Escenario 5 (error general): mapeo de errores no-404 y de red a estado `Error` en `PropertyDetail.razor.cs`.
- Escenario 6 (`imageUrl` nulo): placeholder Lucide `image-off` con clase `property-detail-placeholder` en `PropertyDetail.razor` y `app/frontend/src/RealtorWeb/wwwroot/app.css`.
- Escenario 7 (volver al listado): acción `Volver al listado` disponible en éxito y en estados `NotFound`/`Error` vía `PropertyDetailState`.

### Protocolo de medición manual (Escenario 8) — Pendiente de QA con backend en vivo

- La medición de completitud (muestra de 20 aperturas, umbral ≥19 para SC-002) y la validación responsive en los 3 viewports de referencia (1366x768, 1024x768, 390x844 para SC-006) deben ejecutarse en QA manual contra un backend disponible siguiendo el protocolo del Escenario 8.
