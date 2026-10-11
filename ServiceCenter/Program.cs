using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using ServiceCenter.data;
using ServiceCenter.Messaging;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.Configure<SqsOptions>(builder.Configuration.GetSection("Sqs"));
builder.Services.AddScoped<ServiceRequestQueue>();
builder.Services.AddHostedService<ServiceRequestQueueWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/ServiceRequest/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ServiceRequest}/{action=Index}/{id?}");

app.Run();
