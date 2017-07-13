# SharpInjector

Micro DI for netstandard

## How to Use

Install the [SharpInjector package](https://www.nuget.org/packages/SharpInjector/) via Nuget:

```powershell
Install-Package SharpInjector
```


## Registering a singleton

```cs
ServiceLocator.Default.Register<IFoo>(new Foo());
```

## Registering a transient

```cs
ServiceLocator.Default.Register<IFoo>(() => new Foo());
```

## Concrete classes

You don't need to register concrete classes, they will be instanciated as transient if not found.

```cs
var foo = ServiceLocator.Default.Resolve<Foo>();
```

## Overriding registered types

If you try to register a type twice, an exception is thrown. If you don't want this behavior but a registration replacement instead, just set the last paramenter to true:

```cs
ServiceLocator.Default.Register<IFoo>(new Foo());
ServiceLocator.Default.Register<IFoo>(new Foo2()); //throws DuplicateRegistrationException
```

But...

```cs
ServiceLocator.Default.Register<IFoo>(new Foo());
ServiceLocator.Default.Register<IFoo>(new Foo2(), true); //replace the registration
```

Enjoy!




