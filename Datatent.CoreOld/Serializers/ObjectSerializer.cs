using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Datatent.Core.Interfaces;

namespace Datatent.Core.Serializers
{
    public class BinaryObjectSerializer: IObjectSerializer
    {
        public bool SupportSingleField => true;
        private Dictionary<Type, SortedSet<Value>> valueList;

        public BinaryObjectSerializer()
        {

        }

        public byte[] Serialize(object model)
        {
            if (valueList == null)
            {
                valueList = new Dictionary<Type, SortedSet<Value>>();
            }
            if (!valueList.ContainsKey(model.GetType()))
            {
                var properties = model.GetType().GetProperties().OrderBy(info => info.Name);

                foreach (var property in properties)
                {
                    var typeCode = Type.GetTypeCode(property.PropertyType);

                    var val = new Value();
                    val.TypeCode = typeCode;
                    val.Type = property.PropertyType;
                    val.Name = property.Name;
                    val.IsFixedLength = typeCode != TypeCode.Object || typeCode != TypeCode.String;
                }
            }

            throw new NotImplementedException();
        }

        public T Deserialize<T>(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public V DeserializeSingleField<V>(byte[] bytes, string name)
        {
            throw new NotImplementedException();
        }

        private class ValueComparer : IComparer<Value>
        {
            public int Compare(Value x, Value y)
            {
                if (ReferenceEquals(x, null))
                    throw new ArgumentNullException(nameof(x));
                if (ReferenceEquals(y, null))
                    throw new ArgumentNullException(nameof(y));

                return x.Order.CompareTo(y.Order);
            }
        }

        private class Value
        {
            public int Order { get; set; }
            
            public int Offset { get; set; }

            public string Name { get; set; }

            public TypeCode TypeCode { get; set; }

            public Type Type { get; set; }

            public bool IsFixedLength { get; set; }
        }
    }
}
