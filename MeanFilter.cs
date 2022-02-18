using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAMOCA
{
    internal class MeanFilter
    {
        List<Frame> listFrames = new List<Frame>();        

        void filter (List<Frame> listFrames)
        {
            float result;

            for (int i = 0; i <= listFrames.Count - 3; i++)
            {
                if (listFrames.Count >= 3)
                {
                    result = (listFrames[i] + listFrames[i + 1] + listFrames[i + 2]) / 3;
                }

                else if (listFrames.Count == 2)
                {
                    result = listFrames[i] + listFrames[i + 1] / 2;
                }

                else if (listFrames.Count == 1)
                {
                    result = listFrames[i];
                }                

                else if (listFrames.Count < 1)
                {
                    return;
                }
            }            

        }       

    }
}
