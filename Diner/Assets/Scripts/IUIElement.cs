using System.Collections;
using UnityEngine.UI;

public interface IUIElement
{
    Image FillImg { get; }

    float CurrentValue { get; }

    float TotalWeight { get; }

    int Index { get; }

    bool Filled { get; }

    IEnumerator Fill(int value);
}