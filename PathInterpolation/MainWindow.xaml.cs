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
        IList<Vector3D> pathOriginal = new List<Vector3D>();
        IList<Vector3D> pathInterpolation = null;
        IInterpolation interpolation = InterpolationFactory.Instance();


        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Initialize text fields.
            originalPathCount.Content = pathOriginal.Count;
            interpolatedPathCount.Content = 0;

            // Add points.
            pathOriginal.Add(new Vector3D(60.0f, 190.0f, 0.0f));
            pathOriginal.Add(new Vector3D(120.0f, 120.0f, 0.0f));
            pathOriginal.Add(new Vector3D(160.0f, 90.0f, 0.0f));
            pathOriginal.Add(new Vector3D(210.0f, 150.0f, 0.0f));
            pathOriginal.Add(new Vector3D(250.0f, 60.0f, 0.0f));
            pathOriginal.Add(new Vector3D(350.0f, 110.0f, 0.0f));
            pathOriginal.Add(new Vector3D(410.0f, 170.0f, 0.0f));
            pathOriginal.Add(new Vector3D(440.0f, 110.0f, 0.0f));
            pathOriginal.Add(new Vector3D(160.0f, 200.0f, 0.0f));
            pathOriginal.Add(new Vector3D(210.0f, 250.0f, 0.0f));
            pathOriginal.Add(new Vector3D(250.0f, 260.0f, 0.0f));
            pathOriginal.Add(new Vector3D(350.0f, 210.0f, 0.0f));
            
            // Update text field.
            originalPathCount.Content = pathOriginal.Count;

            // Paint.
            Repaint();
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
            pathInterpolation = interpolation.Interpolate((UInt16)samplingRate, pathOriginal);

            Repaint();
        }

        /// <summary>
        /// Updates the text fields.
        /// </summary>
        private void UpdateUserInterface()
        {
            if (polylineOriginal != null && pathOriginal != null)
                originalPathCount.Content = pathOriginal.Count;
            else
                originalPathCount.Content = 0;

            if (polylineInterpolation != null && pathInterpolation != null)
                interpolatedPathCount.Content = pathInterpolation.Count;
            else
                interpolatedPathCount.Content = 0;
        }

        /// <summary>
        /// Repaints all lines with the current points.
        /// </summary>
        private void Repaint()
        {
            ClearScreen();

            // Paint.
            if (polylineOriginal != null && pathOriginal != null)
                Draw(polylineOriginal, pathOriginal, Colors.White, 8);
            if (polylineInterpolation != null && pathInterpolation != null)
                Draw(polylineInterpolation, pathInterpolation, Colors.Yellow, 6);

            UpdateUserInterface();
        }

        /// <summary>
        /// Removes the points and dots from the screen only.
        /// </summary>
        private void ClearScreen()
        {
            if (polylineOriginal != null)
                polylineOriginal.Points.Clear();
            if (polylineInterpolation != null)
                polylineInterpolation.Points.Clear();
            dots.Children.Clear();
        }

        /// <summary>
        /// Deletes the points from all lines.
        /// </summary>
        private void ClearLines()
        {
            pathOriginal = null;
            pathInterpolation = null;
            ClearScreen();
        }

        /// <summary>
        /// Draws a path into a polyline with all points highlighted as dots.
        /// </summary>
        /// <param name="target">The polyline.</param>
        /// <param name="list">The list of points.</param>
        /// <param name="dotColor">The color of each dot.</param>
        /// <param name="dotSize">The width and height of each dot.</param>
        private void Draw(Polyline target, IList<Vector3D> list, Color dotColor, uint dotSize)
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

        /// <summary>
        /// Appends a point to the original path.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(canvas);
            if (pathOriginal == null)
                pathOriginal = new List<Vector3D>();
            pathOriginal.Add(new Vector3D(mousePos.X, mousePos.Y, 0.0f));
            pathInterpolation = null;
            Repaint();
        }

        /// <summary>
        /// Removes all points from all lines.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void clearLinesButton_Click(object sender, RoutedEventArgs e)
        {
            ClearLines();
            UpdateUserInterface();
        }

        /// <summary>
        /// Removes the last point from the original path.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void undoLastButton_Click(object sender, RoutedEventArgs e)
        {
            if (pathOriginal == null)
                return;
            pathOriginal.RemoveAt(pathOriginal.Count - 1);
            pathInterpolation = null;
            Repaint();
        }
    }
}
