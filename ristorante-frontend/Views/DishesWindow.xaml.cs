﻿using System;
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
using System.Windows.Shapes;
using ristorante_frontend.ViewModels;

namespace ristorante_frontend.Views
{
    /// <summary>
    /// Logica di interazione per DishesWindow.xaml
    /// </summary>
    public partial class DishesWindow : Window
    {
        public DishesWindow(DishesListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.RequestClose += () => this.DialogResult = true;
        }
    }
}
