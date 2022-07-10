using System.Threading.Tasks;
using InAppPurchaseTest.Interfaces;
using Xamarin.InAppPurchasing.iOS.Dependencies;
using Xamarin.Forms;

[assembly: Dependency(typeof(MyIAPWrapper))]
namespace Xamarin.InAppPurchasing.iOS.Dependencies
{	
	public class MyIAPWrapper : IMyIAPWrapper
	{ 
		ApplePurchaseService applePurchaseService;
			
		public async Task<string> ReturnPrice(string[] ids)	
        {
			applePurchaseService = new ApplePurchaseService();
			var purchases = await applePurchaseService.GetPrices(ids);
			return purchases[0].Price;			
        }

		public async Task<Purchase[]> ReturnPurchases(string[] ids)
		{
			applePurchaseService = new ApplePurchaseService();
			var purchases = await applePurchaseService.GetPrices(ids);
			return purchases;	
		}

		public async Task<AppleReceipt> BuyNativeAndGetReceipt(Purchase purchase)
        {
			applePurchaseService = new ApplePurchaseService();
			var receipt =  await applePurchaseService.BuyNativeAndGetReceipt(purchase); 
            return receipt as AppleReceipt;
        }

		public async Task<AppleReceipt> BuyNative(Purchase purchase)
        {
			applePurchaseService = new ApplePurchaseService();
			var receipt =  await applePurchaseService.  BuyNativeAndGetReceipt(purchase); 
            return receipt as AppleReceipt;
        }


	}
 }

