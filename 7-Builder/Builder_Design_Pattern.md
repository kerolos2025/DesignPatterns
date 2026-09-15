# Builder Design Pattern

## الفكرة ببساطة

**Builder Pattern** بيستخدم لما يكون عندك Object معقد وعايز تبنيه **خطوة بخطوة** بدل Constructor كبير ومليان Parameters.

مثال:

```csharp
var user = new UserBuilder()
    .SetName("Kero")
    .SetEmail("kero@test.com")
    .MakeAdmin()
    .Build();
```

الفكرة:

```text
SetName()
   ↓
SetEmail()
   ↓
MakeAdmin()
   ↓
Build()
   ↓
Object
```

## إمتى أستخدمه؟

استخدمه لما:

- الـ Object عنده Properties أو Parameters كتير.
- عندك Optional Properties كتير.
- إنشاء الـ Object محتاج خطوات أو Configuration متعددة.
- Constructor بدأ يكبر ويبقى صعب القراءة.

## إمتى أبعد عنه؟

متستخدموش لما:

- الـ Object بسيط.
- عندك 2 أو 3 Properties فقط.
- Object Initializer أو Constructor بسيط كفاية.
- استخدامه هيضيف تعقيد بدون فائدة.

## الفرق بين Builder و Factory

**Factory:**
> أنا عايز أعرف أي Object أعمل.

```csharp
factory.Create("PDF");
```

**Builder:**
> أنا عايز أبني الـ Object ده إزاي.

```csharp
builder
    .SetTitle("Report")
    .SetFormat("PDF")
    .Build();
```

### الخلاصة

> **Builder = Build a complex object step by step.**

استخدمه لما يكون الـ Object **معقد**، ومتستخدموش لمجرد تطبيق Design Pattern.
