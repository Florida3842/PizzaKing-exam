using Microsoft.AspNetCore.Mvc;
using PizzaKing.Interfaces;

public class PaymentController : Controller
{
    private readonly IOrder _orders;
    public PaymentController(IOrder orders) => _orders = orders;

    //payment/start/{orderId}?amount=123.45
    [HttpGet("/payment/start/{orderId:int}")]
    public async Task<IActionResult> Start(int orderId, decimal amount)
    {
        var order = await _orders.GetOrderAsync(orderId);
        if (order == null) return NotFound();

        ViewBag.OrderId = orderId;
        ViewBag.Amount = amount;
        ViewBag.Currency = "UAH";
        return View("Demo");   
    }

    // "успешная оплата" в демо
    [HttpGet("/payment/demo/success")]
    public async Task<IActionResult> DemoSuccess(int orderId)
    {
        var order = await _orders.GetOrderAsync(orderId);
        if (order == null) return NotFound();

        return RedirectToAction("Result");
    }

    // "ошибка оплаты" в демо
    [HttpGet("/payment/demo/fail")]
    public async Task<IActionResult> DemoFail(int orderId)
    {
        var order = await _orders.GetOrderAsync(orderId);
        if (order == null) return NotFound();

        return RedirectToAction("Result");
    }

    [HttpGet("/payment/result")]
    public IActionResult Result() => View("Result");
}
