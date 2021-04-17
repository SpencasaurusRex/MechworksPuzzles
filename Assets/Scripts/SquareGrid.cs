using UnityEngine;

public class SquareGrid : MonoBehaviour
{
    void Start()
    {
        GameController.Instance.SetValidSpace(transform.position.RoundToInt());
    }
}
