using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DataContainer;
using VHDL.type;
using DataContainer.Value;
using VHDL.literal;
using DataContainer.Objects;

namespace DataContainer.Generator
{
    public enum GeneratorType
    {
        Constant,
        Clock,
        Counter,
        DiscretteRandom,
        ContinousRandom
    }

    public abstract class BaseGenerator
    {
        /// <summary>
        /// Временной шаг
        /// </summary>
        protected TimeInterval timeStep;
        public TimeInterval TimeStep
        {
            get { return timeStep; }
            set { timeStep = value; }
        }
 
        /// <summary>
        /// Количество сгенерированных значений
        /// </summary>
        protected int valuesCount;
        public int ValuesCount
        {
            get { return valuesCount; }
        }

        /// <summary>
        /// Размер в битах сгенерированного значения
        /// </summary>
        public virtual int SizeInBits
        {
            get { return 1; }
        }

        public BaseGenerator()
        {
            timeStep = new TimeInterval(1, TimeUnit.fs);
        }

        private static SortedList<UInt64, TimeStampInfo> FormIntegerGeneratedData(BaseGenerator generator, UInt64 StartTime, UInt64 EndTime, ModellingType modellingType)
        {
            SortedList<UInt64, TimeStampInfo> res = new SortedList<UInt64, TimeStampInfo>();

            if (generator is IGeneratorDataFill<Int64>)
            {
                SortedList<UInt64, Int64> valuesForInsert = (generator as IGeneratorDataFill<Int64>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(new IntegerValue(VHDL.builtin.Standard.INTEGER, Convert.ToInt32(el.Value)));
                    res.Add(el.Key, inf);
                }
                return res;
            }
            if (generator is IGeneratorDataFill<bool[]>)
            {
                SortedList<UInt64, bool[]> valuesForInsert = (generator as IGeneratorDataFill<bool[]>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(new IntegerValue(VHDL.builtin.Standard.INTEGER, DataContainer.ValueDump.DataConvertorUtils.ToInt(el.Value)));
                    res.Add(el.Key, inf);
                }
                return res;
            }
            if (generator is IGeneratorDataFill<Double>)
            {
                SortedList<UInt64, Double> valuesForInsert = (generator as IGeneratorDataFill<Double>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(new IntegerValue(VHDL.builtin.Standard.INTEGER, Convert.ToInt32(el.Value)));
                    res.Add(el.Key, inf);
                }
                return res;
            }

            return res;
        }

        private static SortedList<UInt64, TimeStampInfo> FormRealGeneratedData(BaseGenerator generator, UInt64 StartTime, UInt64 EndTime, ModellingType modellingType)
        {
            SortedList<UInt64, TimeStampInfo> res = new SortedList<UInt64, TimeStampInfo>();

            if (generator is IGeneratorDataFill<Int64>)
            {
                SortedList<UInt64, Int64> valuesForInsert = (generator as IGeneratorDataFill<Int64>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(new RealValue(VHDL.builtin.Standard.REAL, Convert.ToDouble(el.Value)));
                    res.Add(el.Key, inf);
                }
                return res;
            }
            if (generator is IGeneratorDataFill<bool[]>)
            {
                SortedList<UInt64, bool[]> valuesForInsert = (generator as IGeneratorDataFill<bool[]>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(new RealValue(VHDL.builtin.Standard.REAL, DataContainer.ValueDump.DataConvertorUtils.ToInt(el.Value)));
                    res.Add(el.Key, inf);
                }
                return res;
            }
            if (generator is IGeneratorDataFill<Double>)
            {
                SortedList<UInt64, Double> valuesForInsert = (generator as IGeneratorDataFill<Double>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(new RealValue(VHDL.builtin.Standard.REAL, Convert.ToDouble(el.Value)));
                    res.Add(el.Key, inf);
                }
                return res;
            }

            return res;
        }

        private static SortedList<UInt64, TimeStampInfo> FormEnumerationGeneratedData(BaseGenerator generator, UInt64 StartTime, UInt64 EndTime, ModellingType modellingType)
        {
            SortedList<UInt64, TimeStampInfo> res = new SortedList<UInt64, TimeStampInfo>();
            if (generator is IGeneratorDataFill<object>)
            {
                SortedList<UInt64, object> valuesForInsert = (generator as IGeneratorDataFill<object>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(new EnumerationValue(modellingType.Type as VHDL.type.EnumerationType, el.Value as EnumerationLiteral));
                    res.Add(el.Key, inf);
                }
                return res;
            }
            return res;
        }

