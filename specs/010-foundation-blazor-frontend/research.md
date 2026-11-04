# Investigación Técnica: Foundation Blazor Frontend

## Decisión 1: Eliminar Bootstrap por completo de la carga y de estáticos

- Decisión: Retirar la referencia a `lib/bootstrap/dist/css/bootstrap.min.css` desde `Components/App.razor` y eliminar el directorio `wwwroot/lib/bootstrap/` del proyecto frontend.
- Rationale: La constitución y `frontend.instructions.md` prohíben frameworks CSS externos; mantenerlo rompería gobernanza y generaría ambigüedad de estilos.
- Alternatives considered:
  - Mantener Bootstrap solo para utilidades: rechazado por incumplimiento explícito de la regla de stack.
  - Mantener Bootstrap temporalmente y desactivarlo luego: rechazado por riesgo de deuda técnica y divergencia inmediata con la spec.

## Decisión 2: `wwwroot/app.css` como fuente única de verdad visual

- Decisión: Centralizar tokens de diseño, layout base, componentes comunes, estados de pantalla y clases de íconos en `wwwroot/app.css`, eliminando dependencia de estilos heredados de plantilla.
- Rationale: Cumple la directiva del skill `blazor-app-css-design-system` y habilita consistencia visual reutilizable para specs futuras.
- Alternatives considered:
  - Mantener CSS por componente (`MainLayout.razor.css`, `NavMenu.razor.css`) como fuente principal: rechazado por dispersión de diseño y menor gobernanza.
  - Usar estilos inline en componentes Razor: rechazado por prohibición explícita.

## Decisión 3: Reemplazar layout de plantilla por shell desktop first

- Decisión: Sustituir `MainLayout`/`NavMenu` heredados por un shell con sidebar izquierda estable y área principal amplia, con reglas responsive para tablet y móvil (modo compacto/drawer).
- Rationale: El layout pedido es un requisito funcional central de la fundación frontend y debe existir antes de cualquier feature de negocio.
- Alternatives considered:
  - Mantener layout de plantilla y aplicar solo retoques visuales: rechazado porque no satisface la experiencia objetivo.
  - Priorizar layout mobile-first en esta iniciativa: rechazado por requerimiento explícito desktop first.

## Decisión 4: Sistema de íconos principal con Lucide para Blazor

- Decisión: Integrar el paquete `Lucide.Blazor` como proveedor principal de íconos (componentes Razor/SVG), con tamaños y color heredado vía clases `.icon`, `.icon-sm`, `.icon-md`, `.icon-lg` en `app.css`.
- Rationale: Cumple la constitución y evita dependencia de Bootstrap Icons o librerías mixtas.
- Alternatives considered:
  - Íconos SVG manuales por archivo: rechazado por mayor costo de mantenimiento y menor consistencia.
  - Bootstrap Icons o Font Awesome: rechazado por prohibición normativa.

## Decisión 5: Andamiaje Refit tipado con `IHttpClientFactory`

- Decisión: Registrar clientes Refit por módulo en `Program.cs` usando `AddRefitClient<T>()` y `ConfigureHttpClient`, dejando un cliente base apuntando a `RealtorApi` sin endpoints de negocio en esta spec.
- Rationale: Cumple `frontend.instructions.md`: sin `HttpClient` directo en componentes y sin `RestService.For<T>()` fuera del registro DI.
- Alternatives considered:
  - Crear instancias con `RestService.For<T>()` en componentes: rechazado por acoplamiento y violación explícita.
  - Usar `HttpClient` sin Refit para fundación: rechazado por inconsistencia con stack obligatorio.

## Decisión 6: Estados de pantalla base reutilizables

- Decisión: Definir patrones CSS para `loading`, `empty`, `error` y `success` en `app.css` como bloques reutilizables por features futuras.
- Rationale: Establece una base uniforme de UX sin implementar todavía pantallas de negocio.
- Alternatives considered:
  - Posponer estados para specs posteriores: rechazado por alcance explícito de la spec 010.
  - Implementar componentes funcionales completos de estado: rechazado por exceder fundación visual.

## Resultado de investigación

No quedan incógnitas técnicas pendientes (`NEEDS CLARIFICATION = 0`).
Queda habilitada la fase de diseño con decisiones cerradas para Bootstrap, CSS base, layout, Lucide y Refit.
