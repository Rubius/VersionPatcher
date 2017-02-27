if (Get-PSDrive -Name "/" -ErrorAction SilentlyContinue)
{
    Import-Module './core/PropertiesEditor.dll'
}
else
{
    Import-Module './PropertiesEditor.dll'
}
Write-Debug "PropertiesEditor module loaded"

if($env:BUILD_SOURCESDIRECTORY)
{
    cd $env:BUILD_SOURCESDIRECTORY
}
Write-Host 'Current directory:'
Get-Location

git version

function GetVersionFromLog ($currentTag)
{
    if($currentTag)
    {
        if($currentTag -isnot [system.array]) { $currentTag = @($currentTag)}
        $tagFound = $false
        foreach ($tag in $currentTag)
        {
            Write-Host "CURRENT TAG: $tag"

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
    Write-Host "Checking for current version"
    
    $currentTag = git tag -l v*.* --points-at HEAD
    Write-Host "FOUND TAG $currentTag"

    GetVersionFromLog $currentTag
}

function GetLastVersion
{
    Write-Host "Trying to get latest version"
    $currentTag = git describe --tags --match v*.* --abbrev=0

    Write-Host "LATEST VERSION $currentTag"

    GetVersionFromLog $currentTag
}

function Patch ([string]$type,$version,[string[]]$include,[string]$command) 
{
    Write-Host "Performing $type patching with $($version.Version) ($($version.Code))..."
    gci -Include $include -Recurse | foreach {Invoke-Expression $command} | Measure-Object | select -ExpandProperty Count | Write-Host -NoNewline
    Write-Host " files patched."
}

function PatchFiles ($version)
{
    Patch -type 'AssemblyInfo' -version $version -include @('AssemblyInfo.cs') -command 'Edit-AssemblyInfo -File $_ -Version $version.Version'
    Patch -type 'InfoPlist' -version $version -include @('Info.plist') -command 'Edit-InfoPlist -File $_ -Version $version.Version'

    if($version.Code)
    {
        Patch -type 'AndroidManifest' -version $version -include 'AndroidManifest.xml' -command 'Edit-AndroidManifest -File $_ -Version $version.Version -VersionCode $version.Code'
    }
}

$currentVersion = GetCurrentVersion
if($currentVersion)
{
    PatchFiles $currentVersion
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
    Write-Debug "$tagfound"

    if($tagfound -eq $null)
    {
        $puttag = git tag $newtag
        Write-Debug "$puttag"

        Write-Host "TAG CREATED"

        $unconfig = git config --unset credential.helper
        $pushtags = git push --porcelain origin "`"$newtag`""
        Write-Debug "$pushtags"

        Write-Host "TAG PUSHED"
    }
    else
    {
        Write-Host "No need to push existing tag"
    }

    PatchFiles $lastVersion
}