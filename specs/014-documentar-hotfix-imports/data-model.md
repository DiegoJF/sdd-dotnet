# Modelo conceptual retrospectivo: Hotfix de imports en Features

## Resumen

La iniciativa no introduce entidades de dominio de negocio. El modelo se enfoca en entidades de trazabilidad tecnica para describir incidente, alcance del hotfix, evidencia y riesgo operativo.

## Entidades conceptuales

## 1) IncidenteDeRenderizado

- Descripcion: Evento de inestabilidad funcional en vistas Razor del modulo de propiedades.
- Atributos:
  - `id` (string): Identificador de referencia retrospectiva.
  - `causaRaiz` (string): Descripcion de por que ocurrio el problema.
  - `impactoVisible` (string): Comportamiento observado por usuario/equipo.
  - `estado` (enum): `Detectado`, `Corregido`, `Documentado`.
- Reglas:
  - Debe tener una `causaRaiz` explicita antes de cerrar `Documentado`.

## 2) AlcanceDelHotfix

- Descripcion: Delimitacion tecnica de componentes y superficies afectadas.
- Atributos:
  - `archivoPrincipal` (string): Archivo del cambio aplicado.
  - `componentesRelacionados` (list<string>): Componentes impactados por alcance de imports.
  - `capasInvolucradas` (list<enum>): `Frontend`, `Backend`, `Persistencia`.
- Reglas:
  - Para esta iniciativa, `capasInvolucradas` solo incluye `Frontend`.
  - `archivoPrincipal` debe pertenecer al arbol `Features`.

## 3) EvidenciaDeValidacion

- Descripcion: Resultado reproducible de verificacion tecnica posterior al hotfix.
- Atributos:
  - `tipo` (enum): `Build`, `Test`.
  - `comando` (string): Comando ejecutado.
  - `resultado` (enum): `Exitoso`, `Fallido`, `ConAdvertencias`.
  - `fecha` (date): Fecha de ejecucion.
- Reglas:
  - Debe existir al menos una evidencia `Build` y una evidencia `Test` para cierre de plan retrospectivo.

## 4) RiesgoResidual

- Descripcion: Riesgo no eliminado por la correccion aplicada.
- Atributos:
  - `categoria` (enum): `Cobertura`, `Mantenibilidad`, `Dependencias`.
  - `descripcion` (string): Detalle del riesgo.
  - `severidad` (enum): `Baja`, `Media`, `Alta`.
  - `mitigacionSugerida` (string): Accion recomendada sin redisenar hotfix.
- Reglas:
  - Cada riesgo debe mapearse a una categoria unica para seguimiento.

## Relaciones

- `IncidenteDeRenderizado` 1..1 `AlcanceDelHotfix`
- `IncidenteDeRenderizado` 1..N `EvidenciaDeValidacion`
- `IncidenteDeRenderizado` 1..N `RiesgoResidual`

## Mapeo a evidencias reales de la feature

- `IncidenteDeRenderizado`: inconsistencia de imports compartidos en vistas de `Features`.
- `AlcanceDelHotfix.archivoPrincipal`: `app/frontend/src/RealtorWeb/Features/_Imports.razor`.
- `EvidenciaDeValidacion`:
  - `dotnet build app/frontend/src/RealtorWeb/RealtorWeb.csproj -v minimal` (exitoso)
  - `dotnet test app/frontend/test/RealtorWeb/RealtorWebTests.csproj -v minimal` (exitoso con advertencias)
- `RiesgoResidual`: cobertura funcional acotada, imports transversales y advertencia de dependencia vulnerable en test.
