using System.Reactive.Linq;
using LINQPad.Controls;
using LINQPad.Uncapsulator;

namespace LINQPadHelpers.Controls;

/// <summary>Override of FilePicker to select a directory.</summary>
public class DirectoryFilePicker : FilePicker
{
    public DirectoryFilePicker() : base() { }
    public DirectoryFilePicker(string path) : base() => Text = path;

    protected override void ProcessButtonClick()
    {
        var openFileDialog = new System.Windows.Forms.FolderBrowserDialog()
                             {
                                 OkRequiresInteraction = true
                             };
        if (Directory.Exists(Text))
        {
            openFileDialog.InitialDirectory = Text;
            openFileDialog.OkRequiresInteraction = false;
        }

        if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) 
            return;
        Text = openFileDialog.SelectedPath;
        this.Uncapsulate().TextInput.ToObject()?.Invoke(this, EventArgs.Empty);
    }
    public IObservable<System.Reactive.EventPattern<object>> ClickObservable() => Observable.FromEventPattern(o => Click += o, o => Click -= o);
    public IObservable<System.Reactive.EventPattern<object>> TextInputObservable() => Observable.FromEventPattern(o => TextInput += o, o => TextInput -= o);
}
