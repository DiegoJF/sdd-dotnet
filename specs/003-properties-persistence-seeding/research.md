# Investigación Técnica: Persistencia y Seeding Inicial de Propiedades

## Decisión 1: Estrategia de seeding nativa de EF Core

- Decisión: Usar `UseSeeding()` y `UseAsyncSeeding()` en el registro de `AppDbContext`, delegando en `DatabaseSeeder.Seed` y `DatabaseSeeder.SeedAsync`.
- Rationale: Cumple `database.instructions.md` y el skill `database-migration-and-seeding`; asegura ejecución del seed también cuando no haya migraciones pendientes.
- Alternatives considered:
  - Invocar seeder manual desde `MigrationExtensions`: rechazado por estar prohibido y duplicar responsabilidad.
  - `HasData`: rechazado por restricciones de la spec y por no cubrir lógica condicional.

## Decisión 2: Aplicación de migraciones al arranque

- Decisión: Mantener `await app.MigrateAsync();` siempre antes de `app.Run()`, con `Database.MigrateAsync()` en proveedores relacionales.
- Rationale: Obliga flujo consistente de migración + seeding automático en cada arranque.
- Alternatives considered:
  - Condicionar ejecución a migraciones pendientes: rechazado por prohibición explícita.
  - Ejecutar `dotnet ef database update` como paso obligatorio: rechazado por reglas del repositorio.

## Decisión 3: Idempotencia y política de conflicto de propiedades

- Decisión: Aplicar upsert para propiedades existentes, preservando `Id` y actualizando campos funcionales permitidos.
- Rationale: Evita duplicados y corrige deriva de datos entre ejecuciones sin romper referencias.
- Alternatives considered:
  - Insert-only (ignorar existentes): rechazado por riesgo de desalineación funcional.
  - Fallar al detectar diferencias: rechazado por impacto operativo innecesario.

## Decisión 4: Manejo de faltantes críticos de seed

- Decisión: Fail-fast si falta `seed-manifest.json` o JSON críticos (`properties.json`, `properties-statuses.json`).
- Rationale: Evita estados parciales e inconsistentes en inventario base.
- Alternatives considered:
  - Continuar sin seed: rechazado por pérdida silenciosa de datos.
  - Modo degradado con warnings: rechazado por ambigüedad operacional.

## Decisión 5: Manejo de imagen faltante por registro

- Decisión: Omitir solo la propiedad afectada, registrar error estructurado y continuar.
- Rationale: Prioriza disponibilidad y evita persistir `ImageUrl` inválidas.
- Alternatives considered:
  - Abortar todo el seed: rechazado por alto costo operativo ante fallos puntuales.
  - Placeholder global: rechazado por introducir datos artificiales no solicitados.

## Decisión 6: Ruta pública final para imágenes

- Decisión: Persistir `ImageUrl` con patrón público `/assets/properties/{archivo}` y sincronizar imágenes desde `support/seed-data/images/properties` a `wwwroot/assets/properties` en runtime.
- Rationale: Desacopla persistencia de rutas físicas internas y deja URLs consumibles por API/clientes.
- Alternatives considered:
  - Guardar ruta física del repositorio: rechazado por prohibición explícita.
  - Guardar solo nombre de archivo: rechazado por no cumplir requisito de ruta pública final persistida.

## Decisión 7: Modelo de estado de propiedad persistido como string

- Decisión: Persistir estado mediante conversión enum/string en configuración EF Core.
- Rationale: Mantiene legibilidad en base de datos y estabilidad semántica de valores de negocio.
- Alternatives considered:
  - Persistir como entero: rechazado por menor claridad y mayor riesgo de drift semántico.
  - Tabla relacional sin enum: diferido; para esta spec se prioriza conversión requerida por alcance.

## Decisión 8: Una migración por cambio funcional coherente

- Decisión: Generar una única migración para `PropertyStatus` y `Property` en esta spec.
- Rationale: Sigue regla de cohesión funcional de `ef-core-migrations` y `database.instructions.md`.
- Alternatives considered:
  - Una migración por entidad: rechazado por prohibición explícita.
  - Postergar migración hasta features de endpoints: rechazado por necesidad de persistencia inmediata.
