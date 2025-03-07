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

        private Models.Menu _selectedMenu;
        public Models.Menu SelectedMenu
        {
            get { return _selectedMenu; }
            set
            {
                if (_selectedMenu != value)
                {
                    _selectedMenu = value;
                    OnPropertyChanged(nameof(SelectedMenu));
                }
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

            /*
            this.RemoveDishIntoMenuCommand = new GenericCommand<Dish>(async (dish) =>
            {
                var updateApiResult = await ApiService.DeleteDishIntoMenu(SelectedMenu.Id, dish.Id);

                if (updateApiResult.Data == 0)
                {
                    MessageBox.Show($"ERRORE! {updateApiResult.ErrorMessage}");
                    return;
                }

                _ = Initialize();
            });
            */

            //NON FUNZIA -> NON CALCOLARE IL CONVERTER
            this.RemoveDishIntoMenuCommand = new GenericCommand<Tuple<int,int>>(async (menuAndDishIds) =>
            {
                int idMenu = menuAndDishIds.Item1;
                int idDish = menuAndDishIds.Item2;

                var updateApiResult = await ApiService.DeleteDishIntoMenu(idMenu, idDish);

                if (updateApiResult.Data == 0)
                {
                    MessageBox.Show($"ERRORE! {updateApiResult.ErrorMessage}");
                    return;
                }

                _ = Initialize();
            });

            this.OpenDishesListWindowCommand = new GenericCommand<Models.Menu>((menu) =>
            {
                var viewModel = new DishesListViewModel(menu.Id);
                var window = new DishesWindow(viewModel);

                window.ShowDialog();

                _ = Initialize();
            });
        }

        public async Task Initialize()
        {
            var menuApiResult = await ApiService.GetMenus();

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
