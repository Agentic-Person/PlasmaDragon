# Task 14: Visual Effects and Polish

## Status: Pending

## Priority: LOW - Final polish phase

## Description
Add visual effects, particle systems, UI animations, and overall polish to make the game feel professional and engaging. Focus on feedback, juice, and game feel.

## Prerequisites
- Core gameplay systems complete
- Basic UI implemented
- Performance baseline established

## Step-by-Step Instructions

### 1. Create Plasma Orb Visual Effects
`Assets/_Project/Scripts/Effects/PlasmaOrbVFX.cs`:
```csharp
using UnityEngine;

public class PlasmaOrbVFX : MonoBehaviour
{
    [Header("Core Components")]
    public GameObject innerCore;
    public GameObject outerGlow;
    public TrailRenderer energyTrail;
    public Light orbLight;
    
    [Header("Particles")]
    public ParticleSystem chargingParticles;
    public ParticleSystem travelParticles;
    public ParticleSystem impactParticles;
    
    [Header("Animation")]
    public AnimationCurve pulseCurve;
    public float pulseSpeed = 2f;
    public float rotationSpeed = 180f;
    
    private float pulseTimer;
    private Material innerMaterial;
    private Material outerMaterial;
    
    void Start()
    {
        // Get materials
        innerMaterial = innerCore.GetComponent<Renderer>().material;
        outerMaterial = outerGlow.GetComponent<Renderer>().material;
        
        // Start effects
        if (travelParticles != null)
            travelParticles.Play();
    }
    
    void Update()
    {
        // Pulse effect
        pulseTimer += Time.deltaTime * pulseSpeed;
        float pulseValue = pulseCurve.Evaluate(Mathf.PingPong(pulseTimer, 1f));
        
        // Scale pulsing
        innerCore.transform.localScale = Vector3.one * (0.8f + pulseValue * 0.2f);
        outerGlow.transform.localScale = Vector3.one * (1.2f + pulseValue * 0.3f);
        
        // Light intensity pulsing
        if (orbLight != null)
        {
            orbLight.intensity = 2f + pulseValue * 1f;
            orbLight.range = 10f + pulseValue * 5f;
        }
        
        // Rotation
        innerCore.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        outerGlow.transform.Rotate(Vector3.up, -rotationSpeed * 0.5f * Time.deltaTime);
        
        // Material emission
        if (innerMaterial != null)
        {
            Color emissionColor = Color.Lerp(
                new Color(0.5f, 0.3f, 1f), 
                new Color(1f, 0.6f, 1f), 
                pulseValue
            );
            innerMaterial.SetColor("_EmissionColor", emissionColor * 2f);
        }
    }
    
    public void OnImpact()
    {
        // Stop travel effects
        if (travelParticles != null)
            travelParticles.Stop();
        
        // Play impact
        if (impactParticles != null)
        {
            impactParticles.transform.parent = null;
            impactParticles.Play();
            Destroy(impactParticles.gameObject, 3f);
        }
        
        // Camera shake
        CameraShaker.Instance?.Shake(0.5f, 0.3f);
    }
}
```

### 2. Create Camera Shake System
`Assets/_Project/Scripts/Effects/CameraShaker.cs`:
```csharp
using UnityEngine;
using Cinemachine;

public class CameraShaker : MonoBehaviour
{
    [Header("Shake Settings")]
    public float smoothTime = 0.1f;
    public AnimationCurve shakeFalloff;
    
    private static CameraShaker instance;
    public static CameraShaker Instance => instance;
    
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;
    private float shakeIntensity;
    private float shakeDuration;
    
    void Awake()
    {
        instance = this;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    
    public void Shake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration = duration;
        shakeTimer = duration;
    }
    
    public void ShakeFromPosition(Vector3 position, float maxIntensity, float radius)
    {
        float distance = Vector3.Distance(transform.position, position);
        float falloff = 1f - Mathf.Clamp01(distance / radius);
        
        Shake(maxIntensity * falloff, 0.3f);
    }
    
    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            
            float progress = 1f - (shakeTimer / shakeDuration);
            float currentIntensity = shakeIntensity * shakeFalloff.Evaluate(progress);
            
            noise.m_AmplitudeGain = currentIntensity;
            noise.m_FrequencyGain = currentIntensity * 2f;
        }
        else
        {
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;
        }
    }
}
```

