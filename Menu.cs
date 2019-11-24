using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    public class Menu
    {
        private MenuItem SelectedItem;
        public Stack<MenuItem> Path;
        private StringBuilder SB;
        public bool HasUpdate = false;

        public Menu(string text, List<MenuItem> items)
        {
            this.Path = new Stack<MenuItem>();
            this.Path.Push(new MenuItem(text, items));
            this.SB = new StringBuilder();

            if (this.Path.Peek().SubItems.Count > 0)
            {
                this.SelectedItem = this.Path.Peek().SubItems[0];
                HasUpdate = true;
            }
        }

        public void Up()
        {
            int index = this.Path.Peek().SubItems.IndexOf(this.SelectedItem) - 1;
            if (index >= 0)
            {
                this.SelectedItem = this.Path.Peek().SubItems[index];
                HasUpdate = true;
            }
        }

        public void Down()
        {
            int index = this.Path.Peek().SubItems.IndexOf(this.SelectedItem) + 1;
            if (index < this.Path.Peek().SubItems.Count)
            {
                this.SelectedItem = this.Path.Peek().SubItems[index];
                HasUpdate = true;
            }
        }

        public void Choose()
        {
            var currentItem = this.SelectedItem;
            if (currentItem.DoAction())
            {
                if (currentItem.SubItems.Count > 0)
                {
                    currentItem.SubItems.Add(new MenuItem("Back", (item) => { this.Back(); return true; }));
                    this.Path.Push(currentItem);
                    this.SelectedItem = this.Path.Peek().SubItems[0];
                }
                HasUpdate = true;
            }
        }

        public void Back()
        {
            this.Path.Peek().SubItems.RemoveAt(this.Path.Peek().SubItems.Count - 1);
            this.Path.Pop();
            this.SelectedItem = this.Path.Peek().SubItems[0];
            HasUpdate = true;
        }

        public void SetSelectedIndex(int i)
        {
            this.SelectedItem = this.Path.Peek().SubItems[i];
        }

        // Return string of rendered menu
        public string Draw(int textWidth = 70, int textHeight = 10)
        {
            // Clear the String Builder.
            this.SB.Clear();

            var currentItem = this.Path.Peek();
            if (currentItem == null)
            {
                throw new Exception("Menu: currentItem is null");
            }

            // Add Breadcrumbs
            SB.AppendLine(this.Breadcrumbs());
            SB.AppendLine();

            foreach (var item in currentItem.SubItems)
            {
                SB.AppendFormat("{0} {1}{2}\n", item == this.SelectedItem ? "»" : "  ", item.GetText(), item.SubItems.Count > 0 ? " ..." : "");
            }

            return SB.ToString();
        }

        private string Breadcrumbs()
        {
            if (this.Path.Count == 1)
            {
                return "   " + this.Path.Peek().GetText();
            }
            else
            {
                var item = this.Path.Pop();
                var s = this.Breadcrumbs() + " / " + item.GetText();
                this.Path.Push(item);
                return s;
            }
        }
    }
}
