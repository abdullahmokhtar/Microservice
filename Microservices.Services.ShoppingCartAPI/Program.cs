using Microservice.MessageBus;
using Microservices.Services.ShoppingCartAPI;
using Microservices.Services.ShoppingCartAPI.Data;
using Microservices.Services.ShoppingCartAPI.Extensions;
using Microservices.Services.ShoppingCartAPI.Service;
using Microservices.Services.ShoppingCartAPI.Service.IService;
using Microservices.Services.ShoppingCartAPI.Utlity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

builder.Services.AddHttpClient("Coupon", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ServiceUrls:CouponAPI"]);
}).AddHttpMessageHandler<BackendAPIAuthHttpClientHandler>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
