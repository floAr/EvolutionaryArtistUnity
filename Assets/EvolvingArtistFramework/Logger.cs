using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Logger : IDisposable
{
    private static StreamWriter _writer;

    public Logger(string logFolder)
    {
        var file = $"{logFolder}/log.txt";
        // File.Create(file);
        _writer = new StreamWriter(file);
    }

    private string ChromosomeToLog(EAChromosome chromo)
    {
        var result = "";
        var floats = chromo.ToFloatingPoints();
        foreach (var item in floats)
        {
            result += $"{item}*";
        }
        return result;
    }

    public EAChromosome LogToChromosome(string log, ref EAChromosome result)
    {
        var parsed = log.Split('*');

       
        var floats = from f in parsed where f.Length>0 select float.Parse(f);
        result.LoadFloatingPoints(floats.ToArray());
        return result;
    }

    public void Log(EAChromosome bestCandidate)
    {
        _writer.WriteLine(ChromosomeToLog(bestCandidate));
    }

    public void Dispose()
    {
        _writer.Flush();
        _writer.Dispose();
        _writer = null;

    }



    public IEnumerable<EAChromosome> Replay(string logFolder, Experiment exp)
    {
        using (StreamReader _reader = new StreamReader(logFolder + "/log.txt"))
        {
            var result = new EAChromosome(exp.StrokesPerGeneration, exp.GetGeneCount());
            while (!_reader.EndOfStream)
            {
                yield return LogToChromosome(_reader.ReadLine(), ref result);
            }
        }
    }


}
