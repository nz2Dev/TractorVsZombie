---
description: Code Style Guide
---
// turbo-all
Follow these rules for all C# code in this project:

### 1. Conditional Statements
- One-line conditional statements and their bodies MUST be on separate lines.
- DO NOT use curly braces `{}` for single-line bodies of `if`, `foreach`, `while`, etc.

```csharp
// CORRECT
if (condition)
    ExecuteAction();

// INCORRECT
if (condition) { ExecuteAction(); }

// INCORRECT
if (condition) ExecuteAction();
```

### 2. Safety Checks
- DO NOT use safety checks like `TryGetKey` or null checks if the logic assumes the key/object must exist.
- Access dictionary elements directly by key: `registry[id]`.

### 3. Model Pattern
- Constructors should ONLY take immutable fields (e.g., ID, Config).
- Mutable fields (Position, Health, State IDs) should be assigned via properties after instantiation.

```csharp
// CORRECT
var model = new EntityModel(id, config);
model.Position = position;
model.Health = config.maxHealth;
```

### 4. Expression Bodies
- Use expression bodies `=>` for simple one-line properties and methods when appropriate.

### 5. Comments and Logging
- DO NOT write comments or logs that explain what the code does or notify what happened.
- DO NOT use `Debug.Log` for status notifications (e.g., "Building destroyed").
- ONLY write abstraction comments on class or method level if the abstraction is not obvious from the signature or the concept is complex.
