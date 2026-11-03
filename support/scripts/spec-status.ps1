[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidateSet('Approve', 'Begin', 'Complete')]
    [string]$Transition,

    [string]$FeatureDirectory
)

$ErrorActionPreference = 'Stop'

function Get-CheckboxCounts {
    param([Parameter(Mandatory)][string]$Path)

    $insideFence = $false
    $insideComment = $false
    $total = 0
    $open = 0
    foreach ($line in Get-Content -LiteralPath $Path -Encoding utf8) {
        if ($insideComment) {
            if ($line -match '-->') {
                $insideComment = $false
            }
            continue
        }

        if ($line -match '<!--') {
            if ($line -notmatch '-->') {
                $insideComment = $true
            }
            continue
        }

        if ($line -match '^\s*```') {
            $insideFence = -not $insideFence
            continue
        }

        if (-not $insideFence -and $line -match '^\s*-\s+\[([ xX])\]') {
            $total++
            if ($Matches[1] -eq ' ') {
                $open++
            }
        }
    }

    return [pscustomobject]@{ Total = $total; Open = $open }
}

$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))

if (-not $FeatureDirectory) {
    $featureConfigPath = Join-Path $repositoryRoot '.specify\feature.json'
    if (-not (Test-Path -LiteralPath $featureConfigPath)) {
        throw 'No existe .specify/feature.json; no se puede resolver la spec activa.'
    }

    $featureConfig = Get-Content -LiteralPath $featureConfigPath -Raw -Encoding utf8 | ConvertFrom-Json
    $FeatureDirectory = [string]$featureConfig.feature_directory
}

$featurePath = [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot $FeatureDirectory))
$repositoryPrefix = $repositoryRoot.TrimEnd('\') + '\'
if (-not $featurePath.StartsWith($repositoryPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw 'La carpeta de la spec debe permanecer dentro del repositorio.'
}

$specPath = Join-Path $featurePath 'spec.md'
if (-not (Test-Path -LiteralPath $specPath)) {
    throw "No existe el archivo requerido: $specPath"
}

$specContent = Get-Content -LiteralPath $specPath -Raw -Encoding utf8
$statusMatches = [regex]::Matches($specContent, '(?m)^\*\*Estado\*\*:\s*(Borrador|Aprobada|En implementación|Implementada)\s*$')
if ($statusMatches.Count -ne 1) {
    throw 'La spec debe contener exactamente una línea **Estado** con un estado canónico.'
}

$currentStatus = $statusMatches[0].Groups[1].Value
$targetStatus = $currentStatus
$blockers = [System.Collections.Generic.List[string]]::new()

switch ($Transition) {
    'Approve' {
        if ($currentStatus -eq 'Borrador') {
            $unresolved = [regex]::IsMatch($specContent, '\[NEEDS CLARIFICATION:')
            if ($unresolved) {
                $blockers.Add('la spec todavía contiene marcadores [NEEDS CLARIFICATION].')
            }

            $checklistDirectory = Join-Path $featurePath 'checklists'
            if (Test-Path -LiteralPath $checklistDirectory) {
                $checklistFiles = Get-ChildItem -LiteralPath $checklistDirectory -File -Filter '*.md' -Recurse
                foreach ($checklistFile in $checklistFiles) {
                    $checklistCounts = Get-CheckboxCounts -Path $checklistFile.FullName
                    if ($checklistCounts.Open -gt 0) {
                        $blockers.Add("el checklist '$($checklistFile.Name)' tiene $($checklistCounts.Open) elemento(s) pendiente(s).")
                    }
                }
            }

            if ($blockers.Count -eq 0) {
                $targetStatus = 'Aprobada'
            }
        }
        elseif ($currentStatus -ne 'Aprobada') {
            $blockers.Add("la transición Borrador -> Aprobada no se permite desde '$currentStatus'.")
        }
    }

    'Begin' {
        if ($currentStatus -eq 'Aprobada') {
            $targetStatus = 'En implementación'
        }
        elseif ($currentStatus -ne 'En implementación') {
            $blockers.Add("la implementación solo puede comenzar desde 'Aprobada'; estado actual: '$currentStatus'.")
        }
    }

    'Complete' {
        if ($currentStatus -notin @('En implementación', 'Implementada')) {
            $blockers.Add("la implementación solo puede cerrarse desde 'En implementación'; estado actual: '$currentStatus'.")
        }

        $tasksPath = Join-Path $featurePath 'tasks.md'
        if (-not (Test-Path -LiteralPath $tasksPath)) {
            $blockers.Add('no existe tasks.md.')
        }
        else {
            $taskCounts = Get-CheckboxCounts -Path $tasksPath
            if ($taskCounts.Total -eq 0) {
                $blockers.Add('tasks.md no contiene tareas verificables.')
            }
            elseif ($taskCounts.Open -gt 0) {
                $blockers.Add("tasks.md contiene $($taskCounts.Open) tarea(s) sin completar.")
            }
        }

        $quickstartPath = Join-Path $featurePath 'quickstart.md'
        if (-not (Test-Path -LiteralPath $quickstartPath)) {
            $blockers.Add('no existe quickstart.md con evidencia de validación.')
        }
        else {
            $quickstartContent = Get-Content -LiteralPath $quickstartPath -Raw -Encoding utf8
            if ($quickstartContent -notmatch '(?im)^## Evidencia de validación\s*$') {
                $blockers.Add("quickstart.md no contiene el encabezado '## Evidencia de validación'.")
            }
            if ($quickstartContent -notmatch '(?im)^\s*-\s*Comando:\s*`[^`]+`\s*$') {
                $blockers.Add('quickstart.md no registra al menos un comando o escenario ejecutado.')
            }
            if ($quickstartContent -notmatch '(?im)^\s*-\s*Resultado:\s*Correcto\s*$') {
                $blockers.Add('quickstart.md no registra un resultado satisfactorio con el formato requerido.')
            }
            if ($quickstartContent -match '(?im)^\s*-\s*Resultado:\s*(Fallido|Error)\s*$') {
                $blockers.Add('quickstart.md registra al menos una validación fallida.')
            }
        }

        if ($blockers.Count -eq 0) {
            $targetStatus = 'Implementada'
        }
    }
}

if ($blockers.Count -gt 0) {
    if ($currentStatus -eq 'En implementación') {
        Write-Output 'Estado mantenido: En implementación.'
    }
    $blockerReport = ($blockers | ForEach-Object { "BLOQUEO: $_" }) -join [Environment]::NewLine
    throw $blockerReport
}

if ($targetStatus -ne $currentStatus) {
    $statusPattern = [regex]::new('(?m)^(\*\*Estado\*\*:\s*)(Borrador|Aprobada|En implementación|Implementada)\s*$')
    $updatedContent = $statusPattern.Replace($specContent, ('$1' + $targetStatus), 1)
    [System.IO.File]::WriteAllText($specPath, $updatedContent, [System.Text.UTF8Encoding]::new($false))
}

Write-Output "Estado de la spec: $currentStatus -> $targetStatus."