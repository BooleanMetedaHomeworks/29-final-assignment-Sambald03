using System;
using System.Collections.Generic;
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

namespace ristorante_frontend.Views.Pages
{
    /// <summary>
    /// Interaction logic for DishesPage.xaml
    /// </summary>
    public partial class DishesPage : Page
    {
        public DishesPage()
        {
            InitializeComponent();
        }

        private void OnBackBtn(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Views/MenusPage.xaml", UriKind.Relative));
        }
    }
}
