using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMvc.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SalesWebMvcContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SalesWebMvcContext")
        ?? throw new InvalidOperationException("Connection string 'SalesWebMvcContext' not found.");

    var serverVersion = new MySqlServerVersion(new Version(8, 0, 39));

    options.UseMySql(connectionString, serverVersion, mySqlOptions =>
    {
        mySqlOptions.MigrationsAssembly("SalesWebMvc");
    });
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<SeedingService>(); //Registra o serviço no sistema da injeção de dependencias da aplicação

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seedingService = scope.ServiceProvider.GetRequiredService<SeedingService>();
    seedingService.Seed();
}

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

app.Run();
