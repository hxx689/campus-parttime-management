# 校园兼职招聘管理系统 - 本地构建脚本
# 用法：powershell -ExecutionPolicy Bypass -File scripts/build.ps1

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$sln  = Join-Path $root 'src/backend/CampusPartTime.sln'

Write-Host '[1/3] 还原 NuGet 包...' -ForegroundColor Cyan
if (Get-Command nuget -ErrorAction SilentlyContinue) {
    nuget restore $sln
} else {
    Write-Host '未检测到 nuget 命令，用 Visual Studio 打开解决方案会自动还原'
}

Write-Host '[2/3] 编译解决方案...' -ForegroundColor Cyan
$msb = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe'
& $msb $sln /t:Rebuild /p:Configuration=Release /v:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw '编译失败' }

Write-Host '[3/3] 校验数据库脚本结构...' -ForegroundColor Cyan
python (Join-Path $root 'tests/verify_db_script.py')

Write-Host '构建完成，产物位于 src/backend/CampusPartTime/bin' -ForegroundColor Green
