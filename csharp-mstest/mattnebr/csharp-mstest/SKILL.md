---
name: csharp-mstest
description: Generate MSTest unit tests for C# classes following strict naming conventions, assertion patterns, and structural standards. Use this skill whenever asked to write, scaffold, or review C# unit tests.
scope: This skill applies only to MSTest unit tests for C# classes. It does not apply to integration tests, UI tests, or non-C# languages.
---
# C# MSTest Skill

Generate complete, compilable MSTest unit test classes for C# code. Every test class produced by this skill must follow the standards below exactly. When in doubt, prefer explicitness and consistency over brevity.

---

## Test Framework

- **Framework**: MSTest (`Microsoft.VisualStudio.TestTools.UnitTesting`)
- **Mocking Library**: Moq (use when dependencies need to be mocked)
- **Target**: Modern .NET (nullable reference types enabled, records supported)

---

## Project and File Conventions

Each .NET solution has two MSTest projects:

- **`{SolutionName}.Test.Unit`** — Unit tests only. Runs on every branch push. Must be fast and self-contained.
- **`{SolutionName}.Test.Integration`** — Integration tests only. Runs on pull requests. May take longer due to external dependencies.

This skill generates code for the `{SolutionName}.Test.Unit` project. Unit tests must never contain connection strings to non-in-memory databases or depend on external services. Those concerns belong in the integration test project.

Test file naming: `{ClassName}Tests.cs` (one test class per class under test)

### Namespace Derivation

The test class namespace must mirror the class-under-test namespace, prefixed with `{SolutionName}.Test.Unit`. This prevents namespace drift and keeps test files discoverable.

| Class Under Test Namespace | Test Class Namespace               |
| -------------------------- | ---------------------------------- |
| `Solution.Sdk.Saas`        | `Solution.Test.Unit.Sdk.Saas`      |
| `Solution.Net`             | `Solution.Test.Unit.Net`           |
| `MyApp.Services.Billing`   | `MyApp.Test.Unit.Services.Billing` |

---

## Test Method Naming

Use the Roy Osherove naming convention. Every test method name has exactly three sections separated by underscores:

```
{UnitOfWork}_{StateUnderTest}_{ExpectedBehavior}
```

### Rules

1. Use only alphanumeric characters (a-z, A-Z, 0-9) and underscores
2. All sections use PascalCase
3. The name must have exactly three underscore-separated sections
4. Do not include the class name in the test method name — the test class already identifies the class under test

### Examples

```
Constructor_NoParameters_InitializesWithDefaultValues
MakeRequestAsync_NullRequest_ThrowsArgumentNullException
Name_SetValidValue_ReturnsSetValue
Dispose_CalledTwice_DoesNotThrowException
ProcessAsync_CancellationRequested_ThrowsOperationCanceledException
```

```
// ❌ Wrong — class name does not belong in the test method name
MyClass_Constructor_NoParameters
MyClass_Name_SetValidValue_ReturnsSetValue
```

---

## Test Class Structure

Every generated test file must contain exactly one test class. Every file must include all of these structural elements in this order:

1. File-scoped namespace
2. Using statements
3. XML summary on the test class
4. `[TestClass]` attribute
5. `#region` directives organizing methods
6. Test methods and private helpers inside their respective regions

### Template

```csharp
namespace SolutionName.Test.Unit.SubNamespace;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SolutionName.SubNamespace;
using System;
using System.Threading;
using System.Threading.Tasks;
// Additional using statements as needed for the class under test

/// <summary>
/// Contains unit tests for verifying the behavior of the <see cref="MyClass"/> class.
/// </summary>
[TestClass]
public class MyClassTests
{
    #region Public Methods

    /// <summary>
    /// Unit test to verify that the constructor initializes with default values.
    /// </summary>
    [TestMethod]
    public void Constructor_NoParameters_InitializesWithDefaultValues()
    {
        // Arrange (Given)

        // Act (When)
        var instance = new MyClass();

        // Assert (Then)
        Assert.IsNotNull(
            instance,
            "Instance should be created successfully.");
    }

    #endregion Public Methods

    #region Private Methods

    // Helper methods go here

    #endregion Private Methods
}
```

---

## Test Method Body: Arrange / Act / Assert

Every test method uses the AAA pattern with `Given / When / Then` comment labels:

```csharp
[TestMethod]
public void Method_Scenario_ExpectedResult()
{
    // Arrange (Given)
    var input = "test";

    // Act (When)
    var result = instance.SomeMethod(input);

    // Assert (Then)
    Assert.AreEqual(
        expectedValue,
        result,
        "Detailed message explaining what should happen.");
}
```

All three comment lines (`// Arrange (Given)`, `// Act (When)`, `// Assert (Then)`) must be present in every test, even if a section is empty.

