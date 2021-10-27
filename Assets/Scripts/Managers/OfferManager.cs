using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfferManager : MonoBehaviour
{
    public static OfferManager Instance;
    public static bool offerWindowShowing;
    public OutfitManager offerSkin;
    public OutfitManager character;

    [Space(20)]
    public int temporaryOutfitDuration = 3;
    public int nextOffer = 10;

    private Animator animator;
    private string kitName;
    
    // Start is called before the first frame update
    public void SetUp()
    {
        Instance = this;
        var leftToOffer = OfferMapper.OfferLeft();
        var duration = OutfitMapper.GetTempraryDuration();
        Debug.Log("Left to offer: " + leftToOffer.ToString());
        Debug.Log("Duration of temprory: " + duration.ToString());

        if (duration > 0)
        {
            OutfitMapper.SetTempraryDuration(duration - 1);
        }
        else
        {
            character.RemoveTemproryKit();
            character.ResetOutfits();
        }

        if (leftToOffer > 0)
        {
            OfferMapper.SetNextOfferFrom(leftToOffer - 1);
        }
        else
        {
            offerWindowShowing = true;
            animator = GetComponent<Animator>();
            OfferMapper.SetNextOfferFrom(nextOffer);
            gameObject.SetActive(true);
            kitName = offerSkin.GenerateRandomOutfitKit();
        }
    }

    public void AcceptOffer()
    {
        character.SetUpKit(kitName, temporaryOutfitDuration);
        character.ResetOutfits();

        offerWindowShowing = false;
        gameObject.SetActive(false);
    }

    public void CancelOffer()
    {
        offerWindowShowing = false;
        gameObject.SetActive(false);
    }
}
