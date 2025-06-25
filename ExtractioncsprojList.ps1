param (
    [string]$SolutionPath = "Hutzper.Project.PJ01022.sln",
    [string]$SlnfPath = "Hutzper.Library.slnf"
)

# .sln に含まれる .csproj パスを抽出（.wapproj を除外）
$slnProjects = Select-String -Path $SolutionPath -Pattern '^Project\(.*\)' |
    ForEach-Object { ($_ -split ',')[1].Trim().Trim('"') } |
    Where-Object { $_ -like "*.csproj" -and $_ -notlike "*.wapproj" }

# JSON 作成
$slnf = @{
    solution = @{
        path     = $SolutionPath
        projects = $slnProjects
    }
}

# 出力
$slnf | ConvertTo-Json -Depth 5 | Out-File -Encoding utf8 -FilePath $SlnfPath

Write-Host "✅ Generated $SlnfPath with valid Windows-style paths."
