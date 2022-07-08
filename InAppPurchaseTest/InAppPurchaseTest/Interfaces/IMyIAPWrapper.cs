using Xamarin.InAppPurchasing;
using System.Threading.Tasks;

namespace InAppPurchaseTest.Interfaces
{
	public interface IMyIAPWrapper
	{
		Task<string> ReturnPrice(string[] ids);
		
		Task<Purchase[]> ReturnPurchases(string[] ids);

		Task<AppleReceipt> BuyNativeAndGetReceipt(Purchase purchase);
	}
}

