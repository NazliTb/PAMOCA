using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAMOCA
{
    internal class Frame
    {

        List<float[]> Joints = new List<float[]>();

        List<float[]> positionRotation = new List<float[]>();


        Joint rootJoint;

        public List<float[]> getPositionRotation()
        {
            return this.positionRotation;
        }

        public void setPositionRotation(List<float[]> positionRotation)
        {
            this.Joints = positionRotation;
        }


        public List<float[]> getJoints()
        {
            return this.Joints;
        }

        public void setJoints(List<float[]> Joints)
        {
            this.Joints = Joints;
        }


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
