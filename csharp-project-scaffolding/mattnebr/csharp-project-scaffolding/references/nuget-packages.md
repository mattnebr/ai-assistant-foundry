# NuGet Package Reference

## Packages Added to All Project Types

Always add the latest stable versions of:

- `StyleCop.Analyzers`
- `SonarAnalyzer.CSharp`
- `Microsoft.CodeAnalysis.Analyzers`
- `Microsoft.CodeAnalysis.NetAnalyzers`
- `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.DependencyInjection`
- `Microsoft.Extensions.Logging`

## Additional Packages by Project Type

### Console Application

In addition to the packages above:

- `Microsoft.Extensions.Hosting`

### MSTest Project

In addition to the base packages:

- `MSTest.TestAdapter`
- `MSTest.TestFramework`
- `Microsoft.NET.Test.Sdk`
- `Moq`

### xUnit Project

In addition to the base packages:

- `xunit`
- `xunit.runner.visualstudio`
- `Microsoft.NET.Test.Sdk`
- `Moq`

### NUnit Project

In addition to the base packages:

- `NUnit`
- `NUnit3TestAdapter`
- `Microsoft.NET.Test.Sdk`
- `Moq`

## Package Reference Format

Analyzer packages should use `PrivateAssets="all"` and `IncludeAssets` to prevent them leaking into consuming projects:

```xml
<ItemGroup>
  <PackageReference Include="StyleCop.Analyzers" Version="x.x.x">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>
  <PackageReference Include="SonarAnalyzer.CSharp" Version="x.x.x">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>
  <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="x.x.x">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>
  <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="x.x.x">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>
</ItemGroup>
```