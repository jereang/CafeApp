// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing.NUnit3

// Directories
let buildDir  = "./build/"
let deployDir = "./deploy/"
let testDir = "./test/"
let nunitRunnerPath = "./packages/NUnit.ConsoleRunner/tools/"

// Filesets
let appReferences  =
    !! "/**/app/**/*.csproj"
    ++ "/**/app/**/*.fsproj"
let testReferences = 
    !! "/**/test/**/*.csproj"
    ++ "/**/test/**/*.fsproj"

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir; testDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
    |> Log "AppBuild-Output: "
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
    -- "*.zip"
    |> Zip buildDir (deployDir + "ApplicationName." + version + ".zip")
)

Target "BuildTests" (fun _ ->
    MSBuildDebug testDir "Build" testReferences
    |> Log "TestBuild-Output: "
)

Target "RunTests" (fun _ ->
    !! (testDir + "*.Tests.dll")
    |> NUnit3 (fun p -> {p with ShadowCopy = false})
)

// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"
"Clean"
  ==> "Build"
  ==> "BuildTests"
  ==> "RunTests"

// start build
RunTargetOrDefault "RunTests"
