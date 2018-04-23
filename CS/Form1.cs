using System;
using System.Windows.Forms;
using DevExpress.XtraRichEdit;
using DevExpress.Services;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Commands;

namespace RichWordsIterator {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            LoadContent();
            SubstituteKeyboardService();
        }

        private void LoadContent() {
            richEditControl1.LoadDocument(Application.StartupPath + @"\..\..\" + "TextFile.txt", DocumentFormat.PlainText);
        }

        private void SubstituteKeyboardService() {
            IKeyboardHandlerService service = richEditControl1.GetService<IKeyboardHandlerService>();
            MyKeyboardHandlerServiceWrapper wrapper = new MyKeyboardHandlerServiceWrapper(service);
            richEditControl1.RemoveService(typeof(IKeyboardHandlerService));
            richEditControl1.AddService(typeof(IKeyboardHandlerService), wrapper);
        }
    }

    public class MyKeyboardHandlerServiceWrapper : KeyboardHandlerServiceWrapper {
        public MyKeyboardHandlerServiceWrapper(IKeyboardHandlerService service)
            : base(service) { }

        public override void OnKeyDown(KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.F6:
                    SelectNextWord();
                    break;
                case Keys.F7:
                    SelectPreviousWord();
                    break;
                default:
                    base.OnKeyDown(e);
                    break;
            }
        }

        private void SelectNextWord() {
            IRichEditControl targetControl = ((RichEditKeyboardHandlerService)this.Service).Control;

            if (targetControl.Document.Selection.End.ToInt() == targetControl.Document.Range.End.ToInt() - 1)
                return;

            targetControl.Document.CaretPosition = targetControl.Document.CreatePosition(targetControl.Document.Selection.Start.ToInt() + 1);

            NextWordCommand nextWordCommand = new NextWordCommand(targetControl);
            nextWordCommand.Execute();

            ExtendNextWordCommand extendNextWordCommand = new ExtendNextWordCommand(targetControl);
            extendNextWordCommand.Execute();
        }

        private void SelectPreviousWord() {
            IRichEditControl targetControl = ((RichEditKeyboardHandlerService)this.Service).Control;

            if (targetControl.Document.Selection.Start.ToInt() == 0)
                return;

            targetControl.Document.CaretPosition = targetControl.Document.CreatePosition(targetControl.Document.Selection.Start.ToInt() - 1);

            PreviousWordCommand previousWordCommand = new PreviousWordCommand(targetControl);
            previousWordCommand.Execute();

            ExtendNextWordCommand extendNextWordCommand = new ExtendNextWordCommand(targetControl);
            extendNextWordCommand.Execute();
        }

    }

}