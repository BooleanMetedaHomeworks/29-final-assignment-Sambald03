﻿using System;
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

            var menuApiResult = await ApiService.GetMenuById(IdMenu);

            if (menuApiResult.Data == null)
            {
                MessageBox.Show($"ERRORE! {menuApiResult.ErrorMessage}");
                return;
            }

            List<Dish> dishes = dishApiResult.Data;

            for (int i = 0; i < dishes.Count; i++)
            {
                foreach (int menuDishId in menuApiResult.Data.DishIds)
                {
                    if (dishes[i].Id == menuDishId)
                    {
                        dishes.Remove(dishes[i]);
                        break;
                    }
                }
            }

            Dishes = new ObservableCollection<Dish>(dishes);
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
