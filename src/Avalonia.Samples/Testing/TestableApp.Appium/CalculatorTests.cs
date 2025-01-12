using OpenQA.Selenium.Appium;
using Xunit;

namespace TestableApp.Appium;

[Collection("Default")]
public class CalculatorTests
{
    private readonly AppiumDriver _session;


    public CalculatorTests(DefaultAppFixture fixture)
    {
        _session = fixture.Session;
    }

    [Fact]
    public void Should_Add_Numbers()
    {
        // Assert:
        const string ByAccessibilityId = "AccessibilityId";
        var firstOperandInput = _session.FindElement(ByAccessibilityId, "FirstOperandInput")!;
        var secondOperandInput = _session.FindElement(ByAccessibilityId, "SecondOperandInput")!;
        var addButton = _session.FindElement(ByAccessibilityId, "AddButton")!;
        var resultBox = _session.FindElement(ByAccessibilityId, "ResultBox")!;

        // Act:
        firstOperandInput.Clear();
        firstOperandInput.SendKeys("10");
        secondOperandInput.Clear();
        secondOperandInput.SendKeys("20");

        addButton.Click();

        // Assert:
        Assert.Equal("30", resultBox.Text);
    }
    
    [Fact]
    public void Cannot_Divide_By_Zero()
    {
        // Assert:
        const string ByAccessibilityId = "AccessibilityId";
        var firstOperandInput = _session.FindElement(ByAccessibilityId, "FirstOperandInput")!;
        var secondOperandInput = _session.FindElement("AccessibilityId", "SecondOperandInput")!;
        var divideButton = _session.FindElement("AccessibilityId", "DivideButton")!;
        var resultBox = _session.FindElement("AccessibilityId", "ResultBox")!;

        // Act:
        firstOperandInput.Clear();
        firstOperandInput.SendKeys("10");
        secondOperandInput.Clear();
        secondOperandInput.SendKeys("0");

        divideButton.Click();

        // Assert:
        Assert.Equal("Cannot divide by zero!", resultBox.Text);
    }
}