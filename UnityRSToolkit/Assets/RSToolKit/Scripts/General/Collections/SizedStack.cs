namespace RSToolkit.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class SizedStack <T>
    {
        #region Fields
 
        private int _capacity;
        private LinkedList<T> _list;
         
        #endregion
 
        #region Constructors
 
        public SizedStack(int capacity)
        {
            _capacity = capacity;
            _list = new LinkedList<T>();
             
        }
 
        #endregion

        private T pop(){
            var value = _list.First.Value;
            _list.RemoveFirst();
            return value;
        }
 
        #region Public Stack Implementation
 
        public bool Push(T value, bool force = false)
        {
            if (IsFull())
            {
                if(force){
                    _list.RemoveLast();
                }else{
                    return false;
                }
            }
            _list.AddFirst(value);
            return true;
        }
 
        public T Pop()
        {
            if (_list.Count > 0)
            {
                return pop();
            }
            else
            {
                throw new InvalidOperationException("The Stack is empty");
            }
 
             
        }

        public T PopOrDefault(){

            if (Any())
            {
                return pop();
            }else{
                return default(T);
            }
        }
 
        public T Peek()
        {
            if (Any())
            {
                return _list.First.Value;
            }
            else
            {
                throw new InvalidOperationException("The Stack is empty");
            }
             
        }
        public T PeekOrDefault()
        {
            if (Any())
            {
                return _list.First.Value;
            }
            else
            {
                return default(T);
            }
        }

        public bool Any(){
            return _list.Count > 0;
        }
 
        public void Clear()
        {
            _list.Clear();
             
        }
 
        public int Count
        {
            get { return _list.Count; }
        }
 
        /// <summary>
        /// Checks if the top object on the stack matches the value passed in
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsTop(T value)
        {
            if (this.Count > 0)
            {
                return Peek().Equals(value);
            }
            return false;
        }
 
        public bool Contains(T value)
        {
            if (this.Count > 0)
            {
                return _list.Contains(value);
            }
            return false;
        }

        public bool IsFull(){
            return Count == _capacity;
        }
 
    public IEnumerator GetEnumerator()
    {
            return _list.GetEnumerator();
    }
 
        #endregion
 
    }

}