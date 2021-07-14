using System;
using System.Collections.Generic;
//using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CanvasRenderer), typeof(ParticleSystem))]
public class UIParticleSystem : MaskableGraphic
{

    [Space]
   
    //[FoldoutGroup("UIParticleSystem")]
    public Texture ParticleImage;
    //[FoldoutGroup("UIParticleSystem")]
    public UIParticlesSortMode SortMode;
    //[FoldoutGroup("UIParticleSystem")]
    public UIParticleSystemRenderMode RenderMode;
    //[FoldoutGroup("UIParticleSystem")]
    public ParticleSystemAnimationMode AnimationMode;
   
    //[FoldoutGroup("UIParticleSystem")]
    public float SpeedScale = 0f;
    //[FoldoutGroup("UIParticleSystem")]
    public float LengthScale = 1f;

    //[FoldoutGroup("UIParticleSystem")]
    public Vector3 Pivot = Vector3.zero;
    //[FoldoutGroup("UIParticleSystem")]
    public bool ScaleShapeByRectTransform;
   //[FoldoutGroup("UIParticleSystem")]
    public bool UpdateMaterialDirty;

    private bool _initialized;
    private RectTransform _rectTransform;

    private ParticleSystem _particleSystem;
    private ParticleSystemRenderer _particleSystemRenderer;

    // отдельные модули системы частиц, для удобства
    private ParticleSystem.MainModule _main;
    private ParticleSystem.TextureSheetAnimationModule _textureSheetAnimation;
    private ParticleSystem.ShapeModule _shape;

    // частицы
    private ParticleSystem.Particle[] _particles;
    private float[] _particlesLifeProgress;
    private float[] _particlesSpeedProgress;
    private float[] _particleElapsedLifetime;

    private Vector3[] _quadCorners = 
    {
        new Vector3(-.5f, -.5f, 0),
        new Vector3(-.5f, .5f, 0),
        new Vector3(.5f, .5f, 0),
        new Vector3(.5f, -.5f, 0)
    };

    private Vector2[] _simpleUV = 
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),
    };


    public override Texture mainTexture => ParticleImage;


    protected void Initialize()
    {
        if (_initialized)
            return;
        _initialized = true;

        _rectTransform = GetComponent<RectTransform>();
        _particleSystem = GetComponent<ParticleSystem>();

        _main = _particleSystem.main;
        _textureSheetAnimation = _particleSystem.textureSheetAnimation;
        _shape = _particleSystem.shape;

        _particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
        _particleSystemRenderer.enabled = false;
        _particleSystemRenderer.material = null;

        int maxCount = _main.maxParticles;
        _particles = new ParticleSystem.Particle[maxCount];

        _particlesLifeProgress = new float[maxCount];
        _particleElapsedLifetime = new float[maxCount];
        _particlesSpeedProgress = new float[maxCount];
    }

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ScaleShape();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
#if UNITY_EDITOR
        Initialize();
