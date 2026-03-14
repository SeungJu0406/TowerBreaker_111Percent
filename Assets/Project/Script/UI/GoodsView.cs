using NSJ_MVVM;
using TMPro;

public class GoodsView : BaseView
{
    private TMP_Text _chestText;
    private TMP_Text _skullsText;

    protected override void InitAwake()
    {

    }

    protected override void InitGetUI()
    {
        _chestText =GetUI<TMP_Text>("ChestText");
        _skullsText = GetUI<TMP_Text>("SkullsText");
    }

    protected override void InitStart()
    {
        if (UserDataManager.Instance != null)
        {
            UserDataManager.Instance.OnSkullCountChanged += UpdateSkullCount;
            UserDataManager.Instance.OnChestCountChanged += UpdateChestCount;
            // 초기값 업데이트
            UpdateSkullCount(UserDataManager.Instance.SkullCount);
            UpdateChestCount(UserDataManager.Instance.ChestCount);
        }
    }

    private void OnDestroy()
    {
        if (UserDataManager.Instance != null)
        {
            UserDataManager.Instance.OnSkullCountChanged -= UpdateSkullCount;
            UserDataManager.Instance.OnChestCountChanged -= UpdateChestCount;
        }
    }

    protected override void SubscribeEvents()
    {

    }

    void UpdateSkullCount(int count)
    {
        _skullsText.text = $"{count}";
    }
    void UpdateChestCount(int count)
    {
        _chestText.text = $"{count}";
    }
}