### 3. Create Dragon Flight Effects
`Assets/_Project/Scripts/Effects/DragonFlightVFX.cs`:
```csharp
using UnityEngine;

public class DragonFlightVFX : MonoBehaviour
{
    [Header("Wing Effects")]
    public ParticleSystem leftWingTrail;
    public ParticleSystem rightWingTrail;
    public Gradient normalGradient;
    public Gradient boostGradient;
    
    [Header("Boost Effects")]
    public ParticleSystem boostFlames;
    public GameObject speedLines;
    public Light dragonGlow;
    
    [Header("Audio")]
    public AudioSource flightAudio;
    public AudioClip wingFlapSound;
    public AudioClip boostSound;
    public float basePitch = 1f;
    public float maxPitch = 1.5f;
    
    private DragonFlightController flightController;
    private float targetAudioVolume = 0.5f;
    
    void Start()
    {
        flightController = GetComponent<DragonFlightController>();
    }
    
    void Update()
    {
        if (flightController == null) return;
        
        float speed = flightController.GetCurrentSpeed();
        float speedPercent = flightController.GetSpeedPercentage();
        bool isBoosting = flightController.IsBoosting();
        
        // Wing trails
        UpdateWingTrails(speedPercent, isBoosting);
        
        // Boost effects
        UpdateBoostEffects(isBoosting, speedPercent);
        
        // Audio
        UpdateAudio(speedPercent, isBoosting);
        
        // Dragon glow
        if (dragonGlow != null)
        {
            dragonGlow.intensity = Mathf.Lerp(0.5f, 2f, speedPercent);
            dragonGlow.color = Color.Lerp(Color.white, new Color(0.5f, 0.8f, 1f), speedPercent);
        }
    }
    
    void UpdateWingTrails(float speedPercent, bool isBoosting)
    {
        // Emission rate based on speed
        var leftEmission = leftWingTrail.emission;
        var rightEmission = rightWingTrail.emission;
        
        float emissionRate = Mathf.Lerp(10f, 100f, speedPercent);
        leftEmission.rateOverTime = emissionRate;
        rightEmission.rateOverTime = emissionRate;
        
        // Color gradient
        var leftMain = leftWingTrail.main;
        var rightMain = rightWingTrail.main;
        
        Gradient currentGradient = isBoosting ? boostGradient : normalGradient;
        leftMain.startColor = currentGradient;
        rightMain.startColor = currentGradient;
        
        // Lifetime based on speed
        leftMain.startLifetime = Mathf.Lerp(0.5f, 2f, speedPercent);
        rightMain.startLifetime = Mathf.Lerp(0.5f, 2f, speedPercent);
    }
    
    void UpdateBoostEffects(bool isBoosting, float speedPercent)
    {
        // Boost flames
        if (boostFlames != null)
        {
            if (isBoosting && !boostFlames.isPlaying)
            {
                boostFlames.Play();
                PlayBoostSound();
            }
            else if (!isBoosting && boostFlames.isPlaying)
            {
                boostFlames.Stop();
            }
        }
        
        // Speed lines
        if (speedLines != null)
        {
            speedLines.SetActive(speedPercent > 0.7f);
            
            if (speedLines.activeSelf)
            {
                // Adjust speed line intensity
                var renderer = speedLines.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Color color = renderer.material.color;
                    color.a = Mathf.Lerp(0f, 0.5f, (speedPercent - 0.7f) / 0.3f);
                    renderer.material.color = color;
                }
            }
        }
    }
    
    void UpdateAudio(float speedPercent, bool isBoosting)
    {
        if (flightAudio == null) return;
        
        // Volume based on speed
        targetAudioVolume = Mathf.Lerp(0.3f, 1f, speedPercent);
        flightAudio.volume = Mathf.Lerp(flightAudio.volume, targetAudioVolume, Time.deltaTime * 5f);
        
        // Pitch based on speed
        float targetPitch = Mathf.Lerp(basePitch, maxPitch, speedPercent);
        if (isBoosting) targetPitch += 0.2f;
        
        flightAudio.pitch = Mathf.Lerp(flightAudio.pitch, targetPitch, Time.deltaTime * 3f);
    }
    
    public void PlayWingFlap()
    {
        if (wingFlapSound != null)
        {
            AudioSource.PlayClipAtPoint(wingFlapSound, transform.position, 0.7f);
        }
    }
    
    void PlayBoostSound()
    {
        if (boostSound != null)
        {
            AudioSource.PlayClipAtPoint(boostSound, transform.position);
        }
    }
}
```

