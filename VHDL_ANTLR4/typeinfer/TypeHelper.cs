﻿using VHDL.type;
using VHDL.declaration;
using VHDL.Object;
using VHDL.expression;

namespace VHDL.parser.typeinfer
{
    class TypeHelper : ISubtypeIndicationVisitor
    {
        private ISubtypeIndication baseType;

        // FIXME
        public static ISubtypeIndication GetType(object elem)
        {
            ISubtypeIndication type;
            if (elem is VhdlObject)
                type = ((VhdlObject)elem).Type;
            else if (elem is Name)
                type = ((Name)elem).Type;
            else //if (dst is ISubtypeIndication)
                type = ((ISubtypeIndication)elem);
            return type;
        }

        public static ISubtypeIndication GetBaseType(ISubtypeIndication type)
        {
            var getter = new TypeHelper();
            type.accept(getter);
            return getter.baseType;
        }

        public static string GetTypeName(ISubtypeIndication indication)
        {
            var unresolved_type = indication as UnresolvedType;
            if (unresolved_type != null)
                return unresolved_type.Identifier;
            var type = indication as Type;
            if (type != null)
                return type.Identifier;
            ISubtypeIndication base_type = indication.BaseType;
            if (base_type != null)
                return GetTypeName(base_type);
            return "";
        }

        public virtual void visit(Subtype item)
        {
            item.SubtypeIndication.accept(this);
        }

        public virtual void visit(ResolvedSubtypeIndication item)
        {
            item.BaseType.accept(this);
        }

        public virtual void visit(Type item)
        {
            baseType = item;
        }

        public void visit(ISubtypeIndication item)
        {
            if (item == null)
                return;
            visit(item.BaseType);        
        }

        public void visit(IndexSubtypeIndication item)
        {
            if (item == null)
                return;
            baseType = item.BaseType;
        }

        public void visit(RangeSubtypeIndication item)
        {
            if (item == null)
                return;
            baseType = item.BaseType;
        }

        public void visit(UnresolvedType item)
        {
            return;            
        }
    }
}
