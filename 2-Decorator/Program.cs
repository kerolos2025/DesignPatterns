using _2_Decorator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<OrderService>();

builder.Services.AddScoped<IOrderService>(sp =>
{
    var realService =
        sp.GetRequiredService<OrderService>();

    var logger =
        sp.GetRequiredService<
            ILogger<LoggingOrderService>>();

    IOrderService loggingDecorator =
        new LoggingOrderService(
            realService,
            logger);

    var cachlogger =
        sp.GetRequiredService<
            ILogger<CacheOrderService>>();

    IOrderService cacheDecorator =
        new CacheOrderService(
            loggingDecorator, cachlogger);

    return cacheDecorator;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