### 4. Create Combat Hit Effects
`Assets/_Project/Scripts/Effects/CombatEffects.cs`:
```csharp
using UnityEngine;
using DG.Tweening; // If using DOTween

public class CombatEffects : MonoBehaviour
{
    [Header("Hit Effects")]
    public GameObject hitSparkPrefab;
    public GameObject bloodPrefab;
    public GameObject shieldHitPrefab;
    
    [Header("Death Effects")]
    public GameObject deathExplosionPrefab;
    public float dissolveTime = 1.5f;
    
    [Header("Materials")]
    public Material dissolveMaterial;
    
    public static void PlayHitEffect(Vector3 position, Vector3 normal, HitType type)
    {
        GameObject effectPrefab = null;
        
        switch (type)
        {
            case HitType.Metal:
                effectPrefab = Resources.Load<GameObject>("Effects/HitSpark");
                break;
            case HitType.Flesh:
                effectPrefab = Resources.Load<GameObject>("Effects/BloodHit");
                break;
            case HitType.Shield:
                effectPrefab = Resources.Load<GameObject>("Effects/ShieldHit");
                break;
        }
        
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, 
                Quaternion.LookRotation(normal));
            Destroy(effect, 2f);
        }
    }
    
    public void PlayDeathEffect()
    {
        // Explosion
        if (deathExplosionPrefab != null)
        {
            Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
        }
        
        // Dissolve effect
        StartCoroutine(DissolveEffect());
    }
    
    IEnumerator DissolveEffect()
    {
        // Get all renderers
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        
        // Switch to dissolve material
        foreach (var renderer in renderers)
        {
            Material[] mats = renderer.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = dissolveMaterial;
            }
            renderer.materials = mats;
        }
        
        // Animate dissolve
        float elapsed = 0;
        while (elapsed < dissolveTime)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / dissolveTime;
            
            foreach (var renderer in renderers)
            {
                renderer.material.SetFloat("_DissolveAmount", progress);
            }
            
            yield return null;
        }
        
        Destroy(gameObject);
    }
    
    public enum HitType
    {
        Metal,
        Flesh,
        Shield
    }
}
```

