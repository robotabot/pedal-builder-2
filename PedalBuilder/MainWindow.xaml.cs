using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PedalBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PedalsContext _context = new PedalsContext();
        private decimal pedalCost = new decimal(0.00);
        private Component _selectedComponent = new Component();
        private Pedal _selectedPedal = new Pedal();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource pedalViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("pedalViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            _context.Pedals.Load();
            pedalViewSource.Source = _context.Pedals.Local;

            System.Windows.Data.CollectionViewSource componentViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("componentViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            _context.Components.Load();
            componentViewSource.Source = _context.Components.Local;

            
            System.Windows.Data.CollectionViewSource partViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("partViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            _context.Parts.Load();
            partViewSource.Source = _context.Parts.Local;
        }

        private void btnPedalUpdate_Click(object sender, RoutedEventArgs e)
        {
            foreach (var pedal in _context.Pedals.Local.ToList().Where(pedal => pedal.Name == null))
            {
                _context.Pedals.Remove(pedal);
            }

            _context.SaveChanges();
            pedalDataGrid.Items.Refresh();
        }

        private void btnUpdateComponent_Click(object sender, RoutedEventArgs e)
        {
            foreach (var component in _context.Components.Local.ToList().Where(component => component.Type == null))
            {
                _context.Components.Remove(component);
            }

            _context.SaveChanges();
            componentDataGrid.Items.Refresh();
        }

        private void pedalDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (pedalDataGrid.SelectedItem.GetType().FullName != "MS.Internal.NamedObject")
            {
                _selectedPedal = (Pedal)(pedalDataGrid.SelectedItem);
                lblPedalName.Content = _selectedPedal.Name;

                foreach (Part part in partDataGrid.Items)
                {
                    if (part.Component.Price != null) pedalCost += part.Component.Price.Value;
                }

                lblPedalCost.Content = pedalCost.ToString(CultureInfo.InvariantCulture);

            }
        }

        private void btnAddComponentToPedal_Click(object sender, RoutedEventArgs e)
        {
            if (componentDataGrid.SelectedItem.GetType().FullName != "MS.Internal.NamedObject" && _selectedPedal != null && txtPartName.Text.Length > 0)
            {
                _selectedComponent = (Component)(componentDataGrid.SelectedItem);
                Part part = new Part();
                part.Component_ComponentId = _selectedComponent.ComponentId;
                part.Name = txtPartName.Text;
                part.Pedal_PedalId = _selectedPedal.PedalId;
                _context.Parts.Add(part);
                _context.SaveChanges();
            }
        }
    }
}
