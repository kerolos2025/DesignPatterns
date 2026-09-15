# Factory Design Pattern

## الفكرة ببساطة

**Factory Pattern** بيخلي إنشاء الـ Objects في مكان واحد بدل ما كل جزء
في التطبيق يعمل `new` ويقرر بنفسه أي Class يستخدم.

> **Factory = مسؤولة عن اختيار وإنشاء الـ Object المناسب.**

------------------------------------------------------------------------

## مثال بسيط

عندنا Notification بأنواع مختلفة:

``` csharp
public interface INotification
{
    void Send(string message);
}

public class EmailNotification : INotification
{
    public void Send(string message)
    {
        Console.WriteLine("Send Email");
    }
}

public class SmsNotification : INotification
{
    public void Send(string message)
    {
        Console.WriteLine("Send SMS");
    }
}
```

الـ Factory:

``` csharp
public class NotificationFactory
{
    public INotification Create(string type)
    {
        return type.ToLower() switch
        {
            "email" => new EmailNotification(),
            "sms" => new SmsNotification(),
            _ => throw new ArgumentException("Invalid type")
        };
    }
}
```

الاستخدام:

``` csharp
var notification = factory.Create("email");

notification.Send("Hello");
```

بدل ما الـ Controller يعمل `new EmailNotification()` أو
`new SmsNotification()`، الـ Factory هي اللي بتختار الـ Object المناسب.

------------------------------------------------------------------------

## إمتى أستخدم Factory؟

استخدمها لما:

-   عندك أكتر من Implementation لنفس Interface.
-   محتاج تختار الـ Implementation بناءً على نوع أو Input.
-   Creation logic بدأ يكبر أو يتكرر في أكتر من مكان.
-   عايز تخلي الـ Consumer مش مسؤول عن تفاصيل إنشاء الـ Object.

## إمتى أبعد عنها؟

متستخدمهاش لو:

-   عندك Class بسيطة ومجرد `new` كفاية.
-   عندك Implementation واحدة ومفيش اختيار.
-   الـ Factory نفسها هتزود تعقيد من غير Benefit.

------------------------------------------------------------------------

## Factory vs Strategy

الفرق الأساسي:

  Factory                  Strategy
  ------------------------ --------------------------------
  تختار وتُنشئ الـ Object   تختار طريقة تنفيذ الـ Behavior
  **What object?**         **How to do it?**
  Creation                 Behavior

### الخلاصة

> **Factory decides what object to create.**\
> **Strategy decides how a behavior should be performed.**