---

## XML Documentation

Every test method must have an XML summary that describes the behavior in plain English. Do not restate the method name — the summary should explain *why* the test matters, not parrot the naming segments.

```csharp
// ✅ Correct — describes behavior in plain English
/// <summary>
/// Unit test to verify that the constructor creates an instance with all properties
/// set to their default values when no parameters are provided.
/// </summary>
[TestMethod]
public void Constructor_NoParameters_InitializesWithDefaultValues()

// ❌ Wrong — restates the method name
/// <summary>
/// Constructor no parameters initializes with default values.
/// </summary>
[TestMethod]
public void Constructor_NoParameters_InitializesWithDefaultValues()
```

The test class itself must also have an XML summary:

```csharp
/// <summary>
/// Contains unit tests for verifying the behavior of the <see cref="ClassName"/> class.
/// </summary>
[TestClass]
public class ClassNameTests
```

---

## Assertion Rules

### Every Assert Must Have a Message

Every `Assert` call must include a descriptive string message as its last parameter. No exceptions.

```csharp
// ✅ Correct
Assert.AreEqual(
    expected,
    actual,
    "Property should return the value that was set.");

// ❌ Wrong — missing message
Assert.AreEqual(expected, actual);

// ❌ Wrong — inline lambda in message
Assert.AreEqual(expected, actual, () => $"Expected {expected} but got {actual}");

// ❌ Wrong — string interpolation in message
Assert.AreEqual(expected, actual, $"Expected {expected} but got {actual}");
```

### Multi-Line Assert Formatting

Format Assert calls with each parameter on its own line:

```csharp
Assert.AreEqual(
    expected,
    actual,
    "Message describing expected behavior.");
```

### Specialized Assert Methods (Avoid MSTEST0037)

Do NOT use `Assert.IsTrue()` or `Assert.IsFalse()` when a specialized assert exists. Using them triggers MSTEST0037 warnings.

| Condition                 | Correct Assert                                       | Wrong Assert                                            |
| ------------------------- | ---------------------------------------------------- | ------------------------------------------------------- |
| String contains substring | `StringAssert.Contains(actual, substring, msg)`      | `Assert.IsTrue(actual.Contains(substring), msg)`        |
| String starts with        | `StringAssert.StartsWith(actual, prefix, msg)`       | `Assert.IsTrue(actual.StartsWith(prefix), msg)`         |
| String ends with          | `StringAssert.EndsWith(actual, suffix, msg)`         | `Assert.IsTrue(actual.EndsWith(suffix), msg)`           |
| String matches regex      | `StringAssert.Matches(actual, pattern, msg)`         | `Assert.IsTrue(Regex.IsMatch(actual, pattern), msg)`    |
| Collection contains item  | `CollectionAssert.Contains(collection, item, msg)`   | `Assert.IsTrue(collection.Contains(item), msg)`         |
| Collection count          | `Assert.HasCount(collection, expectedCount, msg)`    | `Assert.AreEqual(expectedCount, collection.Count, msg)` |
| Collection subset         | `CollectionAssert.IsSubsetOf(subset, superset, msg)` | N/A                                                     |
| LINQ Any/All              | `Assert.IsTrue(col.Any(x => cond), msg)`             | (acceptable — no specialized alternative)               |

### Assert.IsTrue for LINQ Is Acceptable

When there is no specialized alternative (e.g., LINQ `.Any()` with a predicate), `Assert.IsTrue` is acceptable:

```csharp
Assert.IsTrue(
    collection.Any(x => x.Id == expectedId),
    "Collection should contain an item with the expected ID.");
```

Or restructure to avoid it:

```csharp
var match = collection.FirstOrDefault(x => x.Id == expectedId);
Assert.IsNotNull(
    match,
    "Collection should contain an item with the expected ID.");
```

---

## Exception Testing

**CRITICAL**: Do NOT use `Assert.ThrowsException<T>` or `Assert.ThrowsExceptionAsync<T>`. This project uses try/catch with a bool flag instead. This policy ensures consistent exception testing patterns across the codebase and allows richer assertion messages and property validation on caught exceptions.

Always use try/catch with a bool flag or captured exception variable.

### Synchronous Exception Test

```csharp
/// <summary>
/// Unit test to verify that the method throws ArgumentNullException when parameter is null.
/// </summary>
[TestMethod]
public void Method_NullParameter_ThrowsArgumentNullException()
{
    // Arrange (Given)
    object? nullParam = null;
    bool exceptionThrown = false;

    // Act (When)
    try
    {
        instance.SomeMethod(nullParam!);
    }
    catch (ArgumentNullException)
    {
        exceptionThrown = true;
    }

    // Assert (Then)
    Assert.IsTrue(
        exceptionThrown,
        "Method should throw ArgumentNullException when parameter is null.");
}
```

