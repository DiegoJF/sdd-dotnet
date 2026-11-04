# Modelo de Datos - Fundación de la Solución Realtor

Este documento describe entidades conceptuales de foundation (estructurales), no entidades de dominio de negocio.

## 1. SolucionRealtor

- Propósito: Contenedor principal de composición técnica de la iniciativa.
- Campos:
  - `RutaSolucion` (string, requerido): valor esperado `app/Realtor.sln`.
  - `EstadoCompilacion` (enum, requerido): `NoVerificado | Exitoso | Fallido`.
  - `FechaVerificacion` (datetime?, opcional).
- Reglas de validación:
  - `RutaSolucion` debe existir físicamente al finalizar la implementación.
  - Debe referenciar proyectos base de backend, frontend y pruebas.
- Transiciones de estado:
  - `NoVerificado -> Exitoso` cuando `dotnet build` complete sin errores.
  - `NoVerificado -> Fallido` si la verificación de compilación falla.
  - `Fallido -> Exitoso` tras corrección y nueva verificación exitosa.

## 2. ProyectoBase

- Propósito: Representar cada proyecto inicial sin negocio.
- Campos:
  - `Nombre` (string, requerido): `RealtorApi` o `RealtorWeb`.
  - `Tipo` (enum, requerido): `Backend | Frontend`.
  - `Ruta` (string, requerido).
  - `TieneLogicaNegocio` (bool, requerido): siempre `false` en foundation.
- Reglas de validación:
  - Debe existir exactamente un `ProyectoBase` de tipo `Backend` y uno de tipo `Frontend`.
  - `TieneLogicaNegocio` debe permanecer en `false` para cumplir RF-007 y RF-008.

## 3. ProyectoPruebaBase

- Propósito: Estructura inicial de pruebas por capa.
- Campos:
  - `Nombre` (string, requerido): `RealtorApiTests` o `RealtorWeb` (tests frontend según ruta definida).
  - `CapaObjetivo` (enum, requerido): `Backend | Frontend`.
  - `Ruta` (string, requerido).
  - `FrameworkPruebas` (string, requerido): `xUnit` como baseline.
- Reglas de validación:
  - Debe existir un proyecto de pruebas por cada capa principal.
  - La ruta debe alinearse con RF-003 y RF-005.

## 4. BaselineVersion

- Propósito: Asegurar coherencia de versión técnica de la iniciativa.
- Campos:
  - `ArchivoFuente` (string, requerido): `global.json`.
  - `SdkVersion` (string, requerido): versión declarada en `global.json`.
  - `PermiteModificacionEnFoundation` (bool, requerido): `false`.
- Reglas de validación:
  - Toda inicialización de proyecto debe respetar esta versión.
  - No se admite modificar `global.json` durante la iniciativa 001.

## Relaciones

- `SolucionRealtor` 1..1 contiene 2 `ProyectoBase`.
- `SolucionRealtor` 1..1 contiene 2 `ProyectoPruebaBase`.
- `SolucionRealtor` 1..1 se valida contra 1 `BaselineVersion`.

## Estado de verificación actual (2026-07-07)

- `RutaSolucion`: `app/Realtor.sln` (existe).
- `EstadoCompilacion`: `Exitoso`.
- `FechaVerificacion`: `2026-07-07`.
- Proyectos referenciados en solución: backend, frontend, test backend y test frontend.
