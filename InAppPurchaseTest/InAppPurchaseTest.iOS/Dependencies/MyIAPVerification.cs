using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.InAppBilling;
using Foundation;
/*
[assembly: Dependency(typeof(MyIAPVerification))]

    public class MyIAPVerification : IInAppBillingVerifyPurchase
    {
        
        public async Task<bool> VerifyPurchase (string signedData, string signature, string productId = null, string transactionId = null)
        {
            await Task.Delay(1);

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
            NSData receiptUrl = null;
            if (NSBundle.MainBundle.AppStoreReceiptUrl != null)
            {
                receiptUrl = NSData.FromUrl(NSBundle.MainBundle.AppStoreReceiptUrl);
            }

            string theReceipt = receiptUrl?.GetBase64EncodedString(NSDataBase64EncodingOptions.None);

            bool returnResult = signedData ==  theReceipt ? true : false;

            return returnResult;
        }
    }
*/