### 5. Create UI Animation System
`Assets/_Project/Scripts/UI/UIAnimator.cs`:
```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public AnimationCurve scaleCurve;
    public AnimationCurve fadeCurve;
    public float animationDuration = 0.3f;
    
    // Damage number popup
    public static void ShowDamageNumber(Vector3 worldPos, float damage, Color color)
    {
        GameObject popup = new GameObject("DamagePopup");
        popup.transform.SetParent(GameObject.Find("WorldCanvas").transform);
        
        Text text = popup.AddComponent<Text>();
        text.text = damage.ToString("0");
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 48;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        
        // Position
        popup.transform.position = worldPos;
        
        // Animate
        Instance.StartCoroutine(Instance.AnimateDamageNumber(popup.transform, text));
    }
    
    IEnumerator AnimateDamageNumber(Transform popup, Text text)
    {
        float elapsed = 0;
        Vector3 startPos = popup.position;
        
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            
            // Move up
            popup.position = startPos + Vector3.up * elapsed * 2f;
            
            // Fade out
            Color color = text.color;
            color.a = 1f - elapsed;
            text.color = color;
            
            // Scale
            popup.localScale = Vector3.one * scaleCurve.Evaluate(elapsed);
            
            yield return null;
        }
        
        Destroy(popup.gameObject);
    }
    
    // Health bar damage flash
    public void FlashHealthBar(Image healthBar)
    {
        StartCoroutine(HealthBarFlash(healthBar));
    }
    
    IEnumerator HealthBarFlash(Image healthBar)
    {
        Color originalColor = healthBar.color;
        
        // Flash white
        healthBar.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        
        // Flash red
        healthBar.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        
        // Return to original
        healthBar.color = originalColor;
    }
    
    // Button hover effects
    public void OnButtonHover(Transform button)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleButton(button, 1.1f));
    }
    
    public void OnButtonExit(Transform button)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleButton(button, 1f));
    }
    
    IEnumerator ScaleButton(Transform button, float targetScale)
    {
        float startScale = button.localScale.x;
        float elapsed = 0;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            float scale = Mathf.Lerp(startScale, targetScale, scaleCurve.Evaluate(t));
            button.localScale = Vector3.one * scale;
            
            yield return null;
        }
    }
    
    // Screen transitions
    public void FadeIn(CanvasGroup group, System.Action onComplete = null)
    {
        StartCoroutine(Fade(group, 0f, 1f, onComplete));
    }
    
    public void FadeOut(CanvasGroup group, System.Action onComplete = null)
    {
        StartCoroutine(Fade(group, 1f, 0f, onComplete));
    }
    
    IEnumerator Fade(CanvasGroup group, float from, float to, System.Action onComplete)
    {
        float elapsed = 0;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            group.alpha = Mathf.Lerp(from, to, fadeCurve.Evaluate(t));
            
            yield return null;
        }
        
        group.alpha = to;
        onComplete?.Invoke();
    }
    
    private static UIAnimator instance;
    public static UIAnimator Instance => instance;
    
    void Awake()
    {
        instance = this;
    }
}
```

