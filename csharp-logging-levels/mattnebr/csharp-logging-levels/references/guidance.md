# ILogger Logging Level Guidance

Best practices for developers using `Microsoft.Extensions.Logging`. Optimized for production observability and local debugging. Audience: developers who search application logs in systems like Splunk.

---

## 1. Logging Philosophy

### 1.1 Purpose

- Logs exist primarily for **developers**.
- Logs support **local debugging**, **production observability**, and **post-incident analysis**.
- Logs are treated as **immutable audit trails** — append-only records that are never modified or deleted after writing. This refers to their diagnostic role in debugging and investigation, not to formal regulatory audit logging which may require additional guarantees around completeness, retention, and tamper-proofing.

### 1.2 Environment Posture

|Environment|Default Minimum Level|Intent|
|---|---|---|
|Production|Information|Informative enough for reconstruction, not noisy|
|Local / Development|Debug|Chatty, developer-focused|
|Deep Troubleshooting|Trace|Enabled temporarily in any environment|

### 1.3 Exception Philosophy

- Exceptions are the **most important log entries**.
- **Catch and re-throw (`throw;`)** → Error. The catching layer has the best context about the failure. Always include the exception object as the first parameter.
- **Catch and recover** → Debug or Trace. The code handled it gracefully.
- **Business rule failures** → Debug (expected behavior, not exceptional).
- **Transient failures with exhausted retries** → Warning (the only case where a handled failure escalates above Debug).

### 1.4 Logs vs. Metrics

Logs capture _context_ about individual events. Metrics capture _trends_ across many events. If the primary value of a data point is aggregation over time — request counts, latency percentiles, error rates — emit it as a metric rather than a log entry. Logging high-frequency data that exists only to be counted or averaged creates volume without value.

---

## 2. Logging Levels

Each log level indicates the severity and intended visibility of an event. Use the lowest level consistent with the intent.

### Trace (LogLevel 0) — Finest-Grained Diagnostic Data

Data that is only useful when stepping through logic at the lowest level. Trace logs are high-volume and should never appear in production unless temporarily enabled for targeted troubleshooting.

Use for:

- Individual iterations inside loops
- Repository-level operations (queries, cache lookups)
- SQL/ORM command details
- Low-level state transitions

### Debug (LogLevel 1) — Development Troubleshooting

Diagnostics that help a developer understand _why_ code followed a particular path. Debug logs are the default floor during local development.

Use for:

- Method entry/exit in non-trivial flows
- Branching decisions and conditional logic
- Cache hits/misses
- Handled exceptions where the code recovers (no `throw;`)
- Validation outcomes (pass or fail)
- Detailed service-level operations
- Per-batch summaries when processing collections

### Information (LogLevel 2) — Normal Application Flow Summary

High-level milestones that tell the story of what the application _did_. These are the logs a developer reads first when investigating production behavior. Every Information entry should represent a **milestone** — a discrete, meaningful event that a developer would want to see when reconstructing what happened in production.

Milestone categories:

- **Workflow bookends** — start and completion of an orchestrated operation (e.g., `"Starting workflow for order {OrderId}"`, `"Completed workflow for order {OrderId}"`)
- **State transitions** — a business entity moving to a new lifecycle state (e.g., `"Order {OrderId} transitioned to Shipped"`, `"Claim {ClaimId} approved"`)
- **Integration completions** — a significant external system interaction completed (e.g., `"Payment authorized for order {OrderId}"`, `"Email notification sent for {CustomerId}"`)
- **Progress checkpoints** — periodic markers during long-running operations (e.g., `"Batch {BatchId} progress: 500 of 1000 records processed (50%)"`)
- **Aggregate results** — final summary of a batch or collection operation (e.g., `"Processed 1000 records for batch {BatchId}, 8 failed"`)
- **Threshold crossings** — a system-level condition changing state (e.g., `"Circuit breaker opened for PaymentGateway"`, `"Queue depth exceeded {Threshold}"`)

If a log entry does not fit one of these categories, it likely belongs at Debug or Trace.

### Warning (LogLevel 3) — Unexpected but Recoverable

Something happened that was not part of the normal path, but the system continued operating. Warnings signal conditions that may need attention before they become errors.

Use for:

- Transient failures where retries are **exhausted** but the system degrades gracefully
- External service latency exceeding acceptable thresholds
- Partial failures where the system continues with reduced functionality
- Configuration values falling back to defaults unexpectedly

