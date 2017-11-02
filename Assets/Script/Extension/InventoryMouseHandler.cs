using System;
using Assets.Script.CharacterFolder;
using Assets.Script.InventoryFolder;
using Assets.Scripts;
using Assets.Scripts.Extension;
using Assets.Scripts.InventoryFolder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.Extension
{
    public class InventoryMouseHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public static GameObject itemBeingDragged;
        private Vector3 startPosition;
        private Transform startParent;
        private Transform DragPanel;

        private void Start()
        {
            DragPanel = GameObject.Find("DragPanel").transform.Find("Background");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (gameObject.GetComponent<ComponentItem>().EItemState == EItemState.Armor)        //pokud bereme item z armor slotu
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().ArmorList.Remove(gameObject.GetComponent<ComponentItem>());
                SlotManagement.AddRemoveStats(gameObject.GetComponent<ComponentItem>(), false);     //odečteme staty kvůli odebrání
                transform.parent.GetComponent<ArmorSlot>().Occupied = false;     //nastavíme že neni okupován
            }
            else if (gameObject.GetComponent<ComponentItem>().EItemState == EItemState.Inventory)   //jinak pokud bereme z inventáře
            {
                if (transform.parent.GetComponent<Slot>() != null)      //a pokud neni slot null
                {
                    transform.parent.GetComponent<Slot>().Occupied = false; //nastavíne že neni okupován
                }
            }
            itemBeingDragged = gameObject;          //nastavení přetahovaného objektu na tento item
            startPosition = transform.position;     //uložíme počáteční pozici
            startParent = transform.parent;         //uložíme počátečního rodiče
            GetComponent<CanvasGroup>().blocksRaycasts = false;     //nastavíme canvasgroup aby neblokoval raycasty(nešlo by item pustit)
            gameObject.transform.SetParent(DragPanel);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;       //pokud držíme nastavení pozice na pozici myši
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (transform.parent == startParent && itemBeingDragged.GetComponent<ComponentItem>().EItemState == EItemState.Armor)       //pokud pustíme na stejné místo a je to armor slot
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().ArmorList.Add(itemBeingDragged.GetComponent<ComponentItem>());
                SlotManagement.AddRemoveStats(itemBeingDragged.GetComponent<ComponentItem>());  //přičteme zpět staty
            }
            if (transform.parent == DragPanel)
            {
                itemBeingDragged.SetActive(false);
                GameObject dialog = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/DialogWindow"));
                Transform dialogBackground = dialog.transform.Find("Background");
                dialogBackground.Find("Text").GetComponent<Text>().text = "Do you want to destroy "+itemBeingDragged.GetComponent<ComponentItem>().Name;
                dialogBackground.Find("Yes").GetComponent<Button>().onClick.AddListener(delegate
                {
                    Destroy(itemBeingDragged);
                    Destroy(dialog);

                });
                dialogBackground.Find("No").GetComponent<Button>().onClick.AddListener(delegate
                    {
                        itemBeingDragged.SetActive(true);
                        itemBeingDragged.transform.SetParent(startParent);
                        Destroy(dialog);
                        transform.position = startPosition;     //nastavení pozice na pozici
                        itemBeingDragged = null;    //nastavení že item již není přemýsťovatelný 
                        GetComponent<CanvasGroup>().blocksRaycasts = true;  //zpět nastavení blokování raycastu
                    });
            }
            else
            {                                       
                SlotManagement.ShowStack(itemBeingDragged.GetComponent<ComponentItem>());   //vykreslení stacků   
                transform.position = startPosition;     //nastavení pozice na pozici
                itemBeingDragged = null;    //nastavení že item již není přemýsťovatelný 
                GetComponent<CanvasGroup>().blocksRaycasts = true;  //zpět nastavení blokování raycastu
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowInfo();     //pokud pointer vstoupil do objektu ukáže se info
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            HideInfo();     //pokud pointer vystoupil z objektu skryje se info
        }

        private void ShowInfo()
        {
            transform.Find("Panel").GetComponent<Image>().enabled = true;
            transform.Find("Panel").Find("ItemInfo").GetComponent<Text>().text = GetComponent<ComponentItem>().Name;
        }

        private void HideInfo()
        {
            transform.Find("Panel").GetComponent<Image>().enabled = false;
            transform.Find("Panel").Find("ItemInfo").GetComponent<Text>().text = "";
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right && Input.GetKey(KeyCode.LeftShift))      //tabulka pro rozdělení itemů z 5ti stacků třeba na 2 a 3 stacky
            {
                if (gameObject.GetComponent<ComponentItem>().ActualStack <= 1)
                    return;
                if (gameObject.GetComponent<ComponentItem>().EItemState == EItemState.Inventory)    //pokud je objekt v inventáři (nemůžu to dělat v dropu)
                {
                    GetComponent<CanvasGroup>().blocksRaycasts = false;
                    GameObject stackPrefab = Instantiate(Resources.Load<GameObject>("Prefab/StackPrefab"));     //vytvoření objektu kde vložíme počet stacků
                    Transform background = stackPrefab.transform.Find("Background");
                    background.position = eventData.position - background.GetComponent<RectTransform>().sizeDelta / 2;      //pozicování podlemyši
                    //přidání funkce která ošetřuje aby nebylo vloženo velké číslo
                    background.Find("ExitKey").GetComponent<Button>().onClick.AddListener(delegate
                    {
                        GetComponent<CanvasGroup>().blocksRaycasts = true;
                        Destroy(stackPrefab);
                    });
                    background.Find("InputField").GetComponent<InputField>().onValueChanged.AddListener(
                        delegate
                        {
                            OnStackValueChanged(background);
                        });
                    //přidání funkce na button (vytvoří nový item atd.)
                    background.Find("Button").GetComponent<Button>().onClick.AddListener(
                        delegate
                        {
                            OnClickButtonStack(background);
                        });
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)    //pokud je pouze kliknutí pravým
            {
                //nasaď do armory pokud je v dropu tak hoď do invu nebo prodej (podle otevření obchodu)
                if (gameObject.GetComponent<ComponentItem>().EItemState == EItemState.Inventory)
                {
                    if (!SlotManagement.AddToArmor(gameObject.GetComponent<ComponentItem>()))
                        return;
                    GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventoryList.Remove(gameObject.GetComponent<ComponentItem>());
                    HideInfo();
                }
                else if (gameObject.GetComponent<ComponentItem>().EItemState == EItemState.Drop)    //pokud je objekt v dropu
                {
                    Destroy(gameObject.transform.parent.parent.gameObject); //zničí se objekt
                    SlotManagement.AddToInventory(gameObject);      //přidá se do inventáře
                    HideInfo();     //skrytí info (nemusí být ale pak je prodleva než se to skryje)
                }
            }
        }

        private void OnStackValueChanged(Transform background)
        {
            int input;
            Int32.TryParse(background.Find("InputField").GetComponent<InputField>().text, out input);
            //pokud je vloženo větší číslo než je počet stacků
            if (input > transform.GetComponent<ComponentItem>().ActualStack)
            {
                input = transform.GetComponent<ComponentItem>().ActualStack;    //číslo je upraveno na stack
            }
            background.Find("InputField").GetComponent<InputField>().text = input.ToString();
        }

        private void OnClickButtonStack(Transform background)
        {
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            int input;
            Int32.TryParse(background.Find("InputField").GetComponent<InputField>().text, out input);   //parsování inputu
            if (input == 0)
            {
                return;
            }
            Transform slot = SlotManagement.FindEmptySlot(EItemState.Inventory);    //nalezení prázného slotu
            if (slot == null)
            {

                Debug.LogWarning("Inventory is full");
                Destroy(background.parent.gameObject);
                return;
            }
            slot.GetComponent<Slot>().Occupied = true;      // nastavení slotu že je okupován
            GameObject obj = Instantiate(Resources.Load<GameObject>("Prefab/Item"));    //vytvoření objektu
            NewItem.SetStats(obj, NewItem.IdToItem(transform.GetComponent<ComponentItem>().ID));    //nastavení statů objektu
            if (SlotManagement.StackUnderflow(transform.GetComponent<ComponentItem>(), input))  //pokud je input stejný jak max počet tak nic nedělám
            {
                Debug.Log("UnderFlow");
            }
            else
            {
                transform.GetComponent<ComponentItem>().ActualStack -= input;   //odebrání staků
                obj.GetComponent<ComponentItem>().ActualStack = input;  //nastavení staků na input
                obj.transform.SetParent(slot);  //nastavení rodiče
                GameObject.Find("GameMaster").GetComponent<ComponentInventory>().InventoryList.Add(obj.GetComponent<ComponentItem>());     //přidání do listu
                SlotManagement.ShowStack(transform.GetComponent<ComponentItem>());      //zobrazení stacku
            }
            SlotManagement.ShowStack(obj.GetComponent<ComponentItem>());        //zobrazení stacku
            Destroy(background.parent.gameObject);  //zničení okna pro zadávání počtu stacků
        }
    }
}