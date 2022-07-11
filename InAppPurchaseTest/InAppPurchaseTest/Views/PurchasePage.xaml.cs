using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using InAppPurchaseTest.Models;
using InAppPurchaseTest.ViewModels;


namespace InAppPurchaseTest.Views
{
    public partial class PurchasePage : ContentPage
    {
        PurchaseViewModel vm;

        public PurchasePage()
        {       
            InitializeComponent();
            vm = new PurchaseViewModel();
            BindingContext = vm;
            
        }

        #region OnAppearing
        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.PurchasePage_OnAppearingCommand();  
        }
        #endregion
    }
}
