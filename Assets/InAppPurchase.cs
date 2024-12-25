using UnityEngine;
using System.Collections;
using OnePF;
using System.Collections.Generic;
public class InAppPurchase : MonoBehaviour {
	bool _isInitialized = false;
	Inventory _inventory = null;

	const string PURCHASE_HEARTS="hearts";

	public static InAppPurchase instance = null;
	public delegate void OnRemoveAd();
	public static event  OnRemoveAd onRemoveAd;

	public delegate void OnPurchasePack();
	public static event  OnPurchasePack onPurchasePack;

	public delegate void OnPurchaseFail();
	public static event  OnPurchaseFail onPurchaseFail;

	public delegate void OnRestore();
	public static event  OnRestore onRestore;
	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != null)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);
	}
	// Use this for initialization
	void Start () {
		print ("init inapp");
		OpenIAB.mapSku(PURCHASE_HEARTS, OpenIAB_Android.STORE_GOOGLE, PURCHASE_HEARTS);

		initInApp ();

	}
	public void purchaseHearts(){
		OpenIAB.purchaseProduct(PURCHASE_HEARTS);
	}

	public void querryItem(){
	
		OpenIAB.queryInventory(new string[] { PURCHASE_HEARTS });
	}
	void initInApp(){
		var googlePublicKey = "YOUR_BASE_64";
		var options = new Options();
		options.checkInventoryTimeoutMs = Options.INVENTORY_CHECK_TIMEOUT_MS * 2;
		options.discoveryTimeoutMs = Options.DISCOVER_TIMEOUT_MS * 2;
		options.checkInventory = false;
		options.prefferedStoreNames= new string[] { OpenIAB_Android.STORE_GOOGLE };
		options.verifyMode = OptionsVerifyMode.VERIFY_SKIP;
		options.storeKeys = new Dictionary<string, string> { {OpenIAB_Android.STORE_GOOGLE, googlePublicKey} };
		options.storeSearchStrategy = SearchStrategy.INSTALLER_THEN_BEST_FIT;
		OpenIAB.init(options);
	}
	private void OnEnable()
	{
		// Listen to all events for illustration purposes
		OpenIABEventManager.billingSupportedEvent += billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent += purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent += purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;

	}
	private void OnDisable()
	{
		// Remove all event handlers
		OpenIABEventManager.billingSupportedEvent -= billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent -= purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}
	private void billingSupportedEvent()
	{
		_isInitialized = true;
		Debug.Log("billingSupportedEvent");
	//	querryItem ();
	}
	private void billingNotSupportedEvent(string error)
	{
		Debug.Log("billingNotSupportedEvent: " + error);
	}
	private void queryInventorySucceededEvent(Inventory inventory)
	{
	


		if (inventory != null)
		{

		//	_label = inventory.ToString();
			_inventory = inventory;
			Debug.Log("queryInventorySucceededEvent: " + inventory.ToString());
		
		
	
			
			onRestore();
		}
	}
	private void queryInventoryFailedEvent(string error)
	{
		Debug.Log("queryInventoryFailedEvent: " + error);

//		_label = error;
	}
	private void purchaseSucceededEvent(OnePF.Purchase purchase)
	{
		Debug.Log("purchaseSucceededEvent: " + purchase);
	//	_label = "PURCHASED:" + purchase.ToString();

		if (purchase.Sku.Equals (PURCHASE_HEARTS)) {
			OpenIAB.consumeProduct(purchase);
			PlayerPrefs.SetInt(Constant.HEART,Constant.MAX_HEART);
			PlayerPrefs.Save();
			
			onPurchasePack();
		//	FB.LogAppEvent("Purchase Remove Ad");
		} 
	
	}
	private void purchaseFailedEvent(int errorCode, string errorMessage)
	{
		Debug.Log("purchaseFailedEvent: " + errorMessage);

		if (errorCode == 7){
			onPurchaseFail();
		}
//		_label = "Purchase Failed: " + errorMessage;
	}
	private void consumePurchaseSucceededEvent(OnePF.Purchase purchase)
	{
		Debug.Log("consumePurchaseSucceededEvent: " + purchase);
	//	_label = "CONSUMED: " + purchase.ToString();
	}
	private void consumePurchaseFailedEvent(string error)
	{
		Debug.Log("consumePurchaseFailedEvent: " + error);
	//	_label = "Consume Failed: " + error;
	}
}
