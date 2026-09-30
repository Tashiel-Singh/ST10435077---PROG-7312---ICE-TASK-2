namespace GameScriptManager.Models;

/// <summary>
/// Represents a node within the game script linked list.
/// Stores a story paragraph along with its intended story sequence number.
/// Contains forward (Next) and backward (Previous) pointers for bidirectional navigation.
/// </summary>
public class Node
{
    /// <summary>
    /// Gets or sets the sequence number of the story paragraph.
    /// </summary>
    public int StoryNumber { get; set; }

    /// <summary>
    /// Gets or sets the narrative text content of the paragraph.
    /// </summary>
    public string StoryText { get; set; }

    /// <summary>
    /// Gets or sets the reference to the next node in the linked list.
    /// </summary>
    public Node? Next { get; set; }

    /// <summary>
    /// Gets or sets the reference to the previous node in the linked list.
    /// </summary>
    public Node? Previous { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Node"/> class.
    /// </summary>
    /// <param name="storyNumber">The story sequence number.</param>
    /// <param name="storyText">The story text content.</param>
    public Node(int storyNumber, string storyText)
    {
        StoryNumber = storyNumber;
        StoryText = storyText ?? string.Empty;
        Next = null;
        Previous = null;
    }

    /// <summary>
    /// Returns a string representation of the story node.
    /// </summary>
    public override string ToString()
    {
        return $"[{StoryNumber}] {StoryText}";
    }
}
