using _8_Observer.Observers;
using _8_Observer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddScoped<IOrderObserver, EmailObserver>();
builder.Services.AddScoped<IOrderObserver, LoggingObserver>();
builder.Services.AddScoped<IOrderObserver, SMSObserver>();

builder.Services.AddScoped<OrderService>();



builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
