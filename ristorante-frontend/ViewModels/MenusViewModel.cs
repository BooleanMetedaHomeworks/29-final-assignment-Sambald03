using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ristorante_frontend.Services;
using ristorante_frontend.ViewModels.Commands;
using System.Windows.Input;
using System.Windows;
using ristorante_frontend.Models;
using ristorante_frontend.Views;
using System.Windows.Controls;

namespace ristorante_frontend.ViewModels
{
    public class MenusViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Models.Menu> _menus;
        public ObservableCollection<Models.Menu> Menus
        {
            get { return _menus; }
            private set
            {
                if (value == _menus) { return; }

                _menus = value;
                OnPropertyChanged(nameof(Menus));
            }
        }

        public ICommand AddMenuCommand { get; private set; }
        public ICommand SaveMenuCommand { get; private set; }
        public ICommand DeleteMenuCommand { get; private set; }
        public ICommand RemoveDishIntoMenuCommand { get; private set; }
        public ICommand OpenDishesListWindowCommand { get; private set; }

        public MenusViewModel()
        {
            _ = Initialize();

            this.AddMenuCommand = new MyCommand(async () =>
            {
                Models.Menu newMenu = new Models.Menu()
                {
                    Name = "Nuovo Menu"
                };

                var createApiResult = await ApiService.CreateMenu(newMenu);

                if (createApiResult.Data == null)
                {
                    MessageBox.Show($"ERRORE! {createApiResult.ErrorMessage}");
                    return;
                }

                newMenu.Id = createApiResult.Data;
                Menus.Add(newMenu);
            });

            this.SaveMenuCommand = new GenericCommand<Models.Menu>(async (menu) =>
            {
                var updateApiResult = await ApiService.UpdateMenu(menu);

                if (updateApiResult.Data == 0)
                {
                    MessageBox.Show($"ERRORE! {updateApiResult.ErrorMessage}");
                    return;
                }
            });

            this.DeleteMenuCommand = new GenericCommand<Models.Menu>(async (menu) =>
            {
                var deleteApiResult = await ApiService.DeleteMenu(menu.Id);

                if (deleteApiResult.Data == 0)
                {
                    MessageBox.Show($"ERRORE! {deleteApiResult.ErrorMessage}");
                    return;
                }

                Menus.Remove(menu);
            });

            this.RemoveDishIntoMenuCommand = new GenericCommand<Dish>(async (dish) =>
            {
                var button = Mouse.DirectlyOver as Button;
                if (button?.Tag is Models.Menu menu)
                {
                    var updateApiResult = await ApiService.DeleteDishIntoMenu(menu.Id, dish.Id);

                    if (updateApiResult.Data == 0)
                    {
                        MessageBox.Show($"ERRORE! {updateApiResult.ErrorMessage}");
                        return;
                    }
                }
            });

            this.OpenDishesListWindowCommand = new GenericCommand<Models.Menu>((menu) =>
            {
                var viewModel = new DishesListViewModel(menu.Id);
                var window = new DishesWindow(viewModel);

                window.ShowDialog();
            });
        }

        public async Task Initialize()
        {
            var menuApiResult = await ApiService.GetMenus();
            //var categoryApiResult = await ApiService.GetCategorie();

            if (menuApiResult.Data == null)
            {
                MessageBox.Show($"ERRORE! {menuApiResult.ErrorMessage}");
                return;
            }

            Menus = new ObservableCollection<Models.Menu>(menuApiResult.Data);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
