using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Language;

namespace Interface
{
    public delegate void DataSetChangedHandler(Frame dataSet);
    public delegate void SymbolTableChangedHandler();
    public delegate void DisplayChangedHandler();
    
    public class State
    {
        Frame dataSet;
        public Frame DataSet { 
            get { return dataSet; }
            set { 
                dataSet = value; 
                X = Symbol.Constant;
                Y = Symbol.Constant;
                if (DataSetChanged != null) DataSetChanged(dataSet);
            } 
        }

        List<Symbol> symbolTable;
        public IEnumerable<Symbol> SymbolTable
        {
            get { return symbolTable; }
            set { symbolTable = value.ToList(); if (SymbolTableChanged != null) SymbolTableChanged(); }
        }

        public Symbol X = Symbol.Constant, Y = Symbol.Constant;

        public event DataSetChangedHandler DataSetChanged;
        public event SymbolTableChangedHandler SymbolTableChanged;
        public event DisplayChangedHandler DisplayChanged;

        public State()
        {
        }

        public void NotifyDisplayChanged()
        {
            if (DisplayChanged != null)
                DisplayChanged();
        }
    }
}
