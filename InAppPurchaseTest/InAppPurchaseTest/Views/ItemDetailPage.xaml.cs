using System.ComponentModel;
using Xamarin.Forms;
using InAppPurchaseTest.ViewModels;

namespace InAppPurchaseTest.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
