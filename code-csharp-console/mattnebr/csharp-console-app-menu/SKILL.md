---
name: csharp-console-app-menu
description: 'Scaffold an interactive console menu for a .NET Generic Host BackgroundService (Worker). Use when creating or extending a console app that needs a numbered menu loop backed by a Dictionary<int, Func<CancellationToken, Task>>. Triggers include: "add a menu option", "create a Worker with a menu", "console app menu loop", "BackgroundService menu".'
---

# Console App Menu (BackgroundService Pattern)

## Pattern Overview

The menu is driven by a `Dictionary<int, Func<CancellationToken, Task>>` populated in the constructor. The `ExecuteAsync` loop renders the menu, reads a number, and dispatches to the matching method. `GetMenuLabel` auto-generates display text from PascalCase method names, stripping the `Async` suffix and prepending `Run`.

## Key Members

| Member | Purpose |
|---|---|
| `_menuActions` | `Dictionary<int, Func<CancellationToken, Task>>` — add one entry per menu item |
| `RenderMenu()` | Iterates `_menuActions` ordered by key; calls `GetMenuLabel` per entry |
| `GetMenuLabel(action)` | Strips `Async`, splits PascalCase, prepends `Run` |
| `ExecuteAsync` | The main loop: clear → wait → render → read → dispatch |
| `WaitAsync` | `Task.Delay(_delay, stoppingToken)` — 1-second pace buffer |

## Adding a New Menu Option

1. Register in constructor:
```csharp
this._menuActions = new Dictionary<int, Func<CancellationToken, Task>>
{
    { 1, this.GenerateFooAsync },
    { 2, this.LoadBarAsync },
    { 3, this.ExportBazAsync },
    { 4, this.ProcessFooAsync },  // <-- new entry
};
```

2. Implement the private method:
```csharp
private async Task ProcessFooAsync(CancellationToken stoppingToken)
{
    try
    {
        Console.WriteLine();
        Console.WriteLine("=== Process Foo ===");
        Console.WriteLine();

        var startTime = DateTime.UtcNow;

        // TODO: call orchestrator or service here
        await this._fooService.RunAsync(stoppingToken);

        var duration = DateTime.UtcNow - startTime;

        Console.WriteLine($"✓ Completed in {duration.TotalSeconds:F2} seconds");
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Process Foo failed: {ex.Message}");
        Console.WriteLine($"Error details: {ex}");
        Console.WriteLine();
    }

    await this.WaitAsync(stoppingToken);
}
```

## Boilerplate Worker Skeleton

```csharp
public class Worker : BackgroundService
{
    private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);
    private readonly IHostApplicationLifetime _lifetime;
    private readonly Dictionary<int, Func<CancellationToken, Task>> _menuActions;

    public Worker(IHostApplicationLifetime lifetime)
    {
        this._lifetime = lifetime;
        this._menuActions = new Dictionary<int, Func<CancellationToken, Task>>
        {
            { 1, this.GenerateFooAsync },
            { 2, this.LoadBarAsync },
            { 3, this.ExportBazAsync },
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.Clear();
            await this.WaitAsync(stoppingToken);
            this.RenderMenu();

            Console.Write("Enter the number: ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Invalid input. Press any key to try again...");
                Console.ReadKey();
                continue;
            }

            if (choice == 0)
            {
                Console.WriteLine("Shutting down...");
                await this.WaitAsync(stoppingToken);
                this._lifetime.StopApplication();
                return;
            }

            if (this._menuActions.TryGetValue(choice, out var action))
                await action(stoppingToken);
            else
                Console.WriteLine("Unknown option.");

            Console.WriteLine();
            Console.WriteLine("Press any key to pick another option...");
            Console.ReadKey();
            await Task.Delay(10, stoppingToken);
        }
    }

    private void RenderMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== CHOICES ====================================");
        Console.WriteLine();
        Console.WriteLine("Choose which service you want to run:");
        Console.WriteLine();

        foreach (var kvp in this._menuActions.OrderBy(k => k.Key))
            Console.WriteLine($"{kvp.Key}. {GetMenuLabel(kvp.Value)}");

        Console.WriteLine("0. Exit");
        Console.WriteLine();
    }

    private static string GetMenuLabel(Func<CancellationToken, Task> action)
    {
        string name = action.Method.Name;
        if (name.EndsWith("Async", StringComparison.OrdinalIgnoreCase))
            name = name[..^5];

        string spaced = System.Text.RegularExpressions.Regex
            .Replace(name, "(?<!^)([A-Z])", " $1");

        return spaced.StartsWith("Run ", StringComparison.OrdinalIgnoreCase)
            ? spaced
            : $"Run {spaced}";
    }

    private async Task WaitAsync(CancellationToken stoppingToken) =>
        await Task.Delay(this._delay, stoppingToken);

    // --- Menu action methods ---

    private async Task GenerateFooAsync(CancellationToken stoppingToken)
    {
        try
        {
            Console.WriteLine();
            Console.WriteLine("=== Generate Foo ===");
            Console.WriteLine();

            var startTime = DateTime.UtcNow;
            // TODO: implement
            var duration = DateTime.UtcNow - startTime;

            Console.WriteLine($"✓ Duration: {duration.TotalSeconds:F2} seconds");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Generate Foo failed: {ex.Message}");
            Console.WriteLine($"Error details: {ex}");
        }

        await this.WaitAsync(stoppingToken);
    }

    private async Task LoadBarAsync(CancellationToken stoppingToken)
    {
        try
        {
            Console.WriteLine();
            Console.WriteLine("=== Load Bar ===");
            Console.WriteLine();
            // TODO: implement
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Load Bar failed: {ex.Message}");
            Console.WriteLine($"Error details: {ex}");
        }

        await this.WaitAsync(stoppingToken);
    }

    private async Task ExportBazAsync(CancellationToken stoppingToken)
    {
        try
        {
            Console.WriteLine();
            Console.WriteLine("=== Export Baz ===");
            Console.WriteLine();
            // TODO: implement
            Console.WriteLine("✓ Export completed.");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Export Baz failed: {ex.Message}");
            Console.WriteLine($"Error details: {ex}");
        }

        await this.WaitAsync(stoppingToken);
    }
}
```

## Conventions

- Every menu action method must match the signature `Func<CancellationToken, Task>`.
- Wrap the body in `try/catch` and print `ex.Message` + `ex` for full stack trace.
- End every action with `await this.WaitAsync(stoppingToken)`.
- Use `=== Section Header ===` for Console section titles.
- Use `✓` prefix for success output lines.
- Display timing with `$"{duration.TotalSeconds:F2} seconds"`.
