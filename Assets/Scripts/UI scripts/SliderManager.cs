using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public Sprite leftRoundedBackground;
    public Sprite rightRoundedBackground;
    public Sprite squareBackground;

    public Sprite leftRoundedFill;
    public Sprite rightRoundedFill;
    public Sprite squareFill;

    public Slider templateSlider;
    public List<Slider> sliders = new List<Slider>();

    public Color defaultBackgroundColor;
    public Color defaultFillColor;

    [Range(0f, 1f)]
    public float masterValue;

    private float maxLength;

    private void Update(){
        SetSliderValue(masterValue);
    }

    public void ClearSliders(){
        foreach(Slider slider in sliders){
            Destroy(slider.gameObject);
        }

        sliders.Clear();
    }

    public void CreateSlider(int sliderCount, float length){
        maxLength = length;

        // Instantiate slider and set size, pos.
        for(int i = 0; i < sliderCount; i++){
            Slider slider = Instantiate(templateSlider, transform);
            slider.transform.Find("Background").GetComponent<Image>().color = defaultBackgroundColor;
            slider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = defaultFillColor;

            RectTransform rt = slider.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(length / sliderCount, rt.sizeDelta.y);
            rt.anchoredPosition = new Vector2((length / sliderCount) * i - length / sliderCount, rt.anchoredPosition.y);

            sliders.Add(slider);
        }

        // Set the image sprites accordingly.
        if (sliderCount == 2){
            sliders[0].transform.Find("Background").GetComponent<Image>().sprite = leftRoundedBackground;
            sliders[0].transform.Find("Fill Area").Find("Fill").GetComponent<Image>().sprite = leftRoundedFill;
            sliders[1].transform.Find("Background").GetComponent<Image>().sprite = rightRoundedBackground;
            sliders[1].transform.Find("Fill Area").Find("Fill").GetComponent<Image>().sprite = rightRoundedFill;
        } else if (sliderCount >= 3){
            sliders[0].transform.Find("Background").GetComponent<Image>().sprite = leftRoundedBackground;
            sliders[0].transform.Find("Fill Area").Find("Fill").GetComponent<Image>().sprite = leftRoundedFill;

            for (int i = 1; i < sliderCount - 1; i++){
                sliders[i].transform.Find("Background").GetComponent<Image>().sprite = squareBackground;
                sliders[i].transform.Find("Fill Area").Find("Fill").GetComponent<Image>().sprite = squareFill;
            }

            sliders[sliderCount - 1].transform.Find("Background").GetComponent<Image>().sprite = rightRoundedBackground;
            sliders[sliderCount - 1].transform.Find("Fill Area").Find("Fill").GetComponent<Image>().sprite = rightRoundedFill;
        }
    }

    public void SetSliderLengthPercent(int sliderId, float percent, bool onlyAffectRight = false){
        // Modify slider length.
        RectTransform rt = sliders[sliderId].GetComponent<RectTransform>();
        float oldLength = rt.rect.width;
        rt.sizeDelta = new Vector2(percent * maxLength, rt.sizeDelta.y);
        float deltaLength = oldLength - rt.rect.width;

        // Adjust position based on delta.
		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - deltaLength / 2, rt.anchoredPosition.y);

        RectTransform prevRt = sliders[0].GetComponent<RectTransform>();

        // Split amount changed to other sliders.
        // Formula: previous x + previous size/2 + size/2.
        int startChangeAt = 0;
        if (onlyAffectRight && sliderId != 0){
            startChangeAt = sliderId;
            prevRt = sliders[startChangeAt-1].GetComponent<RectTransform>();
        }

        for(int i = startChangeAt; i < sliders.Count; i++){
            RectTransform rt2 = sliders[i].GetComponent<RectTransform>();

            if (i == sliderId){
                prevRt = rt2;
                continue;
            }

			rt2.sizeDelta = new Vector2(rt2.sizeDelta.x + deltaLength / (sliders.Count - startChangeAt - 1), rt2.sizeDelta.y);
            rt2.anchoredPosition = new Vector2(prevRt.anchoredPosition.x + (prevRt.sizeDelta.x / 2) + (rt2.sizeDelta.x / 2), rt2.anchoredPosition.y);

            prevRt = rt2;
        }
    }

    public void SetSliderValue(float value){
        float lengthToFill = value * maxLength;

        // Calculate length of slider, remove from lengtoToFill. Rinse and repeat.
        for(int i = 0; i < sliders.Count; i++){
            Slider slider = sliders[i];

            RectTransform rt = slider.GetComponent<RectTransform>();
            float length = rt.sizeDelta.x;

            if (lengthToFill - length > 0){
                slider.value = 1f;
            } else {
                slider.value = lengthToFill / length;
            }

            lengthToFill -= length;
        }
    }

    public void SetSliderFillColor(int sliderId, Color color){
        sliders[sliderId].transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = color;
    }

    public void SetSliderFillBackgroundColor(int sliderId, Color color){
        sliders[sliderId].transform.Find("Background").GetComponent<Image>().color = color;
    }
}
