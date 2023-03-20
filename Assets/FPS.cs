using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{

    private double[] frametimes;
    private int front;
    private int size;
    private int printmove;

    // Start is called before the first frame update
    void Start()
    {
        size = 5000;
        frametimes = new double[size];
        printmove = front = 0;
        Debug.Log("Initialized");
    }



    // Update is called once per frame
    void Update()
    {
        frametimes[front++ % size] = Time.deltaTime;
        if (front % size == printmove)
            PrintFramerate();
    }

    void PrintFramerate()
    {
        printmove = (printmove+1) % size;
        double avgFramerate = 0;
        for (int i = 0; i < size; i++)
        {
            avgFramerate += frametimes[i];
        }
        avgFramerate /= size;
        Debug.Log("Framerate " + avgFramerate);
    }
}
