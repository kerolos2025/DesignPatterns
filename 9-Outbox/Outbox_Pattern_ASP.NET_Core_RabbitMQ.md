# Outbox Pattern with ASP.NET Core API + RabbitMQ

## 1. الفكرة العامة

الـ **Outbox Pattern** بيحل مشكلة الـ **Dual Write** لما الـ application
محتاجة تعمل:

1.  Save للـ Database.
2.  Publish Event إلى Message Broker مثل RabbitMQ.

بدون Outbox ممكن يحصل:

``` text
Database       ✅
RabbitMQ       ❌
```

فيكون الـ business data اتخزنت، لكن الـ event ماوصلش لباقي الـ services.

الحل هو تخزين الـ business data والـ event في نفس Database Transaction:

``` text
                DB Transaction
                     |
          ┌──────────┴──────────┐
          |                     |
       Orders              OutboxMessages
          |                     |
          └──────────┬──────────┘
                     |
                   Commit
                     |
                     v
              Background Worker
                     |
                     v
                 RabbitMQ
```

------------------------------------------------------------------------

# 2. الـ Architecture

في مثالنا عندنا:

``` text
ASP.NET Core API
       |
       v
   SQL Server
       |
       +---- Orders
       |
       +---- OutboxMessages
                    |
                    v
              Outbox Worker
                    |
                    v
                RabbitMQ
```

الـ API **لا تبعت RabbitMQ مباشرة** أثناء إنشاء الـ Order.

بدل ذلك:

``` text
Create Order
    ↓
Create Outbox Message
    ↓
Save both in same transaction
    ↓
Background Worker publishes message
```

------------------------------------------------------------------------

# 3. تشغيل RabbitMQ باستخدام Docker

استخدم `rabbitmq:4-management`:

``` yaml
services:

  rabbitmq:
    image: rabbitmq:4-management
    container_name: outbox-rabbitmq
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
```

شغل container:

``` powershell
docker compose up -d
```

تأكد:

``` powershell
docker ps
```

RabbitMQ Management UI:

``` text
http://localhost:15672
```

Login:

``` text
Username: guest
Password: guest
```

------------------------------------------------------------------------

# 4. أهم نقطة: Exchange مش Queue

في RabbitMQ فيه فرق مهم:

``` text
Producer
   |
   v
Exchange
   |
   v
Queue
   |
   v
Consumer
```

الـ Producer عادةً بيعمل Publish إلى **Exchange**.

الـ Exchange يقرر الرسالة تروح لأي Queue حسب الـ routing rules.

في مثالنا:

``` text
Exchange: orders
Routing Key: OrderCreated
Queue: order-created
```

------------------------------------------------------------------------

# 5. لماذا لم تظهر Queue في البداية؟

في النسخة الأولى من الكود كنا عاملين:

``` csharp
await channel.ExchangeDeclareAsync(
    exchange: "orders",
    type: ExchangeType.Direct,
    durable: true);
```

وبعدين:

``` csharp
await channel.BasicPublishAsync(
    exchange: "orders",
    routingKey: messageType,
    body: body);
```

لكن لم نعمل:

``` text
Queue
```

ولم نعمل:

``` text
Queue Binding
```

لذلك كانت الرسالة تُرسل إلى الـ Exchange، لكن لا توجد Queue مرتبطة بها
لاستقبالها.

**Exchange لا يحتفظ بالرسائل لمجرد أنه موجود.**

إذا لم توجد Queue مناسبة ومربوطة بالـ Exchange وقت الـ publish، يمكن أن
تضيع الرسالة.

------------------------------------------------------------------------

# 6. إنشاء Queue وربطها بالـ Exchange

الكود الصحيح للتجربة:

``` csharp
await channel.ExchangeDeclareAsync(
    exchange: "orders",
    type: ExchangeType.Direct,
    durable: true);

await channel.QueueDeclareAsync(
    queue: "order-created",
    durable: true,
    exclusive: false,
    autoDelete: false);

await channel.QueueBindAsync(
    queue: "order-created",
    exchange: "orders",
    routingKey: messageType);

var body = Encoding.UTF8.GetBytes(payload);

await channel.BasicPublishAsync(
    exchange: "orders",
    routingKey: messageType,
    body: body);
```

الـ flow أصبح:

``` text
Outbox Worker
      |
      v
Exchange: orders
      |
      | routing key = OrderCreated
      v
Queue: order-created
```

------------------------------------------------------------------------

# 7. Order Model

مثال بسيط:

``` csharp
public class Order
{
    public Guid Id { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }
}
```

------------------------------------------------------------------------

# 8. OutboxMessage Model

``` csharp
public class OutboxMessage
{
    public Guid Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public int RetryCount { get; set; }

    public string? Error { get; set; }
}
```

