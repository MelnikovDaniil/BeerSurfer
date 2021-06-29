using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Extension;

public class BeerManager : MonoBehaviour
{
    public Character character;
    public ParticleSystem beerParticlePrefab;
    public int poolParticlesCount;

    private List<ParticleSystem> particlesPool;

    public void Start()
    {
        var beerParticlesPool = new GameObject("beerParticlesPool")
        {
            hideFlags = HideFlags.HideAndDontSave
        };

        particlesPool = new List<ParticleSystem>();

        for (var i = 0; i < poolParticlesCount; i++)
        {
            particlesPool.Add(Instantiate(beerParticlePrefab, beerParticlesPool.transform));
        }

        character.OnBeerPickUpEvent += BeerPickUp;
    }

    private void BeerPickUp(BeerView beer)
    {
        GameManager.beer++;
        SoundManager.PlaySound("beer");
        beer.Disable();

        var particle = particlesPool.FirstOrDefault(x => !x.isPlaying);
        if (particle == null)
        {
            Debug.LogWarning("Not enough beer paricles in pool");
            particle = particlesPool.First();
        }
        particle.transform.position = beer.transform.position;
        particle.transform.parent = beer.transform.FindParentWithTag("Road");
        particle.Play();

        Destroy(beer.gameObject, 1);
    }

}
