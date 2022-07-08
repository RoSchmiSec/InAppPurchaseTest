using System;
using System.Collections.Generic;
using InAppPurchaseTest.ViewModels;
using InAppPurchaseTest.Views;
using Xamarin.Forms;

namespace InAppPurchaseTest
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(PurchasePage), typeof(PurchasePage));
        }

    }
}