### Async Exception Test

```csharp
/// <summary>
/// Unit test to verify that the async method throws ArgumentNullException when parameter is null.
/// </summary>
[TestMethod]
public async Task MethodAsync_NullParameter_ThrowsArgumentNullException()
{
    // Arrange (Given)
    object? nullParam = null;
    bool exceptionThrown = false;

    // Act (When)
    try
    {
        await instance.SomeMethodAsync(nullParam!);
    }
    catch (ArgumentNullException)
    {
        exceptionThrown = true;
    }

    // Assert (Then)
    Assert.IsTrue(
        exceptionThrown,
        "MethodAsync should throw ArgumentNullException when parameter is null.");
}
```

### Exception Test with Parameter Validation

When you need to verify the exception's properties (e.g., `ParamName`), capture the exception instead of using a bool:

```csharp
/// <summary>
/// Unit test to verify that the method throws ArgumentNullException with correct parameter name.
/// </summary>
[TestMethod]
public void Method_NullParameter_ThrowsArgumentNullExceptionWithParamName()
{
    // Arrange (Given)
    object? nullParam = null;
    ArgumentNullException? caughtException = null;

    // Act (When)
    try
    {
        instance.SomeMethod(nullParam!);
    }
    catch (ArgumentNullException ex)
    {
        caughtException = ex;
    }

    // Assert (Then)
    Assert.IsNotNull(
        caughtException,
        "Method should throw ArgumentNullException when parameter is null.");
    Assert.AreEqual(
        "parameterName",
        caughtException.ParamName,
        "Exception should indicate the correct parameter name.");
}
```

### Exception Property Verification

When verifying exception details, compare only specific properties like `ParamName` or `HResult` — never assert against the full `Message` string. Exception messages are implementation details that vary across .NET versions and locales, making full-message assertions brittle.

---

## Async Test Methods

- All async test methods must return `Task`, never `void`
- Use `async Task` in the method signature
- Always pass and respect `CancellationToken` when the method under test accepts one

```csharp
/// <summary>
/// Unit test to verify that the async method completes successfully.
/// </summary>
[TestMethod]
public async Task ProcessAsync_ValidInput_CompletesSuccessfully()
{
    // Arrange (Given)
    var input = "test";

    // Act (When)
    var result = await instance.ProcessAsync(input, CancellationToken.None);

    // Assert (Then)
    Assert.IsNotNull(
        result,
        "Result should not be null for valid input.");
}
```

---

## Record Type Testing

When creating new instances of record types in tests, use **object initializer syntax**. Do NOT use `with` expressions. This keeps object creation explicit and readable in tests — every property value is visible at the point of construction, making it clear what the test is working with without needing to trace back to the original instance.

```csharp
// ✅ Correct — object initializer
var modified = new MyRecord
{
    Name = original.Name,
    Value = newValue,
};

// ❌ Wrong — do not use 'with'
var modified = original with { Value = newValue };
```

---

## Nullable Reference Types

Use nullable annotations in test code to match project settings:

```csharp
// Use nullable types when testing null scenarios
object? nullParam = null;
ArgumentNullException? caughtException = null;
string? result = null;
```

All test class fields that are initialized in `[TestInitialize]` must be declared as nullable. Do not use the `null!` suppression operator to fake non-null initialization:

```csharp
// ✅ Correct — nullable fields initialized in TestInitialize
private MyClass? _instance;
private Mock<ISomeService>? _mockService;

[TestInitialize]
public void TestInitialize()
{
    _mockService = new Mock<ISomeService>(MockBehavior.Strict);
    _instance = new MyClass(_mockService.Object);
}

// ❌ Wrong — null-forgiving operator hides null safety
private MyClass _instance = null!;
private Mock<ISomeService> _mockService = null!;
```

---

## Moq Usage

Use Moq when the class under test has dependencies that need to be mocked. Default to `MockBehavior.Strict` unless the class under test requires loose behavior. Strict mocks enforce better dependency contracts by throwing on any unexpected call, ensuring tests only pass when the class under test interacts with its dependencies exactly as expected.

```csharp
// ✅ Correct — strict mock, all expected calls must be set up
var mockService = new Mock<ISomeService>(MockBehavior.Strict);
mockService
    .Setup(s => s.GetDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync("result");

// ✅ Acceptable — loose mock when the class under test calls many
// methods on the dependency and only some are relevant to this test
var mockLogger = new Mock<ILogger<MyClass>>(MockBehavior.Loose);
```