Do **not** use for:

- Business rule violations (use Debug)
- Validation failures (use Debug)
- Handled exceptions that are part of normal recovery (use Debug)

### Error (LogLevel 4) — Current Operation Failed

The current operation cannot complete. The system is still running, but something broke for this request or workflow.

Use for:

- Exceptions that prevent the operation from completing
- Failed external calls after all retry attempts
- Data inconsistencies that break the current workflow

Always include:

- The full exception via the `Exception` overload
- Contextual identifiers (entity ID, correlation ID, etc.)

### Critical (LogLevel 5) — Application or System Severely Impacted

The application itself is in jeopardy. Critical logs should be extremely rare and almost always trigger an alert.

Use for:

- Data loss or corruption
- Security breaches
- Application startup failures
- Unrecoverable system-wide outages

---

## 3. Layer-Specific Guidance

The Entity → Service → Repository pattern with an Orchestrator coordinating Services produces a natural logging hierarchy. Higher layers summarize; lower layers detail.

### Repository Layer

**Primary levels:** Trace, Debug **Occasionally:** Error (only in catch blocks that re-throw)

Repositories sit at the bottom of the stack. Their logs capture data-access mechanics, not business meaning.

- Query execution and parameters → Trace
- Cache operations → Debug
- Individual retry attempts → Trace
- Retry exhaustion with recovery → Debug
- Handled data-access exceptions → Debug

If a Repository catches an unrecoverable exception and re-throws it, it should log at Error with the exception object and contextual identifiers before re-throwing. If the Repository does not catch the exception at all, it propagates naturally and a higher layer will log it.

### Service Layer

**Primary levels:** Debug **Occasionally:** Trace (for granular internals), Error (in catch blocks that re-throw)

Services contain business logic and validation. Most of their logging is Debug-level diagnostic detail.

- Validation steps and outcomes → Debug
- Branching logic and decision points → Debug
- External API calls and retries → Debug
- Handled exceptions (catch and recover) → Debug

Services should **not** log at Information or above when called by an Orchestrator — the Orchestrator owns the workflow summary. The exception: when a Service is the topmost layer (no Orchestrator above it), it acts as the workflow boundary and should log at Information the same way an Orchestrator would. If a Service catches an exception it cannot recover from and re-throws it, it should log at Error with the exception object and contextual identifiers. The Single Responsibility rule (see Section 4) prevents higher layers from duplicating that Error entry.

### Orchestrator Layer

**Primary levels:** Information, Warning, Error, Critical **Occasionally:** Debug (for complex branching decisions within the Orchestrator itself)

Orchestrators own the narrative of the workflow. Their logs should read like a high-level story of what happened. Every Orchestrator workflow should log at least one Information entry at start and one at completion. This creates paired bookends that make it easy to detect incomplete workflows and measure duration. All Information entries should represent milestones (see milestone categories in Section 2).

- Workflow bookends (start and completion) → Information
- State transitions and integration completions → Information
- Unexpected recoverable conditions → Warning
- Operation-level failures → Error
- System-level failures → Critical

Orchestrators should **prefer** Information and above. If an Orchestrator needs Debug logging for its own internal logic, that is acceptable, but it should be the exception rather than the norm.

### Loops and High-Volume Paths

Loops are the most common source of log noise. Guard them carefully.

- Per-item detail inside a loop → Trace
- Per-batch summary after a loop completes → Debug
- Aggregate result of the entire operation → Information (Orchestrator only)
- Do **not** log Warning or Error inside a loop for routine per-item outcomes. The exception: if each item represents an independent operation and an item fails irrecoverably, that item's failure is an Error by the same blast-radius rule that applies outside loops.

### Partial Failures in Batch Operations

When processing a collection where some items succeed and others fail:

- **Per-item failures** inside the loop → Debug. Each individual failure is a handled outcome, not an unexpected condition.
- **Aggregate summary** after the loop → Information at the Orchestrator level (e.g., `"Processed 42 of 50 items for batch {BatchId}, 8 failed"`).
- **Warning** only if the failure rate or pattern is abnormal — for example, more than a configurable threshold of items failed, suggesting a systemic issue rather than individual bad data.

Do not log each per-item failure at Warning. A batch of 50 items producing 8 Warning entries creates noise and buries genuinely unexpected conditions.

