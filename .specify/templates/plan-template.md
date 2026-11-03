# Plan de Implementación: [NOMBRE DE LA FUNCIONALIDAD]

**Rama**: `[###-nombre-de-la-funcionalidad]` | **Fecha**: [FECHA] | **Especificación**: [spec.md](./spec.md)

**Entrada**: Resumen de `spec.md` y descripción técnica derivada de la investigación.

## Resumen

[Requisito principal y enfoque técnico decidido.]

## Contexto Técnico

- **Lenguaje y versión**: [lenguaje y versión o NEEDS CLARIFICATION].
- **Dependencias principales**: [dependencias aprobadas].
- **Almacenamiento**: [motor y alcance de persistencia, o no aplica].
- **Pruebas**: [framework y estrategia aprobada].
- **Plataforma objetivo**: [plataformas soportadas].
- **Tipo de proyecto**: [servicio web, aplicación web, biblioteca u otro].
- **Objetivos de rendimiento**: [métricas o sin objetivos específicos].
- **Restricciones**: [límites tecnológicos y de alcance].
- **Alcance**: [magnitud funcional y técnica].

## Investigación y Decisiones

- **Decisión**: [opción elegida].
- **Justificación**: [razón y evidencia].
- **Alternativas consideradas**: [opciones descartadas y motivo].
- **Fuentes**: [documentación consultada, si corresponde].

## Verificación Constitucional

### Antes del diseño

- **Solución única**: [cumple o excepción aprobada].
- **Spec-Driven Development**: [artefactos canónicos y trazabilidad].
- **Arquitectura**: [cumplimiento del stack y principios].
- **Calidad**: [pruebas, validación, logging y errores].
- **Idioma**: [documentación en español].

### Después del diseño

[Revalide los gates después de completar el diseño. Toda divergencia debe detenerse y escalarse según la constitución.]

## Estructura del Proyecto

La carpeta de la iniciativa conserva estos tres artefactos de diseño:

```text
specs/[###-nombre-de-la-funcionalidad]/
├── spec.md
├── plan.md
└── tasks.md
```

La única excepción es `quickstart.md`, que se crea durante la implementación exclusivamente como evidencia de validación ejecutada; no se generan `research.md`, `data-model.md`, `contracts/` ni otros documentos auxiliares.

```text
[Árbol de código concreto basado en las rutas aprobadas]
```

**Decisión de estructura**: [Proyectos, referencias y límites entre módulos.]

## Modelo de Datos

[Entidades, relaciones y reglas, o indicar que esta iniciativa no introduce datos ni entidades.]

## Contratos e Interfaces

[Contratos externos y sus límites, o indicar que no se exponen contratos de producto.]

## Estrategia de Validación

- **Comandos y escenarios**: [validaciones reproducibles y resultados esperados].
- **Límites de alcance**: [comprobaciones para confirmar que no se introdujeron elementos fuera de spec].
- La evidencia ejecutada y sus resultados se registran en `quickstart.md` durante `speckit.implement`, no durante la planificación.

## Complejidad

[Registre solo excepciones constitucionales aprobadas, su necesidad y las alternativas más simples descartadas. Si no existen, indique que no hay excepciones.]
