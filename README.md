# DependencyInjection.AutoResolver

Automatically resolves dependency injection. Currently supports - Scoped, Transient, Singleton.

Instead of:
```
    services.AddScoped<IMyClass, MyClass>();
    services.AddTransient<IMyClass, MyClass>();
    services.AddSingleton<IMyClass, MyClass>();
```

Just add interface:
```
public class MyClass : IMyClass, IResolveAsScoped ...
public class MyClass : IMyClass, IResolveAsTransient ...
public class MyClass : IMyClass, IResolveAsSingleton ...
```

If Class  does not have Interface or it should be resolved without interface:

```
public class MyClass : IResolveAsSelf, IResolveAsScoped ...
```

It will be equal to:

```
    services.AddScoped<MyClass>();
```
