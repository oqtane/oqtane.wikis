XCOPY "..\Client\bin\Debug\net8.0\Oqtane.Wiki.Client.Oqtane.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\net8.0\" /Y
XCOPY "..\Client\bin\Debug\net8.0\Oqtane.Wiki.Client.Oqtane.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\net8.0\" /Y
XCOPY "..\Server\bin\Debug\net8.0\Oqtane.Wiki.Server.Oqtane.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\net8.0\" /Y
XCOPY "..\Server\bin\Debug\net8.0\Oqtane.Wiki.Server.Oqtane.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\net8.0\" /Y
XCOPY "..\Shared\bin\Debug\net8.0\Oqtane.Wiki.Shared.Oqtane.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\net8.0\" /Y
XCOPY "..\Shared\bin\Debug\net8.0\Oqtane.Wiki.Shared.Oqtane.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\net8.0\" /Y
XCOPY "..\Server\wwwroot\Modules\Oqtane.Wiki\*" "..\..\oqtane.framework\Oqtane.Server\wwwroot\Modules\Oqtane.Wiki\" /Y /S /I
