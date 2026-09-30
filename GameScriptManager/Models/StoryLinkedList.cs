using System.Collections;
using System.Diagnostics;

namespace GameScriptManager.Models;

/// <summary>
/// Execution metrics returned after running a sorting algorithm on the linked list.
/// </summary>
public record SortResult(
    SortingAlgorithm Algorithm,
    int Comparisons,
    int SwapsOrMoves,
    double ElapsedMilliseconds,
    bool WasAlreadySorted);

/// <summary>
/// A custom doubly linked list implementation specifically designed to manage,
/// sort, and traverse game story script nodes.
/// </summary>
public class StoryLinkedList : IEnumerable<Node>
{
    /// <summary>
    /// Gets the first node in the linked list.
    /// </summary>
    public Node? Head { get; private set; }

    /// <summary>
    /// Gets the last node in the linked list.
    /// </summary>
    public Node? Tail { get; private set; }

    /// <summary>
    /// Gets the total number of nodes in the linked list.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the linked list is empty.
    /// </summary>
    public bool IsEmpty => Count == 0;

    /// <summary>
    /// Initializes an empty <see cref="StoryLinkedList"/>.
    /// </summary>
    public StoryLinkedList()
    {
        Head = null;
        Tail = null;
        Count = 0;
    }

    /// <summary>
    /// Adds a new story paragraph to the end of the linked list.
    /// </summary>
    /// <param name="storyNumber">The story sequence number.</param>
    /// <param name="storyText">The story text.</param>
    /// <returns>The newly added <see cref="Node"/>.</returns>
    public Node Add(int storyNumber, string storyText)
    {
        var newNode = new Node(storyNumber, storyText);
        Add(newNode);
        return newNode;
    }

    /// <summary>
    /// Appends an existing node to the end of the linked list.
    /// </summary>
    /// <param name="node">The node to append.</param>
    public void Add(Node node)
    {
        ArgumentNullException.ThrowIfNull(node);

        node.Next = null;
        node.Previous = null;

        if (Head == null)
        {
            Head = node;
            Tail = node;
        }
        else
        {
            Tail!.Next = node;
            node.Previous = Tail;
            Tail = node;
        }

        Count++;
    }

    /// <summary>
    /// Inserts a new node at the beginning of the linked list.
    /// </summary>
    /// <param name="storyNumber">The story sequence number.</param>
    /// <param name="storyText">The story text.</param>
    /// <returns>The newly added node.</returns>
    public Node AddFirst(int storyNumber, string storyText)
    {
        var newNode = new Node(storyNumber, storyText);

        if (Head == null)
        {
            Head = newNode;
            Tail = newNode;
        }
        else
        {
            newNode.Next = Head;
            Head.Previous = newNode;
            Head = newNode;
        }

        Count++;
        return newNode;
    }

    /// <summary>
    /// Inserts a node at the specified 0-based index.
    /// </summary>
    /// <param name="index">The 0-based index.</param>
    /// <param name="storyNumber">The story sequence number.</param>
    /// <param name="storyText">The story text.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if index is out of bounds.</exception>
    public Node InsertAt(int index, int storyNumber, string storyText)
    {
        if (index < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {Count}.");
        }

        if (index == 0)
        {
            return AddFirst(storyNumber, storyText);
        }

        if (index == Count)
        {
            return Add(storyNumber, storyText);
        }

        var current = GetNodeAt(index)!;
        var newNode = new Node(storyNumber, storyText)
        {
            Next = current,
            Previous = current.Previous
        };

        current.Previous!.Next = newNode;
        current.Previous = newNode;

        Count++;
        return newNode;
    }

    /// <summary>
    /// Removes the first node matching the specified story number.
    /// </summary>
    /// <param name="storyNumber">The story number to remove.</param>
    /// <returns><c>true</c> if removed successfully; otherwise <c>false</c>.</returns>
    public bool Remove(int storyNumber)
    {
        var current = Head;
        while (current != null)
        {
            if (current.StoryNumber == storyNumber)
            {
                RemoveNode(current);
                return true;
            }
            current = current.Next;
        }

        return false;
    }

