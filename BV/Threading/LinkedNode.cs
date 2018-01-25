namespace VB.Common.Threading
{
    public class LinkedNode<T>
    {
        private LinkedNode<T> next;

        private T value;
        
        public LinkedNode() { }

        public LinkedNode(T value) : this(value, null)
        {
            // empty
        }

        public LinkedNode(T value, LinkedNode<T> next)
        {
            Value = value;
            Next = next;
        }

        public LinkedNode<T> Next
        {
            get { return next; }
            set { next = value; }
        }

        public T Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
