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

namespace ristorante_frontend.ViewModels
{
    public class DishesViewModel : INotifyPropertyChanged
    {
        private Categoria _categoria { get; set; }
        public Categoria Categoria
        {
            get { return _categoria; }
            private set
            {
                if (value == _categoria) { return; }

                _categoria = value;
                OnPropertyChanged(nameof(Categoria));
            }
        }


        private ObservableCollection<Categoria> _categorie;
        public ObservableCollection<Categoria> Categorie
        {
            get { return _categorie; }
            private set
            {
                if (value == _categorie) { return; }

                _categorie = value;
                OnPropertyChanged(nameof(Categorie));
            }
        }

        private ObservableCollection<Articolo> _articoli;
        public ObservableCollection<Articolo> Articoli
        {
            get { return _articoli; }
            private set
            {
                if (value == _articoli) { return; }

                _articoli = value;
                OnPropertyChanged(nameof(Articoli));
            }
        }

        public ICommand AddArticoloCommand { get; private set; }
        public ICommand SaveArticoloCommand { get; private set; }
        public ICommand DeleteArticoloCommand { get; private set; }

        public ArticoloViewModel()
        {
            _ = Initialize();

            this.AddArticoloCommand = new MyCommand(async () =>
            {
                Articolo newArticolo = new Articolo()
                {
                    Titolo = "Nuovo Articolo",
                    Contenuto = "Nuova Descrizione",
                    isPrimaPagina = false
                };

                var createApiResult = await ApiService.CreateArticolo(newArticolo);

                if (createApiResult.Data == null)
                {
                    MessageBox.Show($"ERRORE! {createApiResult.ErrorMessage}");
                    return;
                }

                newArticolo.Id = createApiResult.Data;
                Articoli.Add(newArticolo);
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
            var postsApiResult = await ApiService.GetArticoli();
            var categoriaApiResult = await ApiService.GetCategorie();

            if (postsApiResult.Data == null)
            {
                MessageBox.Show($"ERRORE! {postsApiResult.ErrorMessage}");
                return;
            }

            Articoli = new ObservableCollection<Articolo>(postsApiResult.Data);

            if (categoriaApiResult.Data == null)
            {
                MessageBox.Show($"ERRORE! {postsApiResult.ErrorMessage}");
                return;
            }
            Categorie = new ObservableCollection<Categoria>(categoriaApiResult.Data);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
