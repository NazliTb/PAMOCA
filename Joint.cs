using System;
using System.Collections.Generic;

namespace PAMOCA
{
    class Joint
    {

        List<float> dataPoint = new List<float>();
        List<float> channelsPoints = new List<float>();

        //float[] dataPoint;
        List<Joint> listInnerJoints = new List<Joint>();
        String jointName;


        public List<float> getDataPoint()
        {
            return this.dataPoint;
        }

        public void setDataPoint(List<float> dataPoint)
        {
            this.dataPoint = dataPoint;
        }

        public List<float> getChannelsPoints()
        {
            return this.channelsPoints;
        }

        public void setChannelsPoints(List<float> channelsPoints)
        {
            this.channelsPoints = channelsPoints;
        }

        public String getJointName()
        {
            return this.jointName;
        }

        public void setJointName(String jointName)
        {
            this.jointName = jointName;
        }





        public List<Joint> getListInnerJoints()
        {
            return this.listInnerJoints;
        }

        public void setListInnerJoints(List<Joint> listInnerJoints)
        {
            this.listInnerJoints = listInnerJoints;
        }

        public override string ToString()
        {
            return base.ToString() + "==> " + "jointName :" + jointName.ToString() + "dataPoint :" + dataPoint.ToString() +
             "channelsPoints :" + channelsPoints.ToString() + "listInnerJoints :" + listInnerJoints.ToString();
        }

    }
}