<p align="center"><a href="https://www.nuget.org/packages/Oatmilk/"><img src="https://img.shields.io/nuget/v/Oatmilk?style=flat&label=Oatmilk" alt="NuGet Version"></a>
<a href="https://www.nuget.org/packages/Oatmilk.Nunit/"><img alt="NuGet Version" src="https://img.shields.io/nuget/v/Oatmilk.Nunit?style=flat&label=Oatmilk.Nunit"></a>
<a href="https://www.nuget.org/packages/Oatmilk.Xunit/"><img alt="NuGet Version" src="https://img.shields.io/nuget/v/Oatmilk.Xunit?style=flat&label=Oatmilk.Xunit"></a>
<a href="https://www.nuget.org/packages/Oatmilk.MSTest/"><img alt="NuGet Version" src="https://img.shields.io/nuget/v/Oatmilk.MSTest?style=flat&label=Oatmilk.MSTest"></a>
<a href="https://opensource.org/licenses/MIT" rel="nofollow"><img alt="License: MIT" src="https://img.shields.io/badge/License-MIT-yellow.svg"></a>
</p>
<p align="center"><a href="https://github.com/LiamMorrow/Oatmilk/actions"><img src="https://img.shields.io/github/actions/workflow/status/LiamMorrow/Oatmilk/build.yml" alt="GitHub Actions Workflow Status"></a>
<a href="https://codecov.io/github/LiamMorrow/Oatmilk"><img alt="codecov" src="https://codecov.io/github/LiamMorrow/Oatmilk/graph/badge.svg?token=5UVDXIJVGV"></a></p>
<!-- A spacer -->
<p>&nbsp;</p>

<p align="center"><img alt="Oatmilk Mascot - Oatie" src="./Assets/Oatie.png" width=256/></p>

<h2 align="center">🥛 Refreshing .NET Testing</h2>

> "Jest is great, let's bring it to .NET" - Me