### 6. Create Environmental Effects
`Assets/_Project/Scripts/Effects/EnvironmentVFX.cs`:
```csharp
using UnityEngine;

public class EnvironmentVFX : MonoBehaviour
{
    [Header("Atmospheric Effects")]
    public ParticleSystem fogParticles;
    public ParticleSystem dustMotes;
    public WindZone wind;
    
    [Header("Dynamic Sky")]
    public Gradient skyGradient;
    public Light sunLight;
    public Material skyboxMaterial;
    
    [Header("Weather")]
    public ParticleSystem rainSystem;
    public ParticleSystem stormClouds;
    public AudioSource thunderAudio;
    public AudioClip[] thunderSounds;
    
    [Header("Castle Effects")]
    public GameObject[] torches;
    public ParticleSystem[] chimneySmoke;
    public Light[] windowLights;
    
    private float timeOfDay = 0.7f; // Sunset
    
    void Start()
    {
        SetupEnvironment();
    }
    
    void SetupEnvironment()
    {
        // Fog particles
        if (fogParticles != null)
        {
            var main = fogParticles.main;
            main.maxParticles = 500;
            main.startLifetime = 20f;
            main.startSpeed = 1f;
            
            var shape = fogParticles.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(200, 20, 200);
        }
        
        // Dust motes
        if (dustMotes != null)
        {
            var main = dustMotes.main;
            main.maxParticles = 200;
            main.startLifetime = 10f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.5f);
        }
        
        // Setup torches
        foreach (var torch in torches)
        {
            AddTorchEffect(torch);
        }
        
        // Animate castle smoke
        foreach (var smoke in chimneySmoke)
        {
            AnimateSmoke(smoke);
        }
    }
    
    void Update()
    {
        // Animate time of day (optional)
        UpdateLighting();
        
        // Wind variation
        if (wind != null)
        {
            wind.windMain = Mathf.PerlinNoise(Time.time * 0.1f, 0) * 5f + 2f;
            wind.windTurbulence = Mathf.PerlinNoise(0, Time.time * 0.1f) * 2f;
        }
    }
    
    void UpdateLighting()
    {
        // Sun color and intensity
        if (sunLight != null)
        {
            sunLight.color = skyGradient.Evaluate(timeOfDay);
            sunLight.intensity = Mathf.Lerp(0.5f, 1.5f, timeOfDay);
            
            // Sun rotation
            float sunAngle = Mathf.Lerp(15f, 165f, timeOfDay);
            sunLight.transform.rotation = Quaternion.Euler(sunAngle, -30f, 0);
        }
        
        // Update skybox tint
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(0.5f, 1.3f, timeOfDay));
            skyboxMaterial.SetColor("_Tint", skyGradient.Evaluate(timeOfDay));
        }
        
        // Window lights based on time
        bool lightsOn = timeOfDay < 0.3f || timeOfDay > 0.7f;
        foreach (var light in windowLights)
        {
            light.enabled = lightsOn;
        }
    }
    
    void AddTorchEffect(GameObject torch)
    {
        // Add fire particle system
        GameObject fireEffect = new GameObject("FireEffect");
        fireEffect.transform.SetParent(torch.transform);
        fireEffect.transform.localPosition = Vector3.up * 1f;
        
        ParticleSystem fire = fireEffect.AddComponent<ParticleSystem>();
        var main = fire.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 2f;
        main.startSize = 0.5f;
        main.startColor = new Color(1f, 0.5f, 0f);
        
        var emission = fire.emission;
        emission.rateOverTime = 50;
        
        var shape = fire.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 25f;
        shape.radius = 0.1f;
        
        // Add light
        Light torchLight = fireEffect.AddComponent<Light>();
        torchLight.type = LightType.Point;
        torchLight.color = new Color(1f, 0.7f, 0.4f);
        torchLight.intensity = 2f;
        torchLight.range = 10f;
        
        // Flicker
        fireEffect.AddComponent<LightFlicker>();
    }
    
    void AnimateSmoke(ParticleSystem smoke)
    {
        var velocityOverLifetime = smoke.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        
        AnimationCurve xCurve = new AnimationCurve();
        xCurve.AddKey(0f, 0f);
        xCurve.AddKey(0.5f, 2f);
        xCurve.AddKey(1f, -2f);
        
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(1f, xCurve);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(3f);
    }
    
    public void TriggerLightning()
    {
        StartCoroutine(LightningEffect());
    }
    
    IEnumerator LightningEffect()
    {
        // Flash
        RenderSettings.ambientIntensity = 3f;
        sunLight.intensity = 5f;
        
        yield return new WaitForSeconds(0.1f);
        
        // Return to normal
        RenderSettings.ambientIntensity = 1f;
        sunLight.intensity = 1.2f;
        
        // Thunder sound
        if (thunderAudio != null && thunderSounds.Length > 0)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            thunderAudio.clip = thunderSounds[Random.Range(0, thunderSounds.Length)];
            thunderAudio.Play();
        }
    }
}

public class LightFlicker : MonoBehaviour
{
    private Light light;
    private float baseIntensity;
    
    void Start()
    {
        light = GetComponent<Light>();
        baseIntensity = light.intensity;
    }
    
    void Update()
    {
        light.intensity = baseIntensity + Mathf.PerlinNoise(Time.time * 5f, 0) * 0.5f;
    }
}
```

