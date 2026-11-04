$ErrorActionPreference = 'Stop'
$port = 5031
$baseUrl = "http://127.0.0.1:$port"
$project = 'app/backend/src/RealtorApi/RealtorApi.csproj'
$dbPath = 'C:\sdd\app\backend\src\RealtorApi\manual-tests-impl.db'
$validImage = 'C:\sdd\app\backend\src\RealtorApi\valid-upload.png'
$invalidFile = 'C:\sdd\app\backend\src\RealtorApi\invalid-upload.txt'
$largeFile = 'C:\sdd\app\backend\src\RealtorApi\large-upload.png'
$stdoutLog = 'C:\sdd\app\backend\src\RealtorApi\manual-server.out.log'
$stderrLog = 'C:\sdd\app\backend\src\RealtorApi\manual-server.err.log'

Set-Content -Path $invalidFile -Value 'contenido no valido' -NoNewline
$bytes = New-Object byte[] (6MB)
(New-Object Random).NextBytes($bytes)
[System.IO.File]::WriteAllBytes($largeFile, $bytes)
$validImageBytes = [System.Convert]::FromBase64String('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO2ZQ5kAAAAASUVORK5CYII=')
[System.IO.File]::WriteAllBytes($validImage, $validImageBytes)

$env:ASPNETCORE_URLS = $baseUrl
$env:Database__Provider = 'Sqlite'
$env:ConnectionStrings__DefaultConnection = "Data Source=$dbPath"

$proc = Start-Process dotnet -ArgumentList @('run','--no-launch-profile','--project',$project) -PassThru -WindowStyle Hidden -RedirectStandardOutput $stdoutLog -RedirectStandardError $stderrLog

$serverReady = $false
for ($attempt = 1; $attempt -le 30; $attempt++) {
    try {
        $health = Invoke-WebRequest "$baseUrl/health" -Method Get -SkipHttpErrorCheck -TimeoutSec 2
        if ($health.StatusCode -eq 200) {
            $serverReady = $true
            break
        }
    }
    catch {
        Start-Sleep -Milliseconds 500
    }
}

if (-not $serverReady) {
    throw "La API no estuvo disponible en $baseUrl. Revisar logs: $stdoutLog y $stderrLog"
}

try {
    $common = @{
        title = 'Casa manual'
        description = 'Validacion manual'
        address = 'Manual St'
        price = '1999.99'
        status = 'Available'
        bedroomCount = '2'
        bathroomCount = '1'
        areaSquareMeters = '80'
    }

    $noImageStatus = (Invoke-WebRequest "$baseUrl/api/properties" -Method Post -Form $common -SkipHttpErrorCheck).StatusCode

    $withImageForm = $common.Clone()
    $withImageForm.image = Get-Item $validImage
    $withImageResp = Invoke-WebRequest "$baseUrl/api/properties" -Method Post -Form $withImageForm -SkipHttpErrorCheck
    $withImageStatus = $withImageResp.StatusCode
    $withImageBody = $withImageResp.Content | ConvertFrom-Json

    $badForm = $common.Clone()
    $badForm.title = ''
    $badStatus = (Invoke-WebRequest "$baseUrl/api/properties" -Method Post -Form $badForm -SkipHttpErrorCheck).StatusCode

    $invalidImgForm = $common.Clone()
    $invalidImgForm.image = Get-Item $invalidFile
    $invalidImgStatus = (Invoke-WebRequest "$baseUrl/api/properties" -Method Post -Form $invalidImgForm -SkipHttpErrorCheck).StatusCode

    $largeImgForm = $common.Clone()
    $largeImgForm.image = Get-Item $largeFile
    $largeImgStatus = (Invoke-WebRequest "$baseUrl/api/properties" -Method Post -Form $largeImgForm -SkipHttpErrorCheck).StatusCode

    $durations = New-Object System.Collections.Generic.List[double]

    for ($i = 1; $i -le 100; $i++) {
        $form = $common.Clone()
        $form.title = "Perf Casa $i"

        if ($i % 2 -eq 0) {
            $form.image = Get-Item $validImage
        }

        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        $status = (Invoke-WebRequest "$baseUrl/api/properties" -Method Post -Form $form -SkipHttpErrorCheck).StatusCode
        $sw.Stop()

        if ($status -ne 201) {
            throw "Solicitud de performance $i devolvio $status"
        }

        $durations.Add($sw.Elapsed.TotalMilliseconds)
    }

    $sorted = $durations | Sort-Object
    $index = [Math]::Ceiling(0.95 * $sorted.Count) - 1

    if ($index -lt 0) {
        $index = 0
    }

    $p95 = [Math]::Round($sorted[$index], 2)

    [PSCustomObject]@{
        noImageStatus = $noImageStatus
        withImageStatus = $withImageStatus
        withImageUrl = $withImageBody.imageUrl
        badRequestStatus = $badStatus
        unsupportedMediaStatus = $invalidImgStatus
        payloadTooLargeStatus = $largeImgStatus
        performanceSample = 100
        performanceP95ms = $p95
        performanceTargetMs = 3000
        performancePass = ($p95 -lt 3000)
    } | ConvertTo-Json -Depth 4
}
finally {
    if ($proc -and -not $proc.HasExited) {
        Stop-Process -Id $proc.Id -Force
    }

    Remove-Item $invalidFile -ErrorAction SilentlyContinue
    Remove-Item $largeFile -ErrorAction SilentlyContinue
    Remove-Item $validImage -ErrorAction SilentlyContinue
}