        private static SortedList<UInt64, TimeStampInfo> FormSTD_ULOGICGeneratedData(BaseGenerator generator, UInt64 StartTime, UInt64 EndTime, ModellingType modellingType)
        {
            SortedList<UInt64, TimeStampInfo> res = new SortedList<UInt64, TimeStampInfo>();
            if (generator is IGeneratorDataFill<bool>)
            {
                SortedList<UInt64, bool> valuesForInsert = (generator as IGeneratorDataFill<bool>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(new STD_ULOGIC_VALUE(el.Value ? VHDL.builtin.StdLogic1164.STD_ULOGIC_1 : VHDL.builtin.StdLogic1164.STD_ULOGIC_0));
                    res.Add(el.Key, inf);
                }
                return res;
            }

            if (generator is IGeneratorDataFill<object>)
            {
                SortedList<UInt64, object> valuesForInsert = (generator as IGeneratorDataFill<object>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    if (el.Value is string)
                    {
                        TimeStampInfo inf = new TimeStampInfo(new STD_ULOGIC_VALUE((el.Value as string == "1") ? VHDL.builtin.StdLogic1164.STD_LOGIC_1 : VHDL.builtin.StdLogic1164.STD_LOGIC_0));
                        res.Add(el.Key, inf);
                    }
                    if (el.Value is EnumerationLiteral)
                    {
                        TimeStampInfo inf = new TimeStampInfo(new STD_ULOGIC_VALUE(el.Value as EnumerationLiteral));
                        res.Add(el.Key, inf);
                    }
                }
                return res;
            }
            return res;
        }

        private static SortedList<UInt64, TimeStampInfo> FormSTD_LOGICGeneratedData(BaseGenerator generator, UInt64 StartTime, UInt64 EndTime, ModellingType modellingType)
        {
            SortedList<UInt64, TimeStampInfo> res = new SortedList<UInt64, TimeStampInfo>();
            if (generator is IGeneratorDataFill<bool>)
            {
                SortedList<UInt64, bool> valuesForInsert = (generator as IGeneratorDataFill<bool>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(new STD_LOGIC_VALUE(el.Value ? VHDL.builtin.StdLogic1164.STD_LOGIC_1 : VHDL.builtin.StdLogic1164.STD_LOGIC_0));
                    res.Add(el.Key, inf);
                }
                return res;
            }

            if (generator is IGeneratorDataFill<object>)
            {
                SortedList<UInt64, object> valuesForInsert = (generator as IGeneratorDataFill<object>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    if (el.Value is string)
                    {
                        TimeStampInfo inf = new TimeStampInfo(new STD_LOGIC_VALUE((el.Value as string == "1") ? VHDL.builtin.StdLogic1164.STD_LOGIC_1 : VHDL.builtin.StdLogic1164.STD_LOGIC_0));
                        res.Add(el.Key, inf);
                    }
                    if (el.Value is EnumerationLiteral)
                    {
                        TimeStampInfo inf = new TimeStampInfo(new STD_LOGIC_VALUE(el.Value as EnumerationLiteral));
                        res.Add(el.Key, inf);
                    }
                }
                return res;
            }
            return res;
        }

        private static SortedList<UInt64, TimeStampInfo> FormBitGeneratedData(BaseGenerator generator, UInt64 StartTime, UInt64 EndTime, ModellingType modellingType)
        {
            SortedList<UInt64, TimeStampInfo> res = new SortedList<UInt64, TimeStampInfo>();
            if (generator is IGeneratorDataFill<bool>)
            {
                SortedList<UInt64, bool> valuesForInsert = (generator as IGeneratorDataFill<bool>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(new BIT_VALUE(el.Value ? VHDL.builtin.Standard.BIT_1 : VHDL.builtin.Standard.BIT_0));
                    res.Add(el.Key, inf);
                }
                return res;
            }

            if (generator is IGeneratorDataFill<object>)
            {
                SortedList<UInt64, object> valuesForInsert = (generator as IGeneratorDataFill<object>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    if (el.Value is string)
                    {
                        TimeStampInfo inf = new TimeStampInfo(new BIT_VALUE((el.Value as string == "1") ? VHDL.builtin.Standard.BIT_1 : VHDL.builtin.Standard.BIT_0));
                        res.Add(el.Key, inf);
                    }
                    if (el.Value is EnumerationLiteral)
                    {
                        TimeStampInfo inf = new TimeStampInfo(new BIT_VALUE(el.Value as EnumerationLiteral));
                        res.Add(el.Key, inf);
                    }
                }
                return res;
            }
            return res;
        }

