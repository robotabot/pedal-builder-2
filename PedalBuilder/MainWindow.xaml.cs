using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls;

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
        private Order order = new Order();
        private Pedal _selectedPedal;
        private decimal _totalCost = new decimal(0.00);
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
            orderViewSource.Source = groupedList;

            lstPedals.ItemsSource = order.Pedals;
        }

        private void btnPedalUpdate_Click(object sender, RoutedEventArgs e)
        {
            _context.SaveChanges();
            pedalDataGrid.Items.Refresh();
        }

        private void btnUpdateComponent_Click(object sender, RoutedEventArgs e)
        {
            _context.SaveChanges();
            componentDataGrid.Items.Refresh();
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
            }
        }

        private void fillOrderListAndUpdateOrderDataGrid()
        {
            order.Components.Clear();
            groupedList.Clear();

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
                    Checked = false,
                    Quantity = c.Count(),
                    Type = c.First().Type,
                    Value = c.First().Value,
                    Notes = c.First().Notes,
                    Url = c.First().Url,
                    Price = c.First().Price
                });

            foreach (var item in tempList)
            {
                groupedList.Add(item);
                _totalCost += (item.Price.Value * item.Quantity);
            }

            lblOrderPedalsQuantity.Content = order.Pedals.Count;
            lblOrderTotalCost.Content = _totalCost.ToString("#,#.##");
            lstPedals.Items.Refresh();
            orderDataGrid.Items.Refresh();
        }

        //TODO Use Mahapps dialog box, move into SeedDatabase class, add progress dialog.
        private void btnFillResistorsClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Add many common resistors to the component list?\nResistors will have Type, Value, and Price of 0.00 specified.",
                    "Autofill Resistors", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                SeedDatabase.SeedResistors();
                _context.Components.Load();
                componentDataGrid.Items.Refresh();
            }
        }

        private void btnRemovePedalFromOrder_Click(object sender, RoutedEventArgs e)
        {
            if (lstPedals.SelectedValue != null)
            {
                var pedal = (Pedal) (lstPedals.SelectedItem);
                order.Pedals.Remove(pedal);
                fillOrderListAndUpdateOrderDataGrid();
            }
        }
    }
}
