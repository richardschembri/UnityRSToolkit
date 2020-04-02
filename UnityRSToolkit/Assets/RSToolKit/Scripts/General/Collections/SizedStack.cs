namespace RSToolkit.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class SizedStack <T>
    {
        #region Fields
 
        private int m_capacity;
        private LinkedList<T> m_list;
         
        #endregion
 
        #region Constructors
 
        public SizedStack(int capacity)
        {
            m_capacity = capacity;
            m_list = new LinkedList<T>();
             
        }
 
        #endregion

        private T pop(){
            var value = m_list.First.Value;
            m_list.RemoveFirst();
            return value;
        }
 
        #region Public Stack Implementation
 
        public void Push(T value)
        {
            if (m_list.Count == m_capacity)
            {
                m_list.RemoveLast();
            }
            m_list.AddFirst(value);
        }
 
        public T Pop()
        {
            if (m_list.Count > 0)
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
                return m_list.First.Value;
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
                return m_list.First.Value;
            }
            else
            {
                return default(T);
            }
        }

        public bool Any(){
            return m_list.Count > 0;
        }
 
        public void Clear()
        {
            m_list.Clear();
             
        }
 
        public int Count
        {
            get { return m_list.Count; }
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
                return m_list.Contains(value);
            }
            return false;
        }
 
    public IEnumerator GetEnumerator()
    {
            return m_list.GetEnumerator();
    }
 
        #endregion
 
    }

}