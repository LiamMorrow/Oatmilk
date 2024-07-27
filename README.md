<h2 align="center">🐶 .NET tests which are nice to read</h2>

> "Jest is great, let's bring it to .NET" - Liam Morrow

Detestable is a testing library for .NET which allows you to write declarative tests, free from annotations and long method names. It is heavily inspired by the [jest](https://github.com/jestjs/jest) testing framework in the JavaScript ecosystem.

Detestable currently supports running in a test project configured with [xunit](https://github.com/xunit/xunit). You can run your existing xunit `Facts` and `Theories` alongside `Detestable` tests.

## Getting Started

First in your test project, install the detestable package.

#### Dotnet CLI

```bash
dotnet add package Detestable.Xunit
```

#### Package Manager Console

```bash
Install-Package Detestable.Xunit
```

Then create a test class, and create your first `Detestable Test` by using the `Describe` attribute on a method. Be sure to include a static import of `Detestable.TestBuilder`.

```csharp
using Detestable;
using static Detestable.TestBuilder;

public class MyTestClass
{
    [Describe("My Test Suite")]
    public void Spec()
    {
        // Describe as many tests as you'd like
        It("Should pass", () => Assert.True(true));

        It("Is another test, wow!", () => Assert.Equal(1, 1));
    }
}

```

### Test setup and teardown

It is often useful to have setup and teardown methods that run around tests.
Detestable exposes these mechanisms through four methods:

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
        BeforeAll(()=>
        {
            // This will run once if any of the tests are run
        })

        BeforeEach(()=>
        {
            // This will run every single time a test is run
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

## Fluent Syntax

Many people use the [csharpier](https://github.com/belav/csharpier) formatter to format their code. Unfortunately, the way that it chops arguments produces somewhat less than nice to read Detestable test code, as it will put the description of the test on a new line:

```cs
It(
    "I am a test",
    () =>
    {
        // My test code
    }
)

```

For this reason, `Detestable` exposes a fluent API where the body of the test is supplied in a fluent manner, rather than as a second argument:

```cs
It("I am a test")
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

## Features

### Type Safe Test Enumeration

Providing tests with dynamic data has next to zero ceremony. There's no need to annotate with a method with `[Theory]`, or supply data through a different class / member with `[MemberData]`, which has many pitfalls and moves the information of the test far from it. Simply use an `It.Each` or a `Describe.Each` method call and provide the data directly.

```cs

It.Each(
    [1, 3, 5],
    val => $"{val} is odd" // Format string are supported as well: "{0} is odd",
    (value)=>
    {
        (value % 2).Should().Be(1);
    });
```

### Examples

See the [Detestable.Tests package](./Detestable.Tests/) for examples of how tests can be written. All tests for Detestable are written in it!