### Progress Milestones for Long-Running Operations

For batch operations that process large collections, log progress checkpoints at Information on a fixed interval rather than per-item. This gives production visibility into long-running operations without generating a log entry for every record.

- **Per-item updates** → Trace or Debug
- **Progress checkpoints** at a fixed interval (e.g., every 10% or every N items) → Information
- **Final aggregate result** → Information

```csharp
public async Task<BatchResult> ProcessBatchAsync(IReadOnlyList<Record> records, string batchId)
{
    _logger.LogInformation("Starting batch {BatchId}, {TotalCount} records", batchId, records.Count);

    int successCount = 0;
    int failCount = 0;
    int checkpoint = Math.Max(records.Count / 10, 1);

    for (int i = 0; i < records.Count; i++)
    {
        _logger.LogTrace("Processing record {RecordId} in batch {BatchId}", records[i].Id, batchId);

        var success = await ProcessRecordAsync(records[i]);
        if (success) successCount++; else failCount++;

        if ((i + 1) % checkpoint == 0)
        {
            _logger.LogInformation(
                "Batch {BatchId} progress: {Processed} of {Total} records processed ({Percent}%)",
                batchId, i + 1, records.Count, (i + 1) * 100 / records.Count);
        }
    }

    _logger.LogInformation(
        "Completed batch {BatchId}: {SuccessCount} succeeded, {FailCount} failed of {TotalCount}",
        batchId, successCount, failCount, records.Count);

    return new BatchResult(successCount, failCount);
}
```

For small collections (under ~100 items), progress checkpoints are unnecessary — the start and completion bookends are sufficient.

### Special Contexts

Some components fall outside the Entity/Service/Repository/Orchestrator model. Map them to the closest equivalent.

- **Background jobs and scheduled tasks** behave as Orchestrators. Log workflow start, completion, and failures at the same levels an Orchestrator would.
- **Application startup and shutdown.** Successful startup and graceful shutdown are Information. A failure that prevents the application from starting is Critical.
- **Authentication and authorization failures.** A denied request is an expected business outcome, not an unexpected condition. Log at Debug or Information depending on whether the event is useful in production logs. Never include tokens, credentials, or secrets in the log entry.
- **Poison messages and dead-letter scenarios.** A message that repeatedly fails processing and is moved to a dead-letter queue is an Error — the operation failed for that message. The system continuing to process other messages does not downgrade the severity.

---

## 4. Exception Handling Rules

### Catch and Re-Throw (`throw;`)

When a catch block exists to add context before propagating the exception, log at **Error** (or **Critical** if system stability or data integrity is at risk). The catching layer has the best context about the failure — this is the authoritative Error entry.

```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to process order {OrderId}", orderId);
    throw;
}
```

Always include:

- The exception object as the first parameter
- Contextual identifiers via structured message templates

### Catch and Recover

When the code handles the exception and continues without re-throwing, log at **Debug** or **Trace**. These are not production-visible under normal configuration. The code recovered — this is not an Error.

```csharp
try
{
    await _apiClient.SendAsync(request);
}
catch (TimeoutException ex)
{
    _logger.LogDebug(ex, "External API timed out for {OrderId}, retrying", orderId);
    return await RetryAsync(request);
}
```

### Transient Failures with Exhausted Retries

Log at **Warning** — the system recovered, but the condition is noteworthy.

```csharp
catch (TimeoutException ex) when (attempt >= maxRetries)
{
    _logger.LogWarning(ex,
        "External API failed after {MaxRetries} attempts for {OrderId}, proceeding with fallback",
        maxRetries, orderId);
    return Fallback();
}
```

### Business Rule Violations

Log at **Debug**. A business rule returning a failure is expected behavior.

```csharp
if (!order.IsEligibleForDiscount)
{
    _logger.LogDebug("Order {OrderId} is not eligible for discount", order.Id);
    return Result.Failure("Not eligible");
}
```

### Single Responsibility for Exception Logging

An exception should be logged at Error by exactly one layer — the layer that catches and re-throws it. This prevents the classic "triple log" problem where a Repository, Service, and Orchestrator all log the same failure.

The rule: **The layer that catches an exception and re-throws it owns the Error entry. If a higher layer also catches the same exception, it should not log Error again.**

In practice:

