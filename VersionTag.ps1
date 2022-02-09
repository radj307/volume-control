# This is required when calling the script from visual studio's prebuild event for the VolumeControl project
# It prevents the path names from being messed up because of differing working directories.
cd ..

function getVersion()
{
	$TAG="$(git describe --tags --abbrev=0)"
	$RGX=[regex]"\d+?\.\d+?\.\d+"
	$OUT=$RGX.Match($TAG)
	return $OUT.Value
}

# Find all (...).AssemblyInfo.cs files
function findAssemblyFile($projName)
{
	return $(Get-ChildItem "$projName" -Recurse -Filter "*.AssemblyInfo.cs" -Include "$projName.AssemblyInfo.cs" | select FullName).FullName
}

function setVersion($file, $version)
{
	$sr = new-object System.IO.StreamReader($file, [System.Text.Encoding]::GetEncoding("utf-8"))
	$content = $sr.ReadToEnd();
	$sr.Close()


	$version=getVersion;

	[Regex]::Replace($content, "\d+?\.\d+?\.\d+?([\.\d]*)", "$version$1") > $file
	echo "Finished modifying version numbers in $file"
}

$ASMFILES=findAssemblyFile("VolumeControl");
foreach($file in $ASMFILES)
{
	setVersion("$file")
}

return 0