        private static SortedList<UInt64, TimeStampInfo> FormSTD_LOGIC_VECTORGeneratedData(BaseGenerator generator, UInt64 StartTime, UInt64 EndTime, ModellingType modellingType)
        {
            SortedList<UInt64, TimeStampInfo> res = new SortedList<UInt64, TimeStampInfo>();
            int size = modellingType.SizeOf;

            if (generator is IGeneratorDataFill<Int64>)
            {
                SortedList<UInt64, Int64> valuesForInsert = (generator as IGeneratorDataFill<Int64>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(STD_LOGIC_VECTOR_VALUE.CreateSTD_LOGIC_VECTOR_VALUE(el.Value, size));
                    res.Add(el.Key, inf);
                }
                return res;
            }
            if (generator is IGeneratorDataFill<bool[]>)
            {
                SortedList<UInt64, bool[]> valuesForInsert = (generator as IGeneratorDataFill<bool[]>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(STD_LOGIC_VECTOR_VALUE.CreateSTD_LOGIC_VECTOR_VALUE(el.Value));
                    res.Add(el.Key, inf);
                }
                return res;
            }
            if (generator is IGeneratorDataFill<Double>)
            {
                SortedList<UInt64, Double> valuesForInsert = (generator as IGeneratorDataFill<Double>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(STD_LOGIC_VECTOR_VALUE.CreateSTD_LOGIC_VECTOR_VALUE((Int64)el.Value, size));
                    res.Add(el.Key, inf);
                }
                return res;
            }

            return res;
        }

        private static SortedList<UInt64, TimeStampInfo> FormSTD_ULOGIC_VECTORGeneratedData(BaseGenerator generator, UInt64 StartTime, UInt64 EndTime, ModellingType modellingType)
        {
            SortedList<UInt64, TimeStampInfo> res = new SortedList<UInt64, TimeStampInfo>();
            int size = modellingType.SizeOf;

            if (generator is IGeneratorDataFill<Int64>)
            {
                SortedList<UInt64, Int64> valuesForInsert = (generator as IGeneratorDataFill<Int64>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(STD_ULOGIC_VECTOR_VALUE.CreateSTD_ULOGIC_VECTOR_VALUE(el.Value, size));
                    res.Add(el.Key, inf);
                }
                return res;
            }
            if (generator is IGeneratorDataFill<bool[]>)
            {
                SortedList<UInt64, bool[]> valuesForInsert = (generator as IGeneratorDataFill<bool[]>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(STD_ULOGIC_VECTOR_VALUE.CreateSTD_ULOGIC_VECTOR_VALUE(el.Value));
                    res.Add(el.Key, inf);
                }
                return res;
            }
            if (generator is IGeneratorDataFill<Double>)
            {
                SortedList<UInt64, Double> valuesForInsert = (generator as IGeneratorDataFill<Double>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(STD_ULOGIC_VECTOR_VALUE.CreateSTD_ULOGIC_VECTOR_VALUE((Int64)el.Value, size));
                    res.Add(el.Key, inf);
                }
                return res;
            }

            return res;
        }

