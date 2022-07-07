using System.Windows.Data;
using VolumeControl.TypeExtensions;

namespace VolumeControl.WPF.Bindings
{
    /// <summary>
    /// Easy multibinding object.
    /// </summary>
    public class MultiBinding : System.Windows.Data.MultiBinding
    {
        /// <inheritdoc cref="System.Windows.Data.MultiBinding.MultiBinding"/>
        public MultiBinding() : base() { }
        /// <inheritdoc cref="System.Windows.Data.MultiBinding.MultiBinding"/>
        /// <param name="bindings">Any number of binding objects.</param>
        public MultiBinding(params BindingBase[] bindings) : base() => bindings.ForEach(b => this.Bindings.Add(b));

#       pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        // This is required because JESUS CHRIST MICROSOFT FIX YOUR FUCKING SHIT AND STOP MAKING EVERYTHING SO GODDAMN DIFFICULT FOR NO REASON:
        public MultiBinding(BindingBase b1) : this(new[] { b1 }) { }
        public MultiBinding(BindingBase b1, BindingBase b2) : this(new[] { b1, b2 }) { }
        public MultiBinding(BindingBase b1, BindingBase b2, BindingBase b3) : this(new[] { b1, b2, b3 }) { }
        public MultiBinding(BindingBase b1, BindingBase b2, BindingBase b3, BindingBase b4) : this(new[] { b1, b2, b3, b4 }) { }
        public MultiBinding(BindingBase b1, BindingBase b2, BindingBase b3, BindingBase b4, BindingBase b5) : this(new[] { b1, b2, b3, b4, b5 }) { }
        public MultiBinding(BindingBase b1, BindingBase b2, BindingBase b3, BindingBase b4, BindingBase b5, BindingBase b6) : this(new[] { b1, b2, b3, b4, b5, b6 }) { }
        public MultiBinding(BindingBase b1, BindingBase b2, BindingBase b3, BindingBase b4, BindingBase b5, BindingBase b6, BindingBase b7) : this(new[] { b1, b2, b3, b4, b5, b6, b7 }) { }
        public MultiBinding(BindingBase b1, BindingBase b2, BindingBase b3, BindingBase b4, BindingBase b5, BindingBase b6, BindingBase b7, BindingBase b8) : this(new[] { b1, b2, b3, b4, b5, b6, b7, b8 }) { }
        public MultiBinding(BindingBase b1, BindingBase b2, BindingBase b3, BindingBase b4, BindingBase b5, BindingBase b6, BindingBase b7, BindingBase b8, BindingBase b9) : this(new[] { b1, b2, b3, b4, b5, b6, b7, b8, b9 }) { }
        public MultiBinding(BindingBase b1, BindingBase b2, BindingBase b3, BindingBase b4, BindingBase b5, BindingBase b6, BindingBase b7, BindingBase b8, BindingBase b9, BindingBase b10) : this(new[] { b1, b2, b3, b4, b5, b6, b7, b8, b9, b10 }) { }
#       pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
