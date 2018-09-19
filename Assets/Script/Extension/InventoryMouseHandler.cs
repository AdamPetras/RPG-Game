using System;
using System.Linq;
using Assets.Script.CharacterFolder;
using Assets.Script.HUD;
using Assets.Script.InventoryFolder;
using Assets.Script.InventoryFolder.ShopFolder;
using Assets.Scripts;
using Assets.Scripts.Extension;
using Assets.Scripts.InventoryFolder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.Extension
{
    public class InventoryMouseHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
        IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public static GameObject itemBeingDragged;
        private static ComponentItem _dragingItemComponent;
        private static Vector3 startPosition;
        public static Transform startParent;
        private Transform DragPanel;
        private PlayerComponent _playerComponent;
        public static GameObject Dialog;
        private static Transform _dialogBackground;
        private static Button _dialogYes;
        private static Button _dialogNo;
        private static Button _dialogExit;
        private static Transform _dialogText;
        public static GameObject StackPrefab;
        private static Transform _stackBackground;
        private static Transform _stackExitKey;
        private static InputField _stackInput;
        private static Button _stackButton;
        private static GameObject _itemInfo;
        private static Transform _infoBackground;
        private static Text _infoName;
        private static Text _infoSellPrice;
        private static Text _infoDescription;
        private static Image _infoMoneyImage;
        private static ComponentInventory _gameMasterItem;
        private static Transform _clickedItemTransform;
        public bool CanIMove;
        private void Awake()
        {
            if (Dialog == null)
                DialogInstantiate();
            if (StackPrefab == null)
                StackInstantiate();
            if (_itemInfo == null)
                InfoInstantiate();
        }

        private void Start()
        {
            DragPanel = GameObject.Find("DragPanel").transform.Find("Background");
            _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
            _gameMasterItem = GameObject.Find("GameMaster").GetComponent<ComponentInventory>();
        }

        #region GraphicsInstantiate   

        private void StackInstantiate()
        {
            StackPrefab = Instantiate(Resources.Load<GameObject>("Prefab/StackPrefab"));
            StackPrefab.name = "StackPrefab";
            StackPrefab.SetActive(false);
            _stackBackground = StackPrefab.transform.Find("Background");
            _stackExitKey = _stackBackground.Find("ExitKey");
            _stackInput = _stackBackground.Find("InputField").GetComponent<InputField>();
            _stackButton = _stackBackground.Find("Button").GetComponent<Button>();
            _stackExitKey.GetComponent<Button>().onClick.AddListener(delegate
            {
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                StackPrefab.SetActive(false);
            });
            _stackInput.onValueChanged.AddListener(
                delegate
                {
                    OnStackValueChanged();
                });
            //přidání funkce na button (vytvoří nový item atd.)
            _stackButton.onClick.AddListener(delegate
            {
                if (_clickedItemTransform.GetComponent<ComponentItem>().EItemState == EItemState.Shop)
                    OnStackAmount();
                else
                    OnClickButtonStack(_clickedItemTransform.GetComponent<ComponentItem>());
                //_clickedItemTransform.GetComponent<CanvasGroup>().blocksRaycasts = true;

            });
        }

        private void DialogInstantiate()
        {
            Dialog = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/DialogWindow"));
            Dialog.name = "Dialog";
            Dialog.SetActive(false);
            _dialogBackground = Dialog.transform.Find("Background");
            _dialogExit = _dialogBackground.Find("Exit").GetComponent<Button>();
            _dialogYes = _dialogBackground.Find("Yes").GetComponent<Button>();
            _dialogNo = _dialogBackground.Find("No").GetComponent<Button>();
            _dialogText = _dialogBackground.Find("Text");
        }

        private void InfoInstantiate()
        {
            _itemInfo = GameObject.Find("SlotItemInfo");
            if (_itemInfo == null)
            {
                _itemInfo = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Info"));
            }
            _itemInfo.name = "SlotItemInfo";
            _infoBackground = _itemInfo.transform.Find("Background");
            _infoName = _infoBackground.Find("Name").GetComponent<Text>();
            _infoDescription = _infoBackground.Find("Desc").GetComponent<Text>();
            _infoSellPrice = _infoBackground.Find("SellPrice").GetComponent<Text>();
            _infoMoneyImage = _infoBackground.Find("MoneyImage").GetComponent<Image>();
            _infoMoneyImage.enabled = true;
            _itemInfo.GetComponent<Canvas>().enabled = false;
        }

        #endregion

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!CanIMove)
                return;
            if (!EventSystem.current.IsPointerOverGameObject())
                return;
            ComponentItem gameObjComponent = gameObject.GetComponent<ComponentItem>();
            itemBeingDragged = gameObject;          //nastavení přetahovaného objektu na tento item
            if (gameObjComponent.EItemState == EItemState.Armor)        //pokud bereme item z armor slotu
            {
                if (transform.parent.GetComponent<ArmorSlot>() != null)
                {
                    transform.parent.GetComponent<ArmorSlot>().Occupied = false; //nastavíme že neni okupován
                    //Debug.Log("Sub");
                    _playerComponent.ArmorList.Remove(gameObjComponent);
                    SlotManagement.AddRemoveStats(gameObjComponent, false);     //odečteme staty kvůli odebrání
                }
            }
            else if (gameObjComponent.EItemState == EItemState.Inventory)   //jinak pokud bereme z inventáře
            {
                if (transform.parent.GetComponent<Slot>() != null)      //a pokud neni slot null
                {
                    transform.parent.GetComponent<Slot>().Occupied = false; //nastavíne že neni okupován
                }
            }
            else if (gameObjComponent.EItemState == EItemState.Drop)    //pokud je objekt v dropu
            {
                gameObject.transform.parent.parent.Find("Text").GetComponent<Text>().text = "";
                //Destroy(gameObject.transform.parent.parent.gameObject); //zničí se slot            
                HideInfo();     //skrytí info (nemusí být ale pak je prodleva než se to skryje)
            }
            _dragingItemComponent = itemBeingDragged.GetComponent<ComponentItem>();
            startPosition = transform.position;     //uložíme počáteční pozici
            startParent = transform.parent;         //uložíme počátečního rodiče
            GetComponent<CanvasGroup>().blocksRaycasts = false;     //nastavíme canvasgroup aby neblokoval raycasty(nešlo by item pustit)
            gameObject.transform.SetParent(DragPanel);
        }


        private void OnDontDropItem()
        {
            Debug.Log(itemBeingDragged);
            itemBeingDragged.SetActive(true);
            itemBeingDragged.transform.SetParent(startParent);
            Dialog.SetActive(false);
            if (_dragingItemComponent.EItemState == EItemState.Drop)
                startParent.parent.Find("Text").GetComponent<Text>().text = _dragingItemComponent.Name;
            if (_dragingItemComponent.EItemState == EItemState.Armor)
            {
                SlotManagement.AddRemoveStats(_dragingItemComponent);
                startParent.GetComponent<ArmorSlot>().Occupied = true;
            }
            if (_dragingItemComponent.EItemState == EItemState.Inventory)
            {
                startParent.GetComponent<Slot>().Occupied = true;
            }
            //transform.position = startPosition; //nastavení pozice na pozici            
            itemBeingDragged.GetComponent<CanvasGroup>().blocksRaycasts = true; //zpět nastavení blokování raycastu
            itemBeingDragged = null; //nastavení že item již není přemýsťovatelný 
            _dialogYes.onClick.RemoveAllListeners();
            _dialogNo.onClick.RemoveAllListeners();
            _dialogExit.onClick.RemoveAllListeners();
        }
        private void OnDropItem()
        {
            if (_dragingItemComponent.EItemState == EItemState.Inventory)
            {
                _gameMasterItem.InventoryList.Remove(itemBeingDragged.GetComponent<ComponentItem>());
            }
            Destroy(itemBeingDragged);
            Dialog.SetActive(false);
            _dialogYes.onClick.RemoveAllListeners();
            _dialogNo.onClick.RemoveAllListeners();
            _dialogExit.onClick.RemoveAllListeners();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!CanIMove)
                return;
            if (itemBeingDragged == null)
                return;
            transform.position = Input.mousePosition;       //pokud držíme nastavení pozice na pozici myši
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!CanIMove)
                return;
            if (itemBeingDragged == null)
                return;
            if (transform.parent == startParent && itemBeingDragged.GetComponent<ComponentItem>().EItemState == EItemState.Armor)       //pokud pustíme na stejné místo a je to armor slot
            {
                //Debug.Log("Add");
                _playerComponent.ArmorList.Add(itemBeingDragged.GetComponent<ComponentItem>());
                SlotManagement.AddRemoveStats(itemBeingDragged.GetComponent<ComponentItem>());  //přičteme zpět staty
            }
            if (transform.parent == DragPanel)
            {
                itemBeingDragged.SetActive(false);
                Dialog.SetActive(true);
                _dialogYes.GetComponent<Button>().onClick.AddListener(OnDropItem);           
                _dialogNo.GetComponent<Button>().onClick.AddListener(OnDontDropItem);
                _dialogExit.GetComponent<Button>().onClick.AddListener(OnDontDropItem);
                _dialogText.GetComponent<Text>().text = "Do you want to destroy " + itemBeingDragged.GetComponent<ComponentItem>().Name;
            }
            else
            {
                SlotManagement.ShowStack(_dragingItemComponent);   //vykreslení stacků   
                transform.position = startPosition;     //nastavení pozice na počáteční pozici
                itemBeingDragged = null;    //nastavení že item již není přemýsťovatelný 
                GetComponent<CanvasGroup>().blocksRaycasts = true;  //zpět nastavení blokování raycastu
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            InvokeRepeating("ShowInfo", 0.5f, 1);
            //ShowInfo();     //pokud pointer vstoupil do objektu ukáže se info
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            CancelInvoke("ShowInfo");
            HideInfo();     //pokud pointer vystoupil z objektu skryje se info
        }

        private void OnDisable()
        {
            CancelInvoke("ShowInfo");
            HideInfo();
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (StackPrefab != null)
                StackPrefab.SetActive(false);
        }

        private void ShowInfo()
        {
            _itemInfo.GetComponent<Canvas>().enabled = true;
            _infoBackground.transform.position = new Vector3(transform.position.x - 24, transform.position.y + 24,
                transform.position.z);
            ComponentItem item = GetComponent<ComponentItem>();
            _infoName.color = Utilities.ColorByItemRank(item.ERank);
            _infoName.text = item.Name;
            string s = "";
            if (item.ItemStats != null)
            {
                foreach (ItemStats stats in item.ItemStats)
                {
                    s += stats.ESkill + " " + stats.Value + "\n";
                }
            }
            _infoDescription.text = item.Subtype + "\n" + "<color=#008000ff>" + s + "</color>";
            _infoSellPrice.text = "Sell price: " + item.SellPrice;
        }

        private void HideInfo()
        {
            if (_itemInfo != null)
                _itemInfo.GetComponent<Canvas>().enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
                return;
            if (!ComponentInventory.Visible && !ComponentSalesMan.Visible && !Drop.Visible && !HUDStatsView.Visible)
                return;
            if (eventData.button == PointerEventData.InputButton.Right && Input.GetKey(KeyCode.LeftShift))      //tabulka pro rozdělení itemů z 5ti stacků třeba na 2 a 3 stacky
            {
                ComponentItem gameobjItem = gameObject.GetComponent<ComponentItem>();
                if (gameobjItem.ActualStack <= 1 && gameobjItem.EItemState != EItemState.Shop)
                    return;

                if (gameobjItem.EItemState == EItemState.Inventory)    //pokud je objekt v inventáři (nemůžu to dělat v dropu)
                {
                    //vytvoření objektu kde vložíme počet stacků
                    StackPrefab.SetActive(true);
                    _stackBackground.position = eventData.position - _stackBackground.GetComponent<RectTransform>().sizeDelta / 2;      //pozicování podlemyši
                    //přidání funkce která ošetřuje aby nebylo vloženo velké číslo
                }
                else if (gameobjItem.EItemState == EItemState.Shop)
                {
                    _stackBackground.transform.position = eventData.position;
                    StackPrefab.SetActive(true);
                }
                _clickedItemTransform = transform;
            }
            else if (eventData.button == PointerEventData.InputButton.Right)    //pokud je pouze kliknutí pravým
            {
                //nasaď do armory pokud je v dropu tak hoď do invu nebo prodej (podle otevření obchodu)
                ComponentItem gameobjItem = gameObject.GetComponent<ComponentItem>();
                if (gameobjItem.EItemState == EItemState.Inventory)
                {
                   // if (!HUDStatsView.Visible || (gameobjItem.Type != EType.Armour && gameobjItem.Type != EType.Weapon))
                    //{
                        if (ComponentSalesMan.Visible && gameobjItem.ID != 0)
                        {
                            Dialog.SetActive(true);
                            _dialogText.GetComponent<Text>().text =
                                "Do you really want to sell " + gameobjItem.ActualStack + "x" + gameobjItem.Name +
                                " for " + gameobjItem.SellPrice * gameobjItem.ActualStack + " golds?";
                            _clickedItemTransform = gameObject.transform;
                            _dialogYes.onClick.AddListener(delegate
                            {
                                ComponentItem _clickedItem = _clickedItemTransform.GetComponent<ComponentItem>();
                                if (_clickedItem.SellPrice * _clickedItem.ActualStack > 0)
                                    SlotManagement.AddMoney((uint) (_clickedItem.SellPrice * _clickedItem.ActualStack));
                                //Debug.Log(_clickedItem.ActualStack+" "+_clickedItem.SellPrice);
                                _clickedItemTransform.parent.GetComponent<Slot>().Occupied = false;
                                SlotManagement.RemoveFromInventory(_clickedItem);
                                Destroy(_clickedItemTransform.gameObject);
                                Dialog.SetActive(false);
                                _dialogYes.onClick.RemoveAllListeners();
                                _dialogNo.onClick.RemoveAllListeners();
                                _dialogExit.onClick.RemoveAllListeners();
                            });
                            _dialogExit.onClick.AddListener(delegate
                            {
                                Dialog.SetActive(false);
                                _dialogYes.onClick.RemoveAllListeners();
                                _dialogNo.onClick.RemoveAllListeners();
                                _dialogExit.onClick.RemoveAllListeners();
                            });
                            _dialogNo.onClick.AddListener(delegate
                            {
                                Dialog.SetActive(false);
                                _dialogYes.onClick.RemoveAllListeners();
                                _dialogNo.onClick.RemoveAllListeners();
                                _dialogExit.onClick.RemoveAllListeners();
                            });
                        }
                    //}
                    else
                    {
                        if (!SlotManagement.AddToArmor(gameobjItem))
                            return;
                        _gameMasterItem.InventoryList.Remove(gameobjItem);
                        HideInfo();
                    }
                }
                else if (gameobjItem.EItemState == EItemState.Drop)    //pokud je objekt v dropu
                {
                    Drop dr = gameObject.transform.parent.parent.parent.parent.parent.parent.GetComponent<Drop>();
                    dr.DropItemList.Remove(
                        dr.DropItemList.Find(s => s.Name == gameObject.transform.GetComponent<ComponentItem>().Name));
                    gameObject.transform.parent.parent.Find("Text").GetComponent<Text>().text = "";
                    SlotManagement.AddToInventory(gameObject);      //přidá se do inventáře
                    HideInfo();     //skrytí info (nemusí být ale pak je prodleva než se to skryje)
                }
                else if (gameobjItem.EItemState == EItemState.Armor)
                {
                    if (!SlotManagement.AddToInventory(gameobjItem))
                    {
                        Debug.LogWarning("Inventory is full");
                        return;
                    }
                    if (transform.parent.GetComponent<ArmorSlot>() != null)
                    {
                        transform.parent.GetComponent<ArmorSlot>().Occupied = false; //nastavíme že neni okupován
                    }
                    _playerComponent.ArmorList.Remove(gameobjItem);
                    SlotManagement.AddRemoveStats(gameobjItem, false);     //odečteme staty kvůli odebrání
                }
                else if (gameobjItem.EItemState == EItemState.Shop)
                {
                    NewItem item = new NewItem(NewItem.IdToItem(gameobjItem.ID));
                    if (_playerComponent == null)
                        _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
                    if (SlotManagement.AreTheseMoneyEnough(item.BuyPrice))
                    {
                        StackPrefab.SetActive(false);
                        if (!SlotManagement.AddToInventory(item))
                        {
                            Debug.LogWarning("Inventory is full");
                            return;
                        }
                        SlotManagement.MoneyWithdraw(item.BuyPrice);
                    }
                }
            }

        }

        private void OnStackAmount()
        {
            if (_clickedItemTransform == null)
                return;
            int stack;
            Int32.TryParse(_stackInput.text, out stack);
            NewItem item = new NewItem(NewItem.IdToItem(_clickedItemTransform.GetComponent<ComponentItem>().ID));
            Debug.Log("StackAmount " + stack + " Item name " + item.Name);

            if (!SlotManagement.AreTheseMoneyEnough((uint)(stack * item.BuyPrice)))
            {
                return;
            }
            _stackInput.text = "";
            StackPrefab.SetActive(false);
            if (stack <= 0)
            {
                return;
            }
            item.ActualStack = stack;
            if (!SlotManagement.AddToInventory(item))
            {
                Debug.LogWarning("Inventory is full");
                return;
            }
            SlotManagement.MoneyWithdraw((uint)(stack * item.BuyPrice));
        }

        private void OnStackValueChanged()
        {
            int input;
            Int32.TryParse(_stackInput.text, out input);
            ComponentItem transformItem = _clickedItemTransform.GetComponent<ComponentItem>();
            //pokud je vloženo větší číslo než je počet stacků
            if (input > transformItem.ActualStack)
            {
                input = transformItem.ActualStack;    //číslo je upraveno na stack
            }
            _stackInput.text = input.ToString();
        }

        private static void OnClickButtonStack(ComponentItem transformItem)
        {
            int input;
            Int32.TryParse(_stackInput.text, out input);   //parsování inputu
            if (input == 0)
            {
                return;
            }
            Transform slot = SlotManagement.FindEmptySlot(EItemState.Inventory);    //nalezení prázného slotu
            if (slot == null)
            {
                Debug.LogWarning("Inventory is full");
                StackPrefab.SetActive(false);
                return;
            }
            slot.GetComponent<Slot>().Occupied = true;      // nastavení slotu že je okupován
            GameObject obj = Instantiate(Resources.Load<GameObject>("Prefab/Item"));    //vytvoření objektu
            ComponentItem objItem = obj.GetComponent<ComponentItem>();
            NewItem.SetStats(obj, NewItem.IdToItem(transformItem.ID));    //nastavení statů objektu
            if (SlotManagement.StackUnderflow(transformItem, input))  //pokud je input stejný jak max počet tak nic nedělám
            {
                slot.GetComponent<Slot>().Occupied = false;
                Debug.Log("UnderFlow");
                return;
            }
            else
            {
                transformItem.ActualStack -= input;   //odebrání staků
                objItem.ActualStack = input;  //nastavení staků na input
                objItem.EItemState = EItemState.Inventory;
                obj.transform.SetParent(slot);  //nastavení rodiče
                _gameMasterItem.InventoryList.Add(objItem);     //přidání do listu
                SlotManagement.ShowStack(transformItem);      //zobrazení stacku
            }
            SlotManagement.ShowStack(objItem);        //zobrazení stacku
            StackPrefab.SetActive(false);  //zničení okna pro zadávání počtu stacků
        }
    }
}