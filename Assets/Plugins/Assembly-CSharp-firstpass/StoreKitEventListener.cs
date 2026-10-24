using System.Collections.Generic;
using UnityEngine;

public class StoreKitEventListener : MonoBehaviour
{
	private List<StoreKitProduct> _products = new List<StoreKitProduct>();

	private void Start()
	{
		StoreKitManager.productListReceivedEvent += delegate(List<StoreKitProduct> allProducts)
		{
			Debug.Log("received total products: " + allProducts.Count);
			_products = allProducts;
		};
		if (StoreKitBinding.canMakePayments())
		{
			string[] productIdentifiers = new string[1] { "bigammopack" };
			StoreKitBinding.requestProductData(productIdentifiers);
		}
	}

	public void ProvideContent()
	{
		PlayerPrefs.SetInt("InAppBoughtSett", 1);
		PlayerPrefs.Save();
	}

	private void OnEnable()
	{
		StoreKitManager.productPurchaseAwaitingConfirmationEvent += productPurchaseAwaitingConfirmationEvent;
		StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessful;
		StoreKitManager.purchaseCancelledEvent += purchaseCancelled;
		StoreKitManager.purchaseFailedEvent += purchaseFailed;
		StoreKitManager.receiptValidationFailedEvent += receiptValidationFailed;
		StoreKitManager.receiptValidationRawResponseReceivedEvent += receiptValidationRawResponseReceived;
		StoreKitManager.receiptValidationSuccessfulEvent += receiptValidationSuccessful;
		StoreKitManager.productListReceivedEvent += productListReceivedEvent;
		StoreKitManager.productListRequestFailedEvent += productListRequestFailed;
		StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailed;
		StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinished;
		StoreKitManager.paymentQueueUpdatedDownloadsEvent += paymentQueueUpdatedDownloadsEvent;
	}

	private void OnDisable()
	{
		StoreKitManager.productPurchaseAwaitingConfirmationEvent -= productPurchaseAwaitingConfirmationEvent;
		StoreKitManager.purchaseSuccessfulEvent -= purchaseSuccessful;
		StoreKitManager.purchaseCancelledEvent -= purchaseCancelled;
		StoreKitManager.purchaseFailedEvent -= purchaseFailed;
		StoreKitManager.receiptValidationFailedEvent -= receiptValidationFailed;
		StoreKitManager.receiptValidationRawResponseReceivedEvent -= receiptValidationRawResponseReceived;
		StoreKitManager.receiptValidationSuccessfulEvent -= receiptValidationSuccessful;
		StoreKitManager.productListReceivedEvent -= productListReceivedEvent;
		StoreKitManager.productListRequestFailedEvent -= productListRequestFailed;
		StoreKitManager.restoreTransactionsFailedEvent -= restoreTransactionsFailed;
		StoreKitManager.restoreTransactionsFinishedEvent -= restoreTransactionsFinished;
		StoreKitManager.paymentQueueUpdatedDownloadsEvent -= paymentQueueUpdatedDownloadsEvent;
	}

	private void productListReceivedEvent(List<StoreKitProduct> productList)
	{
		Debug.Log("productListReceivedEvent. total products received: " + productList.Count);
		foreach (StoreKitProduct product in productList)
		{
			Debug.Log(product.ToString() + "\n");
		}
	}

	private void productListRequestFailed(string error)
	{
		Debug.Log("productListRequestFailed: " + error);
	}

	private void receiptValidationSuccessful()
	{
		Debug.Log("receipt validation successful");
	}

	private void receiptValidationFailed(string error)
	{
		Debug.Log("receipt validation failed with error: " + error);
	}

	private void receiptValidationRawResponseReceived(string response)
	{
		Debug.Log("receipt validation raw response: " + response);
	}

	private void purchaseFailed(string error)
	{
		Debug.Log("purchase failed with error: " + error);
	}

	private void purchaseCancelled(string error)
	{
		Debug.Log("purchase cancelled with error: " + error);
	}

	private void productPurchaseAwaitingConfirmationEvent(StoreKitTransaction transaction)
	{
		Debug.Log("productPurchaseAwaitingConfirmationEvent: " + transaction);
	}

	private void purchaseSuccessful(StoreKitTransaction transaction)
	{
		Debug.Log("purchased product: " + transaction);
	}

	private void restoreTransactionsFailed(string error)
	{
		Debug.Log("restoreTransactionsFailed: " + error);
	}

	private void restoreTransactionsFinished()
	{
		Debug.Log("restoreTransactionsFinished");
		ProvideContent();
	}

	private void paymentQueueUpdatedDownloadsEvent(List<StoreKitDownload> downloads)
	{
		Debug.Log("paymentQueueUpdatedDownloadsEvent: ");
		foreach (StoreKitDownload download in downloads)
		{
			Debug.Log(download);
		}
	}
}
