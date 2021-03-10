using UnityEngine;

public class TestCase : MonoBehaviour
{
    public ExpectedState[] TestStates;

    void Start() {
        GameController.Instance.OnTick += Tick;
    }

    void Tick() {

    }
}

public class ExpectedState {
    public int Tick;

}