using System;
using System.Collections.Generic;
using InAppPurchaseTest.ViewModels;
using InAppPurchaseTest.Views;
using Xamarin.Forms;

namespace InAppPurchaseTest
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        // This is the App specific flag indicating that the item is purchased
        // It is read in the constructor of e.g. PurchaseViewModel.cs
        public bool Global_Is_Purchased_Flag = false;

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(PurchasePage), typeof(PurchasePage));
        }

    }
}