#endif

        vh.Clear();
        var particlesCount = _particleSystem.GetParticles(_particles);
        for (var i = 0; i < particlesCount; i++)
        {
            _particlesLifeProgress[i] = 1f - _particles[i].remainingLifetime / _particles[i].startLifetime;
            _particleElapsedLifetime[i] = _particles[i].startLifetime - _particles[i].remainingLifetime;
            var maxVelocity = Mathf.Abs(_main.startSpeed.constantMax-_main.startSpeed.constantMin);
            var velocity = Mathf.Abs(_particles[i].velocity.magnitude - _main.startSpeed.constantMin);
            _particlesSpeedProgress[i] = velocity/maxVelocity;
        }

        switch (SortMode)
        {
            case UIParticlesSortMode.None: break;
            case UIParticlesSortMode.OldestInFront:
                Array.Sort(_particleElapsedLifetime, _particles, 0, particlesCount, Comparer<float>.Default);
                Array.Reverse(_particles, 0, particlesCount);
                break;
            case UIParticlesSortMode.YoungestInFront:
                Array.Sort(_particleElapsedLifetime, _particles, 0, particlesCount, Comparer<float>.Default);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

       

        var quadVerts = new UIVertex[4];
        Vector2[] vertexUV ;
        for (var i = 0; i < particlesCount; i++)
        {
            var particle = _particles[i];

            // Position
            Vector3 particlePosition;
            switch (_main.simulationSpace)
            {
                case ParticleSystemSimulationSpace.World:
                    particlePosition = _rectTransform.InverseTransformPoint(particle.position);
                    break;
                case ParticleSystemSimulationSpace.Local:
                    particlePosition = particle.position;
                    break;
                case ParticleSystemSimulationSpace.Custom:
                    if (_main.customSimulationSpace != null)
                        particlePosition =
                            _rectTransform.InverseTransformPoint(
                                _main.customSimulationSpace.TransformPoint(particle.position));
                    else
                        particlePosition = particle.position;
                    break;
                default:
                    particlePosition = particle.position;
                    break;
            }

            // Color
            var vertexColor = particle.GetCurrentColor(_particleSystem) * color;

            // Size
            var particleSize = particle.GetCurrentSize3D(_particleSystem);


            // UV & Animation
          
            if (_textureSheetAnimation.enabled)
            {
               
                var animationFramesCount = _textureSheetAnimation.numTilesX * _textureSheetAnimation.numTilesY;
                var animationFrameSize =
                    new Vector2(1f / _textureSheetAnimation.numTilesX, 1f / _textureSheetAnimation.numTilesY);
 
                float frameProgress = 0;
                int frame =0;
                
                if (AnimationMode == ParticleSystemAnimationMode.Grid)
                    frameProgress = _textureSheetAnimation.frameOverTime.Evaluate(_particlesLifeProgress[i]);
                else if (AnimationMode == ParticleSystemAnimationMode.Sprites)
                    frameProgress = _particlesSpeedProgress[i];
                
                frameProgress = Mathf.Repeat(frameProgress * _textureSheetAnimation.cycleCount, 1);

                switch (_textureSheetAnimation.animation)
                {
                    case ParticleSystemAnimationType.WholeSheet:
                        frame = Mathf.FloorToInt(frameProgress * animationFramesCount);
                        break;
                    case ParticleSystemAnimationType.SingleRow:
                        frame = Mathf.FloorToInt(frameProgress * _textureSheetAnimation.numTilesX);
                        frame += _textureSheetAnimation.rowIndex * _textureSheetAnimation.numTilesX;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            

                frame %= animationFramesCount;

                Vector4 particleUv;


                //particleUV.x = (frame % _textureSheetAnimation.numTilesX) * animationFrameSize.x;
                //particleUV.y = Mathf.FloorToInt(frame / _textureSheetAnimation.numTilesX) * animationFrameSize.y;
                //particleUV.z = particleUV.x + animationFrameSize.x;
                //particleUV.w = particleUV.y + animationFrameSize.y;
                //vertexUV = RectToUV(particleUV);

                // reverse uv
                particleUv.x = (frame % _textureSheetAnimation.numTilesX) * animationFrameSize.x;
                particleUv.y = 1 - Mathf.FloorToInt(frame / _textureSheetAnimation.numTilesX) * animationFrameSize.y;
                particleUv.z = particleUv.x + animationFrameSize.x;
                particleUv.w = particleUv.y - animationFrameSize.y;
                vertexUV = RectToUVFlipV(particleUv);
                
            }
            
            else
                vertexUV = _simpleUV;


            Quaternion rotation;
            switch (RenderMode)
            {
                case UIParticleSystemRenderMode.Billboard:
                    rotation = Quaternion.AngleAxis(particle.rotation, Vector3.forward);
                    break;
                case UIParticleSystemRenderMode.StretchedBillboard:
                    rotation = Quaternion.LookRotation(Vector3.forward, particle.totalVelocity);
                    var speed = particle.totalVelocity.magnitude;
                    particleSize = Vector3.Scale(particleSize, new Vector3(LengthScale + speed * SpeedScale, 1f, 1f));
                    rotation *= Quaternion.AngleAxis(90, Vector3.forward);
                    break;
                default:
                    rotation = Quaternion.AngleAxis(particle.rotation, Vector3.forward);
                    break;
            }

            for (var j = 0; j < 4; j++)
            {
                var cornerPosition = Vector3.Scale(particleSize, _quadCorners[j] + Pivot);
                var vertexPosition = rotation * cornerPosition + particlePosition;
                vertexPosition.z = 0;

                quadVerts[j] = new UIVertex {color = vertexColor, uv0 = vertexUV[j], position = vertexPosition};
            }

            vh.AddUIVertexQuad(quadVerts);
        }
    }

    protected void Update()
    {
        SetVerticesDirty();
        if (UpdateMaterialDirty)
            SetMaterialDirty();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        Initialize();
        base.OnRectTransformDimensionsChange();
        ScaleShape();
    }

    protected void ScaleShape()
    {
        if (!ScaleShapeByRectTransform)
            return;

        var rect = _rectTransform.rect;
        var scale = Quaternion.Euler(_shape.rotation) * new Vector3(rect.width, rect.height, 0);
        scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
        _particleSystem.shape.scale.Set(scale.x, scale.y, scale.z); 
    }


    private Vector2[] RectToUV(Vector4 uvRect)
    {
        return new[]
        {
            new Vector2(uvRect.x, uvRect.y),
            new Vector2(uvRect.x, uvRect.w),
            new Vector2(uvRect.z, uvRect.w),
            new Vector2(uvRect.z, uvRect.y),
        };
    }

    // if uvRect.w < uvRect.y
    private Vector2[] RectToUVFlipV(Vector4 uvRect)
    {
        return new[]
        {
            new Vector2(uvRect.x, uvRect.w),
            new Vector2(uvRect.x, uvRect.y),
            new Vector2(uvRect.z, uvRect.y),
            new Vector2(uvRect.z, uvRect.w),
        };
    }
}

public enum UIParticlesSortMode
{
    None = 0,
    OldestInFront = 1,
    YoungestInFront = 2
}

public enum UIParticleSystemRenderMode
{
    Billboard = 0,
    StretchedBillboard = 1
}