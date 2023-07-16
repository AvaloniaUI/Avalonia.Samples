using OpenQA.Selenium.Appium;
using Xunit;

namespace TestableApp.Appium;

[Collection("Default")]
public class CalculatorTests
{
    private readonly AppiumDriver<AppiumWebElement> _session;


    public CalculatorTests(DefaultAppFixture fixture)
    {
        _session = fixture.Session;
    }

    [Fact]
    public void Should_Add_Numbers()
    {
        // Assert:
        var firstOperandInput = _session.FindElementByAccessibilityId("FirstOperandInput")!;
        var secondOperandInput = _session.FindElementByAccessibilityId("SecondOperandInput")!;
        var addButton = _session.FindElementByAccessibilityId("AddButton")!;
        var resultBox = _session.FindElementByAccessibilityId("ResultBox")!;

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
        var firstOperandInput = _session.FindElementByAccessibilityId("FirstOperandInput")!;
        var secondOperandInput = _session.FindElementByAccessibilityId("SecondOperandInput")!;
        var divideButton = _session.FindElementByAccessibilityId("DivideButton")!;
        var resultBox = _session.FindElementByAccessibilityId("ResultBox")!;

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