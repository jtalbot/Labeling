using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    // Holds a list of symbols and their associated namespace path.
    // The namespace path allows the user to organize the symbols as they see fit, but does not affect computation with the variable currently.
    // Paths look like "Demographic Variables/State Level" with a variable "Population".
    public class SymbolTable : Dictionary<Symbol, string>
    {
    }
}
