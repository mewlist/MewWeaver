MewWeaver - IL Weaving Framework for Unity
===

Introduction
--

MewWeaver adds ability of IL Weaving into your project. 
It depends on [Mono.Cecil](https://www.mono-project.com/docs/tools+libraries/libraries/Mono.Cecil/) library.

By IL Weaving, for example, you can add the [MethodProfiling] attribute to measure the execution time of a function.
It is possible to easily add functions without breaking the structure of the entire program.

```csharp
class SomeClass
{
    [MethodProfiling]
    public void DoSomething()
    {
        // Do domething...
    }
}
```

In result on Log Console

```shell
[System.Void SomeClass::DoSomething()] 00:00:00.0000349
[System.Void SomeClass::DoSomething()] 00:00:00.0000002
[System.Void SomeClass::DoSomething()] 00:00:00.0000001
[System.Void SomeClass::DoSomething()] 00:00:00.0000001
[System.Void SomeClass::DoSomething()] 00:00:00.0000004
...
```

MewWeaver works on Unity2021.3 or later.

Install
--

Open Unity Package Manager.
Select "add package from git URL" and enter the following URL
```
https://github.com/mewlist/MewWeaver.git?path=Assets/MewWeaver
```

Samples
--

#### BeforeAfterWeaver

#### MethodProfilingWeaver


Tutorial
--

Some preparation is needed to weave.

* Custom Attribute
* Static method to inject
* Weaver class

### Custom Attribute

A typical Custom Attribute is like this.

```csharp
[AttributeUsage(AttributeTargets.Method)] 
public class BeforeAttribute : Attribute
{
    public BeforeAttribute() { } 
}
```

This attribute will put on target method.

```csharp

[Before]
void TargetMethod()
{
}
```

### Static method to inject

Static method is injected into method having Custom Attribute. 
If already have a static function, you can use it.

```csharp
public static class InjectedMethod
{
    public class Before
    {
        public static void Do()
        {
            Debug.Log($"Before Do");
        }
    }
}

```

### Create Weaver

To create new Weaver, define class inherited from **IWeaver** and implement **Weave()** method. This class must be placed under Editor folder.
Behavior described using a fluent interface, implement like this.

Editor/MyWeaver.cs
```csharp
public class MyWeaver : IWeaver
{
    public void Weave(AssemblyInjector assemblyInjector)
    {
        assemblyInjector
            .OnAssembly("Assembly-CSharp")
            .OnAttribute<BeforeAttribute>()
            .BeforeDo(Before.Do)
            .Inject();
    }
}
```

MewWeaver automatically find weavers and modify assemblies.




Injector methods
--

#### OnAssembly(assemblyName)

```csharp
.OnAssembly("Assembly-CSharp")
```

OnAssembly tells target assembly to weaver. Typically main assembly "Assembly-CSharp" will be specified.

#### OnMainAssembly()

Same as ```.OnAssembly("Assembly-CSharp")```

```csharp
.OnMainAssembly()
```

#### OnAttribute<T>()

```csharp
.OnAttribute<BeforeAttribute>()
```

Specify custom attribute. This attribute be searched as injection method by Weaver.

#### BeforeDo(staticMethod)

```csharp
.BeforeDo(Before.Do)
```

Set Static Method. This method will be injected into beginning of target function.

#### AfterDo(staticMethod)

```csharp
.AfterDo(After.Do)
```

Set Static Method. This method will be injected into end of target function.

#### Inject()

```csharp
.Inject()
```

When this method is called, AssemblyInjector injects code as declared.


Develop Custom IL Injector
--

Not yet documented.