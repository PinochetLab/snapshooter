using UnityEngine;

namespace Electricity.Wires
{
    public class Current
    {
        public static Color DefaultColor = Color.yellow;
        
        public float Start { get; set; }
        public float End { get; set; }
        public bool EndOn { get; set; }
        
        public Color Color { get; set; }
    }
}