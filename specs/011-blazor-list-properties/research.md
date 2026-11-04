# Investigación Técnica: Listado de propiedades en la página principal

## Decisión 1: Carga explícita desde un método único con cancelación y guardia contra respuestas obsoletas

- Decisión: La pantalla principal debe cargar datos mediante un método único, por ejemplo `LoadPageAsync`, disparado explícitamente por la navegación de páginas. Cada recarga debe crear un `CancellationTokenSource` nuevo, cancelar y disponer la solicitud anterior, y aplicar un número de secuencia para ignorar respuestas tardías.
- Rationale: La paginación por botones no depende de parámetros de ruta o query. Este patrón evita condiciones de carrera cuando el usuario cambia de página con rapidez y mantiene la última interacción como fuente de verdad.
- Alternatives considered:
  - Usar solo `OnParametersSetAsync`: descartado porque la paginación no depende de parámetros de componente en esta versión.
  - Confiar solo en cancelación: descartado porque una respuesta ya resuelta puede llegar tarde y sobrescribir el estado visible.

## Decisión 2: No persistir estado prerenderizado para esta pantalla

- Decisión: No usar `PersistentComponentState` ni una persistencia similar para el listado paginado.
- Rationale: La página representa un listado vivo que cambia por interacción del usuario. Persistir el arranque inicial aporta poco valor y agrega complejidad sin resolver la navegación por páginas.
- Alternatives considered:
  - Persistir la primera carga: descartado por utilidad limitada frente al coste de complejidad.
  - Tratar la pantalla como prerender con estado recuperado: descartado porque no es el principal caso de uso de esta feature.

## Decisión 3: Tarjeta compacta y jerarquizada para cada propiedad

- Decisión: Cada tarjeta debe usar una composición vertical compacta: imagen superior 16:9 con `object-fit: cover`, título en una o dos líneas, precio como dato dominante, estado como chip, dirección en una línea truncada y una fila final de métricas breves para dormitorios, baños y metros cuadrados.
- Rationale: Esa densidad permite mantener legibilidad en una grilla de 3 columnas sin saturar la pantalla ni romper la jerarquía visual.
- Alternatives considered:
  - Tarjeta editorial con imagen grande y descripción larga: descartada por exceso de densidad.
  - Mostrar todos los campos públicos en cada tarjeta: descartado porque penaliza el escaneo rápido y no cabe bien en 3 columnas.

## Decisión 4: Estados de pantalla reutilizables apoyados en el sistema visual existente

- Decisión: Reutilizar el patrón visual base ya presente en `wwwroot/app.css` para los estados `loading`, `empty` y `error`, manteniendo la misma familia de contenedores que las tarjetas.
- Rationale: Así se evita crear un sistema paralelo y se mantiene consistencia visual con la fundación del frontend.
- Alternatives considered:
  - Toasts o mensajes flotantes: descartados porque no comunican bien estados persistentes de listado.
  - Diseños completamente distintos por estado: descartados por fragmentación visual.

## Decisión 5: Responsive con breakpoints estables y grilla explícita 3/2/1

- Decisión: Mantener los breakpoints ya existentes del shell en 1023px y 767px, y definir la grilla de propiedades con cortes previsibles: 3 columnas en escritorio amplio, 2 columnas en tablet/desktop medio y 1 columna en móvil.
- Rationale: El comportamiento exacto 3/2/1 es fácil de validar, consistente con la spec y alineado con el sistema visual del frontend.
- Alternatives considered:
  - `auto-fit` / `auto-fill` sin cortes explícitos: descartado porque vuelve menos predecible el cumplimiento del requisito visual.
  - Masonry: descartado porque desordena la lectura por filas.

## Decisión 6: Cliente tipado Refit por DI sin acceso directo desde componentes

- Decisión: La página debe consumir `IPropertiesApi` a través de DI, con `AddRefitClient<T>()` ya disponible en `Program.cs` y sin `HttpClient` directo ni `RestService.For<T>()` en Razor.
- Rationale: Es la forma coherente con la gobernanza del repo y con las reglas de frontend ya aprobadas.
- Alternatives considered:
  - Llamadas directas con `HttpClient` desde componentes: descartadas por la regla de stack y por acoplamiento innecesario.
  - Crear el cliente Refit fuera de DI: descartado por romper la centralización del registro.

## Resultado de investigación

No quedan incógnitas técnicas pendientes (`NEEDS CLARIFICATION = 0`).
La fase de diseño puede avanzar con las decisiones anteriores como base.