    /// <summary>
    /// Removes the node at the specified 0-based index.
    /// </summary>
    /// <param name="index">The 0-based index.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is invalid.</exception>
    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {Count - 1}.");
        }

        var node = GetNodeAt(index);
        if (node == null) return false;

        RemoveNode(node);
        return true;
    }

    private void RemoveNode(Node node)
    {
        if (node.Previous != null)
        {
            node.Previous.Next = node.Next;
        }
        else
        {
            Head = node.Next;
        }

        if (node.Next != null)
        {
            node.Next.Previous = node.Previous;
        }
        else
        {
            Tail = node.Previous;
        }

        node.Next = null;
        node.Previous = null;
        Count--;
    }

    /// <summary>
    /// Clears all nodes from the linked list.
    /// </summary>
    public void Clear()
    {
        var current = Head;
        while (current != null)
        {
            var next = current.Next;
            current.Next = null;
            current.Previous = null;
            current = next;
        }

        Head = null;
        Tail = null;
        Count = 0;
    }

    /// <summary>
    /// Finds the first node matching the specified story number.
    /// </summary>
    /// <param name="storyNumber">The story number to find.</param>
    /// <returns>The matching node, or <c>null</c> if not found.</returns>
    public Node? Find(int storyNumber)
    {
        var current = Head;
        while (current != null)
        {
            if (current.StoryNumber == storyNumber)
            {
                return current;
            }
            current = current.Next;
        }

        return null;
    }

    /// <summary>
    /// Retrieves the node at the specified 0-based index.
    /// </summary>
    /// <param name="index">The 0-based index.</param>
    /// <returns>The node at that index, or <c>null</c> if out of range.</returns>
    public Node? GetNodeAt(int index)
    {
        if (index < 0 || index >= Count) return null;

        // Optimize search from head or tail based on distance
        if (index <= Count / 2)
        {
            var current = Head;
            for (int i = 0; i < index && current != null; i++)
            {
                current = current.Next;
            }
            return current;
        }
        else
        {
            var current = Tail;
            for (int i = Count - 1; i > index && current != null; i--)
            {
                current = current.Previous;
            }
            return current;
        }
    }

    /// <summary>
    /// Returns the 0-based index of the specified node reference.
    /// </summary>
    /// <param name="node">The node to locate.</param>
    /// <returns>The 0-based index, or -1 if not found.</returns>
    public int IndexOf(Node? node)
    {
        if (node == null) return -1;

        int index = 0;
        var current = Head;
        while (current != null)
        {
            if (ReferenceEquals(current, node))
            {
                return index;
            }
            index++;
            current = current.Next;
        }

        return -1;
    }

    /// <summary>
    /// Determines whether the linked list is currently sorted in ascending order of <see cref="Node.StoryNumber"/>.
    /// </summary>
    public bool IsSorted()
    {
        if (Count <= 1) return true;

        var current = Head;
        while (current?.Next != null)
        {
            if (current.StoryNumber > current.Next.StoryNumber)
            {
                return false;
            }
            current = current.Next;
        }

        return true;
    }

    /// <summary>
    /// Sorts the linked list using the specified algorithm.
    /// </summary>
    /// <param name="algorithm">The sorting algorithm to use.</param>
    /// <returns>A <see cref="SortResult"/> containing execution metrics.</returns>
    public SortResult Sort(SortingAlgorithm algorithm = SortingAlgorithm.InsertionSort)
    {
        return algorithm switch
        {
            SortingAlgorithm.InsertionSort => SortInsertion(),
            SortingAlgorithm.BubbleSort => SortBubble(),
            SortingAlgorithm.MergeSort => SortMerge(),
            _ => SortInsertion()
        };
    }

    /// <summary>
    /// Sorts the linked list using Insertion Sort.
    /// Builds a sorted list by iteratively taking each node from the unsorted chain
    /// and inserting it into its correct position.
    /// Time Complexity: O(N^2) worst/average, O(N) best if already sorted. Space: O(1).
    /// </summary>
    public SortResult SortInsertion()
    {
        var sw = Stopwatch.StartNew();
        int comparisons = 0;
        int moves = 0;
        bool wasSorted = IsSorted();

        if (Count <= 1 || wasSorted)
        {
            sw.Stop();
            return new SortResult(SortingAlgorithm.InsertionSort, comparisons, moves, sw.Elapsed.TotalMilliseconds, wasSorted);
        }

        Node? sortedHead = null;
        Node? sortedTail = null;
        var current = Head;

        while (current != null)
        {
            var next = current.Next;
            current.Next = null;
            current.Previous = null;

            if (sortedHead == null)
            {
                sortedHead = current;
                sortedTail = current;
            }
            else
            {
                comparisons++;
                if (current.StoryNumber < sortedHead.StoryNumber)
                {
                    // Insert at beginning of sorted list
                    current.Next = sortedHead;
                    sortedHead.Previous = current;
                    sortedHead = current;
                    moves++;
                }
                else
                {
                    comparisons++;
                    if (current.StoryNumber >= sortedTail!.StoryNumber)
                    {
                        // Append at end of sorted list
                        sortedTail.Next = current;
                        current.Previous = sortedTail;
                        sortedTail = current;
                        moves++;
                    }
                    else
                    {
                        // Search for insertion position in middle
                        var search = sortedHead;
                        while (search.Next != null)
                        {
                            comparisons++;
                            if (search.Next.StoryNumber > current.StoryNumber)
                            {
                                break;
                            }
                            search = search.Next;
                        }

                        current.Next = search.Next;
                        if (search.Next != null)
                        {
                            search.Next.Previous = current;
                        }
                        search.Next = current;
                        current.Previous = search;
                        moves++;
                    }
                }
            }

            current = next;
        }

        Head = sortedHead;
        Tail = sortedTail;

        sw.Stop();
        return new SortResult(SortingAlgorithm.InsertionSort, comparisons, moves, sw.Elapsed.TotalMilliseconds, wasSorted);
    }

    /// <summary>
    /// Sorts the linked list using Bubble Sort.
    /// Iteratively compares adjacent nodes and swaps values until no swaps are needed.
    /// Time Complexity: O(N^2) worst/average, O(N) best. Space: O(1).
    /// </summary>
    public SortResult SortBubble()
    {
        var sw = Stopwatch.StartNew();
        int comparisons = 0;
        int swaps = 0;
        bool wasSorted = IsSorted();

        if (Count <= 1 || wasSorted)
        {
            sw.Stop();
            return new SortResult(SortingAlgorithm.BubbleSort, comparisons, swaps, sw.Elapsed.TotalMilliseconds, wasSorted);
        }

        bool swapped;
        Node? lastChecked = null;

        do
        {
            swapped = false;
            var current = Head;

            while (current?.Next != null && current.Next != lastChecked)
            {
                comparisons++;
                if (current.StoryNumber > current.Next.StoryNumber)
                {
                    // Swap payload data between adjacent nodes
                    (current.StoryNumber, current.Next.StoryNumber) = (current.Next.StoryNumber, current.StoryNumber);
                    (current.StoryText, current.Next.StoryText) = (current.Next.StoryText, current.StoryText);

                    swaps++;
                    swapped = true;
                }
                current = current.Next;
            }

            lastChecked = current;
        } while (swapped);

        sw.Stop();
        return new SortResult(SortingAlgorithm.BubbleSort, comparisons, swaps, sw.Elapsed.TotalMilliseconds, wasSorted);
    }

    /// <summary>
    /// Sorts the linked list using Merge Sort (divide-and-conquer).
    /// Splits the list into halves, recursively sorts, and merges.
    /// Time Complexity: O(N log N). Space: O(log N) stack.
    /// </summary>
    public SortResult SortMerge()
    {
        var sw = Stopwatch.StartNew();
        int comparisons = 0;
        int merges = 0;
        bool wasSorted = IsSorted();

        if (Count <= 1 || wasSorted)
        {
            sw.Stop();
            return new SortResult(SortingAlgorithm.MergeSort, comparisons, merges, sw.Elapsed.TotalMilliseconds, wasSorted);
        }

        Head = MergeSortRec(Head, ref comparisons, ref merges);

        // Reconstruct Previous pointers and update Tail
        Node? prev = null;
        var curr = Head;
        while (curr != null)
        {
            curr.Previous = prev;
            prev = curr;
            curr = curr.Next;
        }
        Tail = prev;

        sw.Stop();
        return new SortResult(SortingAlgorithm.MergeSort, comparisons, merges, sw.Elapsed.TotalMilliseconds, wasSorted);
    }

    private Node? MergeSortRec(Node? head, ref int comparisons, ref int merges)
    {
        if (head?.Next == null) return head;

        var middle = GetMiddle(head);
        var nextToMiddle = middle?.Next;
        if (middle != null) middle.Next = null;

        var left = MergeSortRec(head, ref comparisons, ref merges);
        var right = MergeSortRec(nextToMiddle, ref comparisons, ref merges);

        return SortedMerge(left, right, ref comparisons, ref merges);
    }

    private Node? SortedMerge(Node? a, Node? b, ref int comparisons, ref int merges)
    {
        merges++;
        if (a == null) return b;
        if (b == null) return a;

        comparisons++;
        Node? result;
        if (a.StoryNumber <= b.StoryNumber)
        {
            result = a;
            result.Next = SortedMerge(a.Next, b, ref comparisons, ref merges);
        }
        else
        {
            result = b;
            result.Next = SortedMerge(a, b.Next, ref comparisons, ref merges);
        }

        return result;
    }

    private static Node? GetMiddle(Node? head)
    {
        if (head == null) return head;

        var slow = head;
        var fast = head.Next;

        while (fast != null)
        {
            fast = fast.Next;
            if (fast != null)
            {
                slow = slow?.Next;
                fast = fast.Next;
            }
        }

        return slow;
    }

    /// <summary>
    /// Combines all paragraphs in the linked list into a single full story script.
    /// </summary>
    /// <param name="includeNumbers">Whether to prefix each paragraph with [Chapter X].</param>
    /// <returns>The formatted combined story script.</returns>
    public string GetCombinedScriptText(bool includeNumbers = true)
    {
        if (IsEmpty) return "No script loaded.";

        var lines = new List<string>();
        var current = Head;
        while (current != null)
        {
            if (includeNumbers)
            {
                lines.Add($"[Paragraph {current.StoryNumber}]\n{current.StoryText}");
            }
            else
            {
                lines.Add(current.StoryText);
            }
            current = current.Next;
        }

        return string.Join("\n\n", lines);
    }

    /// <summary>
    /// Converts the linked list nodes into an array.
    /// </summary>
    public Node[] ToArray()
    {
        var array = new Node[Count];
        int index = 0;
        var current = Head;
        while (current != null)
        {
            array[index++] = current;
            current = current.Next;
        }
        return array;
    }

    /// <summary>
    /// Converts the linked list nodes into a <see cref="List{Node}"/>.
    /// </summary>
    public List<Node> ToList()
    {
        var list = new List<Node>(Count);
        var current = Head;
        while (current != null)
        {
            list.Add(current);
            current = current.Next;
        }
        return list;
    }

    /// <summary>
    /// Creates a deep copy clone of this linked list.
    /// </summary>
    public StoryLinkedList Clone()
    {
        var clone = new StoryLinkedList();
        var current = Head;
        while (current != null)
        {
            clone.Add(current.StoryNumber, current.StoryText);
            current = current.Next;
        }
        return clone;
    }

    /// <inheritdoc />
    public IEnumerator<Node> GetEnumerator()
    {
        var current = Head;
        while (current != null)
        {
            yield return current;
            current = current.Next;
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
