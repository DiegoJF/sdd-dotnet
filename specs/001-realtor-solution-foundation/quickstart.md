# Quickstart de Validación - Fundación de la Solución Realtor

## Objetivo

Validar de forma ejecutable que la iniciativa foundation cumple RF-001..RF-010 y CE-001..CE-004 sin introducir lógica de negocio.

## Prerrequisitos

1. Repositorio clonado y ubicado en raíz del proyecto.
2. SDK de .NET compatible con `global.json`.
3. Shell con acceso a `dotnet`.

## Escenario 1: Verificación de estructura fundacional

1. Confirmar existencia de solución principal:
   - `app/Realtor.sln`
2. Confirmar existencia de proyectos base:
   - `app/backend/src/RealtorApi/`
   - `app/backend/tests/RealtorApiTests/`
   - `app/frontend/src/RealtorWeb/`
   - `app/frontend/test/RealtorWeb/`
3. Confirmar que la solución referencia los cuatro proyectos anteriores.

Resultado esperado:
- CE-001 cumplido (rutas existentes y referenciadas por la solución).

## Escenario 2: Verificación de compilación

Ejecutar desde raíz:

```powershell
dotnet build app/Realtor.sln
```

Resultado esperado:
- Compilación exitosa sin errores.
- CE-002 cumplido.

## Escenario 3: Verificación de alcance sin negocio

1. Inspeccionar arranque de backend (`Program.cs` del proyecto API).
2. Validar que solo incluya:
   - Configuración base de servicios.
   - Middleware de plataforma.
   - Mapeo inicial de endpoint técnico.
3. Verificar ausencia de:
   - Entidades de dominio.
   - Casos de uso funcionales de producto.
   - Lógica de negocio embebida.

Resultado esperado:
- RF-006, RF-007, RF-008, RF-009 y CE-003 cumplidos.

## Escenario 4: Contrato técnico mínimo

Usar el contrato definido en `contracts/backend-bootstrap.openapi.yaml` para validar endpoint de disponibilidad.

Resultado esperado:
- Endpoint técnico responde según contrato.
- La solución queda preparada para evolución por specs (CE-004).

## Evidencia de validación ejecutada (2026-07-07)

- `dotnet build app/Realtor.sln`: exitoso.
- `dotnet test app/Realtor.sln`: exitoso.
- Resultado de pruebas:
   - Total: 3
   - Exitosas: 3
   - Fallidas: 0
   - Omitidas: 0

Proyectos verificados en solución:

- `app/backend/src/RealtorApi/RealtorApi.csproj`
- `app/backend/tests/RealtorApiTests/RealtorApiTests.csproj`
- `app/frontend/src/RealtorWeb/RealtorWeb.csproj`
- `app/frontend/test/RealtorWeb/RealtorWebTests.csproj`

## Referencias de diseño

- Modelo de datos conceptual: `data-model.md`
- Contrato HTTP técnico: `contracts/backend-bootstrap.openapi.yaml`
- Plan de implementación: `plan.md`
