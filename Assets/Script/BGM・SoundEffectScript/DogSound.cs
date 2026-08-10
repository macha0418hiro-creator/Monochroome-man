using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DogSound : MonoBehaviour
{
    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        SoundManager.Instance?.PlaySE(SoundManager.SEType.DogClick);
    }
}