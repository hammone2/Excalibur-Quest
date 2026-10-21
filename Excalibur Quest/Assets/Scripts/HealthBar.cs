using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Slider healthBarSlider;
    private HealthComponent healthComponent;
    [SerializeField] private Slider flashBarSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    //[SerializeField] private TextMeshProUGUI nameText;

    private int maxHealth;

    private bool canFlashBarMove = true;

    public void InitializeHealthBar(HealthComponent healthComponent, string name)
    {
        maxHealth = healthComponent.maxHealth;
        healthBarSlider.maxValue = maxHealth;
        flashBarSlider.maxValue = maxHealth;

        healthBarSlider.value = healthComponent.healthPoints;
        this.healthComponent = healthComponent;

        flashBarSlider.value = healthBarSlider.value;

        //nameText.SetText(name);
        healthText.SetText(healthComponent.healthPoints + "");
    }

    private void Update()
    {
        healthText.SetText(healthComponent.healthPoints + "");
        healthBarSlider.value = Mathf.MoveTowards(healthBarSlider.value, healthComponent.healthPoints, Time.deltaTime * 144f);

        if (canFlashBarMove)
            flashBarSlider.value = Mathf.MoveTowards(flashBarSlider.value, healthComponent.healthPoints, Time.deltaTime * 77f);
    }

    public void TakeDamage()
    {
        StartCoroutine(FlashSlideDelay());
    }

    IEnumerator FlashSlideDelay()
    {
        canFlashBarMove = false;
        yield return new WaitForSeconds(0.5f);
        canFlashBarMove = true;
    }
}
