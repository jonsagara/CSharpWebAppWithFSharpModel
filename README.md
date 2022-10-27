# UPDATE 2022-10-27

[GitHub Issue](https://github.com/dotnet/aspnetcore/issues/44749)

The fix will be released with [.NET 7.0 RTM](https://github.com/dotnet/aspnetcore/issues/44749#issuecomment-1293867468).

Until then, there is a [workaround](https://github.com/dotnet/fsharp/issues/14088#issuecomment-1275845376).

# .NET 7.0 RC2 breaks MVC build where a view references an F# model

When I upgraded my project to .NET SDK `7.0.100-rc.2.22477.23`, my build started failing in `Release` mode. I only noticed this because the build step of my DevOps pipeline started failing.

## Description of bug

I have an existing C# ASP.NET Core MVC application that references an F# class library. The F# library exposes several record types that the MVC application consumes directly. The view model class has a reference to one of these F# records and tries to display it in the corresponding view.

F# record:

```fsharp
type TermReportCards = {
    Term : DateTime
    TermName : string
    ReportCards : ReportCard array
    }
```

C# view model:

```csharp
public class HomeIndexModel
{
    public List<TermReportCards> TermReportCards { get; private set; } = new();
}
```

ASP.NET Core MVC Razor view (.cshtml):

```html
@model HomeIndexModel
@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <!-- ... -->

    @foreach (var targetReportCard in Model.TermReportCards.OrderBy(trc => trc.Term))
    {
        // noop
    }
</div>
```

The application has built and functioned correctly for years, including with .NET SDK `6.0.402`. However, when I switched to .NET SDK `7.0.100-rc.2.22477.23` and updated the nuget packages and project TFMs accordingly, the MVC project no longer compiles in Release mode.

My environment:

- OS: Windows 11 Version 22H2 (OS Build 22621.755)
- Visual Studio 2022: Version 17.4.0 Preview 5.0


## Steps to reproduce the *working* .NET 6 build

1. Clone the repository at https://github.com/jonsagara/CSharpWebAppWithFSharpModel
1. Check out the `net6` branch
1. Run the `remove_bin_obj.bat` batch file to remove any `bin` and `obj` subdirectories
1. Open a command prompt in the root directory and issue the following command: `dotnet.exe build src\WebApp\WebApp.csproj --configuration Release `

### Expected results:

The solution builds successfully.

### Actual results:

The solution builds successfully.

## Steps to reproduce the *broken* .NET 7 RC2 build

1. Clone the repository at https://github.com/jonsagara/CSharpWebAppWithFSharpModel
1. Check out the `net7` branch
1. Run the `remove_bin_obj.bat` batch file to remove any `bin` and `obj` subdirectories
1. Open a command prompt in the root directory and issue the following command: `dotnet.exe build src\WebApp\WebApp.csproj --configuration Release `

### Expected results:

The solution builds successfully.

### Actual results:

The build fails with the following:

```
MSBuild version 17.4.0-preview-22470-08+6521b1591 for .NET
  Determining projects to restore...
  All projects are up-to-date for restore.
C:\Program Files\dotnet\sdk\7.0.100-rc.2.22477.23\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.RuntimeIdentifierInference.targets(257,5): message NETSDK1057: You are using a preview version of .NET. See: https://aka.ms/dotnet-support-policy [C:\Dev\SANDBOX\CSharpWebAppWithFSharpModel\src\WebApp\WebApp.csproj]
  FSharpClassLibrary -> C:\Dev\SANDBOX\CSharpWebAppWithFSharpModel\src\FSharpClassLibrary\bin\Release\net7.0\FSharpClassLibrary.dll
C:\Dev\SANDBOX\CSharpWebAppWithFSharpModel\src\WebApp\Views\Home\Index.cshtml(10,80): error CS1061: 'TermReportCards' does not contain a definition for 'Term' and no accessible extension method 'Term' accepting a first argument of type 'TermReportCards' could be found (are you missing a using directive or an assembly reference?) [C:\Dev\SANDBOX\CSharpWebAppWithFSharpModel\src\WebApp\WebApp.csproj]

Build FAILED.

C:\Dev\SANDBOX\CSharpWebAppWithFSharpModel\src\WebApp\Views\Home\Index.cshtml(10,80): error CS1061: 'TermReportCards' does not contain a definition for 'Term' and no accessible extension method 'Term' accepting a first argument of type 'TermReportCards' could be found (are you missing a using directive or an assembly reference?) [C:\Dev\SANDBOX\CSharpWebAppWithFSharpModel\src\WebApp\WebApp.csproj]
    0 Warning(s)
    1 Error(s)

Time Elapsed 00:00:06.17
```

### The `Domain` module in the F# project

Interestingly, if I comment out the `School` record in `FSharpClassLibrary\Domain.fs` and re-run the build command, the build succeeds:

```fsharp
module internal Domain =
    ()
    
    ///// Name of a learning institution.
    //type School = {
    //    Name : string
    //}
```


```
MSBuild version 17.4.0-preview-22470-08+6521b1591 for .NET
  Determining projects to restore...
  All projects are up-to-date for restore.
C:\Program Files\dotnet\sdk\7.0.100-rc.2.22477.23\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NETRuntimeIdentifierInference.targets(257,5): message NETSDK1057: You are using a preview version of .NET. See: https://aka.ms/dotnet-support-policy [C:\Dev\SANDBOX\CSharpWebAppWithFSharpModel\src\WebApp\WebApp.csproj]
  FSharpClassLibrary -> C:\Dev\SANDBOX\CSharpWebAppWithFSharpModel\src\FSharpClassLibrary\bin\Release\net7.0\FSharpClassLibrary.dll
  WebApp -> C:\Dev\SANDBOX\CSharpWebAppWithFSharpModel\src\WebApp\bin\Release\net7.0\WebApp.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:04.30
```

But as soon as I uncomment the `School` record, the build error returns.

Also of note, if I clean and build the solution from within Visual Studio, I get no error, but since the DevOps pipeline runs a `dotnet build` command, that's what I'm doing here.

---

I'm not sure if this is an ASP.NET Core issue, a .NET SDK issue, an MSBuild issue, or some other issue. If this is not the correct repository, will you please help me find the correct one?

Thank you.