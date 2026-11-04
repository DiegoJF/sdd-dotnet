# Quickstart de Validación: Listado de propiedades en la página principal

## Objetivo

Validar que la ruta `/` de `RealtorWeb` muestra un listado paginado de propiedades consumido con Refit, con estados de carga, vacío y error, y grilla responsive de 3/2/1 columnas.

## Prerrequisitos

- `.NET SDK` alineado con `global.json`.
- Backend `RealtorApi` disponible en entorno de desarrollo o en una URL base configurada para `RealtorWeb`.
- Frontend restaurado correctamente.

## Escenario 1: Compilar el frontend

1. Ejecutar:

```powershell
dotnet restore app/frontend/src/RealtorWeb/RealtorWeb.csproj
dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj
```

2. Resultado esperado:
- Compilación exitosa.
- Sin errores de referencias faltantes para los componentes de listado.

## Escenario 2: Verificar carga inicial en `/`

1. Ejecutar el frontend:

```powershell
dotnet run --project app/frontend/src/RealtorWeb/RealtorWeb.csproj
```

2. Abrir la ruta `/`.
3. Validar que la vista solicite la primera página con `Page=1` y `PageSize=9`.

Resultado esperado:
- Se muestran propiedades en la grilla.
- La vista refleja `página actual`, `total de páginas` y `total de elementos`.

## Escenario 3: Verificar navegación entre páginas

1. Cargar una respuesta con más de una página de resultados.
2. Presionar "anterior" y "siguiente".
3. Confirmar que la vista cambia de página y refresca las tarjetas.

Resultado esperado:
- `HasPrevious` y `HasNext` gobiernan correctamente el estado de los controles.
- La última interacción prevalece si el usuario cambia de página rápidamente.

## Escenario 4: Verificar estados de pantalla

1. Simular respuesta vacía.
2. Simular error del backend o de red.
3. Observar la pantalla durante la carga inicial.

Resultado esperado:
- Estado `empty` cuando no hay resultados.
- Estado `error` cuando falla la consulta.
- Estado `loading` mientras se obtiene la página.

## Escenario 5: Verificar composición visual

1. Revisar la grilla en escritorio, tableta y móvil.
2. Confirmar que las tarjetas muestran título, imagen, precio, estado, dirección, dormitorios, baños y metros cuadrados.
3. Confirmar que la grilla adapta columnas a 3/2/1.

Resultado esperado:
- Escritorio: 3 columnas.
- Tableta: 2 columnas.
- Móvil: 1 columna.

## Escenario 6: Verificar consumo Refit

1. Revisar `Program.cs` y `ServiceCollectionExtensions`.
2. Confirmar que `IPropertiesApi` está registrado por DI.
3. Confirmar que los componentes Razor no usan `HttpClient` directo ni `RestService.For<T>()`.

Resultado esperado:
- Consumo HTTP centralizado mediante Refit tipado.
- Sin acceso directo a infraestructura de red desde Razor.

## Referencias de diseño

- Spec funcional: `spec.md`
- Investigación: `research.md`
- Modelo conceptual: `data-model.md`
- Contrato de frontend: `contracts/frontend-properties-list.contract.yaml`

## Evidencia de validación

Fecha de ejecución: 2026-07-22

### 1. Compilación del frontend

Comando ejecutado:

```powershell
dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj
```

Resultado obtenido:
- Compilación exitosa.
- Ensamblado generado en `app/frontend/src/RealtorWeb/bin/Debug/net11.0/RealtorWeb.dll`.

### 2. Validación de la ruta `/`

Comando ejecutado:

```powershell
dotnet run --project app/frontend/src/RealtorWeb/RealtorWeb.csproj --urls http://127.0.0.1:5055
```

Resultado observado en navegador:
- La ruta `/` renderiza la página principal de propiedades con la nueva composición.
- Cuando el backend `RealtorApi` no está disponible en `http://localhost:5023`, la vista muestra el estado de error controlado con el mensaje de reintento.
- El shell de la aplicación carga sin errores de iconografía luego de corregir el icono de navegación.

### 3. Verificación de consumo Refit

Observación en salida de ejecución:
- La página principal dispara `GET http://localhost:5023/api/properties` mediante el cliente tipado `IPropertiesApi`.

Resultado obtenido:
- El consumo HTTP ocurre por Refit registrado por DI.

### 4. Verificación visual

Observación en navegador:
- La grilla y los estados del listado se apoyan en `wwwroot/app.css`.
- La presentación usa tarjetas propias y estados de carga, vacío y error de la feature.

Resultado obtenido:
- La composición visual queda alineada con la spec y el contrato de la iniciativa.
