using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class StoreKitGUIManager : MonoBehaviourGUI
{
	private List<StoreKitProduct> _products;

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

	private void OnGUI()
	{
		beginColumn();
		if (GUILayout.Button("Get Can Make Payments"))
		{
			bool flag = StoreKitBinding.canMakePayments();
			Debug.Log("StoreKit canMakePayments: " + flag);
		}
		if (GUILayout.Button("Get Product Data"))
		{
			string[] productIdentifiers = new string[1] { "bigammopack" };
			StoreKitBinding.requestProductData(productIdentifiers);
		}
		if (GUILayout.Button("Restore Completed Transactions"))
		{
			StoreKitBinding.restoreCompletedTransactions();
		}
		if (GUILayout.Button("Validate Receipt"))
		{
			List<StoreKitTransaction> allSavedTransactions = StoreKitBinding.getAllSavedTransactions();
			if (allSavedTransactions.Count > 0)
			{
				StoreKitBinding.validateReceipt(allSavedTransactions[0].base64EncodedTransactionReceipt, true);
			}
		}
		endColumn(true);
		if (_products != null && _products.Count > 0 && GUILayout.Button("Purchase Random Product"))
		{
			int index = Random.Range(0, _products.Count);
			StoreKitProduct storeKitProduct = _products[index];
			Debug.Log("preparing to purchase product: " + storeKitProduct.productIdentifier);
			StoreKitBinding.purchaseProduct(storeKitProduct.productIdentifier, 1);
		}
		if (GUILayout.Button("Validate Subscription"))
		{
			List<StoreKitTransaction> allSavedTransactions2 = StoreKitBinding.getAllSavedTransactions();
			foreach (StoreKitTransaction item in allSavedTransactions2)
			{
				if (item.productIdentifier == "sevenDays")
				{
					StoreKitBinding.validateAutoRenewableReceipt(item.base64EncodedTransactionReceipt, "YOUR_SECRET_FROM_ITC", true);
					break;
				}
			}
		}
		if (GUILayout.Button("Get Saved Transactions"))
		{
			List<StoreKitTransaction> allSavedTransactions3 = StoreKitBinding.getAllSavedTransactions();
			Debug.Log("\ntotal transaction received: " + allSavedTransactions3.Count);
			foreach (StoreKitTransaction item2 in allSavedTransactions3)
			{
				Debug.Log(item2.ToString() + "\n");
			}
		}
		if (GUILayout.Button("Turn Off Auto Confirmation of Transactions"))
		{
			StoreKitManager.autoConfirmTransactions = false;
		}
		endColumn();
	}
}
