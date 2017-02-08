using DesktopServerLogical.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DesktopServerLogical.Models
{
    public class BlockControl
    {
        private UIElement _block;
        private List<BlockControl> _childs;
        private BlockType _type;
        private string _value;
        private BlockControl _parent;

        public BlockControl Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public BlockType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public List<BlockControl> Childs
        {
            get { return _childs; }
            set { _childs = value; }
        }

        public UIElement Block
        {
            get { return _block; }
            set { _block = value; }
        }
        public BlockControl(BlockControl parent, UIElement b, BlockType type)
            : this(parent, b, new List<BlockControl>(), type)
        {

        }
        public BlockControl(UIElement b, BlockType type)
            : this(null, b, new List<BlockControl>(), type)
        {

        }
        public BlockControl(UIElement b, BlockType type, List<BlockControl> childs)
            : this(null, b, childs, type)
        {

        }
        public BlockControl(BlockControl parent,UIElement b, List<BlockControl> childs, BlockType type)
        {
            _parent = parent;
            _block = b;
            _childs = childs;
            _type = type;
        }
        public void AddSubBlockControl(BlockControl b)
        {
            _childs.Add(b);
        }
        public bool doesIntersectsWith(BlockControl b, Visual v)
        {
            System.Drawing.Rectangle r1 = Helpers.GetRectangleOfButtonControl(v, _block);
            System.Drawing.Rectangle r2 = Helpers.GetRectangleOfButtonControl(v, b.Block);
            return r1.IntersectsWith(r2);
        }
        public object GetValue()
        {
            object value = null;
            switch (_type)
            {
                case BlockType.PinTriggered:
                    value = ((ComboBox)((Canvas)_block).Children[1]).SelectedItem;
                    break;
                case BlockType.For:
                    value = ((TextBox)((Canvas)_block).Children[1]).Text;
                    break;
                case BlockType.SwitchAction:
                    value = ((ComboBox)((Canvas)_block).Children[1]).SelectedItem;
                    break;
            }
            return value;
        }
    }
}
