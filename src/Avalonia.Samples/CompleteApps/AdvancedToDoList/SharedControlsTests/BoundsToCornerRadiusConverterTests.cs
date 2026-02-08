
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using SharedControls.Converters;
using Xunit;

namespace SharedControlsTests;

/// <summary>
/// Unit tests for the BoundsToCornerRadiusConverter.
/// Tests that the converter correctly calculates corner radius from bounding rectangle dimensions.
/// </summary>
/// <remarks>
/// Purpose of BoundsToCornerRadiusConverter:
/// - Calculates corner radius as half of the smaller dimension (width or height)
/// - Creates circular/rounded bounds for UI elements based on available space
/// - Prevents excessive corner radius that would distort the visual appearance
/// 
/// Conversion logic:
/// - Result = min(width, height) / 2
/// - Example: 100x50 rect → min(100,50)/2 = 25
/// - Used for dynamic UI element sizing and styling
/// 
/// Test focus:
/// - Valid rectangle conversion to correct corner radius
/// - Invalid input handling (no exceptions, returns DoNothing)
/// - ConvertBack operation (intentionally not supported)
/// </remarks>
public class BoundsToCornerRadiusConverterTests
{
    // Our converter to test
    private readonly IValueConverter _converter = BoundsToCornerRadiusConverter.Instance;

    /// <summary>
    /// Tests that valid rectangle input produces correct corner radius calculation.
    /// Verifies the converter uses the smaller dimension to prevent distortion.
    /// </summary>
    /// <param name="x">The X-Position of the Rect</param>
    /// <param name="y">The Y-Position of the Rect</param>
    /// <param name="width">The Width of the Rect</param>
    /// <param name="height">The Height of the Rect</param>
    /// /// <param name="expectedRadius">The expected corner radius value</param>
    /// <remarks>
    /// Expected conversion behavior:
    /// - Calculates corner radius as half of the minimum dimension
    /// - Works for any rectangle dimensions (portrait or landscape)
    /// - Returns a CornerRadius with equal values for all corners
    /// 
    /// Examples:
    /// - 100x50 rectangle → min(100,50)/2 = 25
    /// - 50x100 rectangle → min(50,100)/2 = 25
    /// - 80x80 rectangle → min(80,80)/2 = 40
    /// </remarks>
    [Theory]
    [InlineData(0, 0, 100, 50, 25)]
    [InlineData(0, 0, 50, 100, 25)]
    [InlineData(0, 0, 80, 80, 40)]
    [InlineData(10, 10, 100, 100, 50)]
    public void Convert_WithValidRect_ReturnsCorrectCornerRadius(double x, double y, double width, double height, double expectedRadius)
    {
        // Arrange
        var rect = new Rect(x, y, width, height);
        var expectedRadiusValue = new CornerRadius(expectedRadius);

        // Act
        var result = _converter.Convert(rect, typeof(CornerRadius), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<CornerRadius>(result);
        Assert.Equal(expectedRadiusValue, (CornerRadius)result);
    }

    /// <summary>
    /// Tests that invalid input values are handled gracefully without exceptions.
    /// Verifies the converter returns DoNothing for unsupported data types.
    /// </summary>
    /// <param name="invalidValue">The invalid input value to test</param>
    /// <remarks>
    /// Invalid input scenarios:
    /// - null values
    /// - Non-Rect objects (strings, numbers, etc.)
    /// - Unexpected data types
    /// 
    /// Expected behavior:
    /// - No exceptions thrown during conversion
    /// - Returns BindingOperations.DoNothing for invalid inputs
    /// - Allows XAML binding system to handle the failure gracefully
    /// 
    /// This defensive approach prevents UI crashes from converter errors.
    /// </remarks>
    [Theory]
    [InlineData(null)]
    [InlineData("not a rect")]
    [InlineData(123)]
    public void Convert_WithInvalidValue_ReturnsDoNothing(object? invalidValue)
    {
        // Act
        var result = _converter.Convert(invalidValue, typeof(CornerRadius), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(BindingOperations.DoNothing, result);
    }

    /// <summary>
    /// Tests that ConvertBack operation throws NotSupportedException.
    /// Verifies the one-way conversion nature of this specific converter.
    /// </summary>
    /// <remarks>
    /// Design decision:
    /// - This converter only supports forward conversion (Rect → CornerRadius)
    /// - Reverse conversion (CornerRadius → Rect) is not mathematically meaningful
    /// - Multiple rectangles could produce the same corner radius value
    /// 
    /// Expected behavior:
    /// - ConvertBack throws NotSupportedException immediately
    /// - Prevents misuse of the converter for two-way bindings
    /// - Clearly documents the converter's one-way nature
    /// </remarks>
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