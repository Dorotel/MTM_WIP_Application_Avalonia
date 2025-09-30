# GSC Update Help Content Validation (T107)
# Ensures interactive help and docs include update command references

Describe "GSC Update - Help Content" {
    It "docs/gsc-interactive-help.html mentions update command and parameters" {
        $helpPath = "docs/gsc-interactive-help.html"
        Test-Path $helpPath | Should -BeTrue
        $html = Get-Content -Path $helpPath -Raw
        $html | Should -Match "/gsc/update"
        $html | Should -Match "insert|append|replace|remove|suggest"
    }
}

Write-Host "[UI] Update help content tests executed" -ForegroundColor Yellow
