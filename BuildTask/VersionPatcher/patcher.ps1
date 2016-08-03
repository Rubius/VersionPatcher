param($path)
pwd

Write-Host "Path is $path"

if($path)
{
    Join-Path $path 'PropertiesEditor.dll' | Import-Module
}
else
{
    Import-Module './PropertiesEditor.dll'
}
Write-Host "LOADED"

if($env:BUILD_SOURCESDIRECTORY)
{
    cd $env:BUILD_SOURCESDIRECTORY
}
Write-Host 'Current directory:'
pwd

function GetVersionFromLog
{
    param($currentTag)

    if($currentTag)
    {
        if($currentTag -isnot [system.array]) { $currentTag = @($currentTag)}
        $tagFound = $false
        foreach ($tag in $currentTag)
        {
            Write-Host "CURRENT TAG $tag"

            if ($tag -match '(?<version>\d+\.\d+(\.\d+(\.\d+)?)?)(\((?<code>\d+)\))?')
            {
                if($tagFound)
                {
                    throw "Multiple tags found for current commit"
                }

                @{Version = $Matches['version']; Code = $Matches['code']}

                $a = $Matches['version']
                $b = $Matches['code']
                Write-Host "GOT VERSION $a CODE $b"

                $tagFound = $true
            }
        }
    }
}

function GetCurrentVersion
{

Write-Host "CURRENT VERSION"
    $currentTag = git tag -l v*.* --points-at HEAD

    Write-Host "FOUND TAG $currentTag"

    GetVersionFromLog $currentTag
}

function GetLastVersion
{
Write-Host "LAST VERSION"
    $currentTag = git describe --tags --match v*.* --abbrev=0

    Write-Host "LATEST VERSION $currentTag"

    GetVersionFromLog $currentTag
}

function PatchAssemblyInfos
{
    param($version1)

    Write-Host "PATCH ASSEMBLIES"

    $test = gci -Include 'AssemblyInfo.cs' -Recurse

    Write-Host "Found $($test.Count) files. Going to patch with $version1"

    $test | foreach {Edit-AssemblyInfo -File $_ -AssemblyVersion $version1}
}

function PatchAndroidAssemblies
{
    param($version)
    PatchAssemblyInfos $version.Version
    
    Write-Host "PATCH INFOPLIST"

    gci -Include 'Info.plist' -Recurse | foreach {Edit-InfoPlist -File $_ -Version $version.Version}

    Write-Host "PATCH ANDROID"

    if($version.Code)
    {
        gci -Include 'AndroidManifest.xml' -Recurse | foreach {Edit-AndroidManifest -File $_ -Version $version.Version -VersionCode $version.Code}
    }

}

$currentVersion = GetCurrentVersion
if($currentVersion)
{
    PatchAndroidAssemblies $currentVersion
}
else
{
    $lastVersion = GetLastVersion
    if(!$lastVersion)
    {
        throw "No version tags were found"
    }

    $version = New-Object System.Version($lastVersion.Version)
    $versionString = "$($version.Major).$($version.Minor).$($version.Build).$($version.Revision+1)"
    $newTag = "v$($versionString)"
    $lastVersion.Version = $versionString

    if($lastVersion.Code)
    {
        $incrementedCode = ([convert]::ToInt32($lastVersion.Code))+1

        Write-Host "INCREMENTED CODE $incrementedCode"

        $lastVersion.Code = $incrementedCode
        $newTag = "$newTag($incrementedCode)"

    }

    $tagfound = git tag -l "$newTag"

    if($tagfound -eq $null)
    {
        $puttag = git tag $newtag

        Write-Host "TAG CREATED"

        $pushtags = git push --porcelain origin $newtag

        Write-Host "TAG PUSHED"
    }
    else
    {
        Write-Host "No NEED TO PUSH EXISTING TAG"
    }

    PatchAndroidAssemblies $lastVersion
}