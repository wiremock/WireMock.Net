using System;
using System.Reflection;
using System.Threading.Tasks;

namespace WireMock.Net.Extensions.Routing.Extensions;

internal static class TaskExtensions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Usage",
        "VSTHRD003:Avoid awaiting foreign Tasks",
        Justification = "Await is required here to transform base task to generic one.")]
    public static async Task<object?> ToGenericTaskAsync(this Task task)
    {
        await task;
        var taskType = task.GetType();
        if (!IsAssignableToGenericTaskType(taskType))
        {
            return null;
        }

        return task
            .GetType()
            .GetProperty("Result", BindingFlags.Instance | BindingFlags.Public)!
            .GetValue(task);
    }

    private static bool IsAssignableToGenericTaskType(Type type)
    {
        if (type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Task<>) &&
            type.GetGenericArguments()[0] != Type.GetType("System.Threading.Tasks.VoidTaskResult"))
        {
            return true;
        }

        return type.BaseType is not null && IsAssignableToGenericTaskType(type.BaseType);
    }
}
