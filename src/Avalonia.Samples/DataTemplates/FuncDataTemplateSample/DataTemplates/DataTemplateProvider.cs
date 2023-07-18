using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using FuncDataTemplateSample.Models;

namespace FuncDataTemplateSample.DataTemplates
{
    // Path-Data taken from here: https://icons.getbootstrap.com
    public static class DataTemplateProvider
    {
        // This FuncDataTemplate can be static, as it will not change over time.
        public static FuncDataTemplate<Person> GenderDataTemplate { get; } 
            = new FuncDataTemplate<Person>(
                // Check if we have a valid object and return true if it is valid. 
                (person) => person is not null,

                // Avalonia will provide the Person automatically as the functions parameter.
                BuildGenderPresenter);


        // This private function will return a control that represents our persons sex as a gender symbol.
        private static Control BuildGenderPresenter(Person person)
        {
            // Create a new Path as a presenter. You can also use any other Control.
            // If you want to add more than one control, remember to wrap them inside a Panel.
            Path path = new Path()
            {
                Width = 32,
                Height = 32,

                // We set Stretch to Uniform. That way our Path will be made as big as needed while keeping the aspect ratio.
                Stretch = Stretch.Uniform,

                // Create a Binding for the ToolTip
                [!ToolTip.TipProperty] = new Binding(nameof(person.Sex))
            };


            switch (person.Sex)
            {
                case Sex.Diverse:
                    // We use StreamGeometry.Parse() to get the needed Data.
                    path.Data = StreamGeometry.Parse("M 0,9.3750041 A 9.375,9.375 0 0 1 9.375,4.0698515e-6 h 56.25 A 9.375,9.375 0 0 1 65.625,18.750004 H 32.00625 L 65.625,52.368754 77.7375,40.237504 a 9.3868425,9.3868425 0 1 1 13.275,13.275 l -12.13125,12.1125 15.4125,15.4125 A 74.8125,74.8125 0 0 1 150,56.250004 c 22.125,0 41.98125,9.5625 55.70625,24.7875 l 62.2875,-62.2875 H 215.625 a 9.375,9.375 0 0 1 0,-18.7499999301485 h 75 A 9.375,9.375 0 0 1 300,9.3750041 V 84.375004 a 9.375,9.375 0 0 1 -18.75,0 v -52.36875 l -64.6875,64.6875 A 75,75 0 0 1 159.375,205.6875 V 243.75 H 187.5 a 9.375,9.375 0 0 1 0,18.75 h -28.125 v 28.125 a 9.375,9.375 0 0 1 -18.75,0 V 262.5 H 112.5 a 9.375,9.375 0 0 1 0,-18.75 h 28.125 V 205.6875 A 75,75 0 0 1 83.4375,96.675004 L 65.625,78.881254 53.5125,91.012504 a 9.3868425,9.3868425 0 1 1 -13.275,-13.275 L 52.36875,65.625004 18.75,32.006254 v 33.61875 a 9.375,9.375 0 0 1 -18.75,0 z M 102.9375,100.425 a 56.258862,56.258862 0 1 0 94.125,61.65 56.258862,56.258862 0 0 0 -94.125,-61.65 z");

                    // We can set Fill to any Brush. We can also look up a Brush in Resources, if needed. 
                    path.Fill = new LinearGradientBrush
                    {
                        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                        EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                        GradientStops =
                        {
                            new GradientStop(Colors.Red, 0),
                            new GradientStop(Colors.Orange, 0.2),
                            new GradientStop(Colors.Yellow, 0.4),
                            new GradientStop(Colors.DarkTurquoise, 0.6),
                            new GradientStop(Colors.Blue, 0.8),
                            new GradientStop(Colors.Violet, 1),
                        }
                    };
                    break;
                case Sex.Female:
                    path.Data = StreamGeometry.Parse("m 150,18.748842 a 75,75 0 1 0 0,149.999998 75,75 0 0 0 0,-149.999998 z m -93.75,75 a 93.75,93.75 0 1 1 103.125,93.281248 v 37.96875 h 37.5 a 9.375,9.375 0 0 1 0,18.75 h -37.5 v 46.875 a 9.375,9.375 0 0 1 -18.75,0 v -46.875 h -37.5 a 9.375,9.375 0 0 1 0,-18.75 h 37.5 V 187.03009 A 93.75,93.75 0 0 1 56.25,93.748842 Z");
                    path.Fill = new SolidColorBrush(Colors.DeepPink);
                    break;
                case Sex.Male:
                    path.Data = StreamGeometry.Parse("m 178.125,37.539801 a 9.375,9.375 0 0 1 0,-18.75 h 93.75 a 9.375,9.375 0 0 1 9.375,9.375 V 121.9148 a 9.375,9.375 0 0 1 -18.75,0 V 50.796051 L 185.08125,128.2148 A 93.75,93.75 0 1 1 171.825,114.95855 L 249.24375,37.539801 Z M 112.5,112.5398 a 75,75 0 1 0 0,150 75,75 0 0 0 0,-150 z");
                    path.Fill = new SolidColorBrush(Colors.Blue);
                    break;
                default:
                    // Fall-back value
                    return new TextBlock { Text = "NOT SUPPORTED" };
            }

            return path;
        }
    }
}
