using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EAController2 : MonoBehaviour
{
    public bool AutoStart;
    public Experiment Experiment;

    public ImageComparer Comparer;

    private GeneticAlgorithm m_ga;

    public RenderTexture StartingPoint;

    public RenderTexture SourceRT;
    public RenderTexture Canvas;
    public RenderTexture Candidate;
    public RenderTexture BestCandidate;

    public Action<string> Finished;

    public RenderTexture Buffer;

    Vector4[] shapeData;
    Vector3[] colorData;
    int[] brushData;

    ComputeBuffer shapeBuffer;
    ComputeBuffer colorBuffer;
    ComputeBuffer brushBuffer;

    Texture2D TexBuffer;

    private EAChromosome chromosomeTemplate;


    // brush setting
    public Material BrushMaterial;
    private float _brushToCanvasFactor;

    private bool _restarted = false;

    private int _maxIterations;

    public RenderTexture _blackClear;

    private int _iterations;

    private DateTime _startTime;
    private string _folder;
    private string _outputRoot = "D:/Projects/EvolvingArtistUnity/Output";

    private Logger _logger;

    private float _currentScale;

    public string LogFolder;

    [ContextMenu("Replay")]
    public void ReplayLog()
    {
        var ienumerator = _logger.Replay(LogFolder, Experiment);
       ;
        Experiment.StartNewIteration(_maxIterations - _iterations);
        Graphics.Blit(BlitChromosome(ienumerator.First()), BestCandidate);
    }

    [ContextMenu("Fill SourceRT")]
    public void FillSource()
    {
        Graphics.Blit(Experiment.Source, SourceRT);
    }
    // Start is called before the first frame update
    public void Start()
    {
        BrushMaterial.SetTexture("_Brush", Experiment.BrushPack);
        Setup();
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Run();
        }
        if (m_ga.State == GeneticAlgorithmState.Stopped && !_restarted)
        {
            try
            {
                m_ga.MutationProbability = Mathf.Max(0.2f, m_ga.MutationProbability * 0.9f);
                _restarted = true;
                m_ga.Resume();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                StartNewRound();
            }
        }

    }

    private void OnDestroy()
    {
        _logger.Dispose();
        
        shapeBuffer.Dispose();
        colorBuffer.Dispose();
        brushBuffer.Dispose();

    }

    public void Setup()
    {
        _startTime = DateTime.Now;
        _folder = $"{Experiment.Name}-{_startTime.Year}{_startTime.Month}{_startTime.Day}-{_startTime.Hour}{_startTime.Minute}";
        Directory.CreateDirectory($"{_outputRoot}/{_folder}");
        _logger = new Logger($"{_outputRoot}/{_folder}");


        _maxIterations = Experiment.Iterations;
        _iterations = Experiment.Iterations;

        _brushToCanvasFactor = (float)Canvas.width / (float)Experiment.BrushPack.width;

        Graphics.Blit(Experiment.Source, SourceRT);

        shapeData = new Vector4[Experiment.StrokesPerGeneration];
        colorData = new Vector3[Experiment.StrokesPerGeneration];
        brushData = new int[Experiment.StrokesPerGeneration];

        shapeBuffer = new ComputeBuffer(Experiment.StrokesPerGeneration, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));
        colorBuffer = new ComputeBuffer(Experiment.StrokesPerGeneration, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3)));
        brushBuffer = new ComputeBuffer(Experiment.StrokesPerGeneration, System.Runtime.InteropServices.Marshal.SizeOf(typeof(int)));


        chromosomeTemplate = new EAChromosome(Experiment.StrokesPerGeneration, Experiment.GetGeneCount());
        var population = new Population(50, 100, chromosomeTemplate);



        var fitness = new FuncFitness((c) =>
        {
            var fc = c as EAChromosome;

            return (1 - Comparer.Compare(BlitChromosome(fc), SourceRT));
        });

        var selection = new TournamentSelection();

        var crossover = new UniformCrossover(0.5f);

        var mutation = new FlipBitMutation();
        //  var mutation = new DisplacementMutation();

        var termination = new FitnessStagnationTermination(20);

        ClearRT(_blackClear, Color.black);
        ClearRT(Canvas, Color.white);
        ClearRT(Candidate, Experiment.CanvasColor);


        Comparer.CalcMaxValue(Canvas, _blackClear);


        TexBuffer = new Texture2D(Canvas.width, Canvas.height, TextureFormat.RGB24, false);
        if (StartingPoint == null)
        {
            ClearRT(Canvas, Experiment.CanvasColor);
        }
        else
        {
            Graphics.Blit(StartingPoint, Canvas);
        }

        m_ga = new GeneticAlgorithm(
   population,
   fitness,
   selection,
   crossover,
   mutation);

        m_ga.Termination = termination;
        m_ga.MutationProbability = 0.9f;

         
        m_ga.GenerationRan += (s, e) =>
        {
            Experiment.StartNewGeneration(m_ga.GenerationsNumber);
  
            if (m_ga.GenerationsNumber % 50 == 0 && !_restarted)
            {
                Debug.Log($"Iteration: {_maxIterations-_iterations} - Generation: {m_ga.GenerationsNumber} - Fitness: {m_ga.BestChromosome.Fitness} - Mutation: {m_ga.MutationProbability} - Scale: {_currentScale}");
                BlitChromosome(m_ga.BestChromosome as EAChromosome);
                Graphics.Blit(Candidate, BestCandidate);
                m_ga.Stop();
            }
            else
                _restarted = false;
        };

        m_ga.TerminationReached += (s, e) =>
        {
            _restarted = false;
            StartNewRound();
        };


        if (AutoStart)
            Run();
    }

    public void StartNewRound()
    {
        Debug.Log(Statistics.Instance.GetBuckets());
        Statistics.Instance.ClearBuckets();
        if (_iterations < 0)
        {
            _restarted = true;
            return;
        }

   

        Graphics.Blit(BlitChromosome(m_ga.BestChromosome as EAChromosome), Canvas);
        _logger.Log(m_ga.BestChromosome as EAChromosome);
        _iterations -= 1;

        Experiment.StartNewIteration(_maxIterations - _iterations);

        Buffer = RenderTexture.active;
        RenderTexture.active = Canvas;

        TexBuffer.Resize(Canvas.width, Canvas.height);
        TexBuffer.ReadPixels(new Rect(0, 0, Canvas.width, Canvas.height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes;
        bytes = TexBuffer.EncodeToPNG();

        System.IO.File.WriteAllBytes($"{_outputRoot}/{_folder}/final-{_maxIterations - _iterations}.png", bytes);



        RenderTexture.active = Buffer;
        if (_iterations > 0)
        {         
            Run();
        }
        else
        {
            Finished?.Invoke($"{_outputRoot}/{_folder}");
        }
    }

    [ContextMenu("Run GA")]
    public void Run()
    {
        Experiment.StartNewIteration(_maxIterations - _iterations);
        m_ga.Termination = new FitnessStagnationTermination(20);
        m_ga.MutationProbability = 0.9f;
        m_ga.Start();
    }

    public RenderTexture BlitChromosome(EAChromosome c)
    {

        var values = c.ToFloatingPoints();


        Experiment.ParseValues(ref values, ref shapeData, ref colorData, ref brushData);
        int helper = 0;
        _currentScale = Experiment.BaseSize.Get(ref values, ref helper); // TODO: This is not so nice
        _currentScale *= _brushToCanvasFactor;
        var transparency = Experiment.BrushTransparency.Get(ref values, ref helper); // TODO: This is not so nice

        shapeBuffer.SetData(shapeData);
        colorBuffer.SetData(colorData);
        brushBuffer.SetData(brushData);

        BrushMaterial.SetBuffer("shapeBuffer", shapeBuffer);
        BrushMaterial.SetBuffer("colorBuffer", colorBuffer);
        BrushMaterial.SetBuffer("brushBuffer", brushBuffer);
        BrushMaterial.SetInt("_StrokeCount", Experiment.StrokesPerGeneration);
        BrushMaterial.SetFloat("_BrushBaseSize", _currentScale);
        BrushMaterial.SetFloat("_BrushAlpha", transparency);

        Graphics.Blit(_blackClear, Candidate);
        Graphics.Blit(Canvas, Candidate, BrushMaterial);


        return Candidate;
    }

    public void ClearRT(RenderTexture Target, Color c)
    {
        Buffer = UnityEngine.RenderTexture.active;
        UnityEngine.RenderTexture.active = Target;
        GL.Clear(true, true, c);
        UnityEngine.RenderTexture.active = Buffer;
        Buffer = null;
    }

    [ContextMenu("Performance")]
    public void PerformanceTest()
    {
        Setup();

        var chromosome = new EAChromosome(Experiment.StrokesPerGeneration, Experiment.GetGeneCount());

        var sw = new Stopwatch();
        int iter = 10000;
        sw.Start();
        for (int i = 0; i < iter; i++)
        {
            BlitChromosome(chromosome);
        }
        sw.Stop();
        UnityEngine.Debug.Log($"{iter} blit took {sw.Elapsed} ");


        sw.Restart();
        for (int i = 0; i < iter; i++)
        {
            Comparer.Compare(SourceRT, SourceRT);
        }
        sw.Stop();
        UnityEngine.Debug.Log($"{iter} cmp took {sw.Elapsed} ");
    }




    [ContextMenu("DebugBlit")]
    public void DebugBlit()
    {
        Setup();
        BrushMaterial.SetTexture("_Brush", Experiment.BrushPack);
        for (int i = 0; i < Experiment.StrokesPerGeneration; i++)
        {
            shapeData[i] = new Vector4(
                0.5f, 0.5f, 0.25f*360f , 0.5f); //  { xy = position, z = rotation, w = scale}
            colorData[i] = new Vector4(
               1, 1, 0,  0); // color
        }

        shapeBuffer.SetData(shapeData);
        colorBuffer.SetData(colorData);

        BrushMaterial.SetBuffer("shapeBuffer", shapeBuffer);
        BrushMaterial.SetBuffer("colorBuffer", colorBuffer);
        BrushMaterial.SetInt("_StrokeCount", Experiment.StrokesPerGeneration);
        BrushMaterial.SetFloat("_BrushBaseSize", 1f);

        //  ClearRT(Candidate, Color.black);
        Graphics.Blit(_blackClear, Candidate);
        Graphics.Blit(_blackClear, Canvas);
        Graphics.Blit(Canvas, Candidate, BrushMaterial);

        Graphics.Blit(Candidate, BestCandidate);

    }

}
