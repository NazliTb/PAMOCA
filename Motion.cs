using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAMOCA
{
    internal class Motion
    {
        List<Frame> listFrames = new List<Frame>();
        List<Joint> listJoints = new List<Joint>();

        int nbFrames;
        float frameTime;

        public int getNbFrames()
        {
            return this.nbFrames;
        }

        public void setNbFrames(int nbFrames)
        {
            this.nbFrames = nbFrames;
        }

        public float getFrameTime()
        {
            return this.frameTime;
        }

        public void setFrameTime(float frameTime)
        {
            this.frameTime = frameTime;
        }



        public List<Joint> getListJoints()
        {
            return this.listJoints;
        }

        public void setListJoints(List<Joint> listJoints)
        {
            this.listJoints = listJoints;
        }

        public List<Frame> getListFrames()
        {
            return this.listFrames;
        }

        public void setListFrames(List<Frame> listFrames)
        {
            this.listFrames = listFrames;
        }


        public String[] importFile()
        {

            String[] file = System.IO.File.ReadAllLines(@"C:\Users\nazli\Documents\GitHub\PAMOCA\Fichiers 3D\Waving_4_Take_001.bvh");

            return file;
        }


        public override string ToString()
        {
            return base.ToString() + "==> " + " nbFrames : " + nbFrames.ToString() + " frameTime : " + frameTime.ToString()
            + "listFrames :" + listFrames.ToString();
        }
    }
}
