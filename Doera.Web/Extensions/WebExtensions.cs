using System;
using Elmah.Io.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Doera.Web.Extensions;

public static class WebExtensions
{
    public static void AddElmahIoMonitor(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
    {
        if (!env.IsProduction())
            return;

        var section = configuration.GetSection("ElmahIo");
        var enabled = section.GetValue<bool?>("Enabled") ?? true;

        if (!enabled)
            return;

        var apiKey = section["ApiKey"];
        var logIdRaw = section["LogId"];

        if (string.IsNullOrWhiteSpace(apiKey) || !Guid.TryParse(logIdRaw, out var logId))
            return;

        services.AddElmahIo(o =>
        {
            o.ApiKey = apiKey!;
            o.LogId = logId;
        });
    }

    public static void UseElmahIoMonitor(this WebApplication app) {
        var section = app.Configuration.GetSection("ElmahIo");
        var enabled = section.GetValue<bool?>("Enabled") ?? true;
        var apiKey = section["ApiKey"];
        var logIdRaw = section["LogId"];
        var valid = enabled && !string.IsNullOrWhiteSpace(apiKey) && Guid.TryParse(logIdRaw, out _);

        if (app.Environment.IsProduction() && valid) {
            app.UseElmahIo();
        }
    }
}
