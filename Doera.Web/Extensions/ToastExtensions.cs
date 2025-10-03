using Doera.Application.Abstractions.Results;
using NToastNotify;

namespace Doera.Web.Extensions;

public static class ToastExtensions
{
    // Result -> Warning
    public static void Warn(this IToastNotification toast, Result result)
    {
        if (result?.Errors is null) return;
        foreach (var e in result.Errors)
        {
            var msg = (e?.Description ?? string.Empty).Trim();
            if (msg.Length > 0) toast.AddWarningToastMessage(msg);
        }
    }

    // Result -> Error
    public static void Error(this IToastNotification toast, Result result)
    {
        if (result?.Errors is null) return;
        foreach (var e in result.Errors)
        {
            var msg = (e?.Description ?? string.Empty).Trim();
            if (msg.Length > 0) toast.AddErrorToastMessage(msg);
        }
    }

    // Result<T> -> Warning/Error
    public static void Warn<T>(this IToastNotification toast, Result<T> result)
        => toast.Warn((Result)result);

    public static void Error<T>(this IToastNotification toast, Result<T> result)
        => toast.Error((Result)result);
}