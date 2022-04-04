using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VolumeControl.Core.Controls
{
    public class CenteredNumericUpDown : CenteredUpDown
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CenteredNumericUpDown()
        {
            // Add a handler for the up button
            UpPressed += delegate
            {
                decimal val = Value + Increment;
                if (val > Maximum)
                    val = Maximum;
                Value = val;
            };
            // Add a handler for the down button
            DownPressed += delegate
            {
                decimal val = Value - Increment;
                if (val < Minimum)
                    val = Minimum;
                Value = val;
            };
            KeyPress += delegate (object sender, KeyPressEventArgs e)
            {
                if (char.IsDigit(e.KeyChar))
                    e.Handled = true;
            }!;
            TextChanged += delegate
            {
                Text = Regex.Replace(Text, "(?![1234567890]+)", m => "");
            };
        }
        #endregion Constructor

        #region Members
        private decimal _incr = 1m;
        private decimal _min = 0m, _max = 100m;
        #endregion Members

        #region Events
        /// <summary>
        /// Triggered when the value is changed.
        /// </summary>
        public event EventHandler? ValueChanged
        {
            add => TextChanged += value;
            remove => TextChanged -= value;
        }
        #endregion Events

        #region Properties
        /// <summary>
        /// Gets or sets the amount added or subtracted from the value each time the up/down buttons are pressed.
        /// </summary>
        public decimal Increment
        {
            get => _incr;
            set => _incr = value;
        }
        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public decimal Minimum
        {
            get => _min;
            set => _min = value;
        }
        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public decimal Maximum
        {
            get => _max;
            set => _max = value;
        }
        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public decimal Value
        {
            get
            {
                decimal val = 0m;
                if (Text.Length > 0 && Text.All(char.IsDigit))
                    val = Convert.ToDecimal(Text);
                return val;
            }
            set
            {
                if (value < Minimum)
                    value = Minimum;
                else if (value > Maximum)
                    value = Maximum;
                Text = value.ToString();
            }
        }
        public bool AllowDecimalPlaces { get; set; }
        public bool AllowSeparators { get; set; }
        #endregion Properties
    }
}