        private static SortedList<UInt64, TimeStampInfo> FormBIT_VECTORGeneratedData(BaseGenerator generator, UInt64 StartTime, UInt64 EndTime, ModellingType modellingType)
        {
            SortedList<UInt64, TimeStampInfo> res = new SortedList<UInt64, TimeStampInfo>();
            int size = modellingType.SizeOf;

            if (generator is IGeneratorDataFill<Int64>)
            {
                SortedList<UInt64, Int64> valuesForInsert = (generator as IGeneratorDataFill<Int64>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(BIT_VECTOR_VALUE.CreateBIT_VECTOR_VALUE(el.Value, size));
                    res.Add(el.Key, inf);
                }
                return res;
            }
            if (generator is IGeneratorDataFill<bool[]>)
            {
                SortedList<UInt64, bool[]> valuesForInsert = (generator as IGeneratorDataFill<bool[]>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(BIT_VECTOR_VALUE.CreateBIT_VECTOR_VALUE(el.Value));
                    res.Add(el.Key, inf);
                }
                return res;
            }
            if (generator is IGeneratorDataFill<Double>)
            {
                SortedList<UInt64, Double> valuesForInsert = (generator as IGeneratorDataFill<Double>).InsertValues(StartTime, EndTime);
                foreach (var el in valuesForInsert)
                {
                    TimeStampInfo inf = new TimeStampInfo(BIT_VECTOR_VALUE.CreateBIT_VECTOR_VALUE((Int64)el.Value, size));
                    res.Add(el.Key, inf);
                }
                return res;
            }

            return res;
        }


        public static SortedList<UInt64, TimeStampInfo> FormGeneratedData(BaseGenerator generator, UInt64 StartTime, UInt64 EndTime, ModellingType modellingType)
        {
            SortedList<UInt64, TimeStampInfo> res = new SortedList<UInt64, TimeStampInfo>();
            if (modellingType.Type is IntegerType)
            {
                return FormIntegerGeneratedData(generator, StartTime, EndTime, modellingType);
            }
            if (modellingType.Type is RealType)
            {
                return FormRealGeneratedData(generator, StartTime, EndTime, modellingType);
            }
            if ((modellingType.Type == VHDL.builtin.StdLogic1164.STD_ULOGIC) && (modellingType.Constraints.Count != 0))
            {
                return FormSTD_LOGICGeneratedData(generator, StartTime, EndTime, modellingType);
            }
            if (modellingType.Type == VHDL.builtin.StdLogic1164.STD_ULOGIC)
            {
                return FormSTD_ULOGICGeneratedData(generator, StartTime, EndTime, modellingType);
            }
            if (modellingType.Type == VHDL.builtin.Standard.BIT)
            {
                return FormBitGeneratedData(generator, StartTime, EndTime, modellingType);
            }
            if (modellingType.Type is EnumerationType)
            {
                return FormEnumerationGeneratedData(generator, StartTime, EndTime, modellingType);
            }
            if (modellingType.Type == VHDL.builtin.StdLogic1164.STD_LOGIC_VECTOR)
            {
                return FormSTD_LOGIC_VECTORGeneratedData(generator, StartTime, EndTime, modellingType);
            }
            if (modellingType.Type == VHDL.builtin.StdLogic1164.STD_ULOGIC_VECTOR)
            {
                return FormSTD_ULOGIC_VECTORGeneratedData(generator, StartTime, EndTime, modellingType);
            }
            if (modellingType.Type == VHDL.builtin.Standard.BIT_VECTOR)
            {
                return FormBIT_VECTORGeneratedData(generator, StartTime, EndTime, modellingType);
            }
            return res;
        }

        public void Fill(Signal signal, UInt64 StartTime, UInt64 EndTime)
        {
            
            int valuesCount = (int)((EndTime - StartTime) / timeStep.GetTimeUnitInFS());
            if (valuesCount > signal.Dump.FreeCells)
            {
                throw new Exception("Can't generate too big number of data.\nTry to select smaller time diapasone or bigger time step");
            }
            

            SortedList<UInt64, TimeStampInfo> newData = FormGeneratedData(this, StartTime, EndTime, signal.Type);
            signal.Dump.InsertValues(newData, StartTime, EndTime);
        }

        public virtual string GetStringStartValue() { throw new Exception(); }
        public virtual StringBuilder StringVhdlRealization(KeyValuePair<String, TimeInterval> param) { throw new Exception(); }
        public virtual void StreamVhdlRealization(KeyValuePair<String, TimeInterval> param, StreamWriter sw) { throw new Exception(); }
        //param.Val - END_TIME
        //step - field
    }
}