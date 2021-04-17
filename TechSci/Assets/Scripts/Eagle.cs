using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : Enemy
{
    [SerializeField] private float topCap;
    [SerializeField] private float bottomCap;
    [SerializeField] private float speed = 3f;//
    private Rigidbody2D rb;
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();


    }
    private void Update()
    {
       
    }

    

}