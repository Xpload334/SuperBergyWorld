using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublyLinkedList<T>
{
    public class Node
    {
        public T Value { get; set; }
        public Node Previous { get; set; }
        public Node Next { get; set; }

        public Node(T t)
        {
            Value = t;
            Previous = null;
            Next = null;
        }
    }


    private Node _head;
    public Node First
    {
        get => _head;
        set => _head = value;
    }

    public Node Last
    {
        get
        {
            Node node = GetLastNode();
            return node;
        }
    }

    public Node Next => _head.Next;
    public Node Previous => _head.Previous;

    [SerializeField] private int _count;
    public int Count
    {
        get
        {
            if (_count < 0)
            {
                _count = 0;
            }

            return _count;
        }
        private set => _count = value;
    }

    private Node GetLastNode()
    {
        Node node = _head;
        while (node.Next != null)
        {
            node = node.Next;
        }
        return node;
    }

    public void AddFirst(T t)
    {
        Node newNode = new Node(t);
        Count++;
        if (_head != null)
        {
            newNode.Next = _head;
            _head.Previous = newNode;
        }
        _head = newNode;
    }

    public void AddLast(T t)
    {
        Node newNode = new Node(t);
        Count++;
        if (_head == null)
        {
            _head = newNode;
            return;
        }

        Node lastNode = GetLastNode();
        lastNode.Next = newNode;
        newNode.Previous = lastNode;
    }

    public bool Contains(T t)
    {
        Node node = First;
        if (node.Value.Equals(t))
        {
            return true;
        }

        while (node.Next != null)
        {
            node = node.Next;
            if (node.Value.Equals(t))
            {
                return true;
            }
        }
        return false;
    }

    public Node Find(T t)
    {
        Node node = First;
        if (node.Value.Equals(t))
        {
            return node;
        }

        while (node.Next != null)
        {
            node = node.Next;
            if (node.Value.Equals(t))
            {
                return node;
            }
        }
        return null;
    }

    public void Remove(T t)
    {
        //If first is null, return
        if (First == null)
        {
            return;
        }
        //If node not in list, return
        Node node = Find(t);
        if (node == null)
        {
            return;
        }

        Remove(node);
    }

    public void Remove(Node node)
    {
        //If node to be deleted is head
        if (First.Equals(node))
        {
            First = node.Next;
        }
        
        //Change next only if node to be removed is NOT last node
        if (node.Next != null)
        {
            node.Next.Previous = node.Previous;
        }
        
        //Change previous only if node to be deleted is NOT first node
        if (node.Previous != null)
        {
            node.Previous.Next = node.Next;
        }
        
        //Free memory occupied by node
        return;
    }

    public void RemoveFirst()
    {
        Remove(First);
    }

    public void RemoveLast()
    {
        Remove(Last);
    }
}
