using System;
using System.Collections.Generic;

namespace IngameScript
{
    public class MenuItem
    {
        private Func<MenuItem, bool> ChooseAction;
        private Func<MenuItem, string> TextAction;
        public string Text;
        public List<MenuItem> SubItems;

        // Standard constructor
        public MenuItem(string text, List<MenuItem> subItems = null, Func<MenuItem, bool> chooseAction = null, Func<MenuItem, string> textAction = null)
        {
            this.Text = text;
            this.ChooseAction = chooseAction;
            this.TextAction = textAction;

            // Initialize children
            if (subItems == null)
                this.SubItems = new List<MenuItem>();
            else
                this.SubItems = subItems;

            // If no text action is specified, use the default one
            if (this.TextAction == null)
                this.TextAction = _GetText;
        }

        public MenuItem(string text, Func<MenuItem, bool> chooseAction, Func<MenuItem, string> textAction = null) : this(text, null, chooseAction, textAction) { }
        public MenuItem(Func<MenuItem, string> textAction, Func<MenuItem, bool> chooseAction = null) : this("", chooseAction, textAction) { }

        private string _GetText(MenuItem item)
        {
            return this.Text;
        }

        public string GetText()
        {
            return this.TextAction(this);
        }

        public bool DoAction()
        {
            if (this.ChooseAction == null)
            {
                return true;
            }
            else
            {
                return this.ChooseAction(this);
            }
        }

        public void SetText(string text)
        {
            this.Text = text;
        }

        public void SetTextAction(Func<MenuItem, string> textAction)
        {
            this.TextAction = textAction;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            MenuItem otherItem = obj as MenuItem;
            if (otherItem != null)
                return this.GetText().CompareTo(otherItem.GetText());
            else
                throw new ArgumentException("Object is not a MenuItem");
        }
    }
}