- A **Repository** catches a transient database exception and retries → log each attempt at Trace, log exhaustion at Debug. If it catches an unrecoverable exception and re-throws → log at Error with context before `throw;`.
- A **Service** catches an exception it cannot handle and re-throws → log at Error with context before `throw;`.
- If both a Service and an Orchestrator have catch blocks for the same exception, only the **first** layer to catch and re-throw logs Error. The higher layer should either not catch it, or catch it without duplicating the Error entry.

---

## 5. Structured Logging Best Practices

### Use Message Templates, Not String Interpolation

```csharp
// Correct — structured, searchable in Splunk
_logger.LogInformation("Order {OrderId} processed in {ElapsedMs}ms", orderId, elapsed);

// Incorrect — loses structure
_logger.LogInformation($"Order {orderId} processed in {elapsed}ms");
```

### Use Consistent Message Template Conventions

Adopt a predictable verb tense so logs tell a time-ordered story and are easy to search.

- **Present tense** for in-progress actions: `"Processing order {OrderId}"`
- **Past tense** for completions and outcomes: `"Completed order {OrderId}"`, `"Validation failed for entity {EntityId}"`

This gives logs a natural rhythm: searching for "Processing order" finds in-flight entries, while "Completed order" finds results.

### Include Contextual Identifiers

Every log entry that participates in a traceable workflow should include at least one identifier that allows correlation. Common examples: `OrderId`, `CustomerId`, `CorrelationId`, `RequestId`.

### Use Scopes for Cross-Cutting Context

```csharp
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["CorrelationId"] = correlationId,
    ["OrderId"] = orderId
}))
{
    _logger.LogInformation("Starting order workflow");
    // All logs within this block inherit CorrelationId and OrderId
}
```

### Consider Source-Generated Logging for Hot Paths

For high-throughput code paths, the `[LoggerMessage]` source generator avoids boxing and allocation overhead.

```csharp
public static partial class LogMessages
{
    [LoggerMessage(Level = LogLevel.Trace, Message = "Fetching entity {EntityId}")]
    public static partial void FetchingEntity(this ILogger logger, int entityId);
}
```

This is optional and best suited for Repository and loop-heavy code where logging overhead matters.

---

## 6. Layer Examples

### Repository (Trace / Debug)

```csharp
public async Task<Entity?> GetByIdAsync(int id)
{
    _logger.LogTrace("Fetching entity {EntityId}", id);

    var entity = await _dbContext.Entities.FindAsync(id);

    _logger.LogDebug("Repository lookup for {EntityId}: {Found}", id, entity is not null);

    return entity;
}
```

### Service (Debug)

```csharp
public async Task<ValidationResult> ValidateAsync(Entity entity)
{
    _logger.LogDebug("Validating entity {EntityId}", entity.Id);

    if (!IsValid(entity))
    {
        _logger.LogDebug("Validation failed for entity {EntityId}", entity.Id);
        return ValidationResult.Failure();
    }

    _logger.LogDebug("Validation passed for entity {EntityId}", entity.Id);
    return ValidationResult.Success();
}
```

### Orchestrator (Information)

```csharp
public async Task<WorkflowResult> ExecuteWorkflowAsync(int orderId)
{
    _logger.LogInformation("Starting workflow for order {OrderId}", orderId);

    var validation = await _validationService.ValidateAsync(order);
    if (!validation.IsSuccess)
    {
        _logger.LogInformation(
            "Workflow ended early for order {OrderId}: validation did not pass", orderId);
        return WorkflowResult.Failure("Validation failed");
    }

    var result = await _processingService.ProcessAsync(order);

    _logger.LogInformation("Completed workflow for order {OrderId}", orderId);
    return result;
}
```

> Note: The validation failure above is logged at Information, not Warning, because a failed validation is an expected business outcome — not an unexpected condition.

---

## 7. Anti-Patterns

