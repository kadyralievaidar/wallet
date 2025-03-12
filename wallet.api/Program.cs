using wallet.api.Features.Core;
using wallet.api.Features.Payment;
using wallet.api.Features.Users;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IIdentityUserService, IdentityUserService>();
builder.ConfigureServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
