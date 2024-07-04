$ErrorActionPreference = "Stop"
dotnet.exe publish .\EnderbyteProgramsAPIService.sln -p:PublishProfile=Properties\PublishProfiles\FolderProfile.pubxml
Copy-Item appsettings.Development.json bin\release\net8.0\publish\appsettings.json
scp.exe bin\release\net8.0\publish\* jordan@raspberrypi:/home/jordan/new-api
ssh.exe jordan@raspberrypi "`
    cd ~/new-api;`
    echo `"waiting for old to stop`";`
    killall EnderbyteProgramsAPIService;`
    sleep 10;`
    chmod +x EnderbyteProgramsAPIService;`
    screen -d -m ./EnderbyteProgramsAPIService"