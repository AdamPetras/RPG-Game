﻿using System.Collections.Generic;
using System.Linq;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.HUD;
using Assets.Script.QuestFolder;
using Assets.Scripts.InventoryFolder;
using Assets.Scripts.InventoryFolder.CraftFolder;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Script.InventoryFolder.CraftFolder
{

    public class CraftSettings
    {
        private GetCraftComponent _getCraftComponent;
        private GameObject _craftObject;
        private GameObject _cardPrefab;
        private List<NewItem> _defaultCardList;
        private List<NewItem> _cardList;
        private string _searchString;
        private CraftDataOperating _craftDataOperating;
        private EProfession _eProfession;
        private bool _finding;
        private const int XSIZE = 500;
        private const int YSIZE = 400;
        private PlayerComponent _playerComponent;
        public CraftSettings(GameObject mainObj)
        {
            _craftObject = mainObj;
            _defaultCardList = new List<NewItem>();
            _cardList = new List<NewItem>();
            _getCraftComponent = new GetCraftComponent(mainObj);
            _craftDataOperating = new CraftDataOperating(_getCraftComponent);
            _eProfession = EProfession.None;
            _finding = false;
            SetCraftMenuSize();
            _cardPrefab = Resources.Load("Prefab/ViewCard") as GameObject;
            _getCraftComponent.GetCookingToggle().onValueChanged.AddListener(ChoiseCooking);
            _getCraftComponent.GetSmithingToggle().onValueChanged.AddListener(ChoiseSmithing);
            _getCraftComponent.GetCraftingToggle().onValueChanged.AddListener(ChoiseCrafting);
            _getCraftComponent.GetTailoringToggle().onValueChanged.AddListener(ChoiseTailoring);
            _defaultCardList = Database.ItemDatabase.Where(s => s.EProfession != EProfession.None && s.EProfession != EProfession.Fishing).ToList();
            _craftObject.GetComponent<Canvas>().enabled = true;
            Utilities.ListOfAllObjects.Add(_craftObject);
            _craftObject.SetActive(false);
            Utilities.DisableOrEnableAll(_craftObject);
        }

        private void SetCraftMenuSize()
        {
            _getCraftComponent.GetMainPanel().GetComponent<RectTransform>().sizeDelta = new Vector2(XSIZE, YSIZE);
            _getCraftComponent.GetMainPanel().GetComponent<RectTransform>().localPosition =
                new Vector2(_getCraftComponent.GetMainPanel().GetComponent<RectTransform>().rect.width / 2,
                    _getCraftComponent.GetMainPanel().GetComponent<RectTransform>().rect.height / 2);
        }

        public void Update()
        {
            if (_playerComponent == null)
            {
                _playerComponent = GameObject.FindWithTag("Player").GetComponent<PlayerComponent>();
            }
            if (Input.GetKeyUp(KeyCode.P) && !ComponentCraftMenu.Visible)
            {

                _craftObject.SetActive(true);
                Utilities.DisableOrEnableAll(_craftObject, true);
                ComponentCraftMenu.Visible = true;
            }
            else if (Input.GetKeyUp(KeyCode.P) && ComponentCraftMenu.Visible)
            {
                Debug.Log("showw");
                ComponentCraftMenu.Visible = false;
                Utilities.DisableOrEnableAll(_craftObject);
                _craftObject.SetActive(false);
            }
            if (Input.GetKeyUp(KeyCode.Escape) && ComponentCraftMenu.Visible)
            {
                ComponentCraftMenu.Visible = false;
                Utilities.DisableOrEnableAll(_craftObject);
                _craftObject.SetActive(false);
            }
            if (ComponentCraftMenu.Visible)
            {
                if (_getCraftComponent.GetSearch().GetComponent<InputField>().text != ""
                ) //pokud je naplněno pole s vyhledáváním
                {
                    _finding = true;
                    _searchString = _getCraftComponent.GetSearch().GetComponent<InputField>().text
                        .ToLower(); //uložení řetězce do proměnné
                    CardFound(_defaultCardList);
                }
                else if (_getCraftComponent.GetSearch().GetComponent<InputField>().text == "" && _finding
                ) //pokud vyhledávám a smažu pole s vyhledáváním
                {
                    AllOfList(_cardList, _defaultCardList);
                    _finding = false;
                }
                LoadScrollView(_cardList);
            }
        }
        private void LoadScrollView(List<NewItem> cardList)
        {
            //načtení itemů z listu
            if (cardList.Count != 0)
            {
                //itemy které nejsou instanciované a popř jsou z vybrané složky
                foreach (NewItem card in cardList.Where(s => !s.Instantiate))
                {
                    GameObject cardObject = Object.Instantiate(_cardPrefab);   //instanciování        
                    card.Instantiate = true;    //nastavení že byl instanciován                  
                    card.CraftingObject = cardObject;  //uložení objektu pro případné odstranění(úpravu)
                    if (card.Icon != null)  //pokud je nějaká ikona
                        cardObject.transform.Find("Image").GetComponent<Image>().sprite = card.Icon;    //uložení ikony do proměnné
                    cardObject.transform.Find("Name").GetComponent<Text>().text = card.Name;    //uložení jména do proměnné                    
                    cardObject.transform.SetParent(_getCraftComponent.GetScrollView().transform.Find("ViewPort").transform, false);    //nastavení rodiče
                    cardObject.transform.localScale = Vector3.one;  //nastavení transformu na scaleování
                    cardObject.name = card.ID.ToString();
                    cardObject.GetComponent<Button>().onClick.AddListener(delegate { OnClick(int.Parse(cardObject.name)); });
                }
            }
        }

        private void CardFound(List<NewItem> viewCardList)
        {
            foreach (NewItem viewCard in viewCardList)
            {
                if (viewCard.Name.ToLower().Contains(_searchString))    //pokud list obsahuje item podle zadaného řetězce
                {
                    if (_eProfession == EProfession.None && viewCard.EProfession != EProfession.None)    //hledám neurčitý item
                    {
                        FoundCard(_cardList, viewCard);
                    }
                    else if (_eProfession == viewCard.EProfession)     //hledám určený item 
                    {
                        FoundCard(_cardList, viewCard);
                    }
                }
                else
                {
                    RemoveFromList(_cardList, viewCard);
                }
            }
        }

        private void FoundCard(List<NewItem> viewCardList, NewItem viewCard)
        {
            if (viewCardList.All(s => s.Name != viewCard.Name))     //ošetření aby nebylo v cyklu vkládáno několik itemů
            {
                viewCardList.Add(viewCard); //přidání do listu
            }
        }
        private void AllOfList(List<NewItem> destinationList, List<NewItem> sourceList)
        {
            //if (_eProfession == EProfession.None) //ošetření pokud je vybrané políčko a vyplní se vyhledávací lišta a vymaže
            ClearList(_cardList);  //vymazání listu podle toho co je vybrané
                                   // else ClearList(sourceList);
            _searchString = _getCraftComponent.GetSearch().GetComponent<InputField>().text = "";    //načtení z vyhledávacího pole do řetězce
            _finding = false;
            foreach (NewItem viewCard in sourceList)
            {
                if ((viewCard.EProfession == _eProfession) && !destinationList.Contains(viewCard))  //pokud je zařazen do... a neni již obsažen
                {
                    destinationList.Add(viewCard);  //přidání listu
                }
            }
        }

        private void RemoveFromList(List<NewItem> viewCardList, NewItem viewCard)
        {
            if (viewCardList.Contains(viewCard) || _getCraftComponent.GetSearch().GetComponent<InputField>().text.ToLower() == "")
            {
                viewCardList.Remove(viewCard);  //odstranění vybraného objektu z listu
                Remove(viewCard);
            }
        }

        private void ClearList(List<NewItem> viewCardList)
        {
            foreach (NewItem viewCard in viewCardList)
            {
                if (viewCard != null)
                    Remove(viewCard);
            }
            viewCardList.Clear();   //clearnutí celého listu
        }

        private void Remove(NewItem viewCard)
        {
            viewCard.CraftingObject.GetComponent<Button>().onClick.RemoveAllListeners();
            Object.Destroy(viewCard.CraftingObject);  //destroy objektu
            viewCard.Instantiate = false;   //nastavení proměnné kvůli znovu vytvoření stejného objektu
        }

        private void ChoiseCooking(bool value)
        {
            if (value)
            {
                _eProfession = EProfession.Cooking;
                AllOfList(_cardList, _defaultCardList);
                //...ošetření (pokud je u ohně atd...)
                //...
            }
            else
            {
                _eProfession = EProfession.None;
                ClearList(_cardList);
            }
        }
        private void ChoiseCrafting(bool value)
        {
            if (value)
            {
                _eProfession = EProfession.Crafting;
                AllOfList(_cardList, _defaultCardList);
                //...ošetření (pokud je u stolu(craftingtablu) atd...)
                //...
            }
            else
            {
                _eProfession = EProfession.None;
                ClearList(_cardList);
            }
        }

        private void ChoiseTailoring(bool value)
        {
            if (value)
            {
                _eProfession = EProfession.Tailoring;
                AllOfList(_cardList, _defaultCardList);
                //...ošetření (pokud je u ohně atd...)
                //...
            }
            else
            {
                _eProfession = EProfession.None;
                ClearList(_cardList);
            }
        }
        private void ChoiseSmithing(bool value)
        {
            if (value)
            {
                _eProfession = EProfession.Smithing;
                AllOfList(_cardList, _defaultCardList);
                //...ošetření (pokud je u pece atd...)
                //...
            }
            else
            {
                _eProfession = EProfession.None;
                ClearList(_cardList);
            }
        }

        private void OnClick(int id)
        {
            _craftDataOperating.SelectedItem = NewItem.IdToItem(id);
            _craftDataOperating.ItemClicked(_playerComponent);
        }
    }
}