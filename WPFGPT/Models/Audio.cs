using NAudio.Wave;

namespace WPFGPT.Models;

public class Audio
{
    private WaveInEvent MWavIn { get; set; } = null!;
    private WaveFileWriter MWavWriter { get; set; } = null!;

    public void StartRecording()
    {
        var format = new WaveFormat(44100, 16, 1);
        this.MWavIn = new WaveInEvent { WaveFormat = format };
        this.MWavIn.DataAvailable += this.MWavIn_DataAvailable!;
        this.MWavWriter = new WaveFileWriter("ApeVoice.wav", MWavIn.WaveFormat);
        this.MWavIn.StartRecording();
    }
    
    public void StopRecording()
    {
        this.MWavIn.StopRecording();
        this.MWavIn.Dispose();
        this.MWavWriter.Close();
    }

    private void MWavIn_DataAvailable(object sender, WaveInEventArgs e)
    {
        this.MWavWriter!.Write(e.Buffer, 0, e.BytesRecorded);
    }
}