using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace WPFNodeEditor
{
    public class Node : INotifyPropertyChanged
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public double X { get; set; }
        public double Y { get; set; }
        public string? Title { get; set; }
        public ObservableCollection<ConnectionPoint> Inputs { get; } = new ObservableCollection<ConnectionPoint>();
        public ObservableCollection<ConnectionPoint> Outputs { get; } = new ObservableCollection<ConnectionPoint>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ConnectionPoint : INotifyPropertyChanged
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public Node ParentNode { get; }
        public bool IsInput { get; }
        public Point Position { get; set; }

        public ConnectionPoint(Node parentNode, bool isInput)
        {
            ParentNode = parentNode;
            IsInput = isInput;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Connection : INotifyPropertyChanged
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public ConnectionPoint? From { get; set; }
        public ConnectionPoint? To { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}