أهم الحقول:

  Field           Purpose
  --------------- -------------------------------
  `Id`            Message ID
  `Type`          Event type مثل `OrderCreated`
  `Payload`       الـ Event كـ JSON
  `CreatedAt`     وقت إنشاء الرسالة
  `ProcessedAt`   وقت نجاح الإرسال
  `RetryCount`    عدد محاولات الإرسال
  `Error`         آخر error حصل

------------------------------------------------------------------------

# 9. OrderCreated Event

``` csharp
public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}
```

------------------------------------------------------------------------

# 10. إنشاء Order + Outbox في نفس Transaction

مثال:

``` csharp
await using var transaction =
    await _db.Database.BeginTransactionAsync();

var order = new Order
{
    Id = Guid.NewGuid(),
    ProductName = productName,
    Amount = amount,
    CreatedAt = DateTime.UtcNow
};

_db.Orders.Add(order);

var orderCreatedEvent = new OrderCreatedEvent
{
    OrderId = order.Id,
    ProductName = order.ProductName,
    Amount = order.Amount
};

var outboxMessage = new OutboxMessage
{
    Id = Guid.NewGuid(),
    Type = nameof(OrderCreatedEvent),
    Payload = JsonSerializer.Serialize(orderCreatedEvent),
    CreatedAt = DateTime.UtcNow
};

_db.OutboxMessages.Add(outboxMessage);

await _db.SaveChangesAsync();

await transaction.CommitAsync();
```

المهم هنا:

``` text
Transaction
│
├── INSERT Order
│
├── INSERT OutboxMessage
│
└── COMMIT
```

لو transaction فشلت:

``` text
Order             ❌
Outbox Message    ❌
```

ولو نجحت:

``` text
Order             ✅
Outbox Message    ✅
```

------------------------------------------------------------------------

# 11. Outbox Worker

الـ Worker يبحث عن الرسائل التي:

``` text
ProcessedAt == NULL
```

ثم يرسلها إلى RabbitMQ.

مثال مبسط:

``` csharp
var messages = await db.OutboxMessages
    .Where(x => x.ProcessedAt == null)
    .OrderBy(x => x.CreatedAt)
    .Take(10)
    .ToListAsync();

foreach (var message in messages)
{
    try
    {
        await publisher.PublishAsync(
            message.Type,
            message.Payload);

        message.ProcessedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        message.RetryCount++;
        message.Error = ex.Message;

        await db.SaveChangesAsync();
    }
}
```

------------------------------------------------------------------------

# 12. ماذا يحدث لو RabbitMQ وقع؟

مثلاً:

``` text
POST /api/orders
       |
       v
Database       ✅
Outbox         ✅
       |
       v
RabbitMQ       ❌
```

الـ Outbox message تظل:

``` text
ProcessedAt = NULL
```

والـ Worker يحاول مرة أخرى:

``` text
Retry #1 ❌
Retry #2 ❌
Retry #3 ✅
```

وعند النجاح:

``` text
ProcessedAt = current time
```

وهذا هو السبب الأساسي في استخدام Outbox.

------------------------------------------------------------------------

# 13. كيف ترى الرسالة في RabbitMQ؟

افتح:

``` text
http://localhost:15672
```

ثم:

``` text
Queues and Streams
```

يجب أن تجد:

``` text
order-created
```

اضغط على الـ Queue.

ثم ابحث عن:

``` text
Get messages
```

يمكنك اختيار عدد الرسائل ثم الضغط على:

``` text
Get Message(s)
```

سترى الـ JSON مثل:

``` json
{
  "OrderId": "8f...",
  "ProductName": "iPhone",
  "Amount": 50000
}
```

------------------------------------------------------------------------

# 14. مهم: ماذا لو أنشأت الـ Order قبل إنشاء الـ Queue؟

لو حصل:

``` text
Order Created
      ↓
Outbox Created
      ↓
Worker
      ↓
Exchange
      ↓
No Queue
```

فالرسالة قد لا تصل إلى Queue.

خصوصًا إذا لم تكن هناك Queue مناسبة ومربوطة بالـ Exchange وقت الـ
publish.

لذلك أثناء التعلم:

1.  أنشئ Exchange.
2.  أنشئ Queue.
3.  اعمل Binding.
4.  بعدها أنشئ Order.

إذا كانت الرسالة القديمة أصبحت:

``` text
ProcessedAt != NULL
```

فالـ Worker لن يرسلها مرة أخرى.

اعمل Order جديد للتجربة أو تعامل مع الرسالة القديمة بشكل مناسب.

------------------------------------------------------------------------

# 15. Direct Exchange

في المثال استخدمنا:

``` csharp
ExchangeType.Direct
```

ومعناه أن الـ Exchange يستخدم الـ routing key للمطابقة.