Oatmilk is a testing library for .NET which allows you to write declarative tests, free from annotations and long method names. It is heavily inspired by the [jest](https://github.com/jestjs/jest) and [mocha](https://github.com/mochajs/mocha) testing frameworks in the JavaScript ecosystem.

Oatmilk currently supports running in a test project configured with [xunit](https://github.com/xunit/xunit) or [nunit](https://https://nunit.org/), with limited support for [MSTest](https://github.com/microsoft/testfx/tree/main). You can run your existing xunit/nunit `Facts` and `Tests` alongside `Oatmilk` tests, so you don't need to convert your entire test suite to Oatmilk all at once.

Note that Oatmilk does not intend to be a full test framework, as such, things like mocking and asserting are out of scope. The assertions provided by Xunit/Nunit/MsTest are entirely compatible with Oatmilk. There are also many other great tools for the job. Check out [NSubstitute](https://nsubstitute.github.io/) for your mocking needs.

## Getting Started

First in your test project, install the appropriate Oatmilk package.

#### Dotnet CLI

```bash
dotnet add package Oatmilk.Nunit
# OR dotnet add package Oatmilk.Xunit
# OR dotnet add package Oatmilk.MSTest
```

Then create a test class, and create your first `Oatmilk Test` by using the `Describe` attribute on a method. Be sure to include a static import of `Oatmilk.TestBuilder`. If using MSTest, your test class will still require a `[TestClass]` attribute.

```csharp
using Oatmilk;
using static Oatmilk.TestBuilder;

// [TestClass] if using MSTest
public class MyTestClass
{
    [Describe("My Test Suite")]
    public void Spec()
    {
        // Describe as many tests as you'd like
        It("Should pass", () => Assert.True(true));

        It("Is another test, wow!", () => Assert.Equal(1, 1));
    }

    [Fact]
    public void ExistingXunitTest()
    {
        // You don't need to throw away your existing tests - they can live alongside Oatmilk tests :)
    }
}

```

Your tests should now show up in your IDE, and can be run with:

```bash
dotnet test
```

You can run specific tests by filtering:

```bash
dotnet test --filter "Name~My Test Suite"
```

### Test setup and teardown

It is often useful to have setup and teardown methods that run around tests.
Oatmilk exposes these mechanisms through four methods:

- `BeforeAll` - Runs ONCE before any test has run in its scope and child scopes
- `BeforeEach` - Runs before EVERY test in its scope and child scopes
- `AfterEach` - Runs after EVERY test in its scope and child scopes
- `AfterAll` - Runs ONCE after all tests have run in its scope and child scopes

Each of these mechanisms support async operations.

Scopes are defined by `Describe` blocks and can be nested.

Example:

```cs
public class MyTestClass
{
    [Describe("My Test Suite")]
    public void Spec()
    {
        Guid aGuidThatIsUniqueForEachTest;

        BeforeAll(()=>
        {
            // This will run once if any of the tests are run
        })

        BeforeEach(()=>
        {
            // This will run every single time a test is run
            // You can put initialization code here
            aGuidThatIsUniqueForEachTest = Guid.NewGuid();
        })

        It("Is a test in the top scope", () =>
        {
            // At this stage, the BeforeAll, and BeforeEach defined above will have run for this test
        })

        AfterEach(ctx =>{
            // This will run after every single test in this scope and child scopes
            // ctx contains information about the test which just ran (did it pass?)
        })

        Describe("A nested scope", () =>
        {
            BeforeEach(()=>{
                // This will only run for test which are declared in this scope, or any scopes declared WITHIN this scope
            })

            It("Is a nested test", () => {
                // At this stage:
                // The first BeforeAll will have only run ONCE - even if we are running both tests
                // The BeforeEach in the parent scope ran for this test
                // The BeforeEach in this scope ran for this test
            })
        })

    }
}

```

## Features

### Type Safe Test Enumeration

Providing tests with dynamic data has next to zero ceremony. There's no need to annotate with a method with `[Theory]`, or supply data through a different class / member with `[MemberData]`, which has many pitfalls and moves the information of the test far from it. Simply use an `It.Each` or a `Describe.Each` method call and provide the data directly.

```cs

It.Each(
    [1, 3, 5],
    val => $"Asserts that {val} is odd" // Format string are supported as well: "{0} is odd",
    (value)=>
    {
        (value % 2).Should().Be(1);
    });
```

### Easily Skip Tests

Got a test you'd like to skip for the time being? Simply add a `.Skip` to the test or describe block and it will be skipped.

```cs

It.Skip("should be skipped", () => Assert.True(false));

Describe.Skip("every test in this scope will be skipped", () =>
{
    It("will be skipped", () => Assert.True(false));
})

```

### Isolate Tests With Only

Focusing on a single test in a spec? Simply use `It.Only` or `Describe.Only` and Oatmilk will skip every other test in that spec.

```cs

It.Only("will run", () => Assert.True(true));

It("will not run since another test uses It.Only", () => Assert.True(false));

```

```cs
Describe.Only("A situation where all other tests will be skipped", () =>
{
    It("will run", () => Assert.True(true));
})

It("will not run because the describe block above uses .Only", () => Assert.True(false));

```

### Fluent Syntax

Many people use the [csharpier](https://github.com/belav/csharpier) formatter to format their code. Unfortunately, the way that it chops arguments produces somewhat less than nice to read Oatmilk test code, as it will put the description of the test on a new line:

```cs
It(
    "is a test",
    () =>
    {
        // My test code
    }
)

```

For this reason, `Oatmilk` exposes a fluent API where the body of the test is supplied in a fluent manner, rather than as a second argument:

```cs
It("is a test")
    .When(() =>
    {
        // My test code
    });
```

This is purely stylistic. The two methods are functionally identical. If you don't like the name "When", simply create an extension method on the `ItBlock` and `DescribeBlock` classes with your chosen name. `Describe` has a similar approach, however it uses the method `As`, rather than `When`:

```cs
Describe("My test suite")
    .As(() =>
    {
        // My describe block
    })
```

### IDE Feature Matrix

Each test runner exposes different level of support for Oatmilk. All of them support running the entire suite of tests.

| Feature                                            | Nunit | Xunit | MSTest |
| -------------------------------------------------- | ----- | ----- | ------ |
| Running all tests via `dotnet test`                | ✅    | ✅    | ✅     |
| Running individual test via `dotnet test --filter` | ✅    | ✅    | ✅     |
| Debugging entire test suite via IDE                | ✅    | ✅    | ✅     |
| Debugging single test via IDE                      | ✅    | ✅    | ❌     |
| Run individual tests via IDE                       | ✅    | ✅    | ❌     |
| Jump to `It` block via IDE                         | ✅    | ✅    | ❌     |
| Run/Debug specific `Describe` block via IDE        | ✅    | ❌    | ❌     |
| View tests as hierarchy in IDE                     | ✅    | ❌    | ❌     |
| Jump to nested `Describe` block via IDE            | ❌    | ❌    | ❌     |

## Examples

See the [Oatmilk.Tests packages](./test/) for examples of how tests can be written. All tests for Oatmilk are written in it!
