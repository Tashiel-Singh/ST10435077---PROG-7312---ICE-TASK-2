using GameScriptManager.Models;
using Xunit;

namespace GameScriptManager.Tests;

public class StoryLinkedListTests
{
    [Fact]
    public void NewList_IsEmpty()
    {
        var list = new StoryLinkedList();

        Assert.True(list.IsEmpty);
        Assert.Equal(0, list.Count);
        Assert.Null(list.Head);
        Assert.Null(list.Tail);
    }

    [Fact]
    public void Add_SingleNode_SetsHeadAndTail()
    {
        var list = new StoryLinkedList();
        var node = list.Add(1, "First line");

        Assert.False(list.IsEmpty);
        Assert.Equal(1, list.Count);
        Assert.Same(node, list.Head);
        Assert.Same(node, list.Tail);
        Assert.Null(node.Next);
        Assert.Null(node.Previous);
    }

    [Fact]
    public void Add_MultipleNodes_MaintainsBidirectionalLinks()
    {
        var list = new StoryLinkedList();
        var n1 = list.Add(1, "One");
        var n2 = list.Add(2, "Two");
        var n3 = list.Add(3, "Three");

        Assert.Equal(3, list.Count);
        Assert.Same(n1, list.Head);
        Assert.Same(n3, list.Tail);

        Assert.Same(n2, n1.Next);
        Assert.Same(n1, n2.Previous);

        Assert.Same(n3, n2.Next);
        Assert.Same(n2, n3.Previous);

        Assert.Null(n3.Next);
        Assert.Null(n1.Previous);
    }

    [Fact]
    public void AddFirst_InsertsAtBeginning()
    {
        var list = new StoryLinkedList();
        var n2 = list.Add(2, "Two");
        var n1 = list.AddFirst(1, "One");

        Assert.Equal(2, list.Count);
        Assert.Same(n1, list.Head);
        Assert.Same(n2, list.Tail);
        Assert.Same(n2, n1.Next);
        Assert.Same(n1, n2.Previous);
    }

    [Fact]
    public void InsertAt_Middle_LinksCorrectly()
    {
        var list = new StoryLinkedList();
        list.Add(1, "One");
        list.Add(3, "Three");

        var n2 = list.InsertAt(1, 2, "Two");

        Assert.Equal(3, list.Count);
        Assert.Equal(2, list.GetNodeAt(1)!.StoryNumber);
        Assert.Same(list.Head!.Next, n2);
        Assert.Same(list.Tail!.Previous, n2);
    }

