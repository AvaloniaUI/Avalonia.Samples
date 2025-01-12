using System;
using global::Avalonia.Headless;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Avalonia.Headless.MSTest;

//
// Zusammenfassung:
//     Identifies a nunit test that starts on Avalonia Dispatcher such that awaited
//     expressions resume on the test's "main thread".
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class AvaloniaTestMethodAttribute : TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        using var _session = HeadlessUnitTestSession.GetOrStartForAssembly(testMethod?.MethodInfo.DeclaringType?.Assembly);
        {
            return _session.Dispatch(() => ExecuteTestMethod(testMethod), default).GetAwaiter().GetResult();
        }
    }

    // Unfortunately, NUnit has issues with custom synchronization contexts, which means we need to add some hacks to make it work.
    private async Task<TestResult[]> ExecuteTestMethod(ITestMethod testMethod)
    {
        return [testMethod.Invoke(null)];
    }
}