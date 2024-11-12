using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.IO.RESTApi
{
    public class RESTQueryResponse<T> : RESTResponse, ICollection<T> where T : class, new()
    {
        [JsonProperty("data")] public T[] Data { get; set; }
        [JsonIgnore] public int Length => Data.Length;
        [JsonIgnore] public bool IsEmpty => Data == null || Data.Length == 0;
        [JsonIgnore] public int Count => Data.Length;
        [JsonIgnore] public bool IsReadOnly => true;
        
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)Data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(T item)
        {
            return ((ICollection<T>)Data).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)Data).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new System.NotImplementedException();
        }

        public T this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }
    }
}
