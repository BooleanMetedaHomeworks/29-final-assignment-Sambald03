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
    public class DishesListViewModel : INotifyPropertyChanged
    {
        private int _idMenu;
        public int IdMenu
        {
            get { return _idMenu; }
            private set
            {
                if (value == _idMenu) { return; }

                _idMenu = value;
                OnPropertyChanged(nameof(IdMenu));
            }
        }

        private ObservableCollection<Dish> _dishes;
        public ObservableCollection<Dish> Dishes
        {
            get { return _dishes; }
            private set
            {
                if (value == _dishes) { return; }

                _dishes = value;
                OnPropertyChanged(nameof(Dishes));
            }
        }

        public ICommand AddDishIntoMenuCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        public event Action RequestClose;

        public DishesListViewModel(int idMenu)
        {
            _ = Initialize();

            this.IdMenu = idMenu;

            this.AddDishIntoMenuCommand = new GenericCommand<Dish>(async (dish) =>
            {
                var updateApiResult = await ApiService.CreateDishIntoMenu(IdMenu, dish);

                if (updateApiResult.Data == 0)
                {
                    MessageBox.Show($"ERRORE! {updateApiResult.ErrorMessage}");
                    return;
                }

                CloseWindow();
            });

            this.CloseCommand = new MyCommand(CloseWindow);
        }

        public async Task Initialize()
        {
            var dishApiResult = await ApiService.GetDishes();

            if (dishApiResult.Data == null)
            {
                MessageBox.Show($"ERRORE! {dishApiResult.ErrorMessage}");
                return;
            }

            Dishes = new ObservableCollection<Dish>(dishApiResult.Data);

            /*
            Dishes = new ObservableCollection<Dish>()
            {
                new Dish()
                {
                    Name = "Piatto da scegliere 1",
                    Description = "Descrizione 1",
                    Price = 1
                },
                new Dish()
                {
                    Name = "Piatto da scegliere 2",
                    Description = "Descrizione 2",
                    Price = 2
                },
                new Dish()
                {
                    Name = "Piatto da scegliere 3",
                    Description = "Descrizione 3",
                    Price = 3
                }
            };
            */

        }

        private void CloseWindow()
        {
            RequestClose?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
