using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace PedalBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private PedalContext _context = new PedalContext();
        private List<dynamic> groupedList = new List<dynamic>(); 
        private decimal _pedalCost = new decimal(0.00);
        private Component _selectedComponent = new Component();
        private OrderItem _orderItem = new OrderItem();
        private Order order = new Order();
        private Pedal _selectedPedal;
        private decimal _totalCost = new decimal(0.00);

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(string), typeof(Window), new UIPropertyMetadata(string.Empty));
        public string Status {
            get { return (string) this.GetValue(StatusProperty); }
            set { this.SetValue(StatusProperty, value); }
        }

        
        //TODO Find better way to always update status label
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            CollectionViewSource pedalViewSource = ((CollectionViewSource)(this.FindResource("pedalViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            _context.Pedals.Load();
            pedalViewSource.Source = _context.Pedals.Local;

            CollectionViewSource componentViewSource = ((CollectionViewSource)(this.FindResource("componentViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            _context.Components.Load();
            componentViewSource.Source = _context.Components.Local;

            
            CollectionViewSource partViewSource = ((CollectionViewSource)(this.FindResource("partViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            _context.Parts.Load();
            partViewSource.Source = _context.Parts.Local;

            CollectionViewSource orderViewSource = ((CollectionViewSource) (this.FindResource("orderViewSource")));
            orderViewSource.Source = order.Items;

            lstPedals.ItemsSource = order.Pedals;
        }

        private void btnPedalUpdate_Click(object sender, RoutedEventArgs e)
        {
            var changed = _context.SaveChanges();
            pedalDataGrid.Items.Refresh();
            Status = "";
            Status = changed + " pedals changed.";
        }

        private void btnUpdateComponent_Click(object sender, RoutedEventArgs e)
        {
            var changed = _context.SaveChanges();
            componentDataGrid.Items.Refresh();
            Status = "";
            Status = changed + " components changed.";
        }

        private void pedalDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pedalDataGrid.SelectedValue != null && pedalDataGrid.SelectedValue.ToString() != "{NewItemPlaceholder}")
            {
                _selectedPedal = (Pedal)(pedalDataGrid.SelectedValue);
                lblPedalName.Content = _selectedPedal.Name;

                updatePedalCost();
            }
        }

        private void btnAddComponentToPedal_Click(object sender, RoutedEventArgs e)
        {
            if (componentDataGrid.SelectedValue != null && componentDataGrid.SelectedValue.ToString() != "{NewItemPlaceholder}" && _selectedPedal != null && txtPartName.Text.Length > 0)
            {
                _selectedComponent = (Component)(componentDataGrid.SelectedItem);
                Part part = new Part();
                part.Component_ComponentId = _selectedComponent.ComponentId;
                part.Name = txtPartName.Text;
                part.Pedal_PedalId = _selectedPedal.PedalId;
                Status = "";
                Status = part.Name + " added to " + _selectedPedal.Name + ".";
                _context.Parts.Add(part);
                _context.SaveChanges();
                partDataGrid.Items.Refresh();
                updatePedalCost();
                txtPartName.Clear();
            }
        }

        private void updatePedalCost()
        {
            _pedalCost = (decimal) 0.00;
            foreach (Part part in partDataGrid.Items)
            {
                if (part.Component.Price != null) _pedalCost += part.Component.Price.Value;
            }
            
            lblPedalCost.Content = _pedalCost.ToString("#,#.##");
        }

        private void btnDeletePart_Click(object sender, RoutedEventArgs e)
        {
            if (partDataGrid.SelectedValue != null)
            {
                var part = (Part) (partDataGrid.SelectedItem);
                Status = "";
                Status = part.Name + " removed from " + part.Pedal.Name + ".";
                _context.Parts.Remove(part);
                _context.SaveChanges();
                partDataGrid.Items.Refresh();
                updatePedalCost();
                
            }
        }

        private void txtSearchComponents_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtSearchComponents.Text.ToLower();
            ICollectionView collection = CollectionViewSource.GetDefaultView(componentDataGrid.ItemsSource);
            if (search == "")
            {
                collection.Filter = null;
            }
            else
            {
                collection.Filter = o =>
                {
                    Component c = o as Component;
                    if (c.Value.ToLower().Contains(search))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };
            }
        }

        private void btnAddPedalToOrder_Click(object sender, RoutedEventArgs e)
        {
            short quantity;
            bool result = short.TryParse(txtPedalBuildQuantity.Text, out quantity);

            if (_selectedPedal != null && result)
            {
                for (int i = 0; i < quantity; i++)
                {
                    order.Pedals.Add(_selectedPedal);
                }

                txtPedalBuildQuantity.Clear();
                fillOrderListAndUpdateOrderDataGrid();
                Status = "";
                Status = quantity + " " + _selectedPedal.Name + "  added to order.";
            }
        }

        private void fillOrderListAndUpdateOrderDataGrid()
        {
            order.Components.Clear();
            order.Items.Clear();

            foreach (Pedal pedal in order.Pedals)
            {
                foreach (Part part in pedal.Parts)
                {
                    order.Components.Add(part.Component);
                }
            }

            _totalCost = (decimal) 0.00;
            var tempList = order.Components.GroupBy(c => c.ComponentId)
                .Select(c => new
                {
                    Ordered = false,
                    Quantity = c.Count(),
                    Type = c.First().Type,
                    Value = c.First().Value,
                    Notes = c.First().Notes,
                    Url = c.First().Url,
                    Price = c.First().Price
                });

            foreach (var item in tempList)
            {
                _orderItem.Ordered = item.Ordered;
                _orderItem.Quantity = item.Quantity;
                _orderItem.Type = item.Type;
                _orderItem.Value = item.Value;
                _orderItem.Notes = item.Notes;
                _orderItem.Url = item.Url;
                _orderItem.Price = item.Price;
                order.Items.Add(_orderItem);
                _totalCost += (item.Price.Value * item.Quantity);
            }

            lblOrderPedalsQuantity.Content = order.Pedals.Count;
            lblComponentQuantity.Content = order.Components.Count;
            lblOrderTotalCost.Content = _totalCost.ToString("#,#.##");
            lstPedals.Items.Refresh();
            orderDataGrid.Items.Refresh();
        }

        private async void btnFillResistorsClick(object sender, RoutedEventArgs e)
        {
            var added = 0;

            MessageDialogResult result = await this.ShowMessageAsync("Fill With Resistors", "Add many common resistors to the component list?\nResistors will have Type, Value, and Price of 0.00 specified.", MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Affirmative)
            {
                added = SeedDatabase.SeedResistors();
                _context.Components.Load();
                componentDataGrid.Items.Refresh();
            }

            Status = "";
            Status = added + " components added to the list.";
        }

        private void btnRemovePedalFromOrder_Click(object sender, RoutedEventArgs e)
        {
            if (lstPedals.SelectedValue != null)
            {
                var pedal = (Pedal) (lstPedals.SelectedItem);

                Status = "";
                Status = pedal.Name + " removed from order.";
                order.Pedals.Remove(pedal);
                fillOrderListAndUpdateOrderDataGrid();
            }
        }

        private void componentDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var component = (Component) (componentDataGrid.SelectedItem);
            if (component.Url != null)
            {
                Process.Start(component.Url);
            }
        }

        private void orderDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = (OrderItem) (orderDataGrid.SelectedItem);
            if (item.Url != null)
            {
                Process.Start(item.Url);
            }
        }
    }
}
