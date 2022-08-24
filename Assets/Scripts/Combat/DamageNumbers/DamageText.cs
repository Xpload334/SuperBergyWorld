using CodeMonkey.Utils;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private Vector3 _moveVector;
    
    private TextMeshPro _textMesh;
    private float _lifetime;
    private Color _textColor;

    private const float FadetimeMax = 0.5f;

    private static int _sortingOrder;
    
    /*
     * Create a damage text pop up
     */
    public static DamageText Create(Vector3 position, float damage, bool isCrit)
    {

        return Create(position, damage, isCrit, new Vector3(0, 1, 0));
    }
    
    /*
     * Create a damage text pop up
     */
    public static DamageText Create(Vector3 position, float damage, bool isCrit, Vector3 moveVector)
    {
        Transform damageTextTransform = Instantiate(GameAssets.i.damageText, position, Quaternion.Euler(45,0,0));
        DamageText damageText = damageTextTransform.GetComponent<DamageText>();
        damageText.Setup(damage, isCrit, moveVector);

        return damageText;
    }
    
    private void Awake()
    {
        _textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(float damage, bool isCrit, Vector3 moveVector)
    {
        _textMesh.SetText(damage.ToString());
        if (isCrit)
        {
            _textMesh.fontSize = 16f;
            _textMesh.fontStyle = FontStyles.Italic;
            _textColor = UtilsClass.GetColorFromString("EA0F0F");
        }
        else
        {
            _textMesh.fontSize = 12f;
            _textMesh.fontStyle = FontStyles.Normal;
            _textColor = UtilsClass.GetColorFromString("000000");
        }


        _sortingOrder++;
        _textMesh.sortingOrder = _sortingOrder;
        _textMesh.color = _textColor;
        _lifetime = FadetimeMax;
        _moveVector = (moveVector + new Vector3(0, 1, 0)) * 10f;
    }

    private void Update()
    {
        transform.position += _moveVector * Time.deltaTime;
        _moveVector -= _moveVector * 8f * Time.deltaTime;


        if (_lifetime > FadetimeMax * 0.5f)
        {
            //1st half of the popup
            float increaseScale = 1f;
            transform.localScale += Vector3.one * increaseScale * Time.deltaTime;
        }
        else
        {
            //2nd half of the popup
            float decreaseScale = 1f;
            transform.localScale -= Vector3.one * decreaseScale * Time.deltaTime;
        }
        _lifetime -= Time.deltaTime;
        if (_lifetime < 0)
        {
            //Disappear
            float fadeSpeed = 3f;
            _textColor.a -= fadeSpeed * Time.deltaTime;
            _textMesh.color = _textColor;

            if (_textColor.a < 0)
            {
                //destroy
                Destroy(gameObject);
            }
        }
    }
}
