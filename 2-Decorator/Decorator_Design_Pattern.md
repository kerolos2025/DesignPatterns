# Decorator Design Pattern

## 1. مقدمة

الـ **Decorator Design Pattern** هو أحد الـ **Structural Design
Patterns** في مجموعة Gang of Four (GoF).

فكرته الأساسية هي:

> إضافة مسؤوليات أو سلوكيات جديدة إلى Object موجود بدون تعديل الـ Object
> الأصلي.

بمعنى آخر، بدل ما نعدل الـ class الأساسية كل مرة نحتاج فيها
functionality جديدة، نستخدم Decorator يقوم بتغليف الـ object الأصلي
ويضيف السلوك المطلوب.

------------------------------------------------------------------------

# 2. المشكلة التي يحلها Decorator

تخيل أن لدينا Service مسؤولة عن تنفيذ Business Logic.

مع الوقت قد نحتاج إلى إضافة:

-   Logging
-   Caching
-   Validation
-   Retry
-   Auditing
-   Metrics
-   Authorization

الحل البسيط قد يكون وضع كل هذه المسؤوليات داخل الـ Service نفسها.

مثلاً:

``` text
OrderService
    ├── Business Logic
    ├── Logging
    ├── Caching
    ├── Retry
    ├── Auditing
    └── Metrics
```

وهذا يؤدي إلى أن الـ Service تصبح مسؤولة عن أشياء كثيرة.

المشاكل الناتجة:

-   زيادة حجم الـ class.
-   صعوبة القراءة والصيانة.
-   تداخل الـ Business Logic مع الـ Cross-Cutting Concerns.
-   صعوبة إعادة استخدام نفس السلوك مع Services أخرى.
-   زيادة صعوبة الـ Unit Testing.
-   الحاجة إلى تعديل الـ class الأصلية كلما أردنا إضافة behavior جديد.

------------------------------------------------------------------------

# 3. الحل باستخدام Decorator

بدلاً من وضع كل شيء داخل الـ Service، نقسم المسؤوليات.

مثلاً:

``` text
LoggingDecorator
        ↓
OrderService
```

الـ `OrderService` تظل مسؤولة عن الـ Business Logic فقط.

والـ `LoggingDecorator` مسؤول عن Logging.

إذا احتجنا Cache:

``` text
CacheDecorator
        ↓
LoggingDecorator
        ↓
OrderService
```

كل طبقة لديها مسؤولية واضحة.

------------------------------------------------------------------------

# 4. الفكرة الأساسية

الـ Decorator يحقق 3 أشياء أساسية:

### 1. يطبق نفس الـ Interface

``` text
IOrderService
     ↑
LoggingOrderService
```

### 2. يحتوي على Object من نفس الـ Interface

``` text
LoggingOrderService
        ↓
IOrderService
```

### 3. ينفذ الـ behavior الإضافي ثم يستدعي الـ Object الداخلي

``` text
Logging
   ↓
Inner Service
   ↓
Return Result
```

لذلك الشكل الأساسي للـ Decorator يكون:

``` text
Decorator
    │
    ├── Additional Behavior
    │
    └── Inner Object
              ↓
        Original Object
```

------------------------------------------------------------------------

# 5. Decorator والعلاقة مع Interface

أهم نقطة في Pattern هي أن الـ Decorator والـ Real Service يشتركان في نفس
abstraction.

مثلاً:

``` text
IOrderService
      ↑
      │
 ┌────┴───────────────┐
 │                    │
OrderService    LoggingOrderService
                      │
                      ↓
                IOrderService
```

الـ Controller يتعامل فقط مع:

``` text
IOrderService
```

ولا يحتاج أن يعرف هل الذي حصل عليه هو الـ Real Service أو Decorator.

------------------------------------------------------------------------

# 6. لماذا نستخدم Decorator؟

## 6.1 Separation of Concerns

كل class تكون مسؤولة عن concern محدد.

مثلاً:

``` text
OrderService
    → Business Logic

LoggingOrderService
    → Logging

CacheOrderService
    → Caching
```

------------------------------------------------------------------------

## 6.2 Open/Closed Principle

الـ Decorator يساعدنا على تطبيق مبدأ:

> Software entities should be open for extension but closed for
> modification.

أي يمكننا إضافة behavior جديد بدون تعديل الـ existing implementation.

مثلاً لدينا:

``` text
OrderService
```

ونريد إضافة Logging.

بدلاً من تعديل:

``` text
OrderService
```

نضيف:

``` text
LoggingOrderService
```

------------------------------------------------------------------------

## 6.3 Composition over Inheritance

الـ Decorator يعتمد على Composition بدلاً من الاعتماد على Inheritance.

بدلاً من:

``` text
LoggingOrderService
        ↓
     inherits
        ↓
OrderService
```

نستخدم:

``` text
LoggingOrderService
        ↓
    IOrderService
        ↓
OrderService
```

وهذا يجعل التصميم أكثر مرونة.

------------------------------------------------------------------------

## 6.4 إمكانية تركيب أكثر من Decorator

يمكننا إضافة أكثر من behavior.

مثلاً:

``` text
Caching
   ↓
Logging
   ↓
Retry
   ↓
OrderService
```

كل Decorator يضيف مسؤولية محددة.

------------------------------------------------------------------------

# 7. ترتيب الـ Decorators مهم

ترتيب الـ Decorators يؤثر على الـ behavior.

مثلاً:

``` text
Cache
  ↓
Logging
  ↓
Service
```

إذا حدث Cache Hit، قد لا يصل الطلب إلى Logging Decorator.

أما:

``` text
Logging
  ↓
Cache
  ↓
Service
```

