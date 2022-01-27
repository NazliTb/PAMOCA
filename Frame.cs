using System;
using System.Collections.Generic;

namespace PAMOCA
{
    class Frame
    {



        // List<Joint> listJoints = new List<Joint>();
        Joint rootJoint;

        public Joint getRootJoint()
        {
            return this.rootJoint;
        }

        public void setRootJoint(Joint rootJoint)
        {
            this.rootJoint = rootJoint;
        }





        public override string ToString()
        {
            return base.ToString() + "==> " + "jointName :" + rootJoint.ToString();
        }






    }
}