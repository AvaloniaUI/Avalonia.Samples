using System.Reflection;

namespace Avalonia.Headless.MSTest;

/// <summary>
///   <para>
/// This Attribute identifies a TestMethod that starts on Avalonia Dispatcher such that awaited expressions resume in the test's main thread</para>
///   <para>Class AvaloniaTestMethodAttribute.<br />This class cannot be inherited.
/// <br /> Implements the <see cref="TestMethodAttribute" /></para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class AvaloniaTestMethodAttribute : TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var assembly = testMethod.MethodInfo.DeclaringType!.Assembly;
        var appBuilderEntryPointType = assembly.GetCustomAttribute<AvaloniaTestApplicationAttribute>()
            ?.AppBuilderEntryPointType;

        appBuilderEntryPointType ??= typeof(Application);

        using var _session = HeadlessUnitTestSession.StartNew(appBuilderEntryPointType);
        {
            return _session.Dispatch(() => ExecuteTestMethod(testMethod!), default).GetAwaiter().GetResult();
        }
    }

    /// <summary>Executes the test method.</summary>
    /// <param name="testMethod">The test method.</param>
    /// <returns>TestResult[].</returns>
    private static TestResult[] ExecuteTestMethod(ITestMethod testMethod)
    {
        return [testMethod.Invoke(null)];
    }
}