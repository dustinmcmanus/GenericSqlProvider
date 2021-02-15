using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GenericSqlProvider.SqlServer
{
    public class GenericSqlServerParameterCollection : IDataParameterCollection
    {
        // TODO: Options for backwards compatiblity:
        //public bool AddParameterWhenCreated = true;

        IDataParameterCollection parameterCollection;

        public GenericSqlServerParameterCollection(ref IDataParameterCollection parameterCollection)
        {
            this.parameterCollection = parameterCollection;
        }

        public object this[string parameterName] { get => parameterCollection[parameterName]; set => parameterCollection[parameterName] = value; }
        public object this[int index] { get => parameterCollection[index]; set => parameterCollection[index] = value; }

        public bool IsFixedSize => parameterCollection.IsFixedSize;

        public bool IsReadOnly => parameterCollection.IsReadOnly;

        public int Count => parameterCollection.Count;

        public bool IsSynchronized => parameterCollection.IsSynchronized;

        public object SyncRoot => parameterCollection.SyncRoot;

        public int Add(object value)
        {
            // do nothing. parameters are added in the IdbCommand.CreateParameter method
            return -1;

            // TODO: Make option for backwards compatibility?
            //if (AddParameterWhenCreated) { 
            //    return -1;
            //}
            //else
            //{
            //    return parameterCollection.Add(value);
            //}
        }

        public void Clear()
        {
            parameterCollection.Clear();
        }

        public bool Contains(string parameterName)
        {
            return parameterCollection.Contains(parameterName);
        }

        public bool Contains(object value)
        {
            return parameterCollection.Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            parameterCollection.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return parameterCollection.GetEnumerator();
        }

        public int IndexOf(string parameterName)
        {
            return parameterCollection.IndexOf(parameterName);
        }

        public int IndexOf(object value)
        {
            return parameterCollection.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            parameterCollection.Insert(index, value);
        }

        public void Remove(object value)
        {
            parameterCollection.Remove(value);
        }

        public void RemoveAt(string parameterName)
        {
            parameterCollection.RemoveAt(parameterName);
        }

        public void RemoveAt(int index)
        {
            parameterCollection.RemoveAt(index);
        }
    }
}
