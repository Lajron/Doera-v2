using Doera.Application.Extensions;
using Doera.Application.Interfaces.Caching;
using Doera.Core.Entities;
using Doera.Infrastructure.Data;
using Doera.Infrastructure.Extensions;
using Doera.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NToastNotify;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorOptions(o => o.ViewLocationExpanders.Add(new CustomViewLocationExpander()))
    .AddNToastNotifyToastr(
        builder.Configuration.GetSection("NToastNotify:ToastrOptions").Get<ToastrOptions>()
    );

// Add ElmahIo monitoring
builder.Services.AddElmahIoMonitor(builder.Configuration, builder.Environment);

// Configure Entity Framework and SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
});

// Infrastructure services
builder.Services.AddInfrastructureLayer();

// Application services
builder.Services.AddApplicationLayer();
builder.Services.AddApplicationValidation();

builder.Services.AddAppCaching();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.InitializeDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();

} else {
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");

    app.UseElmahIoMonitor();

    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

// Enable on redirect notifications
app.UseNToastNotify();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
