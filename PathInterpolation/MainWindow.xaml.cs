using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;


namespace PathInterpolation
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IList<Vector3D> originalPath = new List<Vector3D>();
        IInterpolation interpolation = InterpolationFactory.Instance();


        public MainWindow()
        {
            InitializeComponent();

            // Initialize text fields.
            originalPathCount.Content = originalPath.Count;
            interpolatedPathCount.Content = 0;

            // Add points.
            originalPath.Add(new Vector3D(60.0f, 190.0f, 0.0f));
            originalPath.Add(new Vector3D(120.0f, 120.0f, 0.0f));
            originalPath.Add(new Vector3D(160.0f, 90.0f, 0.0f));
            originalPath.Add(new Vector3D(210.0f, 150.0f, 0.0f));
            originalPath.Add(new Vector3D(250.0f, 60.0f, 0.0f));
            originalPath.Add(new Vector3D(350.0f, 110.0f, 0.0f));
            originalPath.Add(new Vector3D(410.0f, 170.0f, 0.0f));
            originalPath.Add(new Vector3D(440.0f, 110.0f, 0.0f));
            originalPath.Add(new Vector3D(160.0f, 200.0f, 0.0f));
            originalPath.Add(new Vector3D(210.0f, 250.0f, 0.0f));
            originalPath.Add(new Vector3D(250.0f, 260.0f, 0.0f));
            originalPath.Add(new Vector3D(350.0f, 210.0f, 0.0f));
            
            // Update text field.
            originalPathCount.Content = originalPath.Count;

            // Paint.
            paint(polyline, originalPath, Colors.White, 8);
        }


        /// <summary>
        /// Click handler for an interpolation.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void Interpolate_Click(object sender, RoutedEventArgs e)
        {
            // Get settings.
            ushort samplingRate;
            if (!UInt16.TryParse(samplingRate_TextField.Text, out samplingRate))
                return;

            // Calculate.
            IList<Vector3D> interpolatedLine = interpolation.Interpolate((UInt16)samplingRate, originalPath);
            if (interpolatedLine == null)
                return;

            // Update the text fields.
            interpolatedPathCount.Content = interpolatedLine.Count;
            originalPathCount.Content = originalPath.Count;

            // Paint.
            polyline.Points.Clear();
            interpolated.Points.Clear();
            dots.Children.Clear();
            paint(polyline, originalPath, Colors.White, 8);
            paint(interpolated, interpolatedLine, Colors.Yellow, 6);
        }



        /// <summary>
        /// Paints a path into a polyline with all points highlighted as dots.
        /// </summary>
        /// <param name="target">The polyline.</param>
        /// <param name="list">The list of points.</param>
        /// <param name="dotColor">The color of each dot.</param>
        /// <param name="dotSize">The width and height of each dot.</param>
        private void paint(Polyline target, IList<Vector3D> list, Color dotColor, uint dotSize)
        {
            foreach (Vector3D point in list)
            {
                target.Points.Add(new Point(point.X, point.Y));
                Ellipse dot = new Ellipse();
                dot.Fill = new SolidColorBrush(dotColor);
                dot.Width = dotSize;
                dot.Height = dotSize;
                Canvas.SetLeft(dot, point.X - 0.5 * dotSize);
                Canvas.SetTop(dot, point.Y - 0.5 * dotSize);
                dots.Children.Add(dot);
            }

        }

        /// <summary>
        /// Event handler for the sampling rate text field key down events.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void samplingRate_TextField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Interpolate_Click(sender, e);
        }
    }
}
