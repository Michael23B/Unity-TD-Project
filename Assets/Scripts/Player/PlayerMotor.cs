﻿using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Run every physics iteration
    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    //Gets a movement vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
        }
    }
    
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    public void PerformRotation()
    {
        rb.MoveRotation(transform.rotation * Quaternion.Euler(rotation));
    }
    
}