### 7. Create Boss Visual Effects
`Assets/_Project/Scripts/Effects/BossVFX.cs`:
```csharp
using UnityEngine;

public class BossVFX : MonoBehaviour
{
    [Header("Aura Effects")]
    public GameObject commandAura;
    public ParticleSystem powerParticles;
    public Material auraMaterial;
    
    [Header("Attack Effects")]
    public GameObject chargeUpEffect;
    public GameObject shieldBubble;
    public LineRenderer[] attackBeams;
    
    [Header("Phase Transitions")]
    public GameObject phaseChangeEffect;
    public float phaseChangeDuration = 2f;
    
    private EliteCommander boss;
    
    void Start()
    {
        boss = GetComponent<EliteCommander>();
        SetupAura();
    }
    
    void SetupAura()
    {
        // Create command aura
        if (commandAura == null)
        {
            commandAura = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            commandAura.transform.SetParent(transform);
            commandAura.transform.localPosition = Vector3.zero;
            commandAura.transform.localScale = Vector3.one * 5f;
            
            // Remove collider
            Destroy(commandAura.GetComponent<Collider>());
            
            // Add aura material
            var renderer = commandAura.GetComponent<Renderer>();
            renderer.material = auraMaterial;
        }
    }
    
    void Update()
    {
        // Pulse aura based on health
        if (boss != null && commandAura != null)
        {
            float healthPercent = boss.GetHealth() / boss.GetMaxHealth();
            
            // More intense as health drops
            float pulseSpeed = Mathf.Lerp(1f, 3f, 1f - healthPercent);
            float pulseIntensity = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;
            
            // Scale
            commandAura.transform.localScale = Vector3.one * (5f + pulseIntensity);
            
            // Color
            Color auraColor = Color.Lerp(Color.white, Color.red, 1f - healthPercent);
            auraColor.a = 0.3f + pulseIntensity * 0.2f;
            
            var renderer = commandAura.GetComponent<Renderer>();
            renderer.material.SetColor("_Color", auraColor);
        }
    }
    
    public void OnAttackCharge(float chargeTime)
    {
        StartCoroutine(ChargeAttack(chargeTime));
    }
    
    IEnumerator ChargeAttack(float duration)
    {
        GameObject charge = Instantiate(chargeUpEffect, transform.position, Quaternion.identity);
        charge.transform.SetParent(transform);
        
        ParticleSystem particles = charge.GetComponent<ParticleSystem>();
        var main = particles.main;
        main.duration = duration;
        
        // Scale up over time
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            charge.transform.localScale = Vector3.one * t * 2f;
            
            yield return null;
        }
        
        Destroy(charge);
    }
    
    public void ActivateShield()
    {
        if (shieldBubble != null)
        {
            shieldBubble.SetActive(true);
            
            // Animate shield appearance
            StartCoroutine(AnimateShield(true));
        }
    }
    
    public void DeactivateShield()
    {
        if (shieldBubble != null)
        {
            StartCoroutine(AnimateShield(false));
        }
    }
    
    IEnumerator AnimateShield(bool activate)
    {
        var renderer = shieldBubble.GetComponent<Renderer>();
        float elapsed = 0;
        float duration = 0.5f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            if (activate)
            {
                shieldBubble.transform.localScale = Vector3.one * Mathf.Lerp(0f, 6f, t);
                renderer.material.SetFloat("_Alpha", Mathf.Lerp(0f, 0.5f, t));
            }
            else
            {
                shieldBubble.transform.localScale = Vector3.one * Mathf.Lerp(6f, 0f, t);
                renderer.material.SetFloat("_Alpha", Mathf.Lerp(0.5f, 0f, t));
            }
            
            yield return null;
        }
        
        if (!activate)
            shieldBubble.SetActive(false);
    }
}
```

### 8. Create Post-Processing Profile
In Unity:
1. Create Post Process Profile: `Assets/_Project/Settings/PP_MainProfile.asset`
2. Configure effects:

```
Bloom:
- Intensity: 0.5
- Threshold: 1.1
- Soft Knee: 0.5
- Clamp: 65472
- Diffusion: 7
- Anamorphic Ratio: 0

Color Grading:
- Mode: High Definition Range
- Tone Mapping: ACES
- Temperature: 10
- Tint: 0
- Post Exposure: 0.5
- Color Filter: Light orange/pink
- Saturation: 10
- Contrast: 10

Vignette:
- Mode: Classic
- Intensity: 0.3
- Smoothness: 0.5
- Roundness: 1

Motion Blur:
- Shutter Angle: 180
- Sample Count: 10
```

