using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TestCase : MonoBehaviour
{
    int tickNumber = 0;
    List<Expectation> expectations = new List<Expectation>();

    void Start() {
        GameController.Instance.OnTick += Tick;
        
        // Parse test cases
        for (int i = 0; i < transform.childCount; i++) {
            var testCase = transform.GetChild(i); 
            string name = testCase.name;

            string singlePattern = "([+-]?\\d*)(\\w)";
            string wholePattern = $"^({singlePattern})+$";
            if (!Regex.IsMatch(name, wholePattern)) {
                continue;
            }

            Expectation expectation = new Expectation();
            Regex r = new Regex(singlePattern);
            var matchInfo = r.Matches(name);
            for (int j = 0; j < matchInfo.Count; j++) {
                var match = matchInfo[j];

                int number = Convert.ToInt32(match.Groups[1].Value);
                string letter = match.Groups[2].Value.ToLower();
                switch (letter) {
                    case "t":
                        expectation.TargetTick = number;
                        break;
                    case "x":
                        expectation.Dx = number;
                        break;
                    case "y":
                        expectation.Dy = number;
                        break;
                    case "z":
                        expectation.Dz = number;
                        break;
                }
            }

            expectation.Objects = new GridObject[testCase.childCount];
            expectation.TargetLocations = new Vector3Int[testCase.childCount];
            var delta = new Vector3Int(expectation.Dx, expectation.Dy, expectation.Dz);
            for (int j = 0; j < testCase.childCount; j++) {
                var go = testCase.GetChild(j).GetComponent<GridObject>();
                expectation.Objects[j] = go;
                expectation.TargetLocations[j] = go.Location + delta;
            }

            expectations.Add(expectation);
        }
    }

    void Tick() {
        for (int i = 0; i < expectations.Count; i++) {
            var expectation = expectations[i];
            if (expectation.TargetTick != tickNumber) {
                continue;
            }

            bool first = true;
            for (int j = 0; j < expectation.Objects.Length; j++) {
                var go = expectation.Objects[j]; 
                if (go.Location != expectation.TargetLocations[j] ) {
                    if (first) print($"{name} failed:");
                    print($"{go.name} expected {expectation.TargetLocations[j]} got {go.Location}");
                    first = false;
                }
            }
        }
        tickNumber++;
    }
}

public class Expectation {
    public GridObject[] Objects;
    public Vector3Int[] TargetLocations;
    public int Dx;
    public int Dy;
    public int Dz;
    public int TargetTick;
}