    [Fact]
    public void InsertAt_InvalidIndex_ThrowsException()
    {
        var list = new StoryLinkedList();
        list.Add(1, "One");

        Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertAt(-1, 0, "Invalid"));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertAt(5, 0, "Invalid"));
    }

    [Fact]
    public void Remove_ByStoryNumber_RemovesHead()
    {
        var list = new StoryLinkedList();
        list.Add(1, "One");
        list.Add(2, "Two");

        bool removed = list.Remove(1);

        Assert.True(removed);
        Assert.Equal(1, list.Count);
        Assert.Equal(2, list.Head!.StoryNumber);
        Assert.Null(list.Head.Previous);
        Assert.Same(list.Head, list.Tail);
    }

    [Fact]
    public void Remove_ByStoryNumber_RemovesTail()
    {
        var list = new StoryLinkedList();
        list.Add(1, "One");
        list.Add(2, "Two");

        bool removed = list.Remove(2);

        Assert.True(removed);
        Assert.Equal(1, list.Count);
        Assert.Equal(1, list.Head!.StoryNumber);
        Assert.Null(list.Head.Next);
        Assert.Same(list.Head, list.Tail);
    }

    [Fact]
    public void Remove_ByStoryNumber_RemovesMiddleNode()
    {
        var list = new StoryLinkedList();
        list.Add(1, "One");
        list.Add(2, "Two");
        list.Add(3, "Three");

        bool removed = list.Remove(2);

        Assert.True(removed);
        Assert.Equal(2, list.Count);
        Assert.Same(list.Head!.Next, list.Tail);
        Assert.Same(list.Tail!.Previous, list.Head);
    }

    [Fact]
    public void Remove_NonExistent_ReturnsFalse()
    {
        var list = new StoryLinkedList();
        list.Add(1, "One");

        bool removed = list.Remove(999);

        Assert.False(removed);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void RemoveAt_RemovesCorrectIndex()
    {
        var list = new StoryLinkedList();
        list.Add(10, "Ten");
        list.Add(20, "Twenty");
        list.Add(30, "Thirty");

        list.RemoveAt(1);

        Assert.Equal(2, list.Count);
        Assert.Equal(10, list.GetNodeAt(0)!.StoryNumber);
        Assert.Equal(30, list.GetNodeAt(1)!.StoryNumber);
    }

    [Fact]
    public void Clear_EmptiesTheList()
    {
        var list = new StoryLinkedList();
        list.Add(1, "One");
        list.Add(2, "Two");

        list.Clear();

        Assert.True(list.IsEmpty);
        Assert.Equal(0, list.Count);
        Assert.Null(list.Head);
        Assert.Null(list.Tail);
    }

    [Fact]
    public void Find_ReturnsMatchingNode()
    {
        var list = new StoryLinkedList();
        list.Add(1, "First");
        list.Add(7, "Seven");

        var found = list.Find(7);
        var notFound = list.Find(99);

        Assert.NotNull(found);
        Assert.Equal("Seven", found!.StoryText);
        Assert.Null(notFound);
    }

    [Fact]
    public void GetNodeAt_ReturnsCorrectNode()
    {
        var list = new StoryLinkedList();
        list.Add(10, "A");
        list.Add(20, "B");
        list.Add(30, "C");

        Assert.Equal(10, list.GetNodeAt(0)!.StoryNumber);
        Assert.Equal(20, list.GetNodeAt(1)!.StoryNumber);
        Assert.Equal(30, list.GetNodeAt(2)!.StoryNumber);
        Assert.Null(list.GetNodeAt(-1));
        Assert.Null(list.GetNodeAt(3));
    }

    [Fact]
    public void IndexOf_ReturnsExpectedIndex()
    {
        var list = new StoryLinkedList();
        var n1 = list.Add(1, "A");
        var n2 = list.Add(2, "B");

        Assert.Equal(0, list.IndexOf(n1));
        Assert.Equal(1, list.IndexOf(n2));
        Assert.Equal(-1, list.IndexOf(new Node(9, "Unlinked")));
        Assert.Equal(-1, list.IndexOf(null));
    }

    [Fact]
    public void Foreach_IteratesAllNodes()
    {
        var list = new StoryLinkedList();
        list.Add(1, "A");
        list.Add(2, "B");
        list.Add(3, "C");

        var numbers = new List<int>();
        foreach (var node in list)
        {
            numbers.Add(node.StoryNumber);
        }

        Assert.Equal(new[] { 1, 2, 3 }, numbers);
    }

    [Fact]
    public void Clone_CreatesIndependentDeepCopy()
    {
        var original = new StoryLinkedList();
        original.Add(1, "A");
        original.Add(2, "B");

        var clone = original.Clone();

        Assert.Equal(original.Count, clone.Count);
        Assert.NotSame(original.Head, clone.Head);
        Assert.Equal(original.Head!.StoryNumber, clone.Head!.StoryNumber);

        clone.Add(3, "C");
        Assert.Equal(2, original.Count);
        Assert.Equal(3, clone.Count);
    }

    [Fact]
    public void GetCombinedScriptText_FormatsOutputCorrectly()
    {
        var list = new StoryLinkedList();
        list.Add(1, "Start.");
        list.Add(2, "End.");

        var text = list.GetCombinedScriptText();

        Assert.Contains("[Paragraph 1]\nStart.", text);
        Assert.Contains("[Paragraph 2]\nEnd.", text);
    }
}
