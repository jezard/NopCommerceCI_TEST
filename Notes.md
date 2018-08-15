Overview
========
We've discussed several times how it might be a good thing to deploy our websites using Git. The issue we seemed to stumble on is whereas deploying scripted sites compiled at runtime e.g. php works very well, the extra build step meant that we'd either need a separate repository containing the published site or to find some other way to do it. After some research, and knowing that we are now all committed to Git VCS it is clear that others are utilising Continuous Integration CI successfully with .Net apps. There are several tools available, but during my googling for the best tools for .Net, TeamCity seemed to come out top (*After trialling it seems very well integrated with VS - It doesn't feel like working with a generic project type, it feels like an extension of VS*). TeamCity is a CI server which *can* run on the production server. It is not open-source but has a extremely usable no-cost version. I created a basic test scenario on my Windows machine comprising TC server (which has a great webmin front end attached) and build project, a NopCommerce Visual Studio project, a GitHub repository and a website on local IIS. 

One of the first tasks was to create a new TC project. There was a requirement to connect to a VCS, I chose GitHub as I'm familiar with it, wanted to keep the test scenario away from our production code repositories and systems, and don't yet have access to the online version of TFS. I authorised* TC on GitHub and copied the GitHub generate clientId and secret to the TC project. I'd imagine that TFS follows a similar process flow. Once I'd then selected which GitHub repository I'd like to use for the TC project (a simple drop down in TC), TC generated a series of suggested build steps, with some recommendations. I followed the recommendations and had to make a few amends to the VS project (mainly incorrect project references, and needed to add MSBuild.Microsoft.VisualStudio.Web.targets to Nop.Web and Nop.Admin [see below])

Once the suggested build steps ran correctly, I added a further step to copy the files from my checkout directory (C:\TeamCity\buildAgent\work\f07da7dc653c78c4) where they are built, to the IIS website directory for my test site.

*When setting myself up as a user on TC I didn't initially use the same email address as that in my GitHub account, causing GH to throw a wobbly. Would recommend using same email address in TC and VCS to avoid this potential issue in TFS online.

The basic flow of the completed CI scenario is as follows:
- code updated in Visual Studio NopCommerce project and committed and pushed to the GitHub repo [branch/master]
- CI server polls at configurable [60 seconds] interval looking for changes in configurable [master] branch 
- CI server completes each of the build steps in turn. There are five in total, the first four were detected automatically (overridable) by the presence of the .sln file and the last created by me copies the files in the Nop.Web directory to the website root. There are more configurable ways to do this (see below) but this works for proof of concept

Additionally, it is easy to revert to /build a previous snapshot by selecting a previous commit in the [Run ...] dropdown menu. Build logs are very verbose and give plenty of detail about what's going on...


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
