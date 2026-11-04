param([string]$BaseUrl = "http://localhost:5023")

$PropertyId = "a1f0e2d4-7c81-4d12-9e61-0b7f0b1a1001"
$FakeId    = "00000000-0000-0000-0000-000000000000"
$ImagePath = "C:\sdd\support\seed-data\images\properties\2.png"
$results   = [ordered]@{}

function Measure-Req($label, $block) {
    $sw = [Diagnostics.Stopwatch]::StartNew()
    $r  = & $block
    $sw.Stop()
    [pscustomobject]@{ Label=$label; Status=$r.Status; Ms=[int]$sw.Elapsed.TotalMilliseconds; Extra=$r.Extra }
}

# E1: PUT sin imagen → 200 OK, imageUrl conservado
$r1 = Measure-Req "E1 sin-imagen" {
    $resp = Invoke-WebRequest "$BaseUrl/api/properties/$PropertyId" -Method PUT `
        -Form @{title="Casa renovada E1";description="Sin imagen";address="2450 SW 22nd St";price="4500";status="Available"} `
        -SkipHttpErrorCheck
    $body = $resp.Content | ConvertFrom-Json
    @{Status=$resp.StatusCode; Extra="imageUrl=$($body.imageUrl)"}
}

# E2: PUT con imagen PNG valida → 200 OK, imageUrl nuevo
$imgBytes  = [IO.File]::ReadAllBytes($ImagePath)
$boundary  = "UpdateBnd"; $CRLF = "`r`n"
$fields    = @{title="Casa con imagen E2";description="Con imagen";address="456 Ocean Ave";price="4800";status="Rented"}
$bodyParts = ($fields.GetEnumerator() | ForEach-Object {
    "--$boundary$CRLF" + "Content-Disposition: form-data; name=`"$($_.Key)`"$CRLF$CRLF" + $_.Value + $CRLF
}) -join ""
$imgHeader = "--$boundary$CRLF" + "Content-Disposition: form-data; name=`"image`"; filename=`"2.png`"$CRLF" + "Content-Type: image/png$CRLF$CRLF"
$tail      = "$CRLF--$boundary--"
$bodyBytes = [Text.Encoding]::UTF8.GetBytes($bodyParts)
$imgHdrBytes = [Text.Encoding]::UTF8.GetBytes($imgHeader)
$tailBytes = [Text.Encoding]::UTF8.GetBytes($tail)
$full = New-Object byte[] ($bodyBytes.Length + $imgHdrBytes.Length + $imgBytes.Length + $tailBytes.Length)
[Buffer]::BlockCopy($bodyBytes,0,$full,0,$bodyBytes.Length)
[Buffer]::BlockCopy($imgHdrBytes,0,$full,$bodyBytes.Length,$imgHdrBytes.Length)
[Buffer]::BlockCopy($imgBytes,0,$full,$bodyBytes.Length+$imgHdrBytes.Length,$imgBytes.Length)
[Buffer]::BlockCopy($tailBytes,0,$full,$bodyBytes.Length+$imgHdrBytes.Length+$imgBytes.Length,$tailBytes.Length)
$r2 = Measure-Req "E2 con-imagen" {
    $resp = Invoke-WebRequest "$BaseUrl/api/properties/$PropertyId" -Method PUT -Body $full `
        -ContentType "multipart/form-data; boundary=$boundary" -SkipHttpErrorCheck
    $body = $resp.Content | ConvertFrom-Json
    @{Status=$resp.StatusCode; Extra="imageUrl=$($body.imageUrl)"}
}

# E3: 404 id inexistente
$r3 = Measure-Req "E3 404" {
    $resp = Invoke-WebRequest "$BaseUrl/api/properties/$FakeId" -Method PUT `
        -Form @{title="x";description="y";address="z";price="1";status="Available"} -SkipHttpErrorCheck
    @{Status=$resp.StatusCode; Extra=""}
}

# E4: 400 campos invalidos
$r4 = Measure-Req "E4 400" {
    $resp = Invoke-WebRequest "$BaseUrl/api/properties/$PropertyId" -Method PUT `
        -Form @{title="";description="y";address="z";price="0";status="Available"} -SkipHttpErrorCheck
    @{Status=$resp.StatusCode; Extra=""}
}

# E5: 415 imagen formato invalido
$invalidFile = [IO.Path]::GetTempFileName(); Set-Content $invalidFile "not an image" -Encoding ASCII
$r5 = Measure-Req "E5 415" {
    $resp = Invoke-WebRequest "$BaseUrl/api/properties/$PropertyId" -Method PUT `
        -Form @{title="x";description="y";address="z";price="1";status="Available";image=(Get-Item $invalidFile)} -SkipHttpErrorCheck
    @{Status=$resp.StatusCode; Extra=""}
}
Remove-Item $invalidFile -Force

# E6: Listado compatibilidad (GET)
$r6 = Measure-Req "E6 GET-listado" {
    $resp = Invoke-WebRequest "$BaseUrl/api/properties" -SkipHttpErrorCheck
    $body = $resp.Content | ConvertFrom-Json
    @{Status=$resp.StatusCode; Extra="count=$($body.items.Count)"}
}

# SC-005: 20 solicitudes sin imagen para medir p95 (muestra reducida)
$latencies = @()
for ($i=1; $i -le 20; $i++) {
    $sw = [Diagnostics.Stopwatch]::StartNew()
    Invoke-WebRequest "$BaseUrl/api/properties/$PropertyId" -Method PUT `
        -Form @{title="Perf$i";description="d";address="a";price="1";status="Available"} `
        -SkipHttpErrorCheck | Out-Null
    $sw.Stop()
    $latencies += [int]$sw.Elapsed.TotalMilliseconds
}
$sorted = $latencies | Sort-Object
$p95 = $sorted[[math]::Ceiling(20*0.95)-1]
$avg = [int]($latencies | Measure-Object -Average).Average

# Output
@($r1,$r2,$r3,$r4,$r5,$r6) | Format-Table -AutoSize
Write-Host "SC-005 latencias (20 req): avg=${avg}ms p95=${p95}ms (umbral <3000ms)"
