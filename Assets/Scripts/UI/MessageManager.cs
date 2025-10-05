using System;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro;

    private void Awake() => _textMeshPro = GetComponent<TextMeshProUGUI>();

    private void Start() => GameManager.Instance.OnUpdateGameText += OnUpdateMessage;

    private void OnUpdateMessage(string newMessage) => _textMeshPro.text = newMessage;
}