فالـ Logging Decorator يستقبل الطلب أولاً، وبالتالي يمكنه تسجيل الـ
request حتى لو كان هناك Cache Hit.

إذن:

> Decorator composition order is part of the design.

------------------------------------------------------------------------

# 8. Decorator في ASP.NET Core

ASP.NET Core Dependency Injection مناسب جداً لتطبيق Decorator Pattern.

لدينا مثلاً:

``` text
IOrderService
```

والـ implementation الأصلية:

``` text
OrderService
```

ثم:

``` text
LoggingOrderService
```

ثم يمكن تركيبهم:

``` text
LoggingOrderService
        ↓
OrderService
```

ولو أضفنا Cache:

``` text
CacheOrderService
        ↓
LoggingOrderService
        ↓
OrderService
```

الـ Controller يظل يتعامل مع:

``` text
IOrderService
```

فقط.

------------------------------------------------------------------------

# 9. لماذا نحتاج إلى تسجيل الـ Real Service في DI؟

عند استخدام:

``` csharp
sp.GetRequiredService<OrderService>();
```

يجب أن يعرف الـ Dependency Injection Container كيف ينشئ `OrderService`.

لذلك نستخدم:

``` csharp
builder.Services.AddScoped<OrderService>();
```

هذا التسجيل ليس جزءاً من Decorator Pattern نفسه.

هو فقط يجعل الـ Real Service متاحة للـ DI Container حتى يستطيع الـ
Decorator تغليفها.

------------------------------------------------------------------------

# 10. متى يكون Decorator مناسباً؟

استخدم Decorator عندما:

-   لديك Service أو Object موجود بالفعل.
-   تريد إضافة behavior جديد.
-   لا تريد تعديل الـ original class.
-   الـ behavior يمكن فصله في مسؤولية مستقلة.
-   قد تحتاج إلى تركيب أكثر من behavior.
-   تريد الحفاظ على Business Logic نظيفاً.

أمثلة شائعة:

``` text
Logging
Caching
Retry
Validation
Auditing
Metrics
Authorization
Performance Tracking
```

------------------------------------------------------------------------

# 11. متى لا نحتاج إلى Decorator؟

لا تستخدم Decorator لمجرد استخدام Design Pattern.

إذا كانت الـ Service بسيطة ولا توجد مسؤوليات إضافية تحتاج إلى فصلها، فلا
يوجد داعٍ لإضافة Decorator.

الهدف من Design Patterns هو:

> حل مشكلة في التصميم.

وليس:

> استخدام Pattern لأن المشروع يجب أن يحتوي على Patterns.

------------------------------------------------------------------------

# 12. Decorator vs Middleware في ASP.NET Core

قد يحدث خلط بين الاثنين.

### Middleware

يعمل على مستوى HTTP Pipeline:

``` text
Request
   ↓
Middleware
   ↓
Middleware
   ↓
Controller
```

مناسب غالباً لـ:

-   Global Exception Handling
-   Request Logging
-   Authentication
-   Correlation ID
-   HTTP-level concerns

### Decorator

يعمل غالباً على مستوى Service:

``` text
Controller
   ↓
Decorator
   ↓
Service
```

مناسب عندما نريد إضافة behavior إلى Service أو Operation محددة.

------------------------------------------------------------------------

# 13. ملخص المشروع العملي

في المشروع العملي قمنا بعمل ASP.NET Core Web API بسيط جداً بدون Database
أو أي infrastructure إضافية.

الهدف كان التركيز على Decorator Pattern فقط.

استخدمنا:

``` text
IOrderService
```

ثم أنشأنا الـ Real Service:

``` text
OrderService
```

وهذه تحتوي على الـ behavior الأساسي.

بعد ذلك أنشأنا:

``` text
LoggingOrderService
```

وهي Decorator تضيف Logging حول الـ Real Service.

الشكل أصبح:

``` text
LoggingOrderService
        ↓
OrderService
```

ثم أضفنا:

``` text
CacheOrderService
```

كـ Decorator آخر.

وأصبح لدينا:

``` text
CacheOrderService
        ↓
LoggingOrderService
        ↓
OrderService
```

------------------------------------------------------------------------

# 14. الفكرة العملية باختصار

عندما يستدعي الـ Controller:

``` text
IOrderService.GetOrder()
```

الطلب يصل أولاً إلى:

``` text
CacheOrderService
```

ثم يقوم الـ Decorator باستدعاء:

``` text
_inner.GetOrder()
```

والـ `_inner` هنا هو:

``` text
LoggingOrderService
```

ثم `LoggingOrderService` يقوم بالـ Logging ويستدعي بدوره:

``` text
_inner.GetOrder()
```

والـ `_inner` هنا هو:

``` text
OrderService
```

إذن التنفيذ يصبح:

``` text
Controller
    ↓
CacheOrderService
    ↓
LoggingOrderService
    ↓
OrderService
    ↓
Return
```

------------------------------------------------------------------------

# 15. أهم شيء يجب تذكره

يمكن تلخيص Decorator Pattern في النقاط التالية:

``` text
Same Interface
      +
Wrapper
      +
Additional Behavior
      +
Inner Object
```

أو بشكل أبسط:

> **Decorator = Wrap an object and add behavior without modifying the
> original object.**

وفي ASP.NET Core:

``` text
Controller
    ↓
IOrderService
    ↓
CacheOrderService
    ↓
LoggingOrderService
    ↓
OrderService
```

الـ Controller لا يهتم بعدد الـ Decorators أو تفاصيلها.

هو يتعامل فقط مع:

``` text
IOrderService
```

وهذه هي الفكرة الأساسية التي تجعل Decorator Pattern مفيداً في تصميم
الأنظمة القابلة للتوسع والصيانة.
