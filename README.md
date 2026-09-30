# 🎮 Game Script Manager — PROG7312 ICE Task 2

**Student Number:** ST10435077  
**Student Name:** Tashiel Singh  
**Module:** PROG7312 — Advanced Programming / Data Structures  
**Activity:** ICE Task 2 — Game Script Manager  
**Target Framework:** .NET 8.0 Windows (WPF) / C# 12  
**Repository:** `ST10435077 - PROG 7312 - ICE TASK 2`  

---

## 📋 Table of Contents
1. [Overview & Objective](#-overview--objective)
2. [Learning Objectives & Compliance Matrix](#-learning-objectives--compliance-matrix)
3. [Architecture & Project Structure](#-architecture--project-structure)
4. [Data Structure: Doubly Linked List](#-data-structure-doubly-linked-list)
5. [Sorting Algorithms & Complexity Analysis](#-sorting-algorithms--complexity-analysis)
6. [Graphical User Interface (GUI) Features](#-graphical-user-interface-gui-features)
7. [Sample Script Data: Cybersphere Narrative](#-sample-script-data-cybersphere-narrative)
8. [Unit Testing & Code Coverage](#-unit-testing--code-coverage)
9. [Build & Execution Guide](#-build--execution-guide)

---

## 🎯 Overview & Objective
In modern game development, narrative scripts, dialogue trees, and cinematic storyboards are frequently received in disordered chunks due to non-linear authoring pipelines. 

**Game Script Manager** is a desktop application engineered in **C# / WPF (.NET 8.0)** that loads a disordered 12-paragraph video game script (*"The Cybersphere Chronicles"*), stores each paragraph in a custom **Linked List** data structure, sorts the nodes into proper narrative sequence using custom sorting algorithms (Insertion Sort, Bubble Sort, Merge Sort), and displays the sorted script across two coordinated presentation modes:
1. **Display Area 1 (Full Script View):** All paragraphs combined in chronological sequence with chapter markers, reading time estimates, keyword search, and clipboard export.
2. **Display Area 2 (One-Line Reader Mode):** A focused scene-by-scene card view featuring interactive **Next** and **Previous** navigation buttons, keyboard hotkeys, and an automated story playback mode.

---

## 🚀 Learning Objectives & Compliance Matrix

| Objective / Instruction | Assignment Requirement | Implementation Details | Source Reference |
| :--- | :--- | :--- | :--- |
| **1. Implement a Linked List** | Create a `Node` class storing `Story number` (int) and `Story text` (string). Create a `LinkedList` class to hold and manage nodes. | Custom `Node` class with `StoryNumber`, `StoryText`, `Next`, and `Previous` pointers. Custom `StoryLinkedList` class with `Head`, `Tail`, `Count`, insertion, deletion, lookup, and enumeration. | [`Node.cs`](file:///d:/Desktop/Media/School/PROG7312/ICE/ICE%202/GameScriptManager/Models/Node.cs)<br>[`StoryLinkedList.cs`](file:///d:/Desktop/Media/School/PROG7312/ICE/ICE%202/GameScriptManager/Models/StoryLinkedList.cs) |
| **2. Populate the Linked List** | Function adding each unordered script paragraph into the list paired with its number. | `SampleScriptData.PopulateSampleScript()` loads all 12 raw unordered paragraphs directly into the `StoryLinkedList`. | [`SampleScriptData.cs`](file:///d:/Desktop/Media/School/PROG7312/ICE/ICE%202/GameScriptManager/Data/SampleScriptData.cs) |
| **3. Sort the Linked List** | Implement sorting algorithm (Bubble Sort, Insertion Sort, Merge Sort) based on `story number`. | Three dedicated algorithms implemented on the linked list: **Insertion Sort** (optimal pointer relocation), **Bubble Sort** (adjacent comparison & swaps), and **Merge Sort** ($O(N \log N)$ divide-and-conquer). | [`StoryLinkedList.cs`](file:///d:/Desktop/Media/School/PROG7312/ICE/ICE%202/GameScriptManager/Models/StoryLinkedList.cs#L305-L480) |
| **4. Build the GUI** | User interface with: Display Area 1 (Full sorted script), Display Area 2 (One line at a time), and Next/Previous navigation buttons. | High-performance WPF UI with cyberpunk dark theme, Display Area 1 card scroller, Display Area 2 narrative reader card, and interactive Next/Previous buttons. | [`MainWindow.xaml`](file:///d:/Desktop/Media/School/PROG7312/ICE/ICE%202/GameScriptManager/MainWindow.xaml)<br>[`MainWindow.xaml.cs`](file:///d:/Desktop/Media/School/PROG7312/ICE/ICE%202/GameScriptManager/MainWindow.xaml.cs) |
| **5. Display Sorted Script** | On load or button click, show sorted game script in both display modes; allow scrolling and line-by-line navigation. | Loads and sorts automatically on launch; instant re-sort button, live stats, and synchronized navigation across both display areas. | [`MainWindow.xaml.cs`](file:///d:/Desktop/Media/School/PROG7312/ICE/ICE%202/GameScriptManager/MainWindow.xaml.cs#L45-L65) |
| **6. Interaction Features** | Switch between Full Script view and One-line navigation mode with smooth transitions. | Tab bar supporting **Full Script View**, **One-Line Navigation Mode**, and **Split Side-by-Side Dual View** with responsive column animations. | [`MainWindow.xaml.cs`](file:///d:/Desktop/Media/School/PROG7312/ICE/ICE%202/GameScriptManager/MainWindow.xaml.cs#L380-L415) |

---

## 🏛 Architecture & Project Structure

```
ICE 2/
├── GameScriptManager.sln                     # Visual Studio Solution File
├── .gitignore                                # Clean repository configuration
├── README.md                                 # Comprehensive project documentation
├── GameScriptManager/                        # Main WPF Desktop Application
│   ├── GameScriptManager.csproj              # .NET 8.0 Windows WPF project
│   ├── App.xaml / App.xaml.cs                # Application lifecycle & styles
│   ├── MainWindow.xaml                       # Cyberpunk themed dual-display GUI
│   ├── MainWindow.xaml.cs                    # UI event coordinator & navigation logic
│   ├── Models/
│   │   ├── Node.cs                           # Linked list node (StoryNumber, StoryText, Next, Prev)
│   │   ├── StoryLinkedList.cs                # Doubly Linked List with sorting algorithms
│   │   └── SortingAlgorithm.cs               # Enum: InsertionSort, BubbleSort, MergeSort
│   ├── Data/
│   │   └── SampleScriptData.cs               # 12 unordered script paragraphs from specification
│   └── Views/
│       ├── AddNodeDialog.xaml                # Modal window to insert custom story scenes
│       └── AddNodeDialog.xaml.cs             # Dialog input validation & node injection
└── GameScriptManager.Tests/                  # Comprehensive xUnit Unit Testing Suite
    ├── GameScriptManager.Tests.csproj        # xUnit Test project (.NET 8.0)
    ├── NodeTests.cs                          # Verification of Node instantiation and pointers
    ├── StoryLinkedListTests.cs               # Add, Remove, Insert, Traverse, IndexOf, Clear tests
    ├── SortingTests.cs                       # Insertion, Bubble, and Merge sort verification
    ├── SampleScriptDataTests.cs              # Validates 12 sample paragraphs and narrative coherence
    └── AlgorithmComparisonTests.cs          # Multi-algorithm benchmarks, edge cases, duplicates
```

---

## ⛓ Data Structure: Doubly Linked List

### Why a Doubly Linked List?
While a Singly Linked List (`Next` pointer only) satisfies basic traversal, a **Doubly Linked List** (`Next` and `Previous` pointers) is the optimal choice for a story script reader:
1. **$O(1)$ Line-by-Line Navigation:** Moving to the **Next** paragraph (`current = current.Next`) and moving back to the **Previous** paragraph (`current = current.Previous`) both execute in constant time $O(1)$ without needing to traverse the list from the `Head`.
2. **Instant Tail Operations:** Direct reference to `Tail` allows $O(1)$ appends and backward traversals.
3. **Robust Pointer Maintenance:** Node removals and insertions cleanly update neighbors in both directions.

### Class Definition: `Node.cs`
```csharp
public class Node
{
    public int StoryNumber { get; set; }
    public string StoryText { get; set; }
    public Node? Next { get; set; }
    public Node? Previous { get; set; }

    public Node(int storyNumber, string storyText)
    {
        StoryNumber = storyNumber;
        StoryText = storyText ?? string.Empty;
    }
}
```

---

## ⚡ Sorting Algorithms & Complexity Analysis

The application provides three built-in sorting algorithms operable directly on the linked list:

### 1. Insertion Sort (`SortInsertion`) — *Default & Recommended*
- **Mechanism:** Builds a sorted linked list by taking each node from the unsorted chain and finding its correct position in the sorted sublist, splicing the pointers in $O(1)$ once the position is identified.
- **Time Complexity:** 
  - Best Case (Already Sorted): $O(N)$
  - Average / Worst Case (Reverse Order): $O(N^2)$
- **Space Complexity:** $O(1)$ auxiliary memory (in-place node relocation).
- **Suitability:** Highly efficient for linked lists because nodes are simply unlinked and re-linked without memory reallocations or array copying.

### 2. Bubble Sort (`SortBubble`)
- **Mechanism:** Iteratively scans adjacent nodes, comparing `current.StoryNumber > current.Next.StoryNumber`. If out of sequence, payloads are swapped. An early-exit flag `swapped` halts execution as soon as a pass occurs with zero swaps.
- **Time Complexity:**
  - Best Case: $O(N)$
  - Average / Worst Case: $O(N^2)$
- **Space Complexity:** $O(1)$ auxiliary memory.

### 3. Merge Sort (`SortMerge`)
- **Mechanism:** Divide-and-conquer algorithm. Uses the *slow and fast pointer* technique to find the middle node, splits the linked list into two sublists, recursively sorts each half, and merges the sorted sublists back together.
- **Time Complexity:** $O(N \log N)$ in all cases (Best, Average, Worst).
- **Space Complexity:** $O(\log N)$ call-stack frames.
- **Suitability:** Asymptotically optimal for large scripts or long narrative branching trees.

---

## 🖥 Graphical User Interface (GUI) Features

Designed with a sleek **Cyberpunk Dark Theme** (`#0D1117`, `#161B22`, `#21262D`) accented with neon cyan (`#00D2FF`) and emerald green (`#3FB950`).

### 1. Dual Display Areas
- **Display Area 1 (Full Story Script):**
  - Displays all 12 paragraphs sequentially with chapter badges.
  - Live search bar to filter paragraphs by keyword.
  - Word count, reading time calculator, and "Copy to Clipboard" utility.
  - "Navigate Here" shortcut button on each paragraph to jump Display Area 2 to that scene.
- **Display Area 2 (One-Line at a Time Reader):**
  - Highlighted story card with chapter badges.
  - Large, readable narrative typography.
  - Real-time progress bar: `Scene X of 12`.
  - Pointer inspector showing `Prev: [X] -> Active: [Y] -> Next: [Z]`.
  - **Previous (◀)** and **Next (▶)** buttons that intelligently enable/disable at boundaries.

### 2. Live Linked List Visualizer Chain
- A real-time visual chain rendered at the bottom:
  `[ 1 ] ➔ [ 2 ] ➔ [ 3 ] ➔ [ 4 ] ➔ ... ➔ [ 12 ]`
- Clicking any node bubble in the chain instantly navigates Display Area 2 to that node.
- Active node glows with cyan neon border.

### 3. Dynamic View Mode Switching
- **📖 Full Script View:** Maximizes Display Area 1 for comprehensive reading.
- **🎮 One-Line Navigation Mode:** Maximizes Display Area 2 for focused scene presentation.
- **⚡ Side-by-Side Dual View:** Displays both areas simultaneously for comparative grading.

### 4. Interactive Sandbox Controls
- **Sort Linked List:** Runs selected algorithm with millisecond timer and comparison counter.
- **Scramble / Shuffle:** Randomizes node order to test sorting repeatedly.
- **Add Story Scene:** Modal dialog allowing students or lecturers to insert custom scenes.
- **Auto-Tour (Story Slide):** Automatically steps through paragraphs every 2.5 seconds.
- **Keyboard Shortcuts:** Arrow Left (Previous), Arrow Right (Next), Home (First), End (Last), Space (Play/Pause).

---

## 📖 Sample Script Data: Cybersphere Narrative

The application comes pre-loaded with the exact 12 unordered script paragraphs specified in the assignment:

| Unordered Index | Story # | Narrative Excerpt |
| :---: | :---: | :--- |
| 1 | **3** | *With every line of code mastered, Alex gains experience points...* |
| 2 | **12** | *The tale of Alex, the IT student-turned-digital-legend, is forever etched...* |
| 3 | **4** | *The Firewall Fortress looms ahead, its defenses formidable...* |
| 4 | **2** | *Armed with a trusty keyboard and a digital sword, Alex enters the Coding Caverns...* |
| 5 | **1** | *In the virtual realm of Cybersphere, our hero, Alex, a determined IT student...* |
| 6 | **7** | *Along the journey, Alex forges alliances with NPC coders, forming "Syntax Sentinels"...* |
| 7 | **10** | *Victory is hard-won, but Alex's leadership and IT prowess lead to the defeat...* |
| 8 | **11** | *Celebrated as a digital hero, Alex stands at the forefront of innovation...* |
| 9 | **9** | *In a climactic battle, Alex and the Syntax Sentinels engage in a virtual war...* |
| 10 | **5** | *Inside the fortress, a virtual reality riddle awaits – a puzzle that can only be solved...* |
| 11 | **6** | *Emerging victorious, Alex discovers the hidden Repository of the Ancients...* |
| 12 | **8** | *The guild faces its nemesis in the form of the Malware Marauders...* |

Upon applying any sorting algorithm, the linked list is reordered into the chronological story sequence (**1 through 12**).

---

## 🧪 Unit Testing & Code Coverage

A dedicated test project (`GameScriptManager.Tests`) using **xUnit** tests every aspect of the data structure, sorting algorithms, and edge cases.

### Test Results: **48 Passed, 0 Failed, 0 Skipped**

```
Test run for GameScriptManager.Tests.dll (.NETCoreApp,Version=v8.0)
Passed!  - Failed: 0, Passed: 48, Skipped: 0, Total: 48, Duration: 18 ms
```

### Key Test Categories:
- **`NodeTests`:** Property initialization, null safety, string formatting, pointer assignments.
- **`StoryLinkedListTests`:** Empty state verification, `Add`, `AddFirst`, `InsertAt`, `Remove`, `RemoveAt`, `Find`, `GetNodeAt`, `IndexOf`, `Clear`, `Clone`, and `IEnumerable` foreach traversal.
- **`SortingTests`:** Insertion Sort, Bubble Sort, and Merge Sort on unordered sample data, empty lists, single elements, and reversed lists.
- **`SampleScriptDataTests`:** Validates exact 12-item dataset and narrative continuity.
- **`AlgorithmComparisonTests`:** Proves all three algorithms produce identical sorted output, handles duplicate keys, and verifies performance metrics.

---

## 🛠 Build & Execution Guide

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Visual Studio 2022 (with .NET desktop development workload) or VS Code / JetBrains Rider

### Clone & Run
```bash
# 1. Clone repository
git clone https://github.com/Tashiel-Singh/ST10435077---PROG-7312---ICE-TASK-2.git
cd "ST10435077 - PROG 7312 - ICE TASK 2"

# 2. Restore and Build Solution
dotnet restore GameScriptManager.sln
dotnet build GameScriptManager.sln

# 3. Execute Unit Tests
dotnet test GameScriptManager.sln

# 4. Launch Application
dotnet run --project GameScriptManager/GameScriptManager.csproj
```

---

*Authored by Tashiel Singh (ST10435077) for PROG7312 ICE Task 2.*
