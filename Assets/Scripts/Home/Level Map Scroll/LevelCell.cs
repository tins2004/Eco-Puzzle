

using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using EasingCore;
using TMPro;
using UnityEngine.SceneManagement;

namespace FancyScrollView.Example09
{
    class LevelCell : FancyCell<ItemData>
    {
        readonly EasingFunction alphaEasing = Easing.Get(Ease.OutQuint);

        [SerializeField] private TMP_Text title = default;
        [SerializeField] private GameObject star = default;
        [SerializeField] private Button playButton = default;
        [SerializeField] private Image background = default;
        [SerializeField] private CanvasGroup canvasGroup = default;
        [SerializeField] private Sprite starEmpty;
        [SerializeField] private Sprite starFilled;

        private HomeManager homeManager;
        ItemData data;

        public override void UpdateContent(ItemData itemData)
        {
            if (homeManager == null)
                homeManager = FindObjectOfType<HomeManager>();

            if (data == null)
                data = itemData;


            for (var i = 0; i < star.transform.childCount; i++)
            {
                var child = star.transform.GetChild(i).GetComponent<Image>();
                if (i < itemData.stars)
                {
                    child.sprite = starFilled;
                }
                else
                {
                    child.sprite = starEmpty;
                }
            }

            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() =>
                                            {
                                                homeManager.audioManager.PlaySFXButton();
                                                // homeManager.CloseLevelMap(0f);

                                                PlayerPrefs.SetInt("CurrentLevel", itemData.levelId);
                                                // SceneManager.LoadScene("Game Scene");
                                                homeManager.sceneTransition.OpenEffect("Game Scene");
                                            });


            UpdateSibling();
        }

        void UpdateSibling()
        {
            var cells = transform.parent.Cast<Transform>()
                .Select(t => t.GetComponent<LevelCell>())
                .Where(cell => cell.IsVisible);

            if (Index == cells.Min(x => x.Index))
            {
                transform.SetAsLastSibling();
            }

            if (Index == cells.Max(x => x.Index))
            {
                transform.SetAsFirstSibling();
            }
        }

        public override void UpdatePosition(float t)
        {
            const float popAngle = -15;
            const float slideAngle = 25;

            const float popSpan = 0.75f;
            const float slideSpan = 0.25f;

            t = 1f - t;

            var pop = Mathf.Min(popSpan, t) / popSpan;
            var slide = Mathf.Max(0, t - popSpan) / slideSpan;

            transform.localRotation = t < popSpan
                ? Quaternion.Euler(0, 0, popAngle * (1f - pop))
                : Quaternion.Euler(0, 0, slideAngle * slide);

            transform.localPosition = Vector3.left * 500f * slide;

            canvasGroup.alpha = alphaEasing(1f - slide);

            background.color = Color.Lerp(Color.gray, Color.white, pop);
        }

        private void OnEnable()
        {
            if (homeManager == null || data == null) return;
            SetLanguageCell();
        }
        
        private void SetLanguageCell()
        {
            TMP_Text buttonText = playButton.GetComponentInChildren<TMP_Text>();

            switch (homeManager.languageManager.currentLanguage)
            {
                case "English":
                    homeManager.languageManager.DisplayEnglishText(title, "Level");
                    homeManager.languageManager.DisplayEnglishText(buttonText, "Play");
                    break;
                case "Vietnamese":
                    homeManager.languageManager.DisplayEnglishText(title, "Màn");
                    homeManager.languageManager.DisplayEnglishText(buttonText, "Chơi");
                    break;
                case "Chinese":
                    homeManager.languageManager.DisplayChineseText(title, "关卡");
                    homeManager.languageManager.DisplayChineseText(buttonText, "玩");
                    break;
                case "Japanese":
                    homeManager.languageManager.DisplayJapaneseText(title, "ステージ");
                    homeManager.languageManager.DisplayJapaneseText(buttonText, "プレイ");
                    break;
                case "Korean":
                    homeManager.languageManager.DisplayKoreanText(title, "단계");
                    homeManager.languageManager.DisplayKoreanText(buttonText, "플레이");
                    break;
                case "Spanish":
                    homeManager.languageManager.DisplayEnglishText(title, "Nivel");
                    homeManager.languageManager.DisplayEnglishText(buttonText, "Jugar");
                    break;
                case "Portuguese":
                    homeManager.languageManager.DisplayEnglishText(title, "Nível");
                    homeManager.languageManager.DisplayEnglishText(buttonText, "Jugar");
                    break;
                case "French":
                    homeManager.languageManager.DisplayEnglishText(title, "Niveau");
                    homeManager.languageManager.DisplayEnglishText(buttonText, "Jouer");
                    break;
                case "German":
                    homeManager.languageManager.DisplayEnglishText(title, "Level");
                    homeManager.languageManager.DisplayEnglishText(buttonText, "Spielen");
                    break;
                case "Russian":
                    homeManager.languageManager.DisplayEnglishText(title, "Уровень");
                    homeManager.languageManager.DisplayEnglishText(buttonText, "Играть");
                    break;
                case "Thai":
                    homeManager.languageManager.DisplayThaiText(title, "ด่าน");
                    homeManager.languageManager.DisplayThaiText(buttonText, "เล่น");
                    break;
                default:
                    homeManager.languageManager.DisplayEnglishText(title, "Level");
                    homeManager.languageManager.DisplayEnglishText(buttonText, "Play");
                    break;
            }

            title.text += " " + data.levelId;
        }
    }
}
