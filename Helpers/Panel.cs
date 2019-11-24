using System.Collections.Generic;
using VRage.Game;

namespace IngameScript.Helpers
{
    public static class Panel
    {
        private const float _wRes = 18944f / 28.8f;
        private const float _wResWD = _wRes * 2;
        private const float _hRes = _wRes * 0.99375f;

        public static Dictionary<MyCubeSize, Dictionary<string, PanelSize>> _panelD = new Dictionary<MyCubeSize, Dictionary<string, PanelSize>>()
{
  { MyCubeSize.Large, new Dictionary<string, PanelSize> {
    { "LargeLCDPanel", new PanelSize(_wRes, _hRes  ) },
    { "LargeLCDPanelWide", new PanelSize(_wResWD, _hRes ) },
    { "LargeLCDPanel2x2", new PanelSize(_wRes, _hRes ) },
    { "LargeBlockCorner_LCD_1", new PanelSize(_wRes, 98.667f ) },
    { "LargeBlockCorner_LCD_2", new PanelSize(_wRes, 98.667f ) },
    { "LargeBlockCorner_LCD_Flat_1", new PanelSize(_wRes, 111.0f ) },
    { "LargeBlockCorner_LCD_Flat_2", new PanelSize(_wRes, 111.0f ) },
    { "FlightLCD", new PanelSize(655f, 240.5f) },
    { "LargeLCDPanelSlope2Base4", new PanelSize(_wRes, _hRes) },
    { "LargeLCDPanelSlope2Base3", new PanelSize(_wRes, _hRes) },
    { "LargeLCDPanelSlope2Base2", new PanelSize(_wRes, _hRes) },
    { "LargeLCDPanelSlope2Base1", new PanelSize(_wRes, _hRes) },
    { "LargeLCDPanelSlope2Tip4", new PanelSize(_wRes, _hRes) },
    { "LargeLCDPanelSlope2Tip3", new PanelSize(_wRes, _hRes) },
    { "LargeLCDPanelSlope2Tip2", new PanelSize(_wRes, _hRes) },
    { "LargeLCDPanelSlope2Tip1", new PanelSize(_wRes, _hRes) },
    { "LargeLCDPanelSlopeV", new PanelSize(_wRes, _hRes) },
    { "LargeLCDPanelSlopeH", new PanelSize(_wRes, _hRes) }}
  },
  { MyCubeSize.Small, new Dictionary<string, PanelSize> {
    { "SmallTextPanel", new PanelSize(_wRes, _hRes) },
    { "SmallLCDPanel", new PanelSize(_wRes, _hRes) },
    { "SmallLCDPanelWide", new PanelSize(_wResWD, _hRes) },
    { "SmallBlockCorner_LCD_1", new PanelSize(_wRes, 98.667f ) },
    { "SmallBlockCorner_LCD_2", new PanelSize(_wRes, 98.667f ) },
    { "SmallBlockCorner_LCD_Flat_1", new PanelSize(_wRes, 111.0f ) },
    { "SmallBlockCorner_LCD_Flat_2", new PanelSize(_wRes, 111.0f ) },
    { "SmallTextPanelSlopeBase4", new PanelSize(_wRes, _hRes) },
    { "SmallTextPanelSlopeBase3", new PanelSize(_wRes, _hRes) },
    { "SmallTextPanelSlopeBase2", new PanelSize(_wRes, _hRes) },
    { "SmallTextPanelSlopeBase1", new PanelSize(_wRes, _hRes) },
    { "SmallTextPanelSlopeTip4", new PanelSize(_wRes, _hRes) },
    { "SmallTextPanelSlopeTip3", new PanelSize(_wRes, _hRes) },
    { "SmallTextPanelSlopeTip2", new PanelSize(_wRes, _hRes) },
    { "SmallTextPanelSlopeTip1", new PanelSize(_wRes, _hRes) },
    { "SmallTextPanelSlopeV", new PanelSize(_wRes, _hRes) },
    { "SmallTextPanelSlopeH", new PanelSize(_wRes, _hRes) }}
  }
};

