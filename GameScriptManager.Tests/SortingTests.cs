using GameScriptManager.Data;
using GameScriptManager.Models;
using Xunit;

namespace GameScriptManager.Tests;

public class SortingTests
{
    private static void AssertListIsSortedAndPointersIntact(StoryLinkedList list)
    {
        Assert.True(list.IsSorted(), "Linked list should report IsSorted() == true.");

        // Check forward traversal and sequence 1..N
        var current = list.Head;
        int expectedNumber = 1;
        Node? prev = null;

        while (current != null)
        {
            Assert.Equal(expectedNumber, current.StoryNumber);
            Assert.Same(prev, current.Previous);

            prev = current;
            current = current.Next;
            expectedNumber++;
        }

        Assert.Equal(list.Count + 1, expectedNumber);
        Assert.Same(list.Tail, prev);
    }

    [Theory]
    [InlineData(SortingAlgorithm.InsertionSort)]
    [InlineData(SortingAlgorithm.BubbleSort)]
    [InlineData(SortingAlgorithm.MergeSort)]
    public void Sort_ReordersSampleDataCorrectly(SortingAlgorithm algorithm)
    {
        var list = SampleScriptData.PopulateSampleScript();

        Assert.False(list.IsSorted(), "Initial sample data should be unordered.");
        Assert.Equal(12, list.Count);
        Assert.Equal(3, list.Head!.StoryNumber); // First unordered element is 3

        var result = list.Sort(algorithm);

        Assert.Equal(algorithm, result.Algorithm);
        AssertListIsSortedAndPointersIntact(list);

        Assert.Equal(1, list.Head!.StoryNumber);
        Assert.Equal(12, list.Tail!.StoryNumber);
    }

    [Theory]
    [InlineData(SortingAlgorithm.InsertionSort)]
    [InlineData(SortingAlgorithm.BubbleSort)]
    [InlineData(SortingAlgorithm.MergeSort)]
    public void Sort_HandlesEmptyList(SortingAlgorithm algorithm)
    {
        var list = new StoryLinkedList();

        var result = list.Sort(algorithm);

        Assert.True(list.IsEmpty);
        Assert.Equal(0, list.Count);
        Assert.Null(list.Head);
        Assert.Null(list.Tail);
    }

    [Theory]
    [InlineData(SortingAlgorithm.InsertionSort)]
    [InlineData(SortingAlgorithm.BubbleSort)]
    [InlineData(SortingAlgorithm.MergeSort)]
    public void Sort_HandlesSingleElementList(SortingAlgorithm algorithm)
    {
        var list = new StoryLinkedList();
        var node = list.Add(42, "Solo node");

        var result = list.Sort(algorithm);

        Assert.Equal(1, list.Count);
        Assert.Same(node, list.Head);
        Assert.Same(node, list.Tail);
        Assert.Null(node.Next);
        Assert.Null(node.Previous);
    }

    [Theory]
    [InlineData(SortingAlgorithm.InsertionSort)]
    [InlineData(SortingAlgorithm.BubbleSort)]
    [InlineData(SortingAlgorithm.MergeSort)]
    public void Sort_HandlesReverseSortedList(SortingAlgorithm algorithm)
    {
        var list = new StoryLinkedList();
        for (int i = 10; i >= 1; i--)
        {
            list.Add(i, $"Paragraph {i}");
        }

        var result = list.Sort(algorithm);

        Assert.Equal(10, list.Count);
        AssertListIsSortedAndPointersIntact(list);
    }

    [Fact]
    public void IsSorted_DetectsDisorderedList()
    {
        var list = new StoryLinkedList();
        list.Add(1, "A");
        list.Add(5, "B");
        list.Add(3, "C");

        Assert.False(list.IsSorted());
    }

    [Fact]
    public void IsSorted_DetectsSortedList()
    {
        var list = new StoryLinkedList();
        list.Add(1, "A");
        list.Add(2, "B");
        list.Add(3, "C");

        Assert.True(list.IsSorted());
    }
}
