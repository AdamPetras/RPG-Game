using System;
using System.Threading;
using Assets.Script.CharacterFolder;
using Assets.Script.StatisticsFolder;
using Assets.Script.TargetFolder;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Script.HUD
{
    public class DamageHud
    {
        private Vector2 positionBeforePl;
        private float damagebefore;
        private Font font;
        private int _fontSize;
        private Color _color;
        private bool _isPlayer;
        private GUIStyle _guiStyleOne;
        public DamageHud(Color color, int fontSize, bool isPlayer)
        {
            font = Resources.Load<Font>("Graphics/Fonts/RaviPrakash");
            _color = color;
            _fontSize = fontSize;
            _isPlayer = isPlayer;
            _guiStyleOne = new GUIStyle();
            _guiStyleOne.fontSize = _fontSize;
            _guiStyleOne.font = font;
            _guiStyleOne.normal.textColor = _color;
        }

       /* public void OnGUI(float damage, bool isCrit)
        {

            positionBeforePl = SetPositionPlayerHud(damage);
            damagebefore = damage;
            string text = "";
            if (!_isPlayer)
                text += "-";
            if (isCrit)
            {
                _guiStyleOne.fontSize = _fontSize + 10;
                text += Math.Round(damage, 0) + "Critical!!";
            }
            else
            {
                _guiStyleOne.fontSize = _fontSize;
                text += Math.Round(damage, 0).ToString();
            }
            GUI.Label(new Rect(positionBeforePl.x, positionBeforePl.y, 100, 50), text, _guiStyleOne);
        }*/

        private Vector2 SetPositionPlayerHud(float damage)
        {
            if (damagebefore != damage && damage != 0)
            {
                return new Vector2(Random.Range(Screen.width / 2 - 75, Screen.width / 2 + 75),
                    Random.Range(Screen.height / 2 - 75, Screen.height / 2 + 75));
            }
            return positionBeforePl;
        }
    }
}
