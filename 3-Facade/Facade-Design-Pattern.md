# Facade Design Pattern

## ما هو Facade؟

الـ **Facade Design Pattern** هو Pattern بيقدم **واجهة بسيطة** للتعامل مع مجموعة من الـ Services أو الـ Components المعقدة.

بدل ما الـ Client أو الـ Controller يتعامل مع أكثر من Service، يتعامل مع **Facade واحدة** وهي تنسق باقي الـ Services.

### الفكرة ببساطة

بدون Facade:

```text
Controller
   ├── ProductService
   ├── PaymentService
   └── OrderService
```

مع Facade:

```text
Controller
      ↓
 OrderFacade
      ├── ProductService
      ├── PaymentService
      └── OrderService
```

الـ Controller هنا لا يحتاج معرفة تفاصيل الـ workflow الداخلي.

## لماذا نستخدمه؟

- تقليل الـ coupling بين الـ Controller والـ Services.
- إخفاء الـ complexity.
- تجميع خطوات عملية واحدة في مكان واضح.
- جعل الـ Controller أبسط وأسهل في القراءة.

## متى أستخدمه؟

استخدم Facade عندما يكون عندك **Use Case أو Workflow** يحتاج التعامل مع عدة Services أو Subsystems.

مثال:

```text
Create Order
   ↓
Check Product
   ↓
Process Payment
   ↓
Create Order
```

يمكن أن تكون كل هذه الخطوات داخل `OrderFacade`.

## متى لا أستخدمه؟

لا تستخدم Facade لمجرد إضافة طبقة جديدة.

إذا كانت العملية بسيطة:

```text
Controller → OrderService
```

فغالبًا لا تحتاج Facade.

كذلك تجنب إنشاء Facade واحد ضخم يتعامل مع كل أجزاء النظام.

## الخلاصة

> **Facade = واجهة بسيطة تخفي تعقيد مجموعة من الـ Services.**

الهدف ليس تقليل عدد الـ Classes، وإنما **إخفاء التعقيد وتقديم طريقة بسيطة لتنفيذ Use Case معين**.
