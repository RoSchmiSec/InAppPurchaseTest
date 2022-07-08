using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using InAppPurchaseTest.Models;
using InAppPurchaseTest.ViewModels;
//using Xamarin.InAppPurchasing.iOS;

namespace InAppPurchaseTest.Views
{
    public partial class PurchasePage : ContentPage
    {
       // public Item Item { get; set; }
        public PurchasePage()
        {       
            InitializeComponent();
            BindingContext = new PurchaseViewModel();
        }
    }
}