- **Sensitive data in logs.** Never log passwords, tokens, PII, connection strings, or secrets.
- **Information+ inside loops.** Routine per-item logs must be Trace or Debug. Error is acceptable only for irrecoverable independent item failures.
- **Validation failures as Warning.** Validation is expected logic; use Debug.
- **Handled exceptions as Error.** If the code caught the exception and recovered (no `throw;`), it is Debug — not Error.
- **Duplicate Error entries across layers.** An exception should be logged at Error by exactly one layer. If a lower layer already logged Error before re-throwing, higher layers should not log Error for the same exception again.
- **Catch-and-rethrow at Debug.** If a catch block re-throws with `throw;`, it must log at Error — not Debug. The catching layer has the best context about the failure.
- **Unstructured string interpolation.** Always use message templates for searchability.
- **Logging without context.** Every log should include at least one identifier relevant to the operation.
- **High-cardinality or free-form structured properties.** Avoid logging raw payloads, user agent strings, full URLs, or serialized objects as structured template parameters. These explode index cardinality in log aggregation systems and degrade search performance. Log a concise identifier or summary instead.

### Before and After

```csharp
// ❌ Bad — catch-and-recover logged at Error, string interpolation, no context
try
{
    await _paymentGateway.ChargeAsync(amount);
}
catch (TimeoutException ex)
{
    _logger.LogError($"Payment failed: {ex.Message}");
    return await FallbackChargeAsync(amount);
}
```

```csharp
// ✅ Good — catch-and-recover at Debug, message template, contextual identifiers
try
{
    await _paymentGateway.ChargeAsync(amount);
}
catch (TimeoutException ex)
{
    _logger.LogDebug(ex, "Payment gateway timed out for order {OrderId}, using fallback", orderId);
    return await FallbackChargeAsync(amount);
}
```

What changed: the level dropped from Error to Debug (the code recovered), string interpolation became a message template, the exception object is passed as a first-class parameter, and `OrderId` provides searchable context.

---

## 8. Frequently Asked Questions

**What counts as a milestone?** A milestone is a discrete, meaningful event worth seeing in production logs. Ask: "If I were investigating this workflow in Splunk, would I want to see this entry?" If the answer is yes, it is a milestone. If the entry only helps during local debugging, it belongs at Debug. The milestone categories in Section 2 (workflow bookends, state transitions, integration completions, progress checkpoints, aggregate results, threshold crossings) provide a concrete checklist.

**Should Orchestrators log every branch or only major milestones?** Only major milestones. Log workflow start, completion, and outcomes that change the workflow's path (e.g., early exit due to validation failure). Internal branching logic within the Orchestrator can be Debug if needed, but most branches do not warrant a log entry.

**Should Services log every external call or only failures?** Log the start of an external call at Debug only if the call is slow or unreliable enough that visibility matters during local debugging. Successful calls that complete quickly do not need their own log entry — the Orchestrator's workflow summary provides sufficient evidence that the call succeeded.

**Should Repository logs include SQL text, parameters, and execution time?** SQL text and parameters → Trace. Execution time → Trace, and useful for identifying slow queries during targeted troubleshooting. Do not log these at Debug or above — EF Core and other ORMs already emit their own diagnostics at lower levels.

**Should we log method entry and exit?** Only for non-trivial methods where the entry/exit context aids debugging — such as methods with complex branching, external calls, or long-running operations. Logging entry/exit for simple getters, mappers, or pass-through methods creates noise without value. Use Debug for entry/exit in Services and Trace in Repositories.

---

## 9. Decision Flow

When writing a log statement, ask in order:

1. **Is the application or system itself in jeopardy?** → Critical
2. **Is this a catch block that re-throws (`throw;`)?** → Error (include the exception object)
3. **Did something unexpected happen, but the system continued?** → Warning
4. **Am I in an Orchestrator (or equivalent) logging a milestone?** (workflow bookend, state transition, integration completion, progress checkpoint, aggregate result, threshold crossing) → Information
5. **Am I capturing a decision, outcome, or diagnostic detail in a Service?** → Debug
6. **Am I logging low-level or per-item data in a Repository or loop?** → Trace

If the answer to more than one question is yes, the _first_ match wins.

---

## 10. Quick Reference

|Concern|Level|
|---|---|
|Repository operations|Trace / Debug|
|Service logic and validation|Debug|
|Orchestrator workflow bookends|Information|
|State transitions and integration completions|Information|
|Progress checkpoints (long-running batches)|Information|
|Aggregate operation results|Information (Orchestrator)|
|Loop iterations|Trace|
|Batch summaries|Debug|
|Catch and re-throw (`throw;`)|Error|
|Catch and recover|Debug / Trace|
|Exhausted retries with fallback|Warning|
|Threshold crossings (circuit breakers, queue depth)|Warning or Information|
|System-level or app-level failure|Critical|
|Business rule violations|Debug|