# Tareas: [NOMBRE DE LA FUNCIONALIDAD]

**Entrada**: Documentos de diseño de `specs/[###-nombre-de-la-funcionalidad]/`.

**Prerrequisitos**: `plan.md` y `spec.md`.

**Pruebas**: Incluya tareas para escribir pruebas solo cuando la especificación lo solicite.

**Organización**: Agrupe las tareas por historia de usuario para permitir implementación y validación independientes.

## Formato

Cada tarea debe seguir este formato:

```text
- [ ] T001 [P] [US1] Descripción de la acción y ruta exacta
```

- `[P]` es opcional y solo se usa si la tarea puede ejecutarse en paralelo, sin dependencias ni archivos compartidos.
- `[US1]`, `[US2]`, etc., identifica la historia de usuario. No se usa en tareas de preparación, fundamentos o pulido.
- Toda tarea debe tener un identificador secuencial, una casilla y una descripción accionable con rutas exactas.

## Fase 1: Preparación

**Propósito**: Inicialización del proyecto y estructura básica.

- [ ] T001 Crear la estructura del proyecto según el plan en `[ruta]`.
- [ ] T002 Inicializar las dependencias aprobadas en `[ruta del proyecto]`.
- [ ] T003 [P] Configurar herramientas de formato, si el plan las requiere, en `[ruta de configuración]`.

## Fase 2: Fundamentos Compartidos

**Propósito**: Completar los requisitos bloqueantes para todas las historias.

- [ ] T004 Configurar los fundamentos compartidos requeridos en `[ruta]`.
- [ ] T005 [P] Configurar el manejo común de errores y logging en `[ruta]`.
- [ ] T006 Configurar los valores de entorno requeridos en `[ruta]`.

**Punto de control**: Los fundamentos están listos; puede comenzar el trabajo de historias.

## Fase 3: Historia de Usuario 1 - [Título] (Prioridad: P1)

**Objetivo**: [Resultado que entrega esta historia].

**Prueba independiente**: [Cómo verificar esta historia por sí sola].

### Pruebas para la Historia de Usuario 1 _(solo si la especificación las solicita)_

- [ ] T007 [P] [US1] Crear pruebas unitarias para [comportamiento] en `[ruta de pruebas]`.

### Implementación de la Historia de Usuario 1

- [ ] T008 [P] [US1] Crear [componente o entidad] en `[ruta]`.
- [ ] T009 [US1] Implementar [comportamiento] en `[ruta]`.
- [ ] T010 [US1] Agregar validación y manejo de errores en `[ruta]`.

**Punto de control**: La historia es funcional y verificable de forma independiente.

## Fase 4: Historia de Usuario 2 - [Título] (Prioridad: P2)

**Objetivo**: [Resultado que entrega esta historia].

**Prueba independiente**: [Cómo verificar esta historia por sí sola].

### Pruebas para la Historia de Usuario 2 _(solo si la especificación las solicita)_

- [ ] T011 [P] [US2] Crear pruebas unitarias para [comportamiento] en `[ruta de pruebas]`.

### Implementación de la Historia de Usuario 2

- [ ] T012 [P] [US2] Crear [componente o entidad] en `[ruta]`.
- [ ] T013 [US2] Implementar [comportamiento] en `[ruta]`.
- [ ] T014 [US2] Integrar con la Historia de Usuario 1, si corresponde, en `[ruta]`.

**Punto de control**: Las historias 1 y 2 funcionan de manera independiente.

## Fase 5: Historia de Usuario 3 - [Título] (Prioridad: P3)

**Objetivo**: [Resultado que entrega esta historia].

**Prueba independiente**: [Cómo verificar esta historia por sí sola].

### Pruebas para la Historia de Usuario 3 _(solo si la especificación las solicita)_

- [ ] T015 [P] [US3] Crear pruebas unitarias para [comportamiento] en `[ruta de pruebas]`.

### Implementación de la Historia de Usuario 3

- [ ] T016 [P] [US3] Crear [componente o entidad] en `[ruta]`.
- [ ] T017 [US3] Implementar [comportamiento] en `[ruta]`.

**Punto de control**: Todas las historias previstas funcionan de manera independiente.

## Fase Final: Pulido y Validación

**Propósito**: Completar tareas transversales y conservar evidencia reproducible.

- [ ] T018 [P] Actualizar la documentación aprobada en `[ruta]`.
- [ ] T019 Ejecutar las validaciones del plan y corregir los fallos en `[ruta o solución]`.
- [ ] T020 Registrar comandos o escenarios ejecutados y sus resultados satisfactorios en `specs/[###-nombre-de-la-funcionalidad]/quickstart.md`.

## Dependencias y Orden de Ejecución

### Dependencias entre fases

- Preparación: no depende de otras tareas.
- Fundamentos: depende de Preparación y bloquea todas las historias.
- Historias: comienzan tras Fundamentos y siguen el orden de prioridad P1, P2, P3, salvo dependencias explícitas.
- Pulido y validación: depende de las historias incluidas en el alcance.

### Dependencias entre historias

- Historia de Usuario 1: comienza después de Fundamentos.
- Historia de Usuario 2: comienza después de Fundamentos; solo depende de US1 si así se declara en sus tareas.
- Historia de Usuario 3: comienza después de Fundamentos; solo depende de otras historias si así se declara en sus tareas.

### Oportunidades de paralelismo

- Marque `[P]` únicamente para tareas que modifican archivos distintos y no dependen de tareas incompletas.
- Tareas que modifican el mismo archivo deben ejecutarse secuencialmente.
- No ejecute una historia antes de completar sus dependencias.

## Estrategia de Implementación

### MVP

1. Completar Preparación y Fundamentos.
2. Completar la Historia de Usuario 1.
3. Detenerse y validar de forma independiente antes de continuar.

### Entrega Incremental

1. Completar Fundamentos.
2. Implementar cada historia en orden de prioridad y validarla antes de avanzar.
3. Completar Pulido y Validación, y registrar evidencia en `quickstart.md`.

## Notas

- Cada historia debe ser verificable de forma independiente.
- Escriba primero las pruebas solo si la especificación solicita pruebas.
- No marque tareas `[X]` hasta completar y verificar su resultado.
- La implementación solo puede declararse `Implementada` cuando todas las tareas están completas y `quickstart.md` contiene evidencia satisfactoria de validación.
- No agregue funcionalidades ni artefactos que no estén aprobados en `spec.md`.
