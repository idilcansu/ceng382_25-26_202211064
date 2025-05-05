using Microsoft.EntityFrameworkCore;
using RazorPagesApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı için DbContext ekleme (tek satır yeterli)
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

// Servisleri container'a ekleme
builder.Services.AddRazorPages();

// Oturum servislerini ekleme
builder.Services.AddDistributedMemoryCache(); // Oturum için gerekli
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// HTTP istek boru hattını yapılandırma
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Oturumu UseAuthorization'dan önce ve UseRouting'den sonra kullanmalısınız
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
