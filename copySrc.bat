del "Unity\Assets\CVARC Assets\CVARC.Core.dll"
del "Unity\Assets\CVARC Assets\Demo.General.dll"
del "Unity\Assets\CVARC Assets\RepairTheStarship.General.dll"

del "Unity\Assets\Cvarc Sources" /s /q
md "Unity\Assets\CVARC Sources\"

md "Unity\Assets\CVARC Sources\Core"
xcopy "CVARC\CVARC.V2\Framework\CVARC.Core\*.*" "Unity\Assets\CVARC Sources\Core" /s /e
del "Unity\Assets\CVARC Sources\Core\Properties" /s /q

md "Unity\Assets\CVARC Sources\Demo"
xcopy "CVARC\CVARC.V2\Demo\DemoCompetitions\*.*" "Unity\Assets\CVARC Sources\Demo" /s /e
del "Unity\Assets\CVARC Sources\Demo\Properties" /s /q

md "Unity\Assets\CVARC Sources\RTS"
xcopy "CVARC\CVARC.V2\RepairTheStarship\RepairTheStarship\*.*" "Unity\Assets\CVARC Sources\RTS" /s /e
del "Unity\Assets\CVARC Sources\RTS\Properties" /s /q


del "Unity\Assets\NET Framework\" /s /q
xcopy "C:\Windows\Microsoft.NET\Framework\v2.0.50727\System*.dll" "Unity\Assets\NET Framework\"
xcopy "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.0\System.Runtime.Serialization.dll" "Unity\Assets\NET Framework\"
xcopy "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.5\System.ServiceModel.Web.dll" "Unity\Assets\NET Framework\"
xcopy "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.5\System.Web.Extensions.dll" "Unity\Assets\NET Framework\"