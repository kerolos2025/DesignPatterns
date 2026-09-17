using _9_Outbox.Data;
using _9_Outbox.Workers;
using Microsoft.EntityFrameworkCore;
using OutboxDemo.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration
            .GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<RabbitMqPublisher>();

builder.Services.AddHostedService<OutboxWorker>();


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
