using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Data;
using Language;
using System.Reflection;
using Layout.Formatters;

namespace Interface
{
    public delegate void SymbolViewChangedHandler();

    public partial class SymbolView : UserControl
    {
        State state;
        public State State
        {
            get { return state; }
            set
            {
                state = value;
                if (state != null)
                {
                    state.SymbolTableChanged += new SymbolTableChangedHandler(populateTreeViews);
                }
            }
        }

        public event SymbolViewChangedHandler SymbolViewChanged;

        public SymbolView()
        {
            InitializeComponent();

            PropertyInfo aProp = typeof(ListBox).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            aProp.SetValue(list, true, null); 
        }

        void populateTreeViews()
        {
            list.SuspendLayout();

            list.Items.Clear();
            
            foreach (var s in state.SymbolTable)
            {
                list.Items.Add(s);
            }

            if(state.SymbolTable.Count() > 0)
                list.SelectedIndex = 0;
            
            list.ResumeLayout();
        }

        public Symbol GetSelected()
        {
            if (list.SelectedItem != null)
                return (list.SelectedItem as Symbol);
            else
                return Symbol.Constant;
        }
        
        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (list.SelectedItem != null && SymbolViewChanged != null)
                SymbolViewChanged();
        }
    }
}
