<#*******************************************************************************
* Copyright (c) {2024} Contributors to the Eclipse Foundation
*
* See the NOTICE file(s) distributed with this work for additional
* information regarding copyright ownership.
*
* This program and the accompanying materials are made available under the
* terms of the Apache License Version 2.0 which is available at
* https://www.apache.org/licenses/LICENSE-2.0
*
* SPDX-License-Identifier: Apache-2.0
*******************************************************************************#>

<#
.SYNOPSIS
This script installs the tools necessary to build and check the solution.
#>

Import-Module (Join-Path $PSScriptRoot Common.psm1) -Function `
    AssertDotnet, `
    GetToolsDir

function Main {
    if ($null -eq (Get-Command "nuget.exe" -ErrorAction SilentlyContinue))
    {
       throw "Unable to find nuget.exe in your PATH"
    }

    AssertDotnet

    $toolsDir = GetToolsDir
    New-Item -ItemType Directory -Force -Path $toolsDir

    Write-Host "Installing Nunit Console Runner ..."
    nuget install NUnit.ConsoleRunner -Version 3.11.1 `
        -OutputDirectory $toolsDir

    nuget install NUnit.Extension.NUnitProjectLoader -Version 3.6.0 `
        -OutputDirectory $toolsDir

    Write-Host "Installing OpenCover ..."
    nuget install OpenCover -Version 4.7.922 -OutputDirectory $toolsDir
    nuget install ReportGenerator -Version 4.6.0 -OutputDirectory $toolsDir

    Write-Host "Restoring dotnet tools for the solution ..."
    dotnet tool restore
}

$previousLocation = Get-Location; try { Main } finally { Set-Location $previousLocation }
