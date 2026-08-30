# LegacyEcommerce smoke-test
# Verifies the site end-to-end: pages render (200), AJAX endpoints respond,
# add-to-cart + coupon work with the real anti-forgery flow.
# Usage:  powershell -ExecutionPolicy Bypass -File scripts\smoke-test.ps1 [-BaseUrl http://localhost:50861]

param(
    [string]$BaseUrl = "http://localhost:50861",
    [int]$TimeoutSec = 20
)

$ErrorActionPreference = "Stop"
$tmp = Join-Path $env:TEMP ("legacy-smoke-" + [Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $tmp | Out-Null

$pass = 0
$fail = 0

function Check([string]$name, [bool]$ok, [string]$detail = "") {
    if ($ok) { $script:pass++; Write-Host "  [PASS] $name $detail" -ForegroundColor Green }
    else { $script:fail++; Write-Host "  [FAIL] $name $detail" -ForegroundColor Red }
}

function Get-HttpCode([string]$url, [string]$method = "GET", [string]$data = "", [string]$cookie = "") {
    $args = @("-s", "-o", "NUL", "-w", "%{http_code}", "--connect-timeout", "5")
    if ($cookie) { $args += @("-b", $cookie, "-c", $cookie) }
    if ($data) { $args += @("-X", $method, "-d", $data) }
    elseif ($method -ne "GET") { $args += @("-X", $method) }
    $code = & curl.exe @args "$url" 2>$null
    return [string]$code
}

function Get-Page([string]$url, [string]$outFile, [string]$cookie = "") {
    $args = @("-s", "-L")
    if ($cookie) { $args += @("-b", $cookie, "-c", $cookie) }
    $args += @("-o", $outFile, "$url")
    & curl.exe @args 2>$null | Out-Null
}

Write-Host "== LegacyEcommerce smoke test ==" -ForegroundColor Cyan
Write-Host "Target: $BaseUrl"
Write-Host ""

Write-Host "[1] Page renders (HTTP 200)" -ForegroundColor Yellow
$pageUrls = @("/", "/Product", "/Product/Detail/1", "/Cart", "/Account/Login", "/Account/Register", "/Cart/MiniCart")
foreach ($u in $pageUrls) {
    $code = Get-HttpCode ($BaseUrl + $u)
    Check "GET $u" ($code -eq "200") "($code)"
}

Write-Host "[2] Add to cart via AJAX (anti-forgery form-field flow)" -ForegroundColor Yellow
$jar = Join-Path $tmp "jar.txt"
$homePage = Join-Path $tmp "home.html"
Get-Page ($BaseUrl + "/") $homePage $jar
$html = [System.IO.File]::ReadAllText($homePage, [System.Text.Encoding]::UTF8)
$m = [regex]::Match($html, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"')
$token = $m.Groups[1].Value
Check "Anti-forgery token extracted" ($token.Length -gt 10)

if ($token.Length -gt 10) {
    $body = "productId=2&quantity=1&__RequestVerificationToken=$token"
    $resp = & curl.exe -s -b $jar -c $jar -X POST -d $body ($BaseUrl + "/Cart/Add") 2>$null
    Check "POST /Cart/Add returns success" ("$resp" -match '"success":\s*true') $resp
    Check "POST /Cart/Add itemCount >= 1" ("$resp" -match '"itemCount":\s*[1-9]') ""

    $coupon = & curl.exe -s -b $jar -c $jar -X POST -d "couponCode=SAVE10&__RequestVerificationToken=$token" ($BaseUrl + "/Cart/ApplyCoupon") 2>$null
    Check "POST /Cart/ApplyCoupon (SAVE10)" ("$coupon" -match "Discount \(SAVE10\)") ("len=" + $coupon.Length)
}

Write-Host "[3] Anti-forgery rejected when token is missing" -ForegroundColor Yellow
$code403 = Get-HttpCode ($BaseUrl + "/Cart/Add") "POST" "productId=1&quantity=1"
Check "POST /Cart/Add without token NOT accepted" ($code403 -ne "200") "($code403)"

Write-Host ""
Write-Host "Summary: $pass passed, $fail failed" -ForegroundColor Cyan
Remove-Item -LiteralPath $tmp -Recurse -Force -ErrorAction SilentlyContinue
if ($fail -gt 0) { exit 1 }
exit 0