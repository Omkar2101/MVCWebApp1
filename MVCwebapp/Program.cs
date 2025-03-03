using MVCWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
// Load configuration
var configuration = builder.Configuration;



// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<MongoDbService>();

// builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "user",
    pattern: "{controller=User}/{action=Create}/{id?}"
);


app.Run();
