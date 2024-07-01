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
This script checks that all the TODOs in the code follow the convention.
#>

$ErrorActionPreference = "Stop"

Import-Module (Join-Path $PSScriptRoot Common.psm1) -Function `
    AssertDotnet,  `
    AssertOpinionatedCsharpTodosVersion

function Main
{
    AssertOpinionatedCsharpTodosVersion

    Set-Location $PSScriptRoot
    Write-Host "Inspecting the TODOs in the code..."
    dotnet opinionated-csharp-todos `
        --inputs '**/*.cs' `
        --excludes 'packages/**' '**/obj/**' 'MsaglWpfControl/**' 'AasxCsharpLib_bkp/**'
    if($LASTEXITCODE -ne 0)
    {
        throw (
            "The opinionated-csharp-todos check failed. " +
            "Please have a close look at the output above, " +
            "in particular the lines prefixed with `"FAILED`"."
        )
    }
}

$previousLocation = Get-Location; try { Main } finally { Set-Location $previousLocation }
