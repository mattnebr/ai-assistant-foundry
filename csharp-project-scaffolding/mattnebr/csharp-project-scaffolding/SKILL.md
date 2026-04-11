---
name: csharp-project-scaffolding
description: 'Scaffolds new C# projects (.csproj) with correct structure, NuGet packages, property settings, and solution integration. Use when asked to create a new C# project, add a project to a Visual Studio solution, scaffold a class library, test project, console application, or any other .NET project type. Triggers include: "create a new project", "add a csproj", "scaffold a class library", "set up a test project", "new console app".'
---

# C# Project Scaffolding

Scaffold new C# projects following established conventions. Always complete the pre-flight checklist before creating any files.

## Pre-Flight Questions (ask all at once)

Before creating anything, collect:

1. **Project name** — full name (e.g., `Foo.Database.User`)
2. **Project type** — class library, console application, test project, or other
   - If test project: which framework?
     - **1. MSTest** (default)
     - **2. xUnit**
     - **3. NUnit**
3. **Target runtime** — which .NET version? (e.g., `net9.0`, `net8.0`)
4. **Add to a Visual Studio solution?** — if yes, which `.sln` file?

## Project Structure Rules

- **No example files** — delete `Class1.cs`, `Program.cs` stubs, or any auto-generated placeholder files (console apps are the exception — see below)
- **No `.editorconfig`** — managed at the solution level
- **Blank project** — only the `.csproj` and required source files

## Standard `.csproj` PropertyGroup

Apply to all project types:

```xml
<PropertyGroup>
  <OutputType>Library</OutputType> <!-- or Exe for console -->
  <TargetFramework>{{TargetFramework}}</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

For console apps, set `<OutputType>Exe</OutputType>`.

## NuGet Packages

See `references/nuget-packages.md` for the full package list by project type. Always use the latest stable version of each package. Analyzer packages (`StyleCop.Analyzers`, `SonarAnalyzer.CSharp`, `Microsoft.CodeAnalysis.Analyzers`, `Microsoft.CodeAnalysis.NetAnalyzers`) must use `PrivateAssets="all"`.

## InternalsVisibleTo (Non-Test Projects Only)

For non-test projects, scan the `.sln` file to find all projects whose names end in `.Test.Unit` or `.Test.Integration`. Add an `<ItemGroup>` for each found, plus the Moq dynamic proxy entry:

```xml
<ItemGroup>
  <InternalsVisibleTo Include="{{ProjectName}}.Test.Unit" />
  <InternalsVisibleTo Include="{{ProjectName}}.Test.Integration" />
  <InternalsVisibleTo Include="DynamicProxyGenAssembly2" />
</ItemGroup>
```

If no test projects are found in the solution, still add the conventionally named entries above based on the new project's name — they may be created later.

## Console Application Files

For console apps, create two source files from the templates in `assets/`:

- `Program.cs` — copy from `assets/Program.cs`, replacing `{{RootNamespace}}` with the project name
- `Worker.cs` — copy from `assets/Worker.cs`, replacing `{{RootNamespace}}` with the project name

## Adding to a Visual Studio Solution

If the user confirms the project should be added to a `.sln`:

```bash
dotnet sln {{SolutionFile}} add {{ProjectFile}}
```

## Build Verification (Required)

After scaffolding, build the project and confirm zero errors and zero warnings:

```bash
dotnet build {{ProjectFile}} --configuration Release
```

- If there are **errors**: fix them before finishing
- If there are **warnings**: fix all warnings before finishing — do not leave the project with any unresolved compile warnings
- Confirm success output to the user
