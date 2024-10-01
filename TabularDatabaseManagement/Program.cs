

using TabularDatabaseManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// ������ ���������� � ���������������
builder.Services.AddControllersWithViews();

// �������� DatabaseService �� Singleton
builder.Services.AddSingleton<DatabaseService>();

var app = builder.Build();

// ������������ ����������
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

// ����������� �������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
