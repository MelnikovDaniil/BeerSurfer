using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TvView : MonoBehaviour
{
    public float elementTime = 2;
    public float swichTime = 0.25f;

    public GameObject noises;

    public GameObject goText;

    [Space(20)]
    public TextMeshPro recordText;
    public TextMeshPro beerText;
    public TextMeshPro beerBonusText;

    private List<GameObject> elements;

    // Start is called before the first frame update
    void Start()
    {
        noises.SetActive(true);
        elements = GenerateElements();
        elements.ForEach(x => x.SetActive(false));
        StartCoroutine(Switching());
    }

    private IEnumerator Switching()
    {
        while (true)
        {
            foreach (var element in elements)
            {
                yield return new WaitForSeconds(swichTime);
                noises.SetActive(false);
                element.SetActive(true);
                yield return new WaitForSeconds(elementTime);
                element.SetActive(false);
                noises.SetActive(true);
            }

            elements = GenerateElements();
        }
    }

    public List<GameObject> GenerateElements()
    {
        var elements = new List<GameObject>();
        var record = RecordMapper.Get();
        var beer = BeerMapper.Get();
        var dobleBeerBonus = DobleBeerBonusMapper.Get();

        if (record == 0)
        {
            elements.Add(goText);
            return elements;
        }
        else
        {
            recordText.text = record.ToString();
            elements.Add(recordText.transform.parent.gameObject);

            beerText.text = beer.ToString();
            elements.Add(beerText.transform.parent.gameObject);

            if (dobleBeerBonus > 0)
            {
                beerBonusText.text = dobleBeerBonus.ToString();
                elements.Add(beerBonusText.transform.parent.gameObject);
            }

            return elements;
        }
    }
}