        public static Dictionary<FontType, Dictionary<char, int>> _fontD = new Dictionary<FontType, Dictionary<char, int>>()
{
  { FontType.Default, new Dictionary<char, int> {
    {' ', 8}, {'!', 8}, {'"', 10}, {'#', 19}, {'$', 20}, {'%', 24}, {'&', 20}, {'(', 9}, {')', 9}, {'*', 11}, {'+', 18}, {',', 9},
    {'-', 10}, {'.', 9}, {'/', 14}, {'0', 19}, {'1', 9}, {'2', 19}, {'3', 17}, {'4', 19}, {'5', 19}, {'6', 19}, {'7', 16}, {'8', 19},
    {'9', 19}, {':', 9}, {';', 9}, {'<', 18}, {'=', 18}, {'>', 18}, {'?', 16}, {'@', 25}, {'A', 21}, {'B', 21}, {'C', 19}, {'D', 21},
    {'E', 18}, {'F', 17}, {'G', 20}, {'H', 20}, {'I', 8}, {'J', 16}, {'K', 17}, {'L', 15}, {'M', 26}, {'N', 21}, {'O', 21}, {'P', 20},
    {'Q', 21}, {'R', 21}, {'S', 21}, {'T', 17}, {'U', 20}, {'V', 20}, {'W', 31}, {'X', 19}, {'Y', 20}, {'Z', 19}, {'[', 9}, {']', 9},
    {'^', 18}, {'_', 15}, {'`', 8}, {'a', 17}, {'b', 17}, {'c', 16}, {'d', 17}, {'e', 17}, {'f', 9}, {'g', 17}, {'h', 17}, {'i', 8},
    {'j', 8}, {'k', 17}, {'l', 8}, {'m', 27}, {'n', 17}, {'o', 17}, {'p', 17}, {'q', 17}, {'r', 10}, {'s', 17}, {'t', 9}, {'u', 17},
    {'v', 15}, {'w', 27}, {'x', 15}, {'y', 17}, {'z', 16}, {'{', 9}, {'|', 6}, {'}', 9}, {'~', 18}, {'\\', 12}, {'\'', 6}}},
  { FontType.Monospace, new Dictionary<char, int> { } }
    //{' ', 24}, {'!', 24}, {'"', 24}, {'#', 24}, {'$', 24}, {'%', 24}, {'&', 24}, {'(', 24}, {')', 24}, {'*', 24}, {'+', 24}, {',', 24},
    //{'-', 24}, {'.', 24}, {'/', 24}, {'0', 24}, {'1', 24}, {'2', 24}, {'3', 24}, {'4', 24}, {'5', 24}, {'6', 24}, {'7', 24}, {'8', 24},
    //{'9', 24}, {':', 24}, {';', 24}, {'<', 24}, {'=', 24}, {'>', 24}, {'?', 24}, {'@', 24}, {'A', 24}, {'B', 24}, {'C', 24}, {'D', 24},
    //{'E', 24}, {'F', 24}, {'G', 24}, {'H', 24}, {'I', 24}, {'J', 24}, {'K', 24}, {'L', 24}, {'M', 24}, {'N', 24}, {'O', 24}, {'P', 24},
    //{'Q', 24}, {'R', 24}, {'S', 24}, {'T', 24}, {'U', 24}, {'V', 24}, {'W', 24}, {'X', 24}, {'Y', 24}, {'Z', 24}, {'[', 24}, {']', 24},
    //{'^', 24}, {'_', 24}, {'`', 24}, {'a', 24}, {'b', 24}, {'c', 24}, {'d', 24}, {'e', 24}, {'f', 24}, {'g', 24}, {'h', 24}, {'i', 24},
    //{'j', 24}, {'k', 24}, {'l', 24}, {'m', 24}, {'n', 24}, {'o', 24}, {'p', 24}, {'q', 24}, {'r', 24}, {'s', 24}, {'t', 24}, {'u', 24},
    //{'v', 24}, {'w', 24}, {'x', 24}, {'y', 24}, {'z', 24}, {'{', 24}, {'|', 24}, {'}', 24}, {'~', 24}, {'\\', 24}, {'\'', 24}}}
};

        public struct PanelSize
        {
            public readonly float Width;
            public readonly float Height;

            public PanelSize(float width, float height)
            {
                this.Width = width;
                this.Height = height;
            }
        }

        public enum FontType { Default, Monospace };
    }
}