```csharp
/// <summary>
/// Unit test to verify that the method calls the dependency.
/// </summary>
[TestMethod]
public async Task ProcessAsync_ValidInput_CallsDependency()
{
    // Arrange (Given)
    var mockService = new Mock<ISomeService>(MockBehavior.Strict);
    mockService
        .Setup(s => s.GetDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync("result");

    var instance = new MyClass(mockService.Object);

    // Act (When)
    await instance.ProcessAsync("input", CancellationToken.None);

    // Assert (Then)
    mockService.Verify(
        s => s.GetDataAsync("input", It.IsAny<CancellationToken>()),
        Times.Once,
        "ProcessAsync should call GetDataAsync exactly once with the provided input.");
}
```

---

## TestInitialize and TestCleanup

Use `[TestInitialize]` and `[TestCleanup]` when multiple tests share the same setup or teardown. Place them before the `#region Public Methods` block:

```csharp
[TestClass]
public class MyClassTests
{
    private MyClass? _instance;
    private Mock<ISomeService>? _mockService;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockService = new Mock<ISomeService>(MockBehavior.Strict);
        _instance = new MyClass(_mockService.Object);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // Dispose resources if needed
    }

    #region Public Methods

    /// <summary>
    /// Unit test to verify that the method returns expected value.
    /// </summary>
    [TestMethod]
    public void Method_ValidInput_ReturnsExpectedValue()
    {
        // Arrange (Given)
        string input = "test";

        // Act (When)
        var result = _instance.Method(input);

        // Assert (Then)
        Assert.AreEqual(
            "expected",
            result,
            "Method should return expected value for valid input.");
    }

    #endregion Public Methods
}
```

---

## this. Keyword

Always use `this.` when accessing instance members in the class under test. In test classes, `this.` is not required for test fields initialized via `[TestInitialize]`, but must be used if the test class itself has instance methods that reference its own members.

---

## Things to NEVER Do

These are hard rules. Violating any of them produces code that will not compile or will fail unexpectedly:

1. **NEVER** use `Assert.ThrowsException<T>` or `Assert.ThrowsExceptionAsync<T>` — this project uses try/catch for exception testing to allow richer assertions and property validation
2. **NEVER** use `with` expressions on records in test code — use object initializer syntax to keep all property values explicit and readable
3. **NEVER** use `Assert.IsTrue(x.Contains(...))` when `StringAssert.Contains` or `CollectionAssert.Contains` applies
4. **NEVER** use `Assert.AreEqual` for collection counts — use `Assert.HasCount`
5. **NEVER** omit the message parameter on any Assert call
6. **NEVER** omit the XML summary on a test method or test class
7. **NEVER** omit the `// Arrange (Given)`, `// Act (When)`, `// Assert (Then)` comments
8. **NEVER** use `async void` for async test methods — always use `async Task`
9. **NEVER** write a test method name that does not follow the three-section `{UnitOfWork}_{StateUnderTest}_{ExpectedBehavior}` pattern
10. **NEVER** include the class name in the test method name — the test class already identifies it
11. **NEVER** put more than one test class in a single file
12. **NEVER** write an XML summary that restates the method name — describe the behavior in plain English
13. **NEVER** assert against a full exception `Message` string — verify only specific properties like `ParamName` or `HResult`
14. **NEVER** use `null!` to initialize test class fields — declare them as nullable and initialize in `[TestInitialize]`
15. **NEVER** use inline lambdas or string interpolation inside Assert messages — use plain string literals only
16. **NEVER** use `SELECT *` or connect to external databases in unit tests

---

## Checklist Before Returning Generated Tests

Before presenting test code, verify every item:

- [ ] Namespace mirrors the class-under-test namespace, prefixed with `{SolutionName}.Test.Unit`
- [ ] File would be named `{ClassName}Tests.cs`
- [ ] File contains exactly one test class
- [ ] `[TestClass]` attribute present on the class
- [ ] XML summary on the test class with `<see cref="ClassName"/>`
- [ ] `#region Public Methods` and `#region Private Methods` present
- [ ] Every test method has `[TestMethod]` attribute
- [ ] Every test method has XML summary that describes behavior in plain English (not restating the method name)
- [ ] Every test method follows Roy Osherove naming
- [ ] Every test method has all three AAA comments
- [ ] Every Assert has a descriptive message using a plain string literal (no lambdas or interpolation)
- [ ] Specialized asserts used where applicable (no MSTEST0037)
- [ ] Exception tests use try/catch with bool flag (no Assert.ThrowsException)
- [ ] Exception assertions verify only specific properties (ParamName, HResult), never full Message strings
- [ ] Async tests return `Task`
- [ ] Records use object initializer syntax (no `with`)
- [ ] Moq used for dependencies where needed, defaulting to `MockBehavior.Strict`
- [ ] Test class fields initialized in `[TestInitialize]` are declared nullable (no `null!`)
- [ ] No external database connections
- [ ] Code compiles as written