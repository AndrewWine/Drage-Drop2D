using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    public event Action OnLeftClick;
    public event Action OnRightClick;
    public GameManager gameManager;
    public AudioManager audioManager;
    public Action NotifyESCPressed;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>(); 
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnLeftClick?.Invoke();
            audioManager.PlaySFX(audioManager.Placing);
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnRightClick?.Invoke();
            audioManager.PlaySFX(audioManager.Gather);

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            NotifyESCPressed?.Invoke();
            Debug.Log("Đã Thoát Game");

        }
    }
}