### 9. Create Material Effects
Shader for plasma/energy effects:
```hlsl
Shader "Custom/PlasmaEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionPower ("Emission Power", Float) = 2.0
        _FresnelPower ("Fresnel Power", Float) = 2.0
        _PulseSpeed ("Pulse Speed", Float) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _EmissionColor;
            float _EmissionPower;
            float _FresnelPower;
            float _PulseSpeed;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // Fresnel effect
                float3 viewDir = normalize(i.viewDir);
                float3 worldNormal = normalize(i.worldNormal);
                float fresnel = pow(1.0 - saturate(dot(viewDir, worldNormal)), _FresnelPower);
                
                // Pulse effect
                float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                
                // Emission
                float3 emission = _EmissionColor.rgb * _EmissionPower * (fresnel + pulse);
                
                col.rgb += emission;
                col.a *= fresnel;
                
                return col;
            }
            ENDCG
        }
    }
}
```

### 10. Performance Optimization for Effects
`Assets/_Project/Scripts/Effects/EffectsOptimizer.cs`:
```csharp
using UnityEngine;

public class EffectsOptimizer : MonoBehaviour
{
    [Header("Quality Settings")]
    public int maxParticles = 1000;
    public float effectsDistance = 100f;
    public bool dynamicBatching = true;
    
    private Camera mainCamera;
    private ParticleSystem[] allParticleSystems;
    
    void Start()
    {
        mainCamera = Camera.main;
        allParticleSystems = FindObjectsOfType<ParticleSystem>();
        
        ApplyOptimizations();
    }
    
    void ApplyOptimizations()
    {
        // Limit particles based on platform
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            foreach (var ps in allParticleSystems)
            {
                var main = ps.main;
                main.maxParticles = Mathf.Min(main.maxParticles, maxParticles / 2);
            }
        }
    }
    
    void Update()
    {
        // LOD for particle systems
        foreach (var ps in allParticleSystems)
        {
            if (ps == null) continue;
            
            float distance = Vector3.Distance(ps.transform.position, 
                mainCamera.transform.position);
            
            if (distance > effectsDistance)
            {
                if (ps.isPlaying) ps.Stop();
            }
            else
            {
                if (!ps.isPlaying && ps.main.loop) ps.Play();
            }
        }
    }
}
```

## Expected Outcomes
- ✅ Plasma orbs have impressive visual trail and glow
- ✅ Camera shake adds impact to explosions
- ✅ Dragon flight feels powerful with effects
- ✅ UI animations are smooth and responsive
- ✅ Environment feels alive and atmospheric
- ✅ Boss encounters have dramatic visual flair
- ✅ Performance remains stable with all effects

## Polish Checklist
- [ ] All weapons have muzzle flash
- [ ] All impacts have particles
- [ ] All enemies have death effects
- [ ] UI transitions are smooth
- [ ] Audio has proper 3D positioning
- [ ] Lighting creates mood
- [ ] Post-processing enhances visuals
- [ ] Performance stays above target

## Common Issues & Solutions

### Issue: Too Many Particles Tank FPS
- Reduce max particles
- Use simpler shaders
- Implement LOD system
- Pool particle systems

### Issue: Effects Look Cheap
- Add glow/bloom
- Use HDR colors
- Layer multiple effects
- Add screen shake

### Issue: UI Feels Static
- Add hover states
- Implement transitions
- Use easing curves
- Add subtle animations

## Time Estimate: 6-8 hours

## Next Steps
Proceed to Task 15: Performance Optimization

---

## Completion Notes
*To be filled after task completion*

**Date Completed**: 
**Time Taken**: 
**Effects Added**: 
**Performance Impact**: 
**Visual Quality**: 
**Player Feedback**: 