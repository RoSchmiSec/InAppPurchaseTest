using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Windows.Input;
using Plugin.InAppBilling;
using Xamarin.InAppPurchasing;
using Xamarin.Essentials;
using InAppPurchaseTest.Interfaces;
using System.Net.Http;
using Newtonsoft.Json.Linq;
    

// Relocated to InAppPurchase.iOS.Dependencies

[assembly: Dependency(typeof(MyIAPVerification))]

    public class MyIAPVerification : IInAppBillingVerifyPurchase
    {
        public async Task<bool> VerifyPurchase (string signedData, string signature, string productId = null, string transactionId = null)
        {
            // The Receipt comes in the parameter signedData.
            // What we have to do, is to check, if this Receipt is a valid.
            // If we find it to be valid, we return true, otherweise false.
            // We can do the validation with the AppStore.
            // see: -https://developer.apple.com/documentation/storekit/in-app_purchase/original_api_for_in-app_purchase/validating_receipts_with_the_app_store

            Uri uriProduction = new Uri($"https://buy.itunes.apple.com/verifyReceipt");   
            Uri uriSandbox = new Uri($"https://sandbox.itunes.apple.com/verifyReceipt");

            
            String workData = signedData;

            workData.Replace(@"+", @"%2B");
            workData.Replace(@"\n", @"");
            workData.Replace(@"\r", @"");

            /*
            workData= signedData.Substring(0,signedData.Length - 2);
            workData = workData + 'a';
            */

            //AppleReceipt appleReceipt = new AppleReceipt() { BundleId= "com.RoSchmiInAppPurchaseTest", Id="productId", Data = Convert.FromBase64String(workData), TransactionId="345" };


            AppleReceipt appleReceipt = null; 
            try
            { 
                appleReceipt = new AppleReceipt() {Data = Convert.FromBase64String(workData), BundleId = "com.RoSchmiInAppPurchaseTest", Id= productId, TransactionId = transactionId};
            }
            catch (Exception except)
            {
                string mess = except.Message;
            }

            

            
            //string jsonContent = new JObject(new JProperty("receipt-data", content)).ToString();

            string jsonContent = new JObject(new JProperty("receipt-data", appleReceipt.Data)).ToString();

            HttpResponseMessage response = null;
            //using (HttpClient httpClient = new HttpClient() { BaseAddress = uriSandbox})
            //{
            HttpClient httpClient = new HttpClient() { BaseAddress = uriProduction};
               
            response = await httpClient.PostAsync(uriSandbox, new StringContent(jsonContent));

            System.Net.HttpStatusCode? statusCode = null;
            string responseContent = null;

            if (response.IsSuccessStatusCode)
            {
                statusCode = response.StatusCode;
            }
            else
            {
                statusCode = response.StatusCode;
            }
                 
            
            if (response != null)
            {
                responseContent = await response.Content.ReadAsStringAsync();
                
            }

            // The Receipt comes in the parameter signedData.
            // What we have to do, is to check, if this Receipt is a valid.
            // If we find it to be valid, we return true, otherweise false.
            // We can do the validation with the AppStore.
            // see: -https://developer.apple.com/documentation/storekit/in-app_purchase/original_api_for_in-app_purchase/validating_receipts_with_the_app_store

            // Did not get it implemented in the time I wanted to spend,so I made a dummy/fake comparison with itself
           
            //AppleReceipt appleReceipt = new AppleReceipt() {Data = Convert.FromBase64String(signedData), BundleId = "", Id= productId};
            //NSUrlSessionHandler httpClient = new NSUrlSessionHandler();
            //var  content = new JsonContent(appleReceipt);
            //private const string BaseUrl = "Hallo";
            //NSUrl requestUrl = NSUrl.FromString(BaseUrl + content);
            
       
            // Retrieve AppleReceipt from iPhone storage
            /*
            NSData receiptUrl = null;
            if (NSBundle.MainBundle.AppStoreReceiptUrl != null)
            {
                receiptUrl = NSData.FromUrl(NSBundle.MainBundle.AppStoreReceiptUrl);
            }

            string theReceipt = receiptUrl?.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
            
            bool returnResult = signedData ==  theReceipt ? true : false;
            */

            return true;
        }
    }

   
