Building on server:
===================

Get and reference this in the project: https://www.nuget.org/packages/MSBuild.Microsoft.VisualStudio.Web.targets/ see also https://github.com/pdonald/nuget-webtargets . I had to add this to both Nop.Web and Nop.Admin projects. I tried installing this different ways, but try adding through "Manage NuGet Packages" in the project context menu first

Deploy:
=======

I've simply set up two Configuration parameters in the project.
```build.file.directory  Presentation\Nop.Web\```
```Presentation\Nop.Web\ C:\Users\JeremyS\Projects\VisualStudio\TestWebRoot\```
These are picked up by the command line editor in TeamCity (Type % to get/see/use the values) when I set up an additional build step (batch).

I then used robocopy to publish the files to my website root (and add an echo to exit 0 [success] rather than 1):
```
@robocopy  %build.file.directory% %publish.web.directory% /s
echo "robocopy done."
```

See also:
https://www.iis.net/downloads/microsoft/web-deploy
https://www.geekytidbits.com/web-deploy-ms-deploy-from-teamcity/ 