//
//  Copyright (C) 2010-2014  Denis Gavrish
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Collections.Generic;
using System;

namespace VHDL.Object
{
    /// <summary>
    /// Variable group.
    /// </summary>
    [Serializable]
    public class VariableGroup : VhdlObjectGroup<Variable>
    {
        private readonly List<Variable> variables;

        /// <summary>
        /// Creates a group of variables.
        /// </summary>
        /// <param name="variables">a list of variables</param>
        public VariableGroup(List<Variable> variables)
        {
            this.variables = new List<Variable>(variables);
        }

        /// <summary>
        /// Creates a group of variables.
        /// </summary>
        /// <param name="variables">a variable number of variables</param>
        public VariableGroup(params Variable[] variables)
            : this(new List<Variable>(variables))
        {
        }

        /// <summary>
        /// Returns the variables in this group.
        /// </summary>
        public override IList<Variable> Elements
        {
            get { return variables; }
        }

        public override IList<VhdlObject> VhdlObjects
        {
            get
            {
                List<VhdlObject> res = new List<VhdlObject>();
                foreach (var v in variables)
                    res.Add(v);
                return res;
            }
        }
    }

}