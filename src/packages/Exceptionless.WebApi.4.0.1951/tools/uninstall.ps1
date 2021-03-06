param($installPath, $toolsPath, $package, $project)

Import-Module (Join-Path $toolsPath exceptionless.psm1)

$configPath = find_config $project

if ($configPath -ne $null) {
	remove_config $configPath 'WebApi'
}