مثلاً:

``` text
Exchange: orders

Routing Key:
OrderCreated
```

Queue:

``` text
order-created
```

Binding:

``` text
order-created
       |
       | routing key = OrderCreated
       v
    orders
```

والـ message:

``` text
OrderCreated
```

تصل للـ Queue التي لديها نفس الـ binding key.

------------------------------------------------------------------------

# 16. الشكل الكامل

``` text
                         ASP.NET Core API
                                |
                                | POST /orders
                                v
                     ┌─────────────────────┐
                     │   DB Transaction    │
                     │                     │
                     │ Orders              │
                     │ OutboxMessages      │
                     └──────────┬──────────┘
                                |
                              Commit
                                |
                                v
                       Background Worker
                                |
                                | Publish
                                v
                       Exchange: orders
                                |
                         Routing Key
                         OrderCreated
                                |
                                v
                       Queue: order-created
                                |
                                v
                             Consumer
```

------------------------------------------------------------------------

# 17. مشكلة Duplicate Messages

Outbox لا يضمن بالضرورة:

``` text
Exactly Once
```

ممكن يحدث:

``` text
Worker
   |
   | Publish
   v
RabbitMQ ✅
   |
   | Worker crashes
   X
ProcessedAt not updated
```

ثم عند تشغيل Worker مرة أخرى:

``` text
Publish again
```

فتصبح الرسالة:

``` text
OrderCreated
OrderCreated
```

لذلك الـ Consumer يجب أن يكون:

``` text
Idempotent
```

أي أن معالجة نفس الرسالة أكثر من مرة لا تسبب تنفيذ العملية business
operation أكثر من مرة.

------------------------------------------------------------------------

# 18. Message ID

يفضل أن يكون لكل Outbox Message ID واضح:

``` csharp
Id = Guid.NewGuid()
```

ويتم إرسال الـ ID مع الـ event أو كـ message metadata.

مثلاً:

``` json
{
  "MessageId": "ABC-123",
  "OrderId": "ORDER-456",
  "ProductName": "iPhone",
  "Amount": 50000
}
```

الـ Consumer يستطيع تسجيل:

``` text
ProcessedMessages

MessageId
ProcessedAt
```

وعند استقبال:

``` text
ABC-123
```

مرة أخرى:

``` text
Already processed
       ↓
Ignore
```

------------------------------------------------------------------------

# 19. إمتى أستخدم Outbox؟

مفيد عندما يكون عندك:

``` text
Database + Message Broker
```

مثل:

``` text
SQL Server + RabbitMQ
SQL Server + Kafka
SQL Server + Azure Service Bus
```

خصوصًا في:

-   Microservices
-   Event-driven architecture
-   Payment systems
-   Order processing
-   Notifications
-   Integrations
-   Systems where losing an event is problematic

------------------------------------------------------------------------

# 20. إمتى ممكن ما أستخدموش؟

لو عندك API بسيطة:

``` text
ASP.NET Core
      |
      v
SQL Server
```

ومفيش messaging أو event publishing، فالـ Outbox غالبًا غير ضروري.

كذلك لو الـ event غير مهم ويمكن فقدانه بدون تأثير business كبير، قد تكون
بساطة الحل أهم.

------------------------------------------------------------------------

# 21. أهم نقاط الـ Interview

### What problem does Outbox Pattern solve?

> It solves the dual-write problem between a database and a message
> broker.

### How does it work?

> The business data and the event are stored in the same database
> transaction. A background worker then publishes pending outbox
> messages to the message broker.

### Does Outbox guarantee exactly-once delivery?

> No. It commonly provides at-least-once delivery, so consumers should
> be idempotent.

### What happens if RabbitMQ is down?

> The message remains in the Outbox table as unprocessed and can be
> retried later.

### Why do we need a Queue?

> The producer publishes to an Exchange. The Exchange routes the message
> to a bound Queue. Without a suitable Queue and Binding, a published
> message may not be available for a consumer.

------------------------------------------------------------------------

# 22. الخلاصة

احفظ الشكل ده:

``` text
API
 |
 | Transaction
 v
+----------------------+
| Database             |
|                      |
| Order                |
| Outbox Message       |
+----------------------+
          |
          | Background Worker
          v
       RabbitMQ
          |
          v
       Exchange
          |
          v
        Queue
          |
          v
       Consumer
```

**الفكرة الأساسية:**

> بدل ما تعمل Database Write و RabbitMQ Publish كعمليتين منفصلتين وممكن
> واحدة تنجح والتانية تفشل، بتخزن الـ event في Outbox داخل نفس
> transaction مع الـ business data، وبعدها Worker مسؤول عن نشره إلى
> RabbitMQ مع إمكانية الـ retry.
