using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using SharedControls.Converters;
using Xunit;

namespace SharedControlsTests;

/// <summary>
/// This is a unit test for our BoundsToCornerRadiusConverter.
/// In production, we should test all other converters similarly. 
/// </summary>
public class BoundsToCornerRadiusConverterTests
{
    // Our converter to test
    private readonly IValueConverter _converter = BoundsToCornerRadiusConverter.Instance;
    
    static Rect _testRect = new Rect(10, 10, 100, 100);
    
    /// <summary>
    /// This test proves that the converter returns the correct value
    /// </summary>
    [Fact]
    public void Convert_WithValidRect_ReturnsCorrectCornerRadius()
    {
        // Arrange
        var rect = new Rect(0, 0, 100, 50); // Width > Height
        var expectedRadius = new CornerRadius(25); // min(100,50)/2

        // Act
        var result = _converter.Convert(rect, typeof(CornerRadius), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<CornerRadius>(result);
        Assert.Equal(expectedRadius, (CornerRadius)result);
    }

    /// <summary>
    /// This test proves that invalid data will not throw an exception; instead it should do nothing. 
    /// </summary>
    /// <param name="invalidValue"></param>
    [Theory]
    [InlineData(null)]
    [InlineData("not a rect")]
    public void Convert_WithInvalidValue_ReturnsDoNothing(object? invalidValue)
    {
        // Act
        var result = _converter.Convert(invalidValue, typeof(CornerRadius), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(BindingOperations.DoNothing, result);
    }

    /// <summary>
    /// The converter isn't able to convert back and should throw in case someone is going to try. 
    /// </summary>
    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        // Arrange
        var validRadius = new CornerRadius(10);

        // Act & Assert
        Assert.Throws<NotSupportedException>(() =>
            _converter.ConvertBack(validRadius, typeof(object), null, CultureInfo.InvariantCulture));
    }
}