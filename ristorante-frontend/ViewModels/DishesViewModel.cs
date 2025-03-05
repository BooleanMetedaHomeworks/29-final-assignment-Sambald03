using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ristorante_frontend.ViewModels.Commands;
using System.Windows.Input;
using System.Windows;
using ristorante_frontend.Models;

namespace ristorante_frontend.ViewModels
{
    public class DishesViewModel : INotifyPropertyChanged
    {
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

        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            private set
            {
                if (value == _categories) { return; }

                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        public ICommand AddDishCommand { get; private set; }
        public ICommand SaveDishCommand { get; private set; }
        public ICommand DeleteDishCommand { get; private set; }

        public DishesViewModel()
        {
            _ = Initialize();

            this.AddDishCommand = new MyCommand(async () =>
            {
                Dish newDish = new Dish()
                {
                    Name = "Nuovo Piatto",
                    Description = "Nuova Descrizione",
                    Price = 0.0m
                };

                var createApiResult = await ApiService.CreateArticolo(newDish);

                if (createApiResult.Data == null)
                {
                    MessageBox.Show($"ERRORE! {createApiResult.ErrorMessage}");
                    return;
                }

                newDish.Id = createApiResult.Data;
                Dishes.Add(newDish);
            });

            this.SaveArticoloCommand = new GenericCommand<Articolo>(async (post) =>
            {
                var updateApiResult = await ApiService.UpdateArticolo(post);

                if (updateApiResult.Data == 0)
                {
                    MessageBox.Show($"ERRORE! {updateApiResult.ErrorMessage}");
                    return;
                }
            });

            this.DeleteArticoloCommand = new GenericCommand<Articolo>(async post =>
            {
                var deleteApiResult = await ApiService.DeleteArticolo(post.Id);

                if (deleteApiResult.Data == 0)
                {
                    MessageBox.Show($"ERRORE! {deleteApiResult.ErrorMessage} {ApiServiceResultType.Error}");
                    return;
                }

                Articoli.Remove(post);
            });
        }

        public async Task Initialize()
        {
            var dishApiResult = await ApiService.GetArticoli();
            var categoryApiResult = await ApiService.GetCategorie();

            if (dishApiResult.Data == null)
            {
                MessageBox.Show($"ERRORE! {dishApiResult.ErrorMessage}");
                return;
            }

            Dishes = new ObservableCollection<Dish>(dishApiResult.Data);

            if (categoryApiResult.Data == null)
            {
                MessageBox.Show($"ERRORE! {categoryApiResult.ErrorMessage}");
                return;
            }

            Categories = new ObservableCollection<Category>(categoryApiResult.Data);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
