

using TabularDatabaseManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Додаємо контролери з представленнями
builder.Services.AddControllersWithViews();

// Реєструємо DatabaseService як Singleton
builder.Services.AddSingleton<DatabaseService>();

var app = builder.Build();

// Налаштування середовища
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Налаштовуємо маршрутизацію
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
