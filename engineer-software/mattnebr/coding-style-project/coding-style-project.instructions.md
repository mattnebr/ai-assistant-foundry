---
applyTo: "**/*.csproj"
---
# Project File Coding Style

## General Guidelines
These are the default settings for each project (.csproj) under a Visual Studio solution.

### Treat Warnings as Error
In C#, the _TreatWarningsAsErrors_ option should be set to true to treat all compiler warnings as errors.

```
<PropertyGroup>
 <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

### Default NuGet Packages

These packages should be installed by default in all new projects:

- [Microsoft.CodeAnalysis.Analyzers](https://www.nuget.org/packages/Microsoft.CodeAnalysis.Analyzers/)
- [Microsoft.CodeAnalysis.NetAnalyzers](https://www.nuget.org/packages/Microsoft.CodeAnalysis.NetAnalyzers)
- [Microsoft.VisualStudio.Threading.Analyzers](https://www.nuget.org/packages/microsoft.visualstudio.threading.analyzers) (optional, recommended when async & await are used)
- [SonarAnalyzer.CSharp](https://www.nuget.org/packages/SonarAnalyzer.CSharp/)
- [StyleCop.Analyzers](https://www.nuget.org/packages/StyleCop.Analyzers/)

> For rationale and configuration tips, see [Matt's article on Roslyn analyzers](https://medium.com/@matthjo/which-roslyn-analyzers-to-use-within-net-for-code-analysis-9a9f71a65e74).

## Naming Conventions
- Use `PascalCase` for solution name.
- The name of the solution is normally `{ShortSolutionName}.{ProjectName}

# References
- Context by Microsoft for [What are solutions and projects in Visual Studio?](](https://learn.microsoft.com/en-us/visualstudio/ide/solutions-and-projects-in-visual-studio?view=vs-2022))
