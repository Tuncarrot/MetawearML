using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeGraph
{
    public class SensorData
    {
        public List<float> X_Value { get; set; }
        public List<float> Y_Value { get; set; }
        public List<float> Z_Value { get; set; }

        public bool RecordData { get; set; }

        public SensorData()
        {
            X_Value = new List<float>();
            Y_Value = new List<float>();
            Z_Value = new List<float>();

            RecordData = false;
        }
    }
}
