﻿//
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
using System.Linq;
using VHDL.expression;
using VHDL.expression.name;

namespace VHDL.parser.antlr
{
    /// <summary>
    /// Temporary name used during meta class generation.
    /// </summary>
    public class TemporaryName : VhdlElement
    {
        private List<Part> parts = new List<Part>();
        private IDeclarativeRegion currentScore;
        private VHDL_ANTLR4.vhdlVisitor visitor;
        private Antlr4.Runtime.ParserRuleContext context;
        public static Stack<object> ExprContext = new Stack<object>();
        public static VHDL.expression.Name CurrentName;

        public TemporaryName(List<Part> parts, VHDL_ANTLR4.vhdlVisitor visitor, Antlr4.Runtime.ParserRuleContext context)
            : this(parts, visitor.currentScope, visitor, context)
        { }

        public TemporaryName(List<Part> parts, IDeclarativeRegion currentScore, VHDL_ANTLR4.vhdlVisitor visitor, Antlr4.Runtime.ParserRuleContext context)
        {
            this.parts = parts;
            this.currentScore = currentScore;
            this.visitor = visitor;
            this.context = context;
        }

        private T resolve<T>(IDeclarativeRegion scope) where T : class
        {
            return ObjectSearcher.Search<T>(scope, parts);
        }

        private List<T> resolveAll<T>(IDeclarativeRegion scope) where T : class
        {
            return ObjectSearcher.SearchAll<T>(scope, parts).Distinct().ToList();
        }

        //used only for error reporting
        private string Identifier
        {
            get
            {
                System.Text.StringBuilder selectedName = new System.Text.StringBuilder();

                bool first = true;
                foreach (Part part in parts)
                {
                    if (part.Type == Part.TypeEnum.SELECTED)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            selectedName.Append('.');
                        }
                        selectedName.Append((part as Part.SelectedPart).Suffix);
                    }
                }

                return (selectedName.Length == 0 ? "unknown" : selectedName.ToString());
            }
        }

        public virtual VHDL.libraryunit.Entity GetEntity()
        {
            VHDL.libraryunit.Entity entity = resolve<VHDL.libraryunit.Entity>(currentScore);
            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new VHDL.ParseError.vhdlUnknownEntityException(context, visitor.FileName, Identifier);
            }
        }

        public virtual VHDL.libraryunit.Configuration GetConfiguration()
        {
            VHDL.libraryunit.Configuration configuration = resolve<VHDL.libraryunit.Configuration>(currentScore);
            if (configuration != null)
            {
                return configuration;
            }
            else
            {
                throw new VHDL.ParseError.vhdlUnknownIdentifierException(context, visitor.FileName, Identifier);
            }
        }

        //TODO: don't use subtype indication
        public virtual VHDL.type.ISubtypeIndication GetTypeMark()
        {
            VHDL.type.ISubtypeIndication type = resolve<VHDL.type.ISubtypeIndication>(currentScore);
            if (type != null)
            {
                return type;
            }
            else
            {
                throw new VHDL.ParseError.vhdlUnknownTypeException(context, visitor.FileName, Identifier);
            }
        }

        public virtual VHDL.declaration.Component GetComponent()
        {
            VHDL.declaration.Component component = resolve<VHDL.declaration.Component>(currentScore);
            if (component != null)
            {
                return component;
            }
            else
            {
                throw new VHDL.ParseError.vhdlUnknownComponentException(context, visitor.FileName, Identifier);
            }
        }

        public virtual Name GetName()
        {
            if (parts.Count == 1)
            {
                var obj = ObjectSearcher.SearchIdentifier<INamedEntity>(currentScore, (parts[0] as Part.SelectedPart).Suffix, x => true);
                return new SimpleName(obj);
            }
            else if (parts.Count > 1)
            {
                var objs = ObjectSearcher.SearchComponents<INamedEntity>(currentScore, parts);
                Name suffix = new SimpleName(objs[objs.Count - 1]);
                for (int i = objs.Count - 2; i >= 0; --i)
                {
                    Name prefix = new SimpleName(objs[i]);
                    suffix = new SelectedName(prefix, suffix);
                }
                return suffix;
            }

            throw new VHDL.ParseError.vhdlUnknownIdentifierException(context, visitor.FileName, Identifier);
        }

        public virtual VHDL.Object.Signal GetSignal()
        {
            VHDL.Object.Signal signal = resolve<VHDL.Object.Signal>(currentScore);
            if (signal != null)
            {
                return signal;
            }
            else
            {
                throw new VHDL.ParseError.vhdlUnknownSignalException(context, visitor.FileName, Identifier);
            }
        }

        public virtual VHDL.expression.FunctionCall ResolveFunctionCall(List<AssociationElement> arguments, VHDL.type.ISubtypeIndication currentAssignTarget, List<VHDL.declaration.IFunction> candidates)
        {
            VHDL.declaration.IFunction declaration = VHDL.parser.typeinfer.TypeInference.ResolveOverloadFunction(currentScore, candidates, arguments, currentAssignTarget);
            VHDL.expression.FunctionCall call = new VHDL.expression.FunctionCall(declaration, arguments);
            return call;
        }

        public virtual VHDL.statement.ProcedureCall ResolveProcedureCall(List<AssociationElement> arguments, List<VHDL.declaration.IProcedure> candidates)
        {
            var declaration = VHDL.parser.typeinfer.TypeInference.ResolveOverloadProcedure(currentScore, candidates, arguments);
            VHDL.statement.ProcedureCall call = new VHDL.statement.ProcedureCall(declaration, arguments);
            return call;
        }

        public virtual VHDL.declaration.ProcedureDeclaration GetProcedure()
        {
            VHDL.declaration.ProcedureDeclaration procedure = resolve<VHDL.declaration.ProcedureDeclaration>(currentScore);
            return procedure;
        }


        public virtual VHDL.statement.ProcedureCall GetProcedureCall(List<AssociationElement> arguments)
        {
            var procedure_candidates = resolveAll<VHDL.declaration.IProcedure>(currentScore);
            return ResolveProcedureCall(arguments, procedure_candidates);
        }

        public virtual VHDL.declaration.IFunction GetFunction()
        {
            VHDL.declaration.IFunction function = resolve<VHDL.declaration.IFunction>(currentScore);
            return function;
        }

        public virtual VHDL.expression.FunctionCall GetFunctionCall(List<AssociationElement> arguments, VHDL.type.ISubtypeIndication currentAssignTarget)
        {
            var function_candidates = resolveAll<VHDL.declaration.IFunction>(currentScore);
            return ResolveFunctionCall(arguments, currentAssignTarget, function_candidates);
        }
    }
}