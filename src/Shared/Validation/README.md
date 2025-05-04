# Validation Behaviors

## ✨ Summary
This is FluentValidation + MediatR pipeline integration, cleanly implemented to:

- Auto-validate requests before handling them

- Log everything for observability

- Work with both regular and streaming MediatR requests

- Keep your handlers minimal and focused (no validation clutter inside handlers)

## 📦 Context
This folder contains `MediatR` pipeline behaviors for automatic request validation using `FluentValidation`.
They are cross-cutting concerns that run before and after handling a request.
Here, the concern is validation — validating requests automatically before executing handlers.

## What is it? 🧐

These behaviors automatically validate requests before they reach their handlers. This ensures that:

- Handlers remain clean and focused (no manual validation code)
- Validation logic is centralized and consistent
- Invalid requests short-circuit the pipeline with clear error messages

## How does it work?

- If a `FluentValidation` validator exists for a request, it will be invoked automatically.
- If validation passes, the request handler executes normally.
- If validation fails, an exception is thrown (handled globally).

## Usage

1. Ensure validators are registered in DI (`FluentValidation`)
2. Register the pipeline behavior in `MediatR`

```csharp
services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
services.AddScoped(typeof(IStreamPipelineBehavior<,>), typeof(StreamRequestValidationBehavior<,>));
```

## Related
- [FluentValidation](https://docs.fluentvalidation.net/en/latest/)
- [MediatR Pipeline Behaviors](https://github.com/jbogard/MediatR/wiki/Behaviors)
- [Request Validation Behaviour](RequestValidationBehavior.cs)

