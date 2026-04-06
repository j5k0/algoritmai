namespace _2 
{
    class Program
    {
        class DoublyLinkedList<T> where T : IEquatable<T>
        {
            protected class Node
            {
                public Node? next;
                public Node? previous;
                public T data;
            
                public Node(Node? next, Node? previous, T data)
                {
                    this.next = next;
                    this.previous = previous;
                    this.data = data;
                }
            }

            private Node? tail = null;
            private Node? head = null;
            private Node? current = null;

            public void Add(T data)
            {
                if(head == null)
                {
                    current = tail = head = new Node(null, null, data);
                }
                else
                {
                    Node newNode = new Node(null, tail, data);
                    tail!.next = newNode;
                    tail = tail.next;
                }
            }

            public bool Remove(T data)
            {
                Node? toRemove = null;
                for(toRemove = head; toRemove != null && !toRemove.data.Equals(data); toRemove = toRemove.next);
                if(toRemove != null)
                {
                    if(toRemove.previous == null || toRemove.next == null)
                    {
                        if(toRemove == tail) tail = tail.previous;
                        if(toRemove == head) head = head.next;
                        if (head != null) head.previous = null;
                        if (tail != null) tail.next = null;
                    }
                    else 
                    {
                        toRemove.previous.next = toRemove.next;
                        toRemove.next.previous = toRemove.previous;
                    }
                    return true;
                }
                return false;
            }

            public T? Get()
            {
                if(current != null)
                    return current.data;
                return default;
            }

            public T? Next()
            {
                if(current != null)
                {
                    T tmp = current.data;
                    current = current.next;
                    return tmp;
                }
                return default;
            }

            public  void Last()
            {
                if(current != null)
                    current = current.previous;
            }

            public bool Exists()
            {
                return current != null;
            }

            public void Reset()
            {
                current = head;
            }
        }

        static void Main(string[] args){
            DoublyLinkedList<int> n = new DoublyLinkedList<int>();
            n.Add(5);
            n.Add(10);
            n.Add(15);
            n.Add(16);
            n.Add(17);
            n.Add(2);

            while(n.Exists()){
                Console.Write(" " + n.Next());
            }
            Console.WriteLine();

            n.Remove(2);
            n.Reset();
            while(n.Exists()){
                Console.Write(" " + n.Next());
            }
            Console.WriteLine();


            n.Remove(5);
            n.Reset();
            while(n.Exists()){
                Console.Write(" " + n.Next());
            }
            Console.WriteLine();

            n.Remove(15);
            n.Reset();
            while(n.Exists()){
                Console.Write(" " + n.Next());
            }
            Console.WriteLine();
        }
    }
}
