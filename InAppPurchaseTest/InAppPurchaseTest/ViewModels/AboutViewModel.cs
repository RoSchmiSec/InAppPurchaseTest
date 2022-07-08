using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.InAppBilling;
using System.Collections.ObjectModel;

using InAppPurchaseTest.Models;
using InAppPurchaseTest.Views;

namespace InAppPurchaseTest.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; }


        public ICommand OpenWebCommand { get; }
        public ICommand Button_InAppPurchase_Clicked_Command{ get; }

        Item item = new Item() {Id = "1", Description = "None", Text= "None"};

        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            Button_InAppPurchase_Clicked_Command = new  Command(Button_InAppPurchase_Clicked_Action);   
        }

        async void Button_InAppPurchase_Clicked_Action()
        {
            //PurchaseViewModel
            // This will push the ItemDetailPage onto the navigation stack
            //await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");

            


           

            Item purchaseItem = new Item() {Id="InAppPurchase", Description="", Text="InAppPurchase"};

            var nameofThePage = nameof(PurchasePage);
            var nameoftheViewModel = nameof(PurchaseViewModel);
            var nameoftheItemId = nameof(PurchaseViewModel.ItemId);

            // Important: No not spaces are allowed in the parameter (e.g. around the = character)
            await Shell.Current.GoToAsync($"{nameof(PurchasePage)}?{nameof(PurchaseViewModel.ItemId)}={purchaseItem.Id}");

           
           bool result = await MakePurchase();


            await Task.Delay(5);
            
        }

        public async Task<bool> MakePurchase()
        {
            if (!CrossInAppBilling.IsSupported)
            {
                return false;
            }

            var billing = Plugin.InAppBilling.CrossInAppBilling.Current;
            try
            {
                
                var connected = await billing.ConnectAsync();


                return !connected ? false : true;
                
                    

                //make additional billin calls
            }
            finally
            {
                await billing.DisconnectAsync();
            }

        }

        
    }
}
