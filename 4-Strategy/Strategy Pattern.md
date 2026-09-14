# Strategy Pattern

## الفكرة

الـ **Strategy Pattern** هو Design Pattern بيستخدم لما يكون عندك **أكتر من طريقة لتنفيذ نفس العملية**.

بدل ما نحط كل الطرق في `if/else` أو `switch` واحد، بنفصل كل طريقة في Class مستقل، وكلهم بيطبقوا نفس Interface.

```text
Strategy
   │
   ├── Strategy A
   ├── Strategy B
   └── Strategy C
```

وبكده نقدر نغيّر طريقة التنفيذ بسهولة بدون تعديل الـ Business Logic الأساسي.

---

## إمتى أستخدمه؟

استخدم **Strategy Pattern** لما:

- يكون عندك أكتر من طريقة لتنفيذ نفس العملية.
- يكون عندك `if/else` أو `switch` كبير ومتزايد.
- تتوقع إضافة طرق تنفيذ جديدة في المستقبل.
- كل طريقة عندها Logic مختلف.
- تريد فصل الـ Business Logic عن طريقة التنفيذ.

### أمثلة

- طرق الدفع: `Visa / Wallet / Bank`
- طرق الشحن: `DHL / FedEx / Aramex`
- طرق إرسال Notifications: `Email / SMS / WhatsApp`
- طرق حساب الخصم أو الضرائب.

---

## إمتى أبعد عنه؟

لا تستخدمه لمجرد وجود `if/else`.

ابعد عنه عندما:

- يكون الـ Logic بسيط جدًا.
- يكون عندك حالتين أو حالات قليلة وواضحة.
- الاختلاف مجرد Values أو Configuration.
- استخدامه سيضيف Classes وتعقيد بدون فائدة حقيقية.

---

## الخلاصة

> **Strategy = نفس العملية + طرق مختلفة لتنفيذها.**

لو عندك:

```text
Same Operation
       ↓
Different Implementations
       ↓
Choose the implementation
```

فالـ **Strategy Pattern** غالبًا اختيار مناسب.