using Microservice.MessageBus;
using Microservices.Service.OrderAPI;
using Microservices.Service.OrderAPI.Data;
using Microservices.Service.OrderAPI.Extensions;
using Microservices.Service.OrderAPI.Utlity;
using Microservices.Services.OrderAPI.Service;
using Microservices.Services.OrderAPI.Service.IService;
using Microservices.Services.OrderAPI.Utlity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingConfig>());
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendAPIAuthHttpClientHandler>();
builder.Services.AddScoped<IMessageBus, MessageBus>();
builder.Services.Configure<TopicAndQueueNames>(builder.Configuration.GetSection("TopicAndQueueNames"));
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddHttpClient("Product", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]);
}).AddHttpMessageHandler<BackendAPIAuthHttpClientHandler>();

builder.Services.Configure<StripeApiKey>(builder.Configuration.GetSection("StripeApiKey"));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.AddAuthentication();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

ApplyMigrations(app);
app.Run();

void ApplyMigrations(IHost app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    dbContext.Database.Migrate();
}