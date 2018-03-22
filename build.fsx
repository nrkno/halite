#r @"packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
open Fake.Git
open Fake.MSBuildHelper
open Fake.Testing.XUnit2
open Fake.PaketTemplate
open Fake.DotNetCli

let buildDir = "./build/"
let testProjects = "./source/*Tests/*.csproj"
let testOutputDir = "./tests/"
let haliteProjectReferences = "./source/Halite/Halite.csproj"
let examplesProjectReferences = !! "./source/Halite.Examples/Halite.Examples.csproj" 
let analyzerProjectReferences = !! "./source/analyzer/Halite.Analyzer/Halite.Analyzer.csproj"
let testProjectReferences = !! "./source/Halite.Tests/Halite.Tests.csproj"
                            ++ "./source/Halite.Examples.Tests/Halite.Examples.Tests.csproj"
                            ++ "./source/analyzer/Halite.Analyzer.Test/Halite.Analyzer.Tests.csproj"
let projectName = "Halite"
let description = "Library for representing HAL objects and links."
let version = environVarOrDefault "version" "0.0.0"
let assemblyGuid = "670c2953-95c3-493c-a39c-987105130378"
let commitHash = Information.getCurrentSHA1(".")
let templateFilePath = "./Halite.paket.template"
let toolPathPaket = ".paket/paket.exe"

Target "Clean" (fun _ ->
  CleanDirs [buildDir; testOutputDir]
)

let buildReleaseProperties = 
  [ "Configuration", "Release"
    "DocumentationFile", "Halite.xml" ]

Target "AddAssemblyVersion" (fun _ -> 
    let assemblyInfos = !!(@"../**/AssemblyInfo.cs") 

    ReplaceAssemblyInfoVersionsBulk assemblyInfos (fun f -> 
        { f with 
            AssemblyFileVersion = version
            AssemblyVersion = version })  
)

Target "Build" (fun _ -> 
    DotNetCli.Build (fun p -> 
        { p with
            Output = "../../" + buildDir
            Configuration = "Release"
            // AdditionalArgs = ["DocumentationFile"; "Halite.xml"]
            Project = haliteProjectReferences }) 
)

Target "BuildExamples" (fun _ -> MSBuild buildDir "Build" buildReleaseProperties examplesProjectReferences |> Log "Building examples project: " )

Target "BuildAnalyzer" (fun _ -> MSBuild buildDir "Build" buildReleaseProperties analyzerProjectReferences |> Log "Building analyzer project: ")

Target "BuildTests" (fun _ ->  MSBuild testOutputDir "Build" [ "Configuration", "Debug" ] testProjectReferences |> Log "TestBuild-Output: ")

Target "RunTests" (fun _ ->
  !! (testOutputDir @@ "*Tests.dll")
  |> xUnit2 (fun p ->
                 { p with HtmlOutputPath = Some (testOutputDir @@ "xunit.html") })
)

Target "CreateNugetPackage" (fun _ -> 
    let description = "Implementation of the HAL specification."
    DotNetCli.Pack (fun c -> 
        { c with
            Configuration = "Release"
            Project = haliteProjectReferences
            AdditionalArgs = [ 
                                "/p:PackageVersion=" + version
                                "/p:Version=" + version]
            OutputPath = "../../" + buildDir
        }
    )
)

Target "PushPackage" (fun _ ->
  Paket.Push (fun p -> 
      {
          p with
            ApiKey = environVarOrDefault "nugetKey" ""
            PublishUrl = "https://www.nuget.org/api/v2/package"
            ToolPath = toolPathPaket
            WorkingDir = "./build/"
      })
)

"Clean"
==> "AddAssemblyVersion"
==> "Build"
==> "BuildExamples"
==> "BuildAnalyzer"
==> "BuildTests"
==> "RunTests"
==> "CreateNugetPackage"
==> "PushPackage"

RunTargetOrDefault "CreateNugetPackage"
