using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleSolvedUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UpdatePuzzlesSolved(TurkPuzzleScript.PuzzlesSolved);
        TurkPuzzleScript.OnPuzzleComplete += UpdatePuzzlesSolved;
    }

    public void UpdatePuzzlesSolved(int newCount)
    {
        GetComponent<TextMeshProUGUI>().text = "Puzzles Solved: " + newCount.ToString();
    }
}
