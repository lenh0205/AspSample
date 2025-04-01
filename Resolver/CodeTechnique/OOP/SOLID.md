> SOLID là principle - tức là nó nên được áp dụng trong mọi trường hợp không giống Design Pattern để giải quyết những vấn đề cụ thể hơn
> lý do rất dễ hiểu là do nghĩa của SOLID rất rộng hơn - nội dung của các nguyên lý đều rất trừu tượng, chung chung để bao hàm nhiều trường hợp
> tiếp cận Design Pattern trước là hợp lý hơn để thực sự hiểu SOLID

# Single Responsibility
* -> a class or module should have one, and only one, reason to change
* => code base is well-designed; easier to debug, unit test
* => khá là liên quan đến **Chain of Responsibility** pattern

## Example
* -> có 1 vẫn đề là có những behavior sẽ cần sử dụng nhiều chỗ, nếu ta coupling behavior này vào 1 class thực hiện nhiều công việc để hoàn thành 1 business logic nhất định
* -> thì để sử dụng lại những behavior cụ thể này ta sẽ cần duplicate code

 * -> khi xảy ra lỗi ta sẽ dễ dàng biết được lỗi thực sự diễn ra ở đâu hơn

* -> sau khi refactor, ta có thể thấy ta dễ dàng mock dependencies như "OrderRepository" để focus vào việc test logic "OrderProcessor" 1 cách nhanh chóng;
* -> và việc mock the database and email service trong 1 tightly coupled class là không dễ dàng

```cs
// Example: Real-life Analogy
// -> like we have a drawer where we put your shoes, coats and toothbrushes together
// -> they do have one thing in common — we use them everyday,
// -> but that doesn't mean they belong together
```

```cs
// Bad Code Example:
public class OrderProcessor
{
    public void ProcessOrder(Order order)
    {
        // Calculate total price
        order.TotalPrice = order.Items.Sum(item => item.Price * item.Quantity);

        // Save order to database
        SaveToDatabase(order);

        // Send confirmation email
        Console.WriteLine($"Email sent to {order.CustomerEmail}: Your order has been processed.");
    }

    private void SaveToDatabase(Order order)
    {
        Console.WriteLine($"Order saved to database for {order.CustomerEmail}.");
    }
}
```

```cs
// Refactor with Single Responsibility
public class OrderProcessor
{
    private readonly OrderCalculator _orderCalculator;
    private readonly OrderRepository _orderRepository;
    private readonly EmailService _emailService;

    public OrderProcessor()
    {
        _orderCalculator = new OrderCalculator();
        _orderRepository = new OrderRepository();
        _emailService = new EmailService();
    }

    public void ProcessOrder(Order order)
    {
        // Calculate total price
        _orderCalculator.CalculateTotal(order);

        // Save order to database
        _orderRepository.Save(order);

        // Send confirmation email
        _emailService.SendConfirmation(order.CustomerEmail);
    }
}

public class OrderCalculator
{
    public void CalculateTotal(Order order)
    {
        order.TotalPrice = order.Items.Sum(item => item.Price * item.Quantity);
    }
}

public class OrderRepository
{
    public void Save(Order order)
    {
        Console.WriteLine($"Order saved to database for {order.CustomerEmail}.");
    }
}

public class EmailService
{
    public void SendConfirmation(string email)
    {
        Console.WriteLine($"Email sent to {email}: Your order has been processed.");
    }
}
```
