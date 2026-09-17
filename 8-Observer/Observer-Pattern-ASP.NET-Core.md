# Observer Pattern in ASP.NET Core

## الفكرة ببساطة

**Observer Pattern** بيستخدم لما يكون عندك **حدث واحد (Event)**، وعايز أكتر من جزء في النظام يعرف إن الحدث حصل ويتصرف بناءً عليه.

مثال:

```text
Create Order
     ↓
Order Created Event
     ↓
 ┌──────────────┬──────────────┐
 ↓              ↓
Email          Logging
Observer       Observer
```

الـ Publisher لا يحتاج يعرف تفاصيل الـ Subscribers.

---

## إمتى أستخدمه؟

استخدم Observer لما يكون:

- عندك Event واحد وله أكثر من Reaction.
- عايز تقلل الـ Coupling بين الأجزاء.
- ممكن تضيف Subscribers جديدة في المستقبل.
- كل Subscriber عنده مسؤولية مستقلة.

مثال:

```text
User Registered
      ↓
 ├── Send Email
 ├── Send Notification
 ├── Create Audit Log
 └── Update Analytics
```

## إمتى أبعد عنه؟

ابعد عنه لو:

- الـ Flow بسيط ومباشر.
- عندك Subscriber واحد فقط ومفيش حاجة تستدعي Pattern.
- استخدامه هيخلي تتبع الـ Business Flow أصعب.
- أنت بتستخدمه لمجرد إن الكود شكله Advanced.

---

# ASP.NET Core Example

مثال بسيط بدون Database.

## 1. Event

```csharp
public record OrderCreatedEvent(int OrderId);
```

## 2. Observer Interface

```csharp
public interface IOrderObserver
{
    Task Handle(OrderCreatedEvent @event);
}
```

## 3. Email Observer

```csharp
public class EmailObserver : IOrderObserver
{
    public Task Handle(OrderCreatedEvent @event)
    {
        Console.WriteLine(
            $"Email sent for Order {@event.OrderId}");

        return Task.CompletedTask;
    }
}
```

## 4. Logging Observer

```csharp
public class LoggingObserver : IOrderObserver
{
    public Task Handle(OrderCreatedEvent @event)
    {
        Console.WriteLine(
            $"Order {@event.OrderId} created");

        return Task.CompletedTask;
    }
}
```

## 5. Publisher / Order Service

```csharp
public class OrderService
{
    private readonly IEnumerable<IOrderObserver> _observers;

    public OrderService(IEnumerable<IOrderObserver> observers)
    {
        _observers = observers;
    }

    public async Task CreateOrder(int orderId)
    {
        Console.WriteLine("Creating Order...");

        var @event = new OrderCreatedEvent(orderId);

        foreach (var observer in _observers)
        {
            await observer.Handle(@event);
        }
    }
}
```

## 6. Controller

```csharp
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Create(int id)
    {
        await _orderService.CreateOrder(id);

        return Ok("Order created");
    }
}
```

## 7. Program.cs

```csharp
builder.Services.AddScoped<IOrderObserver, EmailObserver>();
builder.Services.AddScoped<IOrderObserver, LoggingObserver>();

builder.Services.AddScoped<OrderService>();

builder.Services.AddControllers();
```

---

# Flow

عند تنفيذ:

```http
POST /api/orders/100
```

الـ flow يكون:

```text
OrdersController
       ↓
OrderService
       ↓
OrderCreatedEvent
       ↓
 ┌───────────────┐
 ↓               ↓
EmailObserver   LoggingObserver
```

والـ output:

```text
Creating Order...

Email sent for Order 100

Order 100 created
```

---

# أهم نقطة

`OrderService` لا يعرف أن عندنا:

- `EmailObserver`
- `LoggingObserver`

هو فقط يتعامل مع:

```csharp
IEnumerable<IOrderObserver>
```

وبالتالي لو أضفت:

```csharp
public class SmsObserver : IOrderObserver
{
    public Task Handle(OrderCreatedEvent @event)
    {
        Console.WriteLine(
            $"SMS sent for Order {@event.OrderId}");

        return Task.CompletedTask;
    }
}
```

تحتاج فقط إلى تسجيله:

```csharp
builder.Services.AddScoped<IOrderObserver, SmsObserver>();
```

بدون تعديل `OrderService`.

---

# Observer vs RabbitMQ

الـ Observer غالبًا بيكون **داخل نفس التطبيق**:

```text
Order API
   ↓
Event
 ├── Email Observer
 ├── Logging Observer
 └── Notification Observer
```

أما RabbitMQ فيستخدم غالبًا للتواصل بين Services:

```text
Order Service
      ↓
   RabbitMQ
      ↓
 ┌────┼─────┐
 ↓    ↓     ↓
Email SMS  Audit
Service Service Service
```

لذلك:

- **Observer** → مناسب للـ in-process notifications.
- **RabbitMQ** → مناسب للـ distributed/event-driven communication.

---

# Interview Answer

لو اتسألت: **What is Observer Pattern?**

ممكن تقول:

> Observer Pattern allows one object to notify multiple subscribers when an event occurs, without tightly coupling the publisher to the subscribers.

وفي ASP.NET Core ممكن تستخدم نفس الفكرة مع:

- .NET Events
- MediatR Notifications
- Domain Events

## الخلاصة

```text
One Event
    ↓
Multiple Observers
```

**المشكلة التي يحلها:**

> تقليل الـ coupling وجعل إضافة reactions جديدة للـ event أسهل بدون تعديل الـ publisher.
