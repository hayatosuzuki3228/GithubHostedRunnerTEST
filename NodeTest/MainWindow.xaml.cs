using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeTest
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Node> Nodes { get; set; }
        public ObservableCollection<Connection> Connections { get; set; }
        private ConnectionPoint? _draggedConnectionPoint;

        public MainWindow()
        {
            InitializeComponent();
            Nodes = new ObservableCollection<Node>();
            Connections = new ObservableCollection<Connection>();
            DataContext = this;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(NodeCanvas);
            var newNode = new Node { X = position.X, Y = position.Y, Title = $"Node {Nodes.Count + 1}" };

            // Add input and output connection points
            newNode.Inputs.Add(new ConnectionPoint(newNode, true));
            newNode.Outputs.Add(new ConnectionPoint(newNode, false));

            Nodes.Add(newNode);
        }

        private void ConnectionPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _draggedConnectionPoint = (sender as FrameworkElement)?.DataContext as ConnectionPoint;
            e.Handled = true;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedConnectionPoint != null)
            {
                // Update temporary connection line
                // This part would be implemented in the XAML
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_draggedConnectionPoint != null)
            {
                var endPoint = e.GetPosition(NodeCanvas);
                var targetConnectionPoint = FindConnectionPointAt(endPoint);

                if (targetConnectionPoint != null && CanConnect(_draggedConnectionPoint, targetConnectionPoint))
                {
                    CreateConnection(_draggedConnectionPoint, targetConnectionPoint);
                }
            }
        }

        private ConnectionPoint? FindConnectionPointAt(Point point)
        {
            // Implement logic to find a connection point near the given point
            // This is a simplified version and may need to be more sophisticated in a real application
            foreach (var node in Nodes)
            {
                foreach (var input in node.Inputs)
                {
                    if (IsPointNear(point, input.Position))
                        return input;
                }
                foreach (var output in node.Outputs)
                {
                    if (IsPointNear(point, output.Position))
                        return output;
                }
            }
            return null;
        }

        private bool IsPointNear(Point p1, Point p2, double threshold = 10)
        {
            return Math.Abs(p1.X - p2.X) < threshold && Math.Abs(p1.Y - p2.Y) < threshold;
        }

        private bool CanConnect(ConnectionPoint from, ConnectionPoint to)
        {
            // Check if the connection is valid based on the rules
            if (from.IsInput == to.IsInput)
                return false;

            var output = from.IsInput ? to : from;
            var input = from.IsInput ? from : to;

            // Check if the output is already connected
            return !Connections.Any(c => c.From == output);
        }

        private void CreateConnection(ConnectionPoint from, ConnectionPoint to)
        {
            var connection = new Connection
            {
                From = from.IsInput ? to : from,
                To = from.IsInput ? from : to
            };
            Connections.Add(connection);
        }
    }

    public class Node : INotifyPropertyChanged
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string? Title { get; set; }
        public ObservableCollection<ConnectionPoint> Inputs { get; set; } = new ObservableCollection<ConnectionPoint>();
        public ObservableCollection<ConnectionPoint> Outputs { get; set; } = new ObservableCollection<ConnectionPoint>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Connection
    {
        public ConnectionPoint? From { get; set; }
        public ConnectionPoint? To { get; set; }
    }

    public class ConnectionPoint : INotifyPropertyChanged
    {
        public Node Node { get; set; }
        public bool IsInput { get; set; }
        public Point Position { get; set; }

        public ConnectionPoint(Node node, bool isInput)
        {
            Node = node;
            IsInput = isInput;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}