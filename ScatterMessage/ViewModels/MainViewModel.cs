using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ScatterMessage.Models;

namespace ScatterMessage.ViewModels
{
    public partial class MainViewModel : ObservableItem
    {
        public double WindowHeight { get; set; }           //Rendered window height
        public double WindowWidth  { get; set; }           //Rendered window width

        public string BoxText      { private get; set; }   //Contents of the text box
        public bool   IntoAction   { get; set; }           //Labels are ready to be scattered?
        public int    Force        { get; set; }           //Amount of force to apply
        public int    Friction     { get; set; }           //Amount of friction to slow movement

        private List<ScatterLabel> ScLabels = new List<ScatterLabel>();         //Main data model
        public ObservableCollection<Label> Scatterables { get; private set; }   //Bound to ItemsControl
        
        public ICommand ChangeMessage { get; private set; }     // => UpdateMessage()
        public ICommand MainAction { get; private set; }        // => ExplodeOrReset()
        
        private DispatcherTimer timer = new DispatcherTimer();
        private const int SECOND_IN_TICKS = 10000000;


        //Constructor
        public MainViewModel()
        {            
            //Initialize Scatterables collection
            this.Scatterables = new ObservableCollection<Label>();
            this.BoxText = "oO@Oo";// "Hello, World!";
            UpdateMessage();

            //Initialize user input controls
            this.Force = 20;
            this.OnPropertyChanged(() => Force);
            this.Friction = 1;
            this.OnPropertyChanged(() => Friction);

            //Set up timer
            timer.Tick += new EventHandler(MoveEm);
            timer.Interval = new TimeSpan(SECOND_IN_TICKS / 30);    // 30 fps

            //Assign button command delegates
            this.ChangeMessage = new Command { CanExecuteDelegate = ced => !IntoAction,
                                               ExecuteDelegate = ed => UpdateMessage() };
            this.MainAction = new Command { ExecuteDelegate = ed => ScatterOrReset() };
        }




        private void UpdateMessage()
        /// <summary>
        /// Build private collection of labels from user supplied string
        /// </summary>
        {
            var widths = new Queue<double>();   //stores width of individual labels

            ScLabels.Clear();
            // Build collection of labels -- one for each letter,
            // Measure layout width of each
            // Sum the total width
            foreach (char ltr in BoxText)
            {
                //Define label properties
                Label lbl = new Label();
                lbl.Content = ltr;
                lbl.FontWeight = FontWeights.Bold;

                //Measure label
                lbl.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                widths.Enqueue(lbl.DesiredSize.Width);
                
                //Increment total message width
                ScatterLabel.MessageWidth += lbl.DesiredSize.Width;
                
                //Add to private collection
                ScLabels.Add(new ScatterLabel(lbl));
            }

            // Set label positions
            double rightShift = 0.0;    //adjusts xOffset for each label added
            foreach (ScatterLabel sl in ScLabels)
            {
                sl.SetOff(rightShift - ScatterLabel.MessageWidth * 0.5, 0);
                rightShift += widths.Dequeue();
            }
    
            UpdateScatterables();
        }
        
        
        private void ScatterOrReset()
        /// <summary>
        /// Evaluates IntoAction ? ApplyForce() : Reset()
        /// </summary>
        {
            if (IntoAction)
            {
                foreach (ScatterLabel sl in ScLabels)
                    sl.ApplyForce(Force);

                timer.Start();
            }
            else
            {
                foreach (ScatterLabel sl in ScLabels)
                    sl.Reset();

                timer.Stop();
                UpdateScatterables();

            }

        }


        private void MoveEm(object sender, EventArgs e)
        /// <summary>
        /// Event handler (Timer tick): Move labels and adjust velocity for friction
        /// </summary>
        {
            bool inMotion = false;

            foreach (ScatterLabel sl in ScLabels)
            {
                if (sl.UpdatePosition(Friction, WindowHeight, WindowWidth))
                    inMotion = true;
            }

            if (!inMotion)
                timer.Stop();

            UpdateScatterables();
        }


        private void UpdateScatterables()
        /// <summary>
        /// Rebuild dependency collection from private set
        /// </summary>
        {
            Scatterables.Clear();
            foreach (ScatterLabel sl in ScLabels)
            {
                sl.TransformLayout();
                Scatterables.Add(sl.BoxedLabel);
            }
        }
    }
}