namespace InAppPurchaseTest.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]

	public class PurchaseViewModel : BaseViewModel

    {   
        string myAppProductId;

        private string itemId;
        private string connectionResult;
        private Color connectionColor;
        private ConnState connectionState;
        
        private string itemName;
        private string itemDescription_1;
        private string itemDescription_2;
        private string itemDuration;
        private string itemPrice;

        private bool itemIsPurchased;
        private bool itemReceiptIsVerified;
        private bool itemReceiptIsVerifiedExternally;

        private InAppBillingProduct inAppBillingProduct;

        private AppleReceipt myAppReceipt = null;

        
        PurchaseService _purchaseService;

        #region Region enum ConnState
        public enum ConnState
        {
            ConnectionOK,
            BillingNotSupported,
            AppStoreUnavailable,
            BillingUnavailable,
            PaymentInvalid,
            PaymentNotAllowed,
            OtherError,
            NoInternetConnection
        }
        #endregion
 
        #region EnumerationError Messages
        /// <summary>
        /// Type of purchase error
        /// </summary>
        public enum PurchaseError
    {
        /// <summary>
        /// Billing system unavailable
        /// </summary>
        BillingUnavailable,
        /// <summary>
        /// Developer issue
        /// </summary>
        DeveloperError,
        /// <summary>
        /// Product sku not available
        /// </summary>
        ItemUnavailable,
        /// <summary>
        /// Other error
        /// </summary>
        GeneralError,
        /// <summary>
        /// User cancelled the purchase
        /// </summary>
        UserCancelled,
        /// <summary>
        /// App store unavailable on device
        /// </summary>
        AppStoreUnavailable,
        /// <summary>
        /// User is not allowed to authorize payments
        /// </summary>
        PaymentNotAllowed,
        /// <summary>
        /// One of hte payment parameters was not recognized by app store
        /// </summary>
        PaymentInvalid,
        /// <summary>
        /// The requested product is invalid
        /// </summary>
        InvalidProduct,
        /// <summary>
        /// The product request failed
        /// </summary>
        ProductRequestFailed,
        /// <summary>
        /// Restoring the transaction failed
        /// </summary>
        RestoreFailed,
        /// <summary>
        /// Network connection is down
        /// </summary>
        ServiceUnavailable
    }
        #endregion

        
       // private string pageTitle;


        public ICommand Button_InAppPurchase_Clicked_Command{ get; }
        public ICommand Button_Get_Availabe_Products_Clicked_Command { get; }
        public ICommand Button_Get_Purchase_State_Clicked_Command { get; }
        public ICommand Button_Get_Verify_State_Clicked_Command { get; }
        public ICommand Button_Is_Verified_Externally_Clicked_Command { get; }

        

        #region Region Public variables
        // This is called at start up
        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;

                
                switch (itemId)
                {
                    case "InAppPurchase":
                    {
                        GetConnectionState();                                      
                    }
                        break;
                    default:
                        { }
                        break;
                }               
              
            }
        }

        public bool ItemIsPurchased
        {

            get => itemIsPurchased;
            set => SetProperty(ref itemIsPurchased, value);
        }

        public bool ItemReceiptIsVerified
        {
             get => itemReceiptIsVerified;
            set => SetProperty(ref itemReceiptIsVerified, value);
        }

        public bool ItemReceiptIsVerifiedExternally
        {
             get => itemReceiptIsVerifiedExternally;
            set => SetProperty(ref itemReceiptIsVerifiedExternally, value);
        }
        
        public Color ConnectionColor
        {
            get => connectionColor;
            set => SetProperty(ref connectionColor, value);
        }

        public ConnState ConnectionState
        {
            get => connectionState;
            set
            {
                SetProperty(ref connectionState, value);
                
                    connectionColor = Color.Red;
                    switch (connectionState)
                    {   
                        case ConnState.ConnectionOK:                    
                            ConnectionResult = "Connection o.k.";
                            ConnectionColor = Color.Green;
                            break;
                        case ConnState.BillingNotSupported:
                            ConnectionResult = "Billing not supported";
                            break;
                        case ConnState.AppStoreUnavailable:
                            ConnectionResult = "AppStore not available";
                            break;
                        case ConnState.BillingUnavailable:
                            ConnectionResult = "Billing unavailable";
                            break;
                        case ConnState.PaymentInvalid:
                            ConnectionResult = "Payment invalid";
                            break;
                        case ConnState.PaymentNotAllowed:
                            ConnectionResult = "Payment not allowed";
                            break;
                        case ConnState.OtherError:
                            ConnectionResult = "Undefined Error";
                            break;
                        case ConnState.NoInternetConnection:
                            ConnectionResult = "No Internet";
                            break;
                        default:
                            ConnectionResult = "Undefined Error";
                            break;

                }   
            }
        }

        public String ConnectionResult
        {
                get => connectionResult;           
                set
                {
                    SetProperty(ref connectionResult, value);
                    
                }
        }

       

        public String ItemName
        {
                get => itemName;           
                set => SetProperty(ref itemName, value);                
        }

        public String ItemDescription_1
        {
                get => itemDescription_1;           
                set => SetProperty(ref itemDescription_1, value);                
        }

        public String ItemDescription_2
        {
                get => itemDescription_2;           
                set => SetProperty(ref itemDescription_2, value);                
        }

        public String ItemDuration
        {
                get => itemDuration;           
                set => SetProperty(ref itemDuration, value);                
        }
        
        public String ItemPrice
        {
                get => itemPrice;           
                set => SetProperty(ref itemPrice, value);                
        }
        #endregion 


        // Constructor
        #region Region Constructor PurchaseViewModel()
        public PurchaseViewModel()
        {
            Title = "InAppPurchase";
            myAppProductId = "InAppPurchasePremium01";
            Button_InAppPurchase_Clicked_Command = new Command(Button_InAppPurchase_Clicked_Action);
            Button_Get_Availabe_Products_Clicked_Command = new Command(Button_Get_Availabe_Products_Clicked_Action);
            Button_Get_Purchase_State_Clicked_Command = new Command(Button_Get_Purchase_State_Clicked_Action);
            Button_Get_Verify_State_Clicked_Command = new Command(Button_Get_Verify_State_Clicked_Action);
            Button_Is_Verified_Externally_Clicked_Command = new Command(Button_Is_Verified_Externally_Clicked_Action);
        }
        #endregion


        #region Region Button Clicked Actions

            #region Button_Get_Verify_State_Clicked_Action
            private async void Button_Get_Verify_State_Clicked_Action(object obj)
            {
               ItemReceiptIsVerified = await VerifyThisPurchase_PlugIn(myAppProductId);

                int dummy4772 = 1; 
            }
            #endregion

            #region Button_Is_Verified_Externally_Clicked_Action
            // This method does not make much sense, is left only for future tests
            private async void Button_Is_Verified_Externally_Clicked_Action(object obj)
            {
                
                var wrapper = DependencyService.Get<IMyIAPWrapper>();

                //var theResult = await wrapper.ReturnPrice(new string[] {myAppProductId });

                var purchases = await wrapper.ReturnPurchases(new string[] {myAppProductId });


                //var receipt = await wrapper.BuyNativeAndGetReceipt(purchases[0]);

                // Save the Receipt somewhere else if wanted
                myAppReceipt = await wrapper.BuyNativeAndGetReceipt(purchases[0]);          

                string myAppReceiptString = Convert.ToBase64String(myAppReceipt.Data);

                var verify = DependencyService.Get<IInAppBillingVerifyPurchase>();  
                
                //try to purchase item
                var purchase = await CrossInAppBilling.Current.PurchaseAsync(myAppProductId, ItemType.InAppPurchase, verify);
                if(purchase == null)
	            {
		            //Not purchased, may also throw exception to catch
                    ItemReceiptIsVerifiedExternally = false;
	            }
	            else
	            {
		            //Purchased!
                    ItemReceiptIsVerifiedExternally = true;
                    //return true;
	            }

                int dummy4773 = 1;
            }
            #endregion

            #region Button_Get_Availabe_Products_Clicked_Action
            private async void Button_Get_Availabe_Products_Clicked_Action(object obj)
            {
            var billing = CrossInAppBilling.Current;
            var productIds = new string[] {myAppProductId};

            try
            { 
    
    
                //You must connect
                //var connected = await billing.ConnectAsync(ItemType.InAppPurchase);
                var connected = await billing.ConnectAsync();

    if (!connected)
    {
        //Couldn't connect
        return;
    }

    //check purchases

    IEnumerable<InAppBillingProduct>  items = await billing.GetProductInfoAsync(ItemType.InAppPurchase, productIds);

    // In this App we have only one Item
    ItemDuration = "not limited";
    string tempDescription = string.Empty;
    foreach (var item in items)
    {
        ItemName = item.Name;
        tempDescription = item.Description;      
        ItemPrice = item.LocalizedPrice;   
    }
    int lineLength = 25;
    int splitIndex = 0;
    if (tempDescription.Length > lineLength)
        {
            try
            { 
            splitIndex = tempDescription.LastIndexOf(" ", lineLength);
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
            }
            ItemDescription_1 = splitIndex != -1 ? tempDescription.Substring(0, splitIndex) : tempDescription.Substring(0, lineLength);
            if (tempDescription.Length > splitIndex + 2)
            { 
                ItemDescription_2 = tempDescription.Substring(splitIndex + 1);
            }
            else
            {
                ItemDescription_2 = string.Empty;
            }
        }
        else
        {
            ItemDescription_1 = tempDescription;
            ItemDescription_2 = String.Empty;
        }
       
}
catch(InAppBillingPurchaseException pEx)
{
    //Handle IAP Billing Exception
}
catch (Exception ex)
{
    //Something has gone wrong
}
finally
{    
    await billing.DisconnectAsync();
}

        }
        #endregion

            #region Button_Get_Purchase_State_Clicked_Action
            private async void Button_Get_Purchase_State_Clicked_Action(object obj)
            {
                //ItemIsPurchased = !ItemIsPurchased;
                ItemIsPurchased = await WasItemPurchased_PlugIn(myAppProductId);
            }
            #endregion

        #region Button_InAppPurchase_Clicked_Action
        private async void Button_InAppPurchase_Clicked_Action(object obj)
        {
            var result = await PurchaseItem_PlugIn(myAppProductId);
            int dummy67 = 1;
        }
        #endregion

        #endregion 

        //public async Task<bool> VerifyPurchase(string signedData, string signature, string productId = null, string transactionId = null)

        #region Task VerifyThisPurchase using the PlugIn
        public async Task<bool> VerifyThisPurchase_PlugIn(string productId)
        {
            var billing = CrossInAppBilling.Current;
            try
            {            
                var connected = await billing.ConnectAsync();

	            if (!connected)
	            {
		            //Couldn't connect to billing
		            return false;
	            }
            
                 var verify = DependencyService.Get<IInAppBillingVerifyPurchase>();
            
                // Only a dummy test implemented: Compares the Receipt with itself
                
                //try to purchase item
                var purchase = await CrossInAppBilling.Current.PurchaseAsync(productId, ItemType.InAppPurchase, verify);
	            if(purchase == null)
	            {
		            //Not purchased, may also throw exception to catch
                    return false;
	            }
	            else
	            {
		            //Purchased!
                    return true;
	            }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
	           Debug.WriteLine("Issue: " + purchaseEx);
            }
            catch (Exception ex)
            {	
                Debug.WriteLine("Issue connecting: " + ex);
            }
            finally
            {
                //Disconnect, it is okay if we never connected, this will never throw an exception
                    await CrossInAppBilling.Current.DisconnectAsync();
            }

            return false;
        }
        #endregion
        
        #region Task Purchase an Item using the PlugIn
        public async Task<bool> PurchaseItem_PlugIn(string productId)
        {
            var billing = CrossInAppBilling.Current;
            try
            {
                var connected = await billing.ConnectAsync();
                if (!connected)
                {
                    //we are offline or can't connect, don't try to purchase
                    return false;
                }

                //check purchases
                
                //var purchase = await billing.PurchaseAsync(productId, ItemType.InAppPurchase, payload);

                var purchase = await billing.PurchaseAsync(productId, ItemType.InAppPurchase);
                //possibility that a null came through.

               // "ewoJInNpZ25hdHVyZSIgPSAiQTBWY2VVamRoZ0JsYUtyejBIOUx2bzdybVYvNmZUbFVOdmVsOE9Kc0Y5dyt5WGRDYVpRd3Y3ZG96aVZtblVHbU83dXRhSzlDa0dGRVdMNnJzNDE2b0k5WG56OXFVQUZsenNmMDI4M0kxMzFpQ055d3NiUytDVTJGS2lDb0hjOVgyR3MxZld1VUREWmxQYVYyUDhjL20xZXdsYjcvYk1pMFNvRGVWZlFaR0Z6Z2VYQnlt…"

                if(purchase == null)
                {
                    //did not purchase
                    return false;
                }
                else
                { 
                    if(purchase.State == PurchaseState.Purchased)
                    {   
                        // only on Android needed ?
                        //var ack = await CrossInAppBilling.Current.FinalizePurchaseAsync(purchase.TransactionIdentifier);
                        int dummy45 = 1;
                        return true;
                        // Handle if acknowledge was successful or not
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                //Billing Exception handle this based on the type
                Debug.WriteLine("Error: " + purchaseEx);
                return false;
            }
            catch (Exception ex)
            {
                //Something else has gone wrong, log it
                Debug.WriteLine("Issue connecting: " + ex);
                return false;
            }
            finally
            {
                await billing.DisconnectAsync();
            }
        }
        #endregion

        #region Task WasItemPurchased? using the PlugIn

        public async Task<bool> WasItemPurchased_PlugIn(string productId)
        {
            var billing = CrossInAppBilling.Current;
            try
            { 
                var connected = await billing.ConnectAsync();

            if (!connected)
            {
             //Couldn't connect
                return false;
            }

            //check purchases
            var idsToNotFinish = new List<string>(new [] {"myconsumable"});

            //var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase, idsToNotFinish);
            var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);

            //check for null just in case
            if(purchases?.Any(p => p.ProductId == productId) ?? false)
            {
                //Purchase restored
                // if on Android may be good to 
                return true;
            }
            else
            {
                //no purchases found
                return false;
            }
        }    
        catch (InAppBillingPurchaseException purchaseEx)
        {
            //Billing Exception handle this based on the type
            Debug.WriteLine("Error: " + purchaseEx);
        }
        catch (Exception ex)
            {  
            //Something has gone wrong
        }
        finally
        {    
            await billing.DisconnectAsync();
        }

        return false;
    }
        #endregion

        #region Task TestPurchaseConnection using the PlugIn
        async Task<ConnState> TestPurchaseConnection_PlugIn()
        {
            if (!(Connectivity.NetworkAccess == NetworkAccess.Internet))
            {
                return ConnState.NoInternetConnection;
            }

            if (!CrossInAppBilling.IsSupported)
            {
                return ConnState.BillingNotSupported;
            }

            var billing = Plugin.InAppBilling.CrossInAppBilling.Current;
            ConnState errorReturnState = ConnState.OtherError;
            try
            {   bool connected = false;
                //ItemType nonConsumable = ItemType.InAppPurchase;
                connected = await billing.ConnectAsync(false);
                if (connected)
                {
                    return ConnState.ConnectionOK;
                }
                else
                {
                    return ConnState.AppStoreUnavailable;
                }    
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                var message = string.Empty;            
                switch (purchaseEx.PurchaseError)
                {
                    case (Plugin.InAppBilling.PurchaseError)PurchaseError.AppStoreUnavailable:
                        errorReturnState = ConnState.AppStoreUnavailable;
                        //message = "Currently the app store seems to be unavailable. Try again later.";
                        break;
                    case (Plugin.InAppBilling.PurchaseError)PurchaseError.BillingUnavailable:
                        errorReturnState = ConnState.BillingUnavailable;
                        //message = "Billing seems to be unavailable, please try again later.";
                        break;
                    case (Plugin.InAppBilling.PurchaseError)PurchaseError.PaymentInvalid:
                        errorReturnState = ConnState.PaymentInvalid;
                        //message = "Payment seems to be invalid, please try again.";
                        break;
                    case (Plugin.InAppBilling.PurchaseError)PurchaseError.PaymentNotAllowed:
                        errorReturnState = ConnState.PaymentNotAllowed;
                        //message = "Payment does not seem to be enabled/allowed, please try again.";
                        break;
                       
            }

            return errorReturnState;
                

            //Display message to user
            }
            catch (Exception ex)
            {
                //Something else has gone wrong, log it
                Debug.WriteLine("Issue connecting: " + ex);
                return errorReturnState;
            }
            finally
            {
                await billing.DisconnectAsync();
            }

        }
        #endregion

        #region Region Private Methods
        private async void GetConnectionState()
        {
             ConnectionState = await TestPurchaseConnection_PlugIn();
        }
        #endregion
	}
}

   