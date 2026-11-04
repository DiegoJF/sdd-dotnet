# Plan de Implementacion: Documentacion retrospectiva del hotfix de imports en Features

**Branch**: `hotfix/import-razor-utils` | **Fecha**: 2026-07-24 | **Spec**: `specs/014-documentar-hotfix-imports/spec.md`

**Entrada**: Especificacion funcional desde `specs/014-documentar-hotfix-imports/spec.md`

## Resumen

Documentar tecnicamente, de forma retrospectiva, el hotfix ya aplicado para estabilizar el renderizado de vistas de propiedades en el frontend Blazor. El alcance se limita a describir causa raiz, componentes afectados, solucion aplicada, decisiones tecnicas, riesgos, pruebas y efectos secundarios observables, sin redisenar ni modificar la implementacion existente.

## Contexto Tecnico

**Lenguaje/Version**: C# con .NET 11 (`net11.0`) + Razor Components (Blazor Web App)

**Dependencias Primarias**: ASP.NET Core Components, infraestructura Razor `_Imports`, `Microsoft.AspNetCore.Components.*`

**Storage**: N/A (sin cambios de persistencia)

**Pruebas**: xUnit (suite `RealtorWebTests`) + compilacion de frontend con `dotnet build`

**Plataforma Objetivo**: Aplicacion web frontend en `app/frontend/src/RealtorWeb`

**Tipo de Proyecto**: Web app (frontend Blazor dentro de solucion full-stack)

**Objetivos de Rendimiento**: Mantener continuidad funcional de renderizado sin regresiones observables en el flujo principal de propiedades

**Restricciones**:

- No introducir nueva implementacion ni cambios de codigo de aplicacion.
- No alterar backend, contratos API ni persistencia.
- Mantener la documentacion en espanol y orientada a trazabilidad.
- Basar conclusiones en evidencia observable del estado actual (diff + pruebas + compilacion).

**Escala/Alcance**: Un ajuste localizado en imports de Razor de `Features`, con impacto funcional en vistas de listado/detalle y sus estados de interfaz.

## Revision de Constitucion

*PUERTA: Debe aprobar antes de Fase 0. Revalidar tras Fase 1.*

### Validacion previa (antes de Fase 0)

- **Principio I, solucion unica y compartida**: PASS. Se documenta un ajuste puntual dentro de la solucion existente.
- **Principio II, spec-driven**: PASS. El plan deriva de la spec 014 y no agrega alcance extra.
- **Principio III, arquitectura canonica**: PASS. Se mantiene Blazor Web App con Razor Components.
- **Principio IV, stack obligatorio**: PASS. No se introducen tecnologias nuevas.
- **Principio V, calidad de dominio y contratos**: PASS. No se desplaza logica de negocio ni se alteran contratos backend.
- **Principio VI, idioma**: PASS. Artefactos markdown en espanol.
- **Principio VII, gobernanza**: PASS. Sin conflicto de rutas ni ajustes upstream no autorizados.

Resultado de puerta previa: **APROBADO**.

## Estructura del Proyecto

### Documentacion de la feature

```text
specs/014-documentar-hotfix-imports/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── frontend-hotfix-imports-retrospective.contract.yaml
└── tasks.md
```

### Codigo fuente relevante

```text
app/frontend/src/RealtorWeb/
├── Components/
│   └── _Imports.razor
└── Features/
    ├── _Imports.razor
    └── Properties/
        ├── List/
        │   ├── PropertyList.razor
        │   └── PropertyCard.razor
        └── Detail/
            └── PropertyDetail.razor

app/frontend/test/RealtorWeb/
└── FoundationSmokeTests.cs
```

**Decision de estructura**: Mantener la arquitectura vigente y documentar el hotfix como correccion localizada de alcance frontend, con evidencia de compilacion y prueba de humo.

## Fase 0: Investigacion

### Tareas de investigacion ejecutadas

1. Inspeccionar estado Git para identificar el cambio efectivo del hotfix en el arbol actual.
2. Correlacionar el archivo de hotfix con las vistas funcionales afectadas en `Features/Properties`.
3. Ejecutar compilacion y pruebas relacionadas para obtener evidencia observable y riesgos activos.
4. Consolidar causa raiz, decisiones y efectos secundarios potenciales con base en evidencia.

Resultado: decisiones cerradas en `research.md` sin `NEEDS CLARIFICATION` pendientes.

## Fase 1: Diseno y Contratos

### Entregables completados

1. `data-model.md` con modelo conceptual retrospectivo (incidente, alcance, evidencia y riesgo).
2. `contracts/frontend-hotfix-imports-retrospective.contract.yaml` con contrato funcional de comportamiento esperado y no regresion.
3. `quickstart.md` con guia de validacion reproducible del hotfix ya implementado.
4. Actualizacion de contexto de agente en `.github/copilot-instructions.md` para apuntar a la iniciativa 014.

## Revalidacion de Constitucion (post-diseno)

- **Principio I**: PASS
- **Principio II**: PASS
- **Principio III**: PASS
- **Principio IV**: PASS
- **Principio V**: PASS
- **Principio VI**: PASS
- **Principio VII**: PASS

Resultado de puerta post-diseno: **APROBADO**.

## Seguimiento de Complejidad

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Ninguna | Los artefactos operativos (`research.md`, `data-model.md`, `quickstart.md`, `contracts/`) estan explicitamente permitidos por el Metodo de Trabajo de la Constitucion v1.2.0 como complemento a los tres archivos obligatorios | No aplica: no existe violacion constitucional; los artefactos aportan trazabilidad y evidencia verificable sobre causa raiz, riesgos y cobertura de pruebas |
