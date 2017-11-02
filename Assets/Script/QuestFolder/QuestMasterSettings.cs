using System.Collections;
using System.Linq;
using Assets.Script.CharacterFolder;
using Assets.Script.Extension;
using Assets.Script.HUD;
using Assets.Script.InventoryFolder;
using Assets.Script.Menu;
using Assets.Script.TargetFolder;
using Assets.Scripts.InventoryFolder;
using Assets.Scripts.InventoryFolder.CraftFolder;
using UnityEngine;

namespace Assets.Script.QuestFolder
{
    public class QuestMasterSettings : QuestMaster
    {
        private PlayerComponent _playerComponent;
        public QuestMasterSettings(int id, string name, GameObject prefab, Vector3 position)
        {
            ID = id;
            Name = name;
            Prefab = prefab;
            Position = position;
        }

        public void FindPlayer(PlayerComponent pl)
        {
            _playerComponent = pl;
        }

        public IEnumerator CheckMarks()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (QuestList.Count > 0)
                {

                    if (QuestList.Any(s => s.EQuestState == EQuestState.Complete))
                    {
                        SetMark(false, true, new Color32(118, 17, 17, 1));
                        Mark = GameObject.Find(Name).transform.Find("qMark");
                    }
                    else if (QuestList.Any(s => s.EQuestState == EQuestState.Progress))
                    {
                        SetMark(false, true, new Color32(73, 73, 73, 1));
                        Mark = GameObject.Find(Name).transform.Find("qMark");
                    }
                    else if (QuestList.Any(s => s.EQuestState == EQuestState.None && s.Level <= _playerComponent.Level))
                    {
                        SetMark(true, false, new Color32(118, 17, 17, 1));
                        Mark = GameObject.Find(Name).transform.Find("exMark");
                    }
                }
                else
                {
                    SetMark(false, false, new Color32(118, 17, 17, 1));
                }
            }
        }
        public IEnumerator MarkRotate()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                if (Mark != null)
                    Mark.rotation = Quaternion.AngleAxis(UnityEngine.Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
            }
        }
        private void SetMark(bool exMark, bool qMark, Color32 material)
        {
            GameObject.FindGameObjectsWithTag("QuestMaster").First(s => s.name == Name).transform.Find("qMark").GetComponent<MeshRenderer>().enabled = qMark;
            GameObject.FindGameObjectsWithTag("QuestMaster").First(s => s.name == Name).transform.Find("exMark").GetComponent<MeshRenderer>().enabled = exMark;
            GameObject.FindGameObjectsWithTag("QuestMaster").First(s => s.name == Name).transform.Find("qMark").GetComponent<Renderer>().material.color = material;
        }

        protected override void Submit(ModifyQuest quest)
        {
            _playerComponent.QuestList.Remove(quest);
            base.Submit(quest);
        }

        protected override void Reward(ModifyQuest quest)
        {
            base.Reward(quest);
            _playerComponent.AddExp(quest.Experiences);
            _playerComponent.Money += quest.MoneyReward;
            if (quest.ItemReward != null)
                foreach (QuestItem Qitem in quest.ItemReward)
                {
                    NewItem item = NewItem.IdToItem(Qitem.Item.ID);
                    item.ActualStack = Qitem.Quantity;
                    if (!SlotManagement.AddToInventory(item))
                        Debug.LogWarning("Inventory full");
                }
        }

        public override void Delivery(ModifyQuest quest)
        {
            if (quest.EQuest != EQuest.Delivery)
                return;
            if (quest.ItemToDeliver != null)
                if (quest.QuestMasterSubmit == ID && SlotManagement.FindItemInInventory(quest.ItemToDeliver.ID))
                {
                    if (SlotManagement.FindItemInInventory(quest.ItemToDeliver.ID).ActualStack >=
                        quest.ItemToDeliveryQuantity)
                    {
                        quest.EQuestState = EQuestState.Complete;
                        SlotManagement.DeleteItemByStacks(
                            SlotManagement.FindItemInInventory(quest.ItemToDeliver.ID),
                            quest.ItemToDeliveryQuantity);

                    }
                }
                else quest.EQuestState = EQuestState.Progress;
            else
            {
                quest.EQuestState = EQuestState.Complete;
            }
        }

        public void IfNear()
        {
            if (_playerComponent == null)
            {
                _playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>();
            }
            if ((Utilities.IsMouseHit(1, Name) && Vector3.Distance(Position, _playerComponent.gameObject.transform.position) < 2) || Talking)
            {           //mluví
                ShowAvailable();
                Talking = true;
                QuestMasterObject.Visible = true;
                if (Vector3.Distance(Position, _playerComponent.gameObject.transform.position) > 2 ||
                    EQuestMasterState == EQuestMasterState.None) //pokud se vzdálí nebo ukončí řeč
                {
                    OnExit();
                }

            }
        }


        protected override void Accept(ModifyQuest quest)
        {
            if (_playerComponent.Level >= quest.Level)
            {
                base.Accept(quest);
                quest.EQuestState = EQuestState.Progress;
                _playerComponent.QuestList.Add(quest);
            }
            else Debug.Log("You dont have enough level");
        }
    }
}
