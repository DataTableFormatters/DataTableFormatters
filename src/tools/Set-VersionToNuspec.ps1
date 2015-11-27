<#
    .SYNOPSIS 
      Sets the .nuspec files

    .PARAMETER Version 

    Version of the NuGet package, without including a prerelease suffix. For example, "1.2.5".

    .PARAMETER Production

    If this switch is present, no prerelease suffix will be appended to the NuGet package version.

    .PARAMETER Path

    Directory in which the script will search for nuspec and csproj files. Usually the repository root.
      
    .EXAMPLE
     Set-VersionToNuspec 1.2.5 -Production .\src

     .LINK
      http://www.luisrocha.net/2009/11/setting-assembly-version-with-windows.html
      http://blogs.msdn.com/b/dotnetinterop/archive/2008/04/21/powershell-script-to-batch-update-assemblyinfo-cs-with-new-version.aspx
      http://jake.murzy.com/post/3099699807/how-to-update-assembly-version-numbers-with-teamcity
      https://github.com/ferventcoder/this.Log/blob/master/build.ps1#L6-L19
#>

[CmdletBinding()]
Param(
    [String]
	[Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
	[ValidatePattern('^[0-9]+(\.([0-9]+|\*)){1,3}$')]
	$Version,
    [String]
	[Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
	[ValidateNotNullorEmpty()]
	$Path
)

function Help {
    "Sets the version of .nuspec files`n"
    ".\SetAssemblyVersion.ps1 [Version] -path [Path]`n"
    "   [Version]     The version number to set, for example: 1.1.9301.0"
    "   [Path]        The path to recursively search for .nuspec files.`n"
}

function Update-SourceVersionInNuspec
{
    Param ([string]$CompleteVersion)
 
    foreach ($file in $input) 
    {
        Write-Host "Updating  '$($file.FullName)' -> $CompleteVersion"

        $nuspecVersionPattern = '{version}';
        $nuspecVersion = "$CompleteVersion";
       
        (Get-Content $file.FullName) |
           % {$_ -replace $nuspecVersionPattern, $nuspecVersion } | 
           Out-File $file.FullName -encoding UTF8 -force
    }
}

function Update-AllAssemblyInfoFiles ( $Version )
{
   Write-Host "Searching '$Path'"
   
    if ($Production) {
        $CompleteVersion = $Version;
    }
    else {
        $PrereleaseSuffix = "dev$(Get-Date -format yyyyMMdd-HHmmss)";
        $CompleteVersion = "$Version-$PrereleaseSuffix";
    }
   
   foreach ($file in "nuspec$" ) 
   {
        get-childitem $Path -recurse |? {$_.Name -match $file} | Update-SourceVersionInNuspec $CompleteVersion;
   }
}
 
Update-AllAssemblyInfoFiles $Version