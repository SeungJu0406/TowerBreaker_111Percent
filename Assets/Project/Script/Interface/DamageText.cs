using AutoPool_Tool;
using NSJ_MVVM;
using System.Collections;
using TMPro;
using UnityEngine;
using Utility;

public class DamageText : MonoBehaviour
{
    [System.Serializable]
    struct TextStruct
    {
        public float FontSize;
        public float Duration;
    }

   [SerializeField] private TMP_Text _text;
    //private GameObject _critImage;
    [SerializeField] private TextStruct _textStruct;
    private Vector2 _targetPos;

    Coroutine _gfxRoutine;

    private void OnEnable()
    {
        if (_gfxRoutine == null)
        {
            _gfxRoutine = StartCoroutine(GFXRoutine());
        }

    }
    private void OnDisable()
    {
        if (_gfxRoutine != null)
        {
            StopCoroutine(_gfxRoutine);
            _gfxRoutine = null;
        }
    }

    private void Update()
    {
        _text.transform.position = Camera.main.WorldToScreenPoint(_targetPos);
    }

    public void SetDamageText(Transform target, float damage, bool isCritcal)
    {
        //_critImage.SetActive(isCritcal);

        _text.color = isCritcal ? Color.red : Color.white;

        _text.SetText(damage.ToString("F0"));

        _targetPos = new Vector2(
          Random.Range(target.position.x - 0.3f, target.position.x + 0.3f),
          Random.Range(target.position.y - 0.3f, target.position.y + 0.3f)
            );
    }

    IEnumerator GFXRoutine()
    {
        yield return null;
        TextStruct textStruct = _textStruct;

        _text.fontSize = textStruct.FontSize;


        StartCoroutine(MoveUpRoutine());
        yield return textStruct.Duration.Second();

        float aValue = _text.color.a;
        while (true)
        {
            aValue -= Time.deltaTime * 5;
            _text.color = _text.color.GetColor(aValue);
            if (aValue < 0)
                break;
            yield return null;
        }

        ObjectPool.Return(gameObject);
    }
    IEnumerator MoveUpRoutine()
    {
        while (true)
        {
            _targetPos.y += Time.deltaTime / 2;
            yield return null;
        }
    }

}
