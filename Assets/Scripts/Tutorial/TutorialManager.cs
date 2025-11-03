using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private TutorialData tutorialData;
    private UIManager uiManager;
    private int tutorialStepIndex = 0;
    private bool isTutorialActive = false;
    [SerializeField] public Button nextTextButton;



    public void SetTutorialData(TutorialData data)
    {
        tutorialData = data;
        isTutorialActive = true;

        TMP_Text buttonText = nextTextButton.GetComponentInChildren<TMP_Text>();
        LanguageManager languageManager = LanguageManager.Instance;

        switch (languageManager.currentLanguage)
            {
                case "English":
                    languageManager.DisplayEnglishText(buttonText, "Next");
                    break;
                case "Vietnamese":
                    languageManager.DisplayEnglishText(buttonText, "Tiếp");
                    break;
                case "Chinese":
                    languageManager.DisplayChineseText(buttonText, "下一步");
                    break;
                case "Japanese":
                    languageManager.DisplayJapaneseText(buttonText, "次へ");
                    break;
                case "Korean":
                    languageManager.DisplayKoreanText(buttonText, "다음");
                    break;
                case "Spanish":
                    languageManager.DisplayEnglishText(buttonText, "Siguiente");
                    break;
                case "Portuguese":
                    languageManager.DisplayEnglishText(buttonText, "Próximo");
                    break;
                case "French":
                    languageManager.DisplayEnglishText(buttonText, "Suivant");
                    break;
                case "German":
                    languageManager.DisplayEnglishText(buttonText, "Weiter");
                    break;
                case "Russian":
                    languageManager.DisplayEnglishText(buttonText, "Далее");
                    break;
                case "Thai":
                    languageManager.DisplayThaiText(buttonText, "ถัดไป");
                    break;
                default:
                    languageManager.DisplayEnglishText(buttonText, "Next");
                    break;
            }

        nextTextButton.onClick.AddListener(() =>
                                                {
                                                    if (GetTargetType() == TutorialTargetType.Text)
                                                    {
                                                        AudioManager.Instance.PlaySFXButton();
                                                        NextTutorialStep();
                                                    }
                                                });

        nextTextButton.transform.localScale = Vector3.zero;
    }

    // private void Update()
    // {
    //     if (!isTutorialActive) return;
    //     // Nếu ở trạng thái text khi nhấn sẽ chuyển sang bước tiếp theo
    //     if (GetTargetType() != TutorialTargetType.Text) return;

    //     // PC (chuột trái)
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         // Vector3 pos = Input.mousePosition;
    //         NextTutorialStep();
    //     }

    //     // Mobile (chạm màn hình)
    //     if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    //     {
    //         // Vector3 pos = Input.GetTouch(0).position;
    //         NextTutorialStep();
    //     }
    // }

    public void ResetTutorial()
    {
        tutorialStepIndex = 0;
    }
    
    public int GetCurrentStepIndex()
    {
        return tutorialStepIndex;
    }

    public void NextTutorialStep()
    {
        if (tutorialData == null)
        {
            Debug.LogError("Chưa gán TutorialData cho TutorialManager!");
            return;
        }

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("Không tìm thấy UIManager trong cảnh!");
                return;
            }
        }

        if (tutorialStepIndex < tutorialData.steps.Count - 1)
        {
            tutorialStepIndex++;
            uiManager.ShowCurrentTutorial();
        }
        else
        {
            isTutorialActive = false;
            uiManager.HideMaskTutorial(); // Hiển thị bước cuối cùng
            uiManager.HideTextTutorial();
            Debug.LogWarning("Không thể chuyển sang bước tiếp theo. Đã ở bước cuối cùng.");
        }
    }

    public TutorialTargetType GetTargetType()
    {
        if (tutorialData == null)
        {
            Debug.LogError("Chưa gán TutorialData cho TutorialManager!");
            return TutorialTargetType.None;
        }

        if (isTutorialActive == false) return TutorialTargetType.None;

        return tutorialData.steps[tutorialStepIndex].targetType;
    }

    public object GetTargetAttributes()
    {
        var step = tutorialData.steps[tutorialStepIndex];
        switch (step.targetType)
        {
            case TutorialTargetType.Tile:
                return step.tilePos;
            case TutorialTargetType.Animal:
                return step.animalIndex;
            case TutorialTargetType.UIButton:
                return step.buttonName;
            default:
                return null;
        }
    }

    public string GetTutorialText()
    {
        var step = tutorialData.steps[tutorialStepIndex];
        if (step.text_VN.Length < 0) return null;

        return step.text_VN;
    }
}
