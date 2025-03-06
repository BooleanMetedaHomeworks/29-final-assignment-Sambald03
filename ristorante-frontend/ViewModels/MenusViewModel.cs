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

namespace ristorante_frontend.ViewModels
{
    public class MenusViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Menu> _menus;
        public ObservableCollection<Menu> Menus
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

        public MenusViewModel()
        {
            _ = Initialize();

            this.AddMenuCommand = new MyCommand(async () =>
            {
                Menu newMenu = new Menu()
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

            this.SaveMenuCommand = new GenericCommand<Menu>(async (menu) =>
            {
                var updateApiResult = await ApiService.UpdateMenu(menu);

                if (updateApiResult.Data == 0)
                {
                    MessageBox.Show($"ERRORE! {updateApiResult.ErrorMessage}");
                    return;
                }
            });

            this.DeleteMenuCommand = new GenericCommand<Menu>(async (menu) =>
            {
                var deleteApiResult = await ApiService.DeleteMenu(menu.Id);

                if (deleteApiResult.Data == 0)
                {
                    MessageBox.Show($"ERRORE! {deleteApiResult.ErrorMessage}");
                    return;
                }

                Menus.Remove(menu);
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

            Menus = new ObservableCollection<Menu>(menuApiResult.